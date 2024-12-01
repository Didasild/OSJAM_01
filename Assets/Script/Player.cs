using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("HEALTH")]
    public int initialHealthPoints = 3;
    public TMP_Text healthPointText;
    private int healthPoints;

    [Header("MANA")]
    public int initialManaPoints = 10;
    public int maxManaPoints = 100;
    public TMP_Text manaPointText;
    private int manaPoints;

    [Header("SWORD")]
    public int initialSwordCounter = 0;
    public TMP_Text swordCounterText;
    private int swordCounter = 0;

    [Header("CLICK COUNTER")]
    [NaughtyAttributes.ReadOnly]
    public int clicCounter;

    //Private Variables
    private Cell cellOver;
    private Cell cellClicked;
    private Cell firstCellClicked;

    // Update is called once per frame
    void Update()
    {
        // Convertit la position de la souris en coordonnées du monde
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Effectue un raycast à la position de la souris
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            cellOver = hit.collider.GetComponent<Cell>();
        }
        else
        {
            //Debug.Log("Aucune cellule détectée");
            return;
        }
        if (cellOver == null || GameManager.Instance.currentGameState != GameState.InGame)
        {
            return;
        }

        #region LEFT CLICK
        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            cellClicked = cellOver;
            if (firstCellClicked != cellClicked)
            {
                ResetClickedState();
                return;
            }

            if (cellClicked.currentState == CellState.Reveal && cellClicked.currentType == CellType.Hint)
            {
                ClikOnRevealHintCell(cellClicked);
            }

            if (cellClicked.currentState == CellState.Cover)
            {
                ClickOnCoverCell(cellClicked);
            }
            else if (cellClicked.currentState == CellState.PlantedSword)
            {
                ClickOnPlantedSwordCell(cellClicked);
            }
            else if (cellClicked.currentType == CellType.Gate)
            {
                ClickOnGateCell(cellClicked);
            }
            else if (cellClicked.currentItemType == ItemTypeEnum.Potion)
            {
                ClickOnItemCell(cellClicked, ItemTypeEnum.Potion);
            }
            else if (cellClicked.currentItemType == ItemTypeEnum.Sword)
            {
                ClickOnItemCell(cellClicked, ItemTypeEnum.Sword);
            }
            //Update le compteur de mine restantes
            GameManager.Instance.gridManager.UpdateMineCounter();
            ResetClickedState();
        }

        //Clic gauche Down
        if (Input.GetMouseButtonDown(0))
        {
            firstCellClicked = cellOver;
            SwitchCellsToClickedState();
        }

        //Clic gauche enfoncé (s'update en permanence)
        if (Input.GetMouseButton(0))
        {
            if (cellClicked != cellOver)
            {
                ResetClickedState();
                SwitchCellsToClickedState();
            }

        }
        #endregion

        #region RIGHT CLICK 
        // Clique sur le bouton droit
        if (Input.GetMouseButtonDown(1))
        {
            if (cellOver.currentState == CellState.Cover)
            {
                cellOver.ChangeState(CellState.Flag);
            }
            else if (cellOver.currentState == CellState.Flag && swordCounter >= 1)
            {
                //cellOver.ChangeState(CellState.PlantedSword);
                cellOver.ItemStatetransition(CellState.PlantedSword, 0.35f);
                DecreaseSwordCounter();
            }
            else if (cellOver.currentState == CellState.Flag && swordCounter == 0)
            {
                cellOver.ChangeState(CellState.Cover);
            }
            else if (cellOver.currentState == CellState.PlantedSword)
            {
                cellOver.ChangeState(CellState.Cover);
                IncreaseSwordCounter();
            }
            //Update le compteur de mine restantes
            GameManager.Instance.gridManager.UpdateMineCounter();
        }
        #endregion

        #region MIDDLE CLICK
        // Clique du milieu
        if (Input.GetMouseButtonDown(2))
        {


        }
        #endregion
    }

    #region CLICK METHODS
    public void ClikOnRevealHintCell(Cell cellClicked)
    {
        int mineExploded = 0;

        //Récupère le nombre de drapeaux et de mines autour
        int neighborsFlagged = cellClicked.GetNeighborsState(CellState.Flag);
        int neighborsMine = cellClicked.GetNeighborsType(CellType.Mine);

        //Reveal les case couverte autour
        if (cellClicked.currentType == CellType.Hint && neighborsFlagged == neighborsMine)
        {
            foreach (Cell neighborsCell in cellClicked.neighborsCellList)
            {
                if (neighborsCell.currentState == CellState.Clicked && neighborsCell.currentType == CellType.Mine)
                {
                    mineExploded += 1;
                    neighborsCell.MineExplosion();
                }
                if (neighborsCell.currentState == CellState.Clicked)
                {
                    neighborsCell.ChangeState(CellState.Reveal);
                }
            }
            if (mineExploded >= 1)
            {
                foreach (Cell neighborsCell in cellClicked.neighborsCellList)
                {
                    if (neighborsCell.currentState == CellState.Flag && neighborsCell.currentType != CellType.Mine)
                    {
                        neighborsCell.ChangeState(CellState.Reveal);
                    }
                }
            }
        }
        //Passe en state Clicked les cellules couvertes
        else
        {
            ResetClickedState();
        }
    }

    public void ClickOnCoverCell(Cell cellClicked)
    {
        //Génére les items et assure que le premier clic est vide
        if (clicCounter == 0)//remplacer par la condition que la grid soit totalement couverte
        {
            cellClicked.ChangeType(CellType.Empty);
            cellClicked.RemoveNeighborsMine();
            GameManager.Instance.gridManager.SetItemsType(CellType.Gate, 1);
            GameManager.Instance.gridManager.SetItemsType(CellType.Item, GameManager.Instance.currentFloorSettings.GetNumberOfPotion(), ItemTypeEnum.Potion);
            GameManager.Instance.gridManager.SetItemsType(CellType.Item, GameManager.Instance.currentFloorSettings.GetNumberOfSword(), ItemTypeEnum.Sword);

        }
        //Explose la mine si sans est une
        if (cellClicked.currentType == CellType.Mine)
        {
            cellClicked.MineExplosion();
        }

        //Augmente le compteur de clic et révèle la case
        else
        {
            cellClicked.ChangeState(CellState.Reveal);
        }
        IncreaseClickCount();
    }

    public void ClickOnPlantedSwordCell(Cell cellClicked)
    {
        if (cellClicked.currentType == CellType.Mine)
        {
            cellClicked.MineSwordDestruction(cellClicked.animSwordOnMine);
        }
        else
        {
            cellClicked.ChangeState(CellState.Reveal);
        }
    }

    public void ClickOnGateCell(Cell cellClicked)
    {
        GameManager.Instance.ChangeFloorLevel();
        ResetClickCounter();
    }

    public void ClickOnItemCell(Cell cellClicked, ItemTypeEnum itemType)
    {
        if (itemType == ItemTypeEnum.Potion)
        {
            IncreaseHealth(1);
        }
        else if (itemType == ItemTypeEnum.Sword) 
        {
            IncreaseSwordCounter();
        }
        // Fait disparaitre l'item collecté
        cellClicked.ChangeType(CellType.Empty);
        cellClicked.ChangeItemType(ItemTypeEnum.None);
        cellClicked.UpdateRegardingNeighbors();
    }

    public void SwitchCellsToClickedState()
    {
        cellClicked = cellOver;

        int neighborsFlagged = cellClicked.GetNeighborsState(CellState.Flag);
        int neighborsMine = cellClicked.GetNeighborsType(CellType.Mine);
        //neighborsFlagged != neighborsMine && /// Condition
        if (cellClicked.currentType == CellType.Hint && cellClicked.currentState == CellState.Reveal)
        {
             foreach (Cell neighborsCell in cellClicked.neighborsCellList)
             {
                 if (neighborsCell.currentState == CellState.Cover)
                 {
                     neighborsCell.ChangeState(CellState.Clicked);
                 }
             }
        }
    }

    public void ResetClickedState()
    {
        foreach (Cell neighborsCell in cellClicked.neighborsCellList)
        {
            if (neighborsCell.currentState == CellState.Clicked)
            {
                neighborsCell.ChangeState(CellState.Cover);
            }
        }
    }
    #endregion

    #region HEALTH
    public void ResetHealtPoint()
    {
        healthPoints = initialHealthPoints;
        healthPointText.text = healthPoints.ToString();
    }

    public void DecreaseHealth(int damage)
    {
        healthPoints -= damage;
        healthPointText.text = healthPoints.ToString();
        if (healthPoints <= 0)
        {
            GameManager.Instance.ChangeGameState(GameState.Loose);
        }
    }

    public void IncreaseHealth(int heal)
    {
        healthPoints += heal;
        healthPointText.text = healthPoints.ToString();
    }
    #endregion

    #region MANA
    public void IncreaseMana(int manaIncrease = 1)
    {
        if (manaPoints < maxManaPoints)
        {
            manaPoints += manaIncrease;
            manaPointText.text = manaPoints.ToString();
        }
        else
        {
            return;
        }

    }

    public void DecreaseMana(int manaDecrease)
    {
        if (manaPoints == 0)
        {
            return;
        }
        manaPoints -= manaDecrease;
        manaPointText.text += manaPoints.ToString();
    }

    public bool AsEnoughMana(int manaNecessary)
    {
        bool enoughMana = false;
        if (manaNecessary >= manaPoints)
        {
            enoughMana = true;
        }
        else
        {
            enoughMana = false;
        }
        return enoughMana;
    }

    public void ResetMana()
    {
        manaPoints = initialManaPoints;
        manaPointText.text= manaPoints.ToString();
    }
    #endregion

    #region SWORD
    public void IncreaseSwordCounter(int swordIncrease = 1)
    {
        swordCounter += swordIncrease;
        swordCounterText.text = swordCounter.ToString();
    }

    public void DecreaseSwordCounter(int swordDecrease = 1)
    {
        if (swordCounter == 0)
        {
            return;
        }
        swordCounter -= swordDecrease;
        swordCounterText.text = swordCounter.ToString();
    }
    public void ResetSwordCounter()
    {
        swordCounter = initialSwordCounter;
        swordCounterText.text = swordCounter.ToString();
    }
    #endregion

    #region CLICK COUNTER
    public void ResetClickCounter()
    {
        clicCounter = 0;
    }

    public void IncreaseClickCount()
    {
        clicCounter += 1;
    }
    #endregion

}

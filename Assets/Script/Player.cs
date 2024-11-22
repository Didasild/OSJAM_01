using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Cell cellOver;
    [Header("HEALTH")]
    public int initialHealthPoints = 3;
    public TMP_Text healthPointText;
    private int healthPoints;
    [Header("SWORD")]
    public int initialSwordCounter = 0;
    public TMP_Text swordCounterText;
    private int swordCounter;
    [Header("MINE LEFT")]
    [NaughtyAttributes.ReadOnly]
    public int theoricalMineLeft;
    [NaughtyAttributes.ReadOnly]
    public int realMineLeft;
    public TMP_Text theoricalMineLeftText;

    [Header("CLICK COUNTER")]
    [NaughtyAttributes.ReadOnly]
    public int clicCounter;

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
            Debug.Log("Aucune cellule détectée");
            return;
        }
        if (cellOver == null || GameManager.Instance.currentGameState != GameState.InGame)
        {
            return;
        }

        #region CLIC GAUCHE UP
        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            Cell cellClicked = cellOver;
            if (cellClicked.currentState == CellState.Reveal && cellClicked.currentType == CellType.Hint)
            {
                int neighborsFlagged = 0;
                int neighborsMine = 0;
                int neighborsCover = 0;
                int mineExploded = 0;

                //Récupère le nombre de drapeaux et de mines autour
                neighborsFlagged = cellClicked.GetNeighborsState(CellState.Flag);
                neighborsMine = cellClicked.GetNeighborsType(CellType.Mine);
                neighborsCover = cellClicked.GetNeighborsState(CellState.Cover);

                //Reveal les case couverte autour
                if (cellClicked.currentType == CellType.Hint && neighborsFlagged == neighborsMine)
                {
                    foreach (Cell neighborsCell in cellClicked.neighborsCellList)
                    {
                        if (neighborsCell.currentState == CellState.Cover && neighborsCell.currentType == CellType.Mine)
                        {
                            mineExploded += 1;
                            neighborsCell.MineExplosion();
                        }
                        if (neighborsCell.currentState == CellState.Cover)
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
            }
            if (cellClicked.currentState == CellState.Cover)
            {
                //Génére les items et assure que le premier clic est vide
                if (clicCounter == 0)//remplacer par la condition que la grid soit totalement couverte
                {
                    cellClicked.ChangeType(CellType.Empty);
                    cellClicked.RemoveNeighborsMine();
                    GameManager.Instance.grid.SetItemsType(CellType.Gate, 1);
                    GameManager.Instance.grid.SetItemsType(CellType.Potion, GameManager.Instance.currentFloorSettings.GetNumberOfPotion());
                    GameManager.Instance.grid.SetItemsType(CellType.Sword, GameManager.Instance.currentFloorSettings.GetNumberOfSword());

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
            else if (cellClicked.currentState == CellState.PlantedSword)
            {
                if (cellClicked.currentType == CellType.Mine)
                {
                    cellClicked.DestroyCellType(cellClicked.mineSwordAnim);
                }
                else
                {
                    cellClicked.ChangeState(CellState.Reveal);
                }
            }
            else if (cellClicked.currentType == CellType.Gate)
            {
                GameManager.Instance.ChangeFloorLevel();
                ResetClickCounter();
            }
            else if (cellClicked.currentType == CellType.Potion)
            {
                IncreaseHealth(1);
                cellClicked.ChangeType(CellType.Empty);
                cellClicked.UpdateRegardingNeighbors();
            }
            else if (cellClicked.currentType == CellType.Sword)
            {
                IncreaseSwordCounter();
                cellClicked.ChangeType(CellType.Empty);
                cellClicked.UpdateRegardingNeighbors();
            }
            //Update le compteur de mine restantes
            UpdateMineCounter();

        }

        //Clic gauche Down
        if (Input.GetMouseButton(0))
        {
            
        }
        #endregion

        #region CLIC DROIT
        // Clique sur le bouton droit
        if (Input.GetMouseButtonDown(1))
        {
            if (cellOver.currentState == CellState.Cover)
            {
                cellOver.ChangeState(CellState.Flag);
            }
            else if (cellOver.currentState == CellState.Flag && swordCounter >= 1)
            {
                cellOver.ChangeState(CellState.PlantedSword);
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
            UpdateMineCounter();
        }

        #endregion

        #region CLIC MILIEU
        // Clique du milieu
        if (Input.GetMouseButtonDown(2))
        {


        }
        #endregion
    }

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

    #region SWORD
    public void ResetSwordCounter()
    {
        swordCounter = initialSwordCounter;
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

    public void IncreaseSwordCounter(int swordIncrease = 1)
    {
        swordCounter += swordIncrease;
        swordCounterText.text = swordCounter.ToString();
    }

    #endregion

    public void ResetClickCounter()
    {
        clicCounter = 0;
    }

    public void IncreaseClickCount()
    {
        clicCounter += 1;
    }
    #region MINE COUNTER
    public void UpdateMineCounter()
    {
        theoricalMineLeft = GameManager.Instance.grid.GetTheoricalMineLeft();
        theoricalMineLeftText.text = theoricalMineLeft.ToString();
    }
    #endregion

}

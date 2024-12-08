using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region PARAMETERS
    [Header("HEALTH")]
    public int initialHealthPoints = 3;
    public TMP_Text healthPointText;
    private int _healthPoints;

    [Header("MANA")]
    public int initialManaPoints = 10;
    public int maxManaPoints = 100;
    public TMP_Text manaPointText;
    private int _manaPoints;

    [Header("SWORD")]
    public int initialSwordCounter;
    public TMP_Text swordCounterText;
    private int _swordCounter;

    [Header("CLICK COUNTER")]
    [NaughtyAttributes.ReadOnly]
    public int clicCounter;

    //Private Variables
    private Cell _cellOver;
    private Cell _cellClicked;
    private Cell _firstCellClicked;
    #endregion


    void Update()
    {
        // Convertit la position de la souris en coordonn�es du monde
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Effectue un raycast � la position de la souris
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            _cellOver = hit.collider.GetComponent<Cell>();
        }
        else
        {
            //Debug.Log("Aucune cellule d�tect�e");
            return;
        }
        if (_cellOver == null || GameManager.Instance.currentGameState != GameState.InGame)
        {
            return;
        }

        #region LEFT CLICK
        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            _cellClicked = _cellOver;
            if (_firstCellClicked != _cellClicked)
            {
                ResetClickedState();
                return;
            }

            if (_cellClicked.currentState == CellState.Reveal && _cellClicked.currentType == CellType.Hint)
            {
                ClickOnRevealHintCell(_cellClicked);
            }

            if (_cellClicked.currentState == CellState.Cover)
            {
                ClickOnCoverCell(_cellClicked);
            }
            else if (_cellClicked.currentState == CellState.PlantedSword)
            {
                ClickOnPlantedSwordCell(_cellClicked);
            }
            else if (_cellClicked.currentType == CellType.Gate)
            {
                ClickOnGateCell(_cellClicked);
            }
            else if (_cellClicked.currentItemType == ItemTypeEnum.Potion)
            {
                ClickOnItemCell(_cellClicked, ItemTypeEnum.Potion);
            }
            else if (_cellClicked.currentItemType == ItemTypeEnum.Sword)
            {
                ClickOnItemCell(_cellClicked, ItemTypeEnum.Sword);
            }
            //Update le compteur de mines restantes
            GameManager.Instance.gridManager.UpdateMineCounter();
            ResetClickedState();
        }

        //Clic gauche Down
        if (Input.GetMouseButtonDown(0))
        {
            _firstCellClicked = _cellOver;
            SwitchCellsToClickedState();
        }

        //Clic gauche enfonc� (s'update en permanence)
        if (Input.GetMouseButton(0))
        {
            if (_cellClicked != _cellOver)
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
            if (_cellOver.currentState == CellState.Cover)
            {
                _cellOver.ChangeState(CellState.Flag);
            }
            else if (_cellOver.currentState == CellState.Flag && _swordCounter >= 1)
            {
                //cellOver.ChangeState(CellState.PlantedSword);
                _cellOver.ItemStateTransition(CellState.PlantedSword, 0.35f);
                DecreaseSwordCounter();
            }
            else if (_cellOver.currentState == CellState.Flag && _swordCounter == 0)
            {
                _cellOver.ChangeState(CellState.Cover);
            }
            else if (_cellOver.currentState == CellState.PlantedSword)
            {
                _cellOver.ChangeState(CellState.Cover);
                IncreaseSwordCounter();
            }
            //Update le compteur de mines restantes
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
    private void ClickOnRevealHintCell(Cell cellClicked)
    {
        int mineExploded = 0;

        //R�cup�re le nombre de drapeaux et de mines autour
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

    private void ClickOnCoverCell(Cell cellClicked)
    {
        //V�rifie si la grid doit �tre proc�durale et est compl�tement couverte puis g�n�re les items
        // VOIR POUR UNE MEILLEURE CONDITION GENRE SI GRID PROCEDURAL…
        int nbOfCellsCover = GameManager.Instance.gridManager.GetCellsByState(CellState.Cover).Count;
        int nbOfCells = GameManager.Instance.gridManager.cellList.Count;
        if (nbOfCells == nbOfCellsCover && GameManager.Instance.currentRoomSettings.proceduralRoom)
        {
            cellClicked.ChangeType(CellType.Empty);
            cellClicked.RemoveNeighborsMine();
            if (GameManager.Instance.currentRoomSettings.haveStair)
            {
                GameManager.Instance.gridManager.SetItemsType(CellType.Gate, 1);
            }
            GameManager.Instance.gridManager.SetItemsType(CellType.Item, GameManager.Instance.currentRoomSettings.GetNumberOfPotion(), ItemTypeEnum.Potion);
            GameManager.Instance.gridManager.SetItemsType(CellType.Item, GameManager.Instance.currentRoomSettings.GetNumberOfSword(), ItemTypeEnum.Sword);

        }
        //Explose la mine si sans est une
        if (cellClicked.currentType == CellType.Mine)
        {
            cellClicked.MineExplosion();
        }
        //Augmente le compteur de clic et r�v�le la case
        else
        {
            cellClicked.ChangeState(CellState.Reveal);
        }

        GameManager.Instance.dungeonManager.currentRoom.ChangeRoomSate(RoomState.Started);
        IncreaseClickCount();
    }

    private void ClickOnPlantedSwordCell(Cell cellClicked)
    {
        if (cellClicked.currentType == CellType.Mine)
        {
            cellClicked.MineSwordDestruction(GameManager.CellVisualManager.mineSwordedAnimation);
        }
        else
        {
            cellClicked.ChangeState(CellState.Reveal);
        }
    }

    private void ClickOnGateCell(Cell cellClicked)
    {
        GameManager.Instance.ChangeFloorLevel();
        ResetClickCounter();
    }

    private void ClickOnItemCell(Cell cellClicked, ItemTypeEnum itemType)
    {
        if (itemType == ItemTypeEnum.Potion)
        {
            IncreaseHealth(1);
        }
        else if (itemType == ItemTypeEnum.Sword) 
        {
            IncreaseSwordCounter();
        }
        // Fait disparaitre l'item collect�
        cellClicked.ChangeType(CellType.Empty);
        cellClicked.ChangeItemType(ItemTypeEnum.None);
        cellClicked.UpdateRegardingNeighbors();
    }

    private void SwitchCellsToClickedState()
    {
        _cellClicked = _cellOver;
        
        if (_cellClicked.currentType == CellType.Hint && _cellClicked.currentState == CellState.Reveal)
        {
             foreach (Cell neighborsCell in _cellClicked.neighborsCellList)
             {
                 if (neighborsCell.currentState == CellState.Cover)
                 {
                     neighborsCell.ChangeState(CellState.Clicked);
                 }
             }
        }
    }

    private void ResetClickedState()
    {
        foreach (Cell neighborsCell in _cellClicked.neighborsCellList)
        {
            if (neighborsCell.currentState == CellState.Clicked)
            {
                neighborsCell.ChangeState(CellState.Cover);
            }
        }
    }
    #endregion

    #region HEALTH
    public void ResetHealthPoint()
    {
        _healthPoints = initialHealthPoints;
        healthPointText.text = _healthPoints.ToString();
    }

    public void DecreaseHealth(int damage)
    {
        _healthPoints -= damage;
        healthPointText.text = _healthPoints.ToString();
        if (_healthPoints <= 0)
        {
            GameManager.Instance.ChangeGameState(GameState.Loose);
        }
    }

    public void IncreaseHealth(int heal)
    {
        _healthPoints += heal;
        healthPointText.text = _healthPoints.ToString();
    }
    #endregion

    #region MANA
    public void IncreaseMana(int manaIncrease = 1)
    {
        if (_manaPoints < maxManaPoints)
        {
            _manaPoints += manaIncrease;
            manaPointText.text = _manaPoints.ToString();
        }
    }

    public void DecreaseMana(int manaDecrease)
    {
        if (_manaPoints == 0)
        {
            return;
        }
        _manaPoints -= manaDecrease;
        manaPointText.text += _manaPoints.ToString();
    }

    public bool AsEnoughMana(int manaNecessary)
    {
        bool enoughMana = false;
        if (manaNecessary >= _manaPoints)
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
        _manaPoints = initialManaPoints;
        manaPointText.text= _manaPoints.ToString();
    }
    #endregion

    #region SWORD
    public void IncreaseSwordCounter(int swordIncrease = 1)
    {
        _swordCounter += swordIncrease;
        swordCounterText.text = _swordCounter.ToString();
    }

    public void DecreaseSwordCounter(int swordDecrease = 1)
    {
        if (_swordCounter == 0)
        {
            return;
        }
        _swordCounter -= swordDecrease;
        swordCounterText.text = _swordCounter.ToString();
    }
    public void ResetSwordCounter()
    {
        _swordCounter = initialSwordCounter;
        swordCounterText.text = _swordCounter.ToString();
    }
    #endregion

    #region CLICK COUNTER
    public void ResetClickCounter()
    {
        clicCounter = 0;
    }

    private void IncreaseClickCount()
    {
        clicCounter += 1;
    }
    #endregion

}

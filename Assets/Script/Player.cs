using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

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

    [Header("INFOS")]
    [ReadOnly] public int clicCounter;
    [ReadOnly] public Vector2 mousePosition;
    
    [Serializable]
    public struct IsOverCondition
    {
        public CellState cellState;
        public CellType cellType;
    }
    public List<IsOverCondition> IsOverConditions;

    //Private Variables
    [SerializeField, ReadOnly] private Cell cellOver;
    private Cell _previousCell;
    private Cell _cellClicked;
    private Cell _firstCellClicked;
    private GridManager _gridManager;
    #endregion

    public void Init()
    {
        _gridManager = GameManager.Instance.gridManager;
    }
    private void Update()
    {
        // Convertit la position de la souris en coordonn�es du monde
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Effectue un raycast � la position de la souris
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            cellOver = hit.collider.GetComponent<Cell>();
            if (cellOver != null)
            {
                if (_previousCell != null && _previousCell != cellOver)
                {
                    _previousCell.IsOver(false);
                }
                cellOver.IsOver(true);
                _previousCell = cellOver;
            }
        }
        else
        {
            if (_previousCell != null)
            {
                _previousCell.IsOver(false);
                _previousCell = null;
            }
        }
        if (cellOver == null || GameManager.Instance.currentGameState != GameState.InGame)
        {
            return;
        }

        #region LEFT CLICK
        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            _cellClicked = cellOver;
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
            _gridManager.UpdateMineCounter();
            _gridManager.CheckRoomCompletion();
            ResetClickedState();
        }

        //Clic gauche Down
        if (Input.GetMouseButtonDown(0))
        {
            _firstCellClicked = cellOver;
            SwitchCellsToClickedState();
        }

        //Clic gauche enfonc� (s'update en permanence)
        if (Input.GetMouseButton(0))
        {
            if (_cellClicked != cellOver)
            {
                ResetClickedState();
                SwitchCellsToClickedState();
            }

        }
        #endregion LEFT CLICK

        #region RIGHT CLICK 
        // Clique sur le bouton droit
        if (Input.GetMouseButtonDown(1))
        {
            if (cellOver.currentState == CellState.Cover)
            {
                cellOver.StateTransitionIn(CellState.Flag, 0.2f);
                //cellOver.ChangeState(CellState.Flag);
            }
            else if (cellOver.currentState == CellState.Flag && _swordCounter >= 1f)
            {
                cellOver.StateTransitionIn(CellState.PlantedSword, 0.35f);
                DecreaseSwordCounter();
            }
            else if (cellOver.currentState == CellState.Flag && _swordCounter == 0f)
            {
                cellOver.ChangeState(CellState.Cover);
            }
            else if (cellOver.currentState == CellState.PlantedSword)
            {
                cellOver.ChangeState(CellState.Cover);
                IncreaseSwordCounter();
            }
            //Update le compteur de mines restantes
            _gridManager.UpdateMineCounter();
            _gridManager.CheckRoomCompletion();
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
        //Vérifie si la grid doit être procédurale et est complètement couverte puis génère les items
        if (_gridManager.CheckFirstClickOnProcedural())
        {
            _gridManager.FirstClickGeneration(cellClicked);
        }
        //Explose la mine si c'en est une
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

    private void ClickOnPlantedSwordCell(Cell cellClicked)
    {
        //Vérifie si la grid doit être procédurale et est complètement couverte puis génère les items
        if (_gridManager.CheckFirstClickOnProcedural())
        {
            _gridManager.FirstClickGeneration(cellClicked);
        }
        if (cellClicked.currentType == CellType.Mine)
        {
            cellClicked.MineSwordDestruction(GameManager.VisualManager.mineSwordedAnimation);
        }
        else
        {
            cellClicked.ChangeState(CellState.Reveal);
        }
        IncreaseClickCount();
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
        _cellClicked = cellOver;
        
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
            GameManager.Instance.ChangeGameState(GameState.Lose);
        }
    }

    private void IncreaseHealth(int heal)
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
    private void IncreaseSwordCounter(int swordIncrease = 1)
    {
        _swordCounter += swordIncrease;
        swordCounterText.text = _swordCounter.ToString();
    }

    private void DecreaseSwordCounter(int swordDecrease = 1)
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

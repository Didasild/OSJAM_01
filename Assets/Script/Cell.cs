using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CellState
{
    Cover,
    Flag,
    Cliked,
    Reveal
}

public enum CellType
{
    ToDefine,
    Empty,
    Hint,
    Stair,
    Mine
}

public class Cell : MonoBehaviour
{
    [Header("CELL SETTINGS")]

    [Header("CELL INFORMATIONS")]
    public CellState currentState;
    public CellType currentType;
    public List<Cell> neighborsCellList = new List<Cell>(); //Liste des voisins de la cellule
    [NaughtyAttributes.ReadOnly]
    public Vector2Int _cellPosition;

    [Header("CELL VISUALS")]
    public GameObject cellCover;
    public GameObject cellEmpty;
    public GameObject cellMine;
    public GameObject cellFlag;
    public GameObject cellStair;
    public int numberOfMine;
    public TMP_Text numberText;

    #region INIT
    public void Initialize(Grid grid, Vector2Int cellPosition)
    {
        _cellPosition = cellPosition;
        ChangeState(currentState);
    }

    public void InitializeType(Grid grid)
    {
        neighborsCellList = grid.GetNeighbors(_cellPosition);
    }

    //Initialise le visuel de la case
    public void InitalizeVisual()
    {
        numberOfMine = 0;
        foreach (Cell cell in neighborsCellList)
        {
            if (cell.currentType == CellType.Mine)
            {
                numberOfMine += 1;
            }
        }

        if (currentType == CellType.Mine)
        {
            numberText.text = "";
        }
        else if (numberOfMine >= 1)
        {
            numberText.text = numberOfMine.ToString();
            ChangeType(CellType.Hint);
        }
        else
        {
            numberText.text = "";
            ChangeType(CellType.Empty);
        }
    }

    #endregion

    #region CELL STATE
    public void ChangeState(CellState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case CellState.Cover:
                CoverState();
                break;

            case CellState.Flag:
                FlagState();
                break;

            case CellState.Cliked:
                ClickedState();
                break;

            case CellState.Reveal:
                RevealState();
                break;
        }
    }

    private void CoverState()
    {
        Debug.Log("switch to Cover State");
        cellCover.SetActive(true);
        cellFlag.SetActive(false);
    }

    private void FlagState()
    {
        Debug.Log("switch to Flag State");
        cellFlag.SetActive(true);
        cellCover.SetActive(false);
    }

    private void RevealState()
    {
        Debug.Log("switch to Reveal State");
        cellCover.SetActive(false);
        if (currentType == CellType.Empty)
        {
            foreach (Cell cell in neighborsCellList)
            {
                if (cell.currentType == CellType.Empty && cell.currentState == CellState.Cover)
                {
                    cell.ChangeState(CellState.Reveal);
                }
                if (cell.currentType == CellType.Hint && cell.currentState == CellState.Cover)
                {
                    cell.ChangeState(CellState.Reveal);
                }
                if (cell.currentType == CellType.Stair && cell.currentState == CellState.Cover)
                {
                    cell.ChangeState(CellState.Reveal);
                }
            }
        }

        if (currentType == CellType.Mine)
        {
            GameManager.Instance.player.DecreaseHealth(1);
        }
    }

    private void ClickedState()
    {
        Debug.Log("switch to Clicked State");
    }
    #endregion

    #region CELL TYPE
    public void ChangeType(CellType newType)
    {

        currentType = newType;

        switch (currentType)
        {
            case CellType.ToDefine:
                ToDefineType();
                break;

            case CellType.Empty:
                EmptyType();
                break;

            case CellType.Hint:
                HintType();
                break;

            case CellType.Stair:
                StairType();
                break;

            case CellType.Mine:
                MineType();
                break;
        }

    }
    private void ToDefineType()
    {

    }

    private void EmptyType()
    {
        cellEmpty.SetActive(true);
        cellMine.SetActive(false);
        cellStair.SetActive(false);
    }
    private void HintType()
    {
        cellEmpty.SetActive(true);
        cellMine.SetActive(false);
        cellStair.SetActive(false);
    }

    private void StairType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(false);
        cellStair.SetActive(true);       
    }

    private void MineType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(true);
        cellStair.SetActive(false);
    }
    #endregion

    #region NEIGHBORS MANAGEMENT
    public void ChangeNeighborStates()
    {
        int numberOfFlagNeighbors = 0;
        foreach (Cell neighborsCell in neighborsCellList)
        {
            if (neighborsCell.currentState == CellState.Flag)
            {
                numberOfFlagNeighbors += 1;
            }
            else if (neighborsCell.currentType == CellType.Mine && neighborsCell.currentState == CellState.Reveal)
            {
                numberOfFlagNeighbors += 1;
            }
        }

        if (numberOfFlagNeighbors == numberOfMine)
        {
            foreach (Cell neighbor in neighborsCellList)
            {
                if (neighbor.currentState == CellState.Cover)
                {
                    neighbor.ChangeState(CellState.Reveal);
                }
            }
        }
    }

    public void RemoveNeighborsMine()
    {
        foreach (Cell neighbor in neighborsCellList)
        {
            if (neighbor.currentType == CellType.Mine)
            {
                neighbor.ChangeType(CellType.Empty);
            }
        }
        GameManager.Instance.grid.SetCellsVisuals();
        ChangeState(CellState.Reveal);
    }
    #endregion

}

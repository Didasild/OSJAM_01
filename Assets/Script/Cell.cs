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
    public TMP_Text number;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

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
        int numberOfMine = 0;
        foreach (Cell cell in neighborsCellList)
        {
            if (cell.currentType == CellType.Mine)
            {
                numberOfMine += 1;
            }
        }

        if (currentType == CellType.Mine)
        {
            number.text = "";
        }
        else if (numberOfMine >= 1)
        {
            number.text = numberOfMine.ToString();
            ChangeType(CellType.Hint);
        }
        else
        {
            number.text = "";
            ChangeType(CellType.Empty);
        }
    }

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

            case CellType.Mine:
            MineType();
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
            }
        }
    }

    private void ClickedState()
    {
        Debug.Log("switch to Clicked State");
    }

    private void ToDefineType()
    {

    }

    private void EmptyType()
    {
        cellEmpty.SetActive(true);
        cellMine.SetActive(false);
    }
    private void HintType()
    {
        cellEmpty.SetActive(true);
        cellMine.SetActive(false);
    }

    private void MineType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

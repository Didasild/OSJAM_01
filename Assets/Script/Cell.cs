using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CellState
{
    Cover,
    Flag,
    Sword,
    Cliked,
    Reveal
}

public enum CellType
{
    ToDefine,
    Empty,
    Mine,
    Hint,
    Stair,
    Potion,
    Sword
}

public class Cell : MonoBehaviour
{
    [Header("CELL INFORMATIONS")]
    public CellState currentState;
    public CellType currentType;
    [NaughtyAttributes.ReadOnly]
    public Vector2Int _cellPosition;
    public List<Cell> neighborsCellList = new List<Cell>(); //Liste des voisins de la cellule
    [NaughtyAttributes.ReadOnly]
    public int numberOfNeighborsMine;
    public TMP_Text numberText;



    [Header("CELL VISUALS STATE")]
    public GameObject cellCover;
    public GameObject cellFlag;
    public GameObject cellSword;
    [Header("CELL VISUALS TYPE")]
    public GameObject cellEmpty;
    public GameObject cellMine;
    public GameObject cellStair;
    public GameObject cellItemPotion;
    public GameObject cellItemSword;
    [Header("CELL VISUALS MINE")]
    public GameObject mineSwordAnim;



    #region INIT
    public void Initialize(Grid grid, Vector2Int cellPosition)
    {
        _cellPosition = cellPosition;
        ChangeState(currentState);
    }

    //Initialise le visuel de la case
    public void InitalizeVisual()
    {
        numberOfNeighborsMine = 0;
        foreach (Cell cell in neighborsCellList)
        {
            if (cell.currentType == CellType.Mine)
            {
                numberOfNeighborsMine += 1;
            }
        }

        if (currentType == CellType.Mine)
        {
            numberText.text = "";
        }
        else if (numberOfNeighborsMine >= 1)
        {
            numberText.text = numberOfNeighborsMine.ToString();
            ChangeType(CellType.Hint);
        }
        else
        {
            numberText.text = "";
            ChangeType(CellType.Empty);
        }
    }

    #endregion
    public void GenerateNeighborsList(Grid grid)
    {
        neighborsCellList = grid.GetNeighbors(_cellPosition);
    }

    public void DestroyCellType()
    {
        ChangeState(CellState.Cover);
        StartCoroutine(CO_MineAnimation());
    }
    private IEnumerator CO_MineAnimation()
    {
        mineSwordAnim.SetActive(true);
        ChangeType(CellType.Empty);
        yield return new WaitForSeconds(2f);
        mineSwordAnim.SetActive(false);
        InitalizeVisual();
        foreach (Cell cellInList in neighborsCellList)
        {
            cellInList.InitalizeVisual();
        }
        foreach (Cell cellInList in neighborsCellList)
        {
            if (cellInList.currentType == CellType.Empty)
            {
                cellInList.ChangeState(CellState.Reveal);
            }
        }
        ChangeState(CellState.Reveal);
    }


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

            case CellState.Sword:
                SwordState();
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
        cellSword.SetActive(false);
    }

    private void FlagState()
    {
        Debug.Log("switch to Flag State");
        cellFlag.SetActive(true);
        cellCover.SetActive(false);
        cellSword.SetActive(false);
    }

    private void SwordState()
    {
        Debug.Log("switch to Sword State");
        cellFlag.SetActive(false);
        cellCover.SetActive(false);
        cellSword.SetActive(true);
    }

    private void RevealState()
    {
        Debug.Log("switch to Reveal State");
        cellCover.SetActive(false);
        if (currentType == CellType.Empty || currentType == CellType.Stair || currentType == CellType.Sword || currentType == CellType.Potion)
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
                if (cell.currentType == CellType.Sword && cell.currentState == CellState.Cover)
                {
                    cell.ChangeState(CellState.Reveal);
                }
                if (cell.currentType == CellType.Potion && cell.currentState == CellState.Cover)
                {
                    cell.ChangeState(CellState.Reveal);
                }
            }
        }

        if (currentType == CellType.Mine)
        {
            GameManager.Instance.player.DecreaseHealth(1);
        }
        Debug.Log("switch to Sword State");
        cellFlag.SetActive(false);
        cellCover.SetActive(false);
        cellSword.SetActive(false);
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

            case CellType.Mine:
                MineType();
                break;

            case CellType.Hint:
                HintType();
                break;

            case CellType.Stair:
                StairType();
                break;

            case CellType.Potion:
                PotionType();
                break;

            case CellType.Sword:
                SwordType();
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
        cellItemPotion.SetActive(false);
        cellItemSword.SetActive(false);
    }
    private void MineType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(true);
        cellStair.SetActive(false);
        cellItemPotion.SetActive(false);
        cellItemSword.SetActive(false);
    }
    private void HintType()
    {
        cellEmpty.SetActive(true);
        cellMine.SetActive(false);
        cellStair.SetActive(false);
        cellItemPotion.SetActive(false);
        cellItemSword.SetActive(false);
    }

    private void StairType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(false);
        cellStair.SetActive(true);
        cellItemPotion.SetActive(false);
        cellItemSword.SetActive(false);
    }

    private void PotionType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(false);
        cellStair.SetActive(false);
        cellItemPotion.SetActive(true);
        cellItemSword.SetActive(false);
    }
    private void SwordType()
    {
        cellEmpty.SetActive(false);
        cellMine.SetActive(false);
        cellStair.SetActive(false);
        cellItemPotion.SetActive(false);
        cellItemSword.SetActive(true);
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

        if (numberOfFlagNeighbors == numberOfNeighborsMine)
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

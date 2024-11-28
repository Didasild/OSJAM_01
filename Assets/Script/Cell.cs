using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#region ENUMS
public enum CellState
{
    Cover,
    Clicked,
    Flag,
    PlantedSword,
    Reveal,
    None
}

public enum CellType
{
    Empty,
    Mine,
    Hint,
    Gate,
    NPC,
    Item,
}

public enum ItemTypeEnum
{
    None,
    Potion,
    Sword,
    Coin,
}
#endregion

public class Cell : MonoBehaviour
{
    [Header("CELL INFORMATIONS")]
    public CellState currentState;
    public CellType currentType;
    public ItemTypeEnum currentItemType;
    [NaughtyAttributes.ReadOnly]
    public Vector2Int _cellPosition;
    public List<Cell> neighborsCellList = new List<Cell>(); //Liste des voisins de la cellule
    [NaughtyAttributes.ReadOnly]
    public int numberOfNeighborsMine;
    public TMP_Text numberText;

    [Header("CELL VISUALS STATE")]
    public GameObject cellCover;
    public GameObject cellClicked;
    public GameObject cellFlag;
    public GameObject cellSword;

    [Header("CELL VISUALS TYPE")]
    public GameObject cellEmpty;
    public GameObject cellMine;
    public SpriteRenderer itemVisual;
    public GameObject cellStair;

    [Header("CELL ANIMS STATE")]
    public GameObject animMineExplosion;
    public GameObject animSwordOnMine;
    public GameObject animPlantedSword;

    #region INIT
    public void Initialize(Grid grid, Vector2Int cellPosition)
    {
        _cellPosition = cellPosition;
        ChangeState(currentState);
    }

    //Initialise le visuel de la case
    public void UpdateRegardingNeighbors()
    {
        numberOfNeighborsMine = 0;
        foreach (Cell cell in neighborsCellList)
        {
            if (cell.currentType == CellType.Mine)
            {
                numberOfNeighborsMine += 1;
            }
        }

        if (currentType == CellType.Mine || currentType == CellType.Item || currentType == CellType.Gate)
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

    #region CELL STATE
    public void ChangeState(CellState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case CellState.Cover:
                CoverState();
                break;

            case CellState.Clicked:
                ClickedState();
                break;

            case CellState.Reveal:
                RevealState();
                break;

            case CellState.Flag:
                FlagState();
                break;

            case CellState.PlantedSword:
                SwordState();
                break;
        }
    }

    private void CoverState()
    {
        //Debug.Log(this.name + " switch to Cover State");
        cellClicked.SetActive(false);
        cellFlag.SetActive(false);
        cellSword.SetActive(false);
        cellCover.SetActive(true);
    }

    private void ClickedState()
    {
        //Debug.Log(this.name + " switch to Clicked State");
        cellClicked.SetActive(true);
        cellSword.SetActive(false);
        cellCover.SetActive(false);
    }

    private void RevealState()
    {
        int nbOfNeighborsMine = GetNeighborsType(CellType.Mine);
        if (nbOfNeighborsMine == 0)
        {
            foreach (Cell cell in neighborsCellList)
            {
                if (cell.currentState == CellState.Cover)
                {
                    cell.ChangeState(CellState.Reveal);
                }
            }
        }
        GameManager.Instance.player.IncreaseMana();
        cellClicked.SetActive(false);
        cellFlag.SetActive(false);
        cellSword.SetActive(false);
        cellCover.SetActive(false);
    }

    private void FlagState()
    {
        //Debug.Log("switch to Flag State");
        cellClicked.SetActive(false);
        cellFlag.SetActive(true);
        cellSword.SetActive(false);
        cellCover.SetActive(false);
    }

    private void SwordState()
    {
        //Debug.Log("switch to Sword State");
        cellClicked.SetActive(false);
        cellFlag.SetActive(false);
        cellSword.SetActive(true);
        cellCover.SetActive(false);
    }
    #endregion

    #region CELL TYPE
    public void ChangeType(CellType newType, ItemTypeEnum itemType = ItemTypeEnum.None, bool updateVisual = true)
    {
        currentType = newType;

        switch (currentType)
        {
            case CellType.Empty:
                EmptyType(updateVisual);
                break;

            case CellType.Mine:
                MineType();
                break;

            case CellType.Hint:
                HintType();
                break;

            case CellType.Gate:
                GateType();
                break;

            case CellType.Item:
                ItemType();
                break;
        }
    }

    private void EmptyType(bool updateVisual = true)
    {
        if (updateVisual == true)
        {
            cellEmpty.SetActive(true);
            //cellMine.SetActive(false);
            cellStair.SetActive(false);
        }
        else
        {
            return;
        }
    }
    private void MineType()
    {
        cellEmpty.SetActive(false);
        cellStair.SetActive(false);
    }
    private void HintType()
    {
        cellEmpty.SetActive(true);
        cellStair.SetActive(false);
    }

    private void GateType()
    {
        cellEmpty.SetActive(false);
        cellStair.SetActive(true);
    }

    private void ItemType()
    {
        cellEmpty.SetActive(true);
        cellStair.SetActive(false);
    }
    #endregion

    #region ITEM TYPE
    public void ChangeItemType(ItemTypeEnum newType)
    {
        currentItemType = newType;

        switch (currentItemType)
        {
            case ItemTypeEnum.None:
                NoneItemType();
                break;

            case ItemTypeEnum.Potion:
                PotionType();
                break;

            case ItemTypeEnum.Sword:
                SwordType();
                break;

            case ItemTypeEnum.Coin:
                
                break;
        }
    }
    public void NoneItemType()
    {
        itemVisual.sprite = GameManager.CellVisualManager.UpdateItemVisuel(currentItemType);
    }

    public void PotionType()
    {
        itemVisual.sprite = GameManager.CellVisualManager.UpdateItemVisuel(currentItemType);
    }

    public void SwordType()
    {
        itemVisual.sprite = GameManager.CellVisualManager.UpdateItemVisuel(currentItemType);
    }

    #endregion

    #region CELL MODIFICATIONS METHODS
        public void MineSwordDestruction(GameObject mineAnimType)
    {
        ChangeState(CellState.Cover);
        StartCoroutine(CO_MineDestruction(mineAnimType, 1.9f));
    }
    public void MineExplosion()
    {
        GameManager.Instance.player.DecreaseHealth(1);
        StartCoroutine(CO_MineDestruction(animMineExplosion, 1.4f));
    }
    private IEnumerator CO_MineDestruction(GameObject mineAnimType, float animDuration)
    {
        mineAnimType.SetActive(true);
        ChangeType(CellType.Empty);
        yield return new WaitForSeconds(animDuration);
        mineAnimType.SetActive(false);
        UpdateRegardingNeighbors();
        foreach (Cell cellInList in neighborsCellList)
        {
            cellInList.UpdateRegardingNeighbors();
            if (cellInList.currentType == CellType.Empty)
            {
                cellInList.ChangeState(CellState.Reveal);
            }
        }
        ChangeState(CellState.Reveal);
    }

    public void ItemStatetransition(CellState cellNewState, float animDuration)
    {
        ChangeState(CellState.Cover);
        StartCoroutine(CO_StateTransitionAnim(cellNewState, animDuration));
    }
    private IEnumerator CO_StateTransitionAnim(CellState cellNewState, float animDuration)
    {
        if (cellNewState == CellState.PlantedSword)
        {
            animPlantedSword.SetActive(true);
        }
        yield return new WaitForSeconds(animDuration);
        if (cellNewState == CellState.PlantedSword)
        {
            animPlantedSword.SetActive(false);
            ChangeState(CellState.PlantedSword);
        }

    }
    #endregion

    #region NEIGHBORS MANAGEMENT
    public void GenerateNeighborsList(Grid grid)
    {
        neighborsCellList = grid.GetNeighbors(_cellPosition);
    }

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

    public int GetNeighborsType(CellType typeToGet)
    {
        int numberOfType = 0;
        foreach (Cell neighborsCell in neighborsCellList)
        {
            if (neighborsCell.currentType == typeToGet)
            {
                numberOfType += 1;
            }
        }
        return numberOfType;
    }
    public int GetNeighborsState(CellState stateToGet)
    {
        int numberOfState = 0;
        foreach (Cell neighborsCell in neighborsCellList)
        {
            if (neighborsCell.currentState == stateToGet)
            {
                numberOfState += 1;
            }
        }
        return numberOfState;
    }
    #endregion

}

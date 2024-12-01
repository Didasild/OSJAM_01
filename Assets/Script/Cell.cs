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

    [Header("CELL BASE VISUAL")]
    public GameObject cellEmpty;
    public GameObject cellCover;

    [Header("CELL ADDITIONAL VISUAL")]
    public SpriteRenderer stateVisual;
    public SpriteRenderer typeVisual;
    public SpriteRenderer itemVisual;

    [Header("CELL ANIMS STATE")]
    public GameObject animParent;
    public GameObject animMineExplosion;
    public GameObject animSwordOnMine;
    public GameObject animPlantedSword;

    #region INIT
    public void Initialize(GridManager grid, Vector2Int cellPosition)
    {
        _cellPosition = cellPosition;
        ChangeState(currentState);
    }

    //Update le visual de la cellule
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
                SwordPlantedState();
                break;
        }
    }

    private void CoverState()
    {
        //Debug.Log(this.name + " switch to Cover State");
        stateVisual.sprite = GameManager.CellVisualManager.GetStateVisual(currentState);
        cellCover.SetActive(true);
    }

    private void ClickedState()
    {
        //Debug.Log(this.name + " switch to Clicked State");
        stateVisual.sprite = GameManager.CellVisualManager.GetStateVisual(currentState);
    }

    private void RevealState()
    {
        //Reveal les cellules autour si il n'y a pas de mines
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

        //Augmente le mana a chaque case reveal
        GameManager.Instance.player.IncreaseMana();

        //Update Visual
        cellCover.SetActive(false);
        stateVisual.sprite = GameManager.CellVisualManager.GetStateVisual(currentState);
    }

    private void FlagState()
    {
        //Debug.Log("switch to Flag State");
        stateVisual.sprite = GameManager.CellVisualManager.GetStateVisual(currentState);
    }

    private void SwordPlantedState()
    {
        //Debug.Log("switch to Sword State");
        stateVisual.sprite = GameManager.CellVisualManager.GetStateVisual(currentState);
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
            typeVisual.sprite = GameManager.CellVisualManager.GetTypeVisual(currentType);
        }
        else
        {
            return;
        }
    }
    private void MineType()
    {
        typeVisual.sprite = GameManager.CellVisualManager.GetTypeVisual(currentType);
    }
    private void HintType()
    {
        cellEmpty.SetActive(true);
        typeVisual.sprite = GameManager.CellVisualManager.GetTypeVisual(currentType);
    }

    private void GateType()
    {
        cellEmpty.SetActive(false);
        typeVisual.sprite = GameManager.CellVisualManager.GetTypeVisual(currentType);
    }

    private void ItemType()
    {
        cellEmpty.SetActive(true);
        typeVisual.sprite = GameManager.CellVisualManager.GetTypeVisual(currentType);
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
        itemVisual.sprite = GameManager.CellVisualManager.GetItemVisuel(currentItemType);
    }

    public void PotionType()
    {
        itemVisual.sprite = GameManager.CellVisualManager.GetItemVisuel(currentItemType);
    }

    public void SwordType()
    {
        itemVisual.sprite = GameManager.CellVisualManager.GetItemVisuel(currentItemType);
    }

    #endregion

    #region ANIMATIONS
    public GameObject InstatiateAnimation(GameObject animPrefab)
    {
        {
            if (animPrefab == null)
            {
                Debug.LogError("Prefab ou parent est null !");
                return null;
            }

            // Instancie le prefab
            GameObject instance = Instantiate(animPrefab, animParent.transform);

            // Optionnel : R�initialise la position locale
            instance.transform.localPosition = Vector3.zero;

            // Optionnel : R�initialise l'�chelle locale
            instance.transform.localScale = Vector3.one;

            return instance;
        }
    }

    public void DestroyAnimationPrefab()
    {
        // V�rifie si le GameObject a des enfants
        if (animParent.transform.childCount > 0)
        {
            // Parcours tous les enfants
            for (int i = animParent.transform.childCount - 1; i >= 0; i--)
            {
                // Supprime chaque enfant
                Destroy(animParent.transform.GetChild(i).gameObject);
            }
        }
    }

    #endregion

    #region CELL MODIFICATIONS METHODS
    public void MineSwordDestruction(GameObject mineAnimType)
    {
        ChangeState(CellState.Cover);
        StartCoroutine(CO_MineDestruction(GameManager.CellVisualManager.mineSwordedAnimation, 1.9f));
    }
    public void MineExplosion()
    {
        GameManager.Instance.player.DecreaseHealth(1);
        StartCoroutine(CO_MineDestruction(GameManager.CellVisualManager.mineExplosionAnimation, 1.4f));
    }
    private IEnumerator CO_MineDestruction(GameObject mineAnimType, float animDuration)
    {
        InstatiateAnimation(mineAnimType);
        //mineAnimType.SetActive(true);
        ChangeType(CellType.Empty);
        yield return new WaitForSeconds(animDuration);
        //mineAnimType.SetActive(false);
        DestroyAnimationPrefab();
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
            InstatiateAnimation(GameManager.CellVisualManager.plantedSwordAnimation);
        }
        yield return new WaitForSeconds(animDuration);
        if (cellNewState == CellState.PlantedSword)
        {
            DestroyAnimationPrefab();
            ChangeState(CellState.PlantedSword);
        }

    }
    #endregion

    #region NEIGHBORS MANAGEMENT
    public void GenerateNeighborsList(GridManager gridManager)
    {
        neighborsCellList = gridManager.GetNeighbors(_cellPosition);
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
        GameManager.Instance.gridManager.SetCellsVisuals();
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

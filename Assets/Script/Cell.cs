using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#region ENUMS
public enum CellState
{
    Inactive,
    Cover,
    Reveal,
    Clicked,
    Flag,
    PlantedSword,
}

public enum CellType
{
    Empty,
    Mine,
    Hint,
    Gate,
    Item,
    None,
    Npc,
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
    #region PARAMETERS
    [Header("CELL INFORMATIONS")]
    [NaughtyAttributes.ReadOnly] public CellState currentState;
    [NaughtyAttributes.ReadOnly] public CellType currentType;
    [NaughtyAttributes.ReadOnly] public ItemTypeEnum currentItemType;
    [NaughtyAttributes.ReadOnly] public Vector2Int _cellPosition;
    [NaughtyAttributes.ReadOnly] public List<Cell> neighborsCellList = new List<Cell>(); //Liste des voisins de la cellule
    [NaughtyAttributes.ReadOnly] public int numberOfNeighborsMine;
    
    [Header("CELL BASE VISUAL")]
    public GameObject cellEmpty;
    public GameObject cellCover;
    public TMP_Text numberText;

    [Header("CELL ADDITIONAL VISUAL")]
    public SpriteRenderer stateVisual;
    public SpriteRenderer typeVisual;
    public SpriteRenderer itemVisual;

    [Header("CELL ANIMS STATE")]
    public GameObject animParent;
    
    //Private Variables
    private CellVisualManager _cellVisualManager;
    private Collider2D collider;
    #endregion

    #region INIT
    public void Initialize(Vector2Int cellPosition)
    {
        _cellPosition = cellPosition;
        _cellVisualManager = GameManager.CellVisualManager;
        ChangeState(currentState);
        collider = GetComponent<Collider2D>();
    }

    //Update le visual de la cellule
    public void UpdateRegardingNeighbors(bool haveFoWCell = false)
    {
        if (currentType == CellType.None)
        {
            return;
        }
        
        //Update le type selon les voisins
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
        
        //Update l'état FoW selon les voisins
        if (haveFoWCell)
        {
            int revealNeighbors = 0;
            foreach (Cell cell in neighborsCellList)
            {
                if (cell.currentState == CellState.Reveal && cell.currentType != CellType.None)
                {
                    revealNeighbors += 1;
                    break;
                }
            }
            if (revealNeighbors == 0)
            {
                ChangeState(CellState.Inactive);
            }
        }
        
        //Update le reste du visuel selon le type et l'état
        stateVisual.sprite = _cellVisualManager.GetCellStateVisual(currentState);
        typeVisual.sprite = _cellVisualManager.GetCellTypeVisual(currentType);
        itemVisual.sprite = _cellVisualManager.GetCellItemVisuel(currentItemType);
    }
    #endregion

    #region CELL STATE
    public void ChangeState(CellState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case CellState.Inactive:
                InactiveState();
                break;
            
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

    private void InactiveState()
    {
        stateVisual.sprite = _cellVisualManager.GetCellStateVisual(currentState);
        cellCover.SetActive(true);
    }

    private void CoverState()
    {
        //Debug.Log(this.name + " switch to Cover State");
        stateVisual.sprite = _cellVisualManager.GetCellStateVisual(currentState);
        cellCover.SetActive(true);
    }

    private void ClickedState()
    {
        //Debug.Log(this.name + " switch to Clicked State");
        stateVisual.sprite = _cellVisualManager.GetCellStateVisual(currentState);
    }

    private void RevealState()
    {
        //Optimisable ici je pense plutot que 2 foreach
        foreach (Cell cell in neighborsCellList)
        {
            if (cell.currentState == CellState.Inactive)
            {
                cell.ChangeState(CellState.Cover);
            }
        }
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
        stateVisual.sprite = _cellVisualManager.GetCellStateVisual(currentState);
    }

    private void FlagState()
    {
        //Debug.Log("switch to Flag State");
        stateVisual.sprite = _cellVisualManager.GetCellStateVisual(currentState);
    }

    private void SwordPlantedState()
    {
        //Debug.Log("switch to Sword State");
        stateVisual.sprite = _cellVisualManager.GetCellStateVisual(currentState);
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
            
            case CellType.None:
                NoneType();
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
        if (updateVisual)
        {
            cellEmpty.SetActive(true);
            typeVisual.sprite = _cellVisualManager.GetCellTypeVisual(currentType);
        }
    }
    private void NoneType()
    {
        typeVisual.sprite = null;
        stateVisual.sprite = null;
        itemVisual.sprite = null;
        cellEmpty.SetActive(false);
        cellCover.SetActive(false);
        numberText.text = "";
        Destroy(collider);
    }
    private void MineType()
    {
        typeVisual.sprite = _cellVisualManager.GetCellTypeVisual(currentType);
    }
    private void HintType()
    {
        cellEmpty.SetActive(true);
        typeVisual.sprite = _cellVisualManager.GetCellTypeVisual(currentType);
    }

    private void GateType()
    {
        cellEmpty.SetActive(false);
        typeVisual.sprite = _cellVisualManager.GetCellTypeVisual(currentType);
    }

    private void ItemType()
    {
        cellEmpty.SetActive(true);
        typeVisual.sprite = _cellVisualManager.GetCellTypeVisual(currentType);
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
    private void NoneItemType()
    {
        itemVisual.sprite = _cellVisualManager.GetCellItemVisuel(currentItemType);
    }

    private void PotionType()
    {
        itemVisual.sprite = _cellVisualManager.GetCellItemVisuel(currentItemType);
    }

    private void SwordType()
    {
        itemVisual.sprite = _cellVisualManager.GetCellItemVisuel(currentItemType);
    }

    #endregion

    #region ANIMATIONS
    private void InstantiateAnimation(GameObject animPrefab)
    {
        {
            if (animPrefab == null)
            {
                Debug.LogError("Prefab ou parent est null !");
            }

            // Instancie le prefab
            GameObject instance = Instantiate(animPrefab, animParent.transform);

            // Optionnel : Réinitialise la position locale
            instance.transform.localPosition = Vector3.zero;

            // Optionnel : Réinitialise l'échelle locale
            instance.transform.localScale = Vector3.one;
        }
    }

    private void DestroyAnimationPrefab()
    {
        // Vérifie si le GameObject a des enfants
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
        InstantiateAnimation(mineAnimType);
        ChangeType(CellType.Empty);
        yield return new WaitForSeconds(animDuration);
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

    public void ItemStateTransition(CellState cellNewState, float animDuration)
    {
        ChangeState(CellState.Cover);
        StartCoroutine(CO_StateTransitionAnim(cellNewState, animDuration));
    }
    private IEnumerator CO_StateTransitionAnim(CellState cellNewState, float animDuration)
    {
        if (cellNewState == CellState.PlantedSword)
        {
            InstantiateAnimation(GameManager.CellVisualManager.plantedSwordAnimation);
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

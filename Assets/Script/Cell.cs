using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
    
#region ENUMS
public enum CellState
{
    Cover,
    Reveal,
    Inactive,
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
    [ReadOnly] public CellState currentState;
    [ReadOnly] public CellType currentType;
    [ReadOnly] public ItemTypeEnum currentItemType;
    [ReadOnly] public Vector2Int _cellPosition;
    [ReadOnly] public List<Cell> neighborsCellList = new List<Cell>(); //Liste des voisins de la cellule
    [ReadOnly] public int numberOfNeighborsMine;
    
    [Header("CELL BASE VISUAL")]
    public GameObject cellEmpty;
    public GameObject cellCover;
    public GameObject cellOutline;

    [Header("CELL ADDITIONAL VISUAL")] 
    public GameObject visualParent;
    public SpriteRenderer stateVisual;
    public SpriteRenderer typeVisual;
    public SpriteRenderer itemVisual;
    public SpriteRenderer numberVisual;

    [Header("CELL ANIMS STATE")]
    public GameObject animParent;
    public float debugAnimDuration = 0.5f;
    
    //Private Variables
    private GameManager _gameManager;

    private Collider2D _collider;
    
    private VisualManager _visualManager;
    private SpriteRenderer _emptySprite;
    private SpriteRenderer _outlineSprite;
    #endregion

    #region INIT
    public void Initialize(Vector2Int cellPosition)
    {
        _gameManager = GameManager.Instance;
        _visualManager = GameManager.VisualManager;
        
        _collider = GetComponent<Collider2D>();
        
        _cellPosition = cellPosition;
        
        ChangeState(currentState);

        gameObject.SetActive(false);
    }
    
    //Update le visual de la cellule
    public void UpdateRegardingNeighbors(bool haveFoWCell = false)
    {
        if (currentType == CellType.None || currentState == CellState.Inactive)
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
            numberVisual.sprite = null;
        }
        else if (numberOfNeighborsMine >= 1)
        {
            numberVisual.sprite = _visualManager.GetSprite(numberOfNeighborsMine.ToString());
            
            ChangeType(CellType.Hint);
        }
        else
        {
            numberVisual.sprite = null;
            
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

        if (currentState == CellState.Reveal)
        {
            cellCover.SetActive(false);
        }
        
        //Update le reste du visuel selon le type et l'état
        stateVisual.sprite = _visualManager.GetCellStateVisual(currentState);
        typeVisual.sprite = _visualManager.GetCellTypeVisual(currentType);
        itemVisual.sprite = _visualManager.GetCellItemVisuel(currentItemType);
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
            
            case CellState.Inactive:
                InactiveState();
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
        stateVisual.sprite = _visualManager.GetCellStateVisual(currentState);
        cellCover.SetActive(false);
        cellEmpty.SetActive(false);
        cellOutline.SetActive(false);
        visualParent.SetActive(false);
    }

    private void CoverState()
    {
        //Debug.Log(this.name + " switch to Cover State");
        stateVisual.sprite = _visualManager.GetCellStateVisual(currentState);
        cellCover.SetActive(true);
        cellOutline.SetActive(true);
        visualParent.SetActive(true);
    }

    private void ClickedState()
    {
        //Debug.Log(this.name + " switch to Clicked State");
        visualParent.SetActive(true);
        stateVisual.sprite = _visualManager.GetCellStateVisual(currentState);
    }

    #region REVEAL
    private void RevealState()
    {
        //reactive le parent
        visualParent.SetActive(true);
        cellEmpty.SetActive(true);
        cellOutline.SetActive(true);
        
        //Optimisable ici, je pense plutôt que 2 foreach
        foreach (Cell cell in neighborsCellList)
        {
            if (cell.currentState == CellState.Inactive && cell.currentType != CellType.None)
            {
                cell.ChangeState(CellState.Cover);
            }
        }

        //Reveal les cellules autour si il n'y a pas de mines
        int nbOfNeighborsMine = GetNeighborsType(CellType.Mine);
        if (nbOfNeighborsMine == 0)
        {
            RevealNeighbors();
        }
        
        RevealAndDisableCover();
        stateVisual.sprite = _visualManager.GetCellStateVisual(currentState);
        
        //Augmente le mana a chaque case reveal
        GameManager.Instance.player.IncreaseMana();
    }

    private void RevealNeighbors()
    {
        Sequence sequence = DOTween.Sequence(); // Crée une séquence DOTween
        float delayBetweenCells = 0.05f; // Temps entre chaque reveal

        foreach (Cell cell in neighborsCellList)
        {
            if (cell.currentState == CellState.Cover)
            {
                sequence.AppendInterval(delayBetweenCells) // Ajoute un délai avant chaque animation
                    .AppendCallback(() => cell.ChangeState(CellState.Reveal));
            }
        }
    }
    
    private void RevealAndDisableCover()
    {
        if (cellCover.transform != null && cellCover.activeSelf)
        {
            cellCover.transform.DOKill();
            cellCover.transform.localScale = Vector3.one;
            cellCover.transform.DOScale(0f, 0.5f * 0.3f) // Bump rapide
                .SetEase(Ease.InBack)
                .OnComplete(() => cellCover.SetActive(false));
        }
    }
    #endregion REVEAL


    private void FlagState()
    {
        //Debug.Log("switch to Flag State");
        stateVisual.sprite = _visualManager.GetCellStateVisual(currentState);
        visualParent.SetActive(true);
    }

    private void SwordPlantedState()
    {
        //Debug.Log("switch to Sword State");
        stateVisual.sprite = _visualManager.GetCellStateVisual(currentState);
        visualParent.SetActive(true);
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
            typeVisual.sprite = _visualManager.GetCellTypeVisual(currentType);
        }
    }
    private void NoneType()
    {
        typeVisual.sprite = null;
        stateVisual.sprite = null;
        itemVisual.sprite = null;
        numberVisual.sprite = null;
        
        cellEmpty.SetActive(false);
        cellCover.SetActive(false);
        
        //ATTENTION QUAND JE FERAIS LE POOL IL FAUT CHANGER CA
        Destroy(_collider);
    }
    private void MineType()
    {
        typeVisual.sprite = _visualManager.GetCellTypeVisual(currentType);
    }
    private void HintType()
    {
        cellEmpty.SetActive(true);
        typeVisual.sprite = _visualManager.GetCellTypeVisual(currentType);
    }

    private void GateType()
    {
        cellEmpty.SetActive(false);
        typeVisual.sprite = _visualManager.GetCellTypeVisual(currentType);
    }

    private void ItemType()
    {
        cellEmpty.SetActive(true);
        typeVisual.sprite = _visualManager.GetCellTypeVisual(currentType);
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
        itemVisual.sprite = _visualManager.GetCellItemVisuel(currentItemType);
    }

    private void PotionType()
    {
        itemVisual.sprite = _visualManager.GetCellItemVisuel(currentItemType);
    }

    private void SwordType()
    {
        itemVisual.sprite = _visualManager.GetCellItemVisuel(currentItemType);
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

    public void SpawnAnimation()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, debugAnimDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => _collider.enabled = true);
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
    public IEnumerator CO_DeactiveCoverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cellCover.SetActive(false);
    }
    #endregion

    #region CELL MODIFICATIONS METHODS
    public void MineSwordDestruction(GameObject mineAnimType)
    {
        ChangeState(CellState.Cover);
        StartCoroutine(CO_MineDestruction(_visualManager.mineSwordedAnimation, 1.9f));
    }
    public void MineExplosion()
    {
        GameManager.Instance.player.DecreaseHealth(1);
        StartCoroutine(CO_MineDestruction(_visualManager.mineExplosionAnimation, 1.4f));
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
            InstantiateAnimation(_visualManager.plantedSwordAnimation);
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

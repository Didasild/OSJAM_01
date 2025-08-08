using System;
using UnityEngine;

public class CellVisual : MonoBehaviour
{
    #region FIELDS
    private VisualManager _visualManager;
    #endregion
    
    #region INIT
    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        
    }
    #endregion INIT
    
    #region METHODS
    public Sprite GetCellTypeVisual(Cell cell)
    {
        CellType cellType = cell.currentType;
        Sprite cellTypeVisual = null;
        if (cellType == CellType.Gate)
        {
            cellTypeVisual = _visualManager.GetSprite("Cell_Type_Stair");
        }
        else if (cellType == CellType.Npc)
        {
            cellTypeVisual = GetNpcStateVisual(cell.npc._currentNpcState);
        }
        return cellTypeVisual;
    }

    public Sprite GetNpcStateVisual(DialogUtils.NPCState npcState)
    {
        Sprite cellTypeVisual = null;
        if (npcState == DialogUtils.NPCState.Active)
        {
            cellTypeVisual = _visualManager.GetSprite("Cell_Type_Npc_Active");
        }
        else if (npcState == DialogUtils.NPCState.Inactive)
        {
            cellTypeVisual = _visualManager.GetSprite("Cell_Type_Npc_Inactive");
        }
        return cellTypeVisual;
    }

    public Sprite GetCellStateVisual(CellState cellState)
    {
        Sprite cellStateVisual = null;
        switch (cellState)
        {
            case CellState.Reveal:
            case CellState.Cover:
                return null;
            case CellState.Inactive:
                cellStateVisual = _visualManager.GetSprite("Cell_None");
                break;
            case CellState.Clicked:
                cellStateVisual = _visualManager.GetSprite("Cell_State_Clicked");
                break;
            case CellState.Flag:
                cellStateVisual = _visualManager.GetSprite("Cell_State_Flag");
                break;
            case CellState.PlantedSword:
                cellStateVisual = _visualManager.GetSprite("Cell_State_Sword");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cellState), cellState, null);
        }
        return cellStateVisual;
    }

    public Sprite GetCellItemVisuel(ItemTypeEnum itemType)
    {
        Sprite spriteItemVisual = null;
        if (itemType == ItemTypeEnum.None)
        {
            return null;
        }
        else if (itemType == ItemTypeEnum.Potion)
        {
            spriteItemVisual = _visualManager.GetSprite("Cell_Item_Potion");
        }
        else if (itemType == ItemTypeEnum.Sword)
        {
            spriteItemVisual = _visualManager.GetSprite("Cell_Item_Sword");
        }
        return spriteItemVisual;
    }

    public Sprite GetHintVisual()
    {
        return null;
    }
    #endregion
}

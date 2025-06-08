using System.Collections.Generic;
using UnityEngine;

public class RoomCompletion
{
    [System.Flags]
    public enum RoomCompletionConditions
    {
        Default = 0,
        FlaggedAllMine = 1 << 0,         // 1
        NoActiveNpc = 1 << 1,            // 2
        ToNameTestCondition = 1 << 2,    // 4
    }
    
    private List<Cell> npcList;
    
    private GridManager _gridManager;
    
    public void Init(GridManager gridManager)
    {
        _gridManager = gridManager;
        npcList = new List<Cell>();
    }
    
    public void CheckRoomCompletion(RoomCompletionConditions roomConditions)
    {
        if (GameManager.Instance.FloorManager.currentRoom.currentRoomState == RoomState.Complete)
        {
            return;
        }

        //DEFAULT CONDITION DEFINITION
        if (roomConditions.HasFlag(RoomCompletionConditions.Default) && !FlaggedAllMineCondition())
        {
            return;
        }
        
        if (roomConditions.HasFlag(RoomCompletionConditions.NoActiveNpc) && !NoActiveNpcCondition())
        {
            return;
        }
        if (roomConditions.HasFlag(RoomCompletionConditions.FlaggedAllMine) && !FlaggedAllMineCondition())
        {
            return;
        }
        
        RoomCompleted();
    }
    
    private bool NoActiveNpcCondition()
    {
        if (npcList.Count == 0)
        {
            npcList = _gridManager.GetCellsByType(CellType.Npc);
        }
        foreach (Cell cell in npcList)
        {
            if (cell == null || cell.npc == null)
            {
                return false;
            }
            if (cell.npc._currentNpcState == DialogUtils.NPCState.Active)
            {
                return false;
            }
        }
        npcList = _gridManager.GetCellsByType(CellType.Npc);
        return true;
    }

    private bool FlaggedAllMineCondition()
    {
        if (_gridManager.GetCellsByState(CellState.Flag).Count != _gridManager.GetCellsByType(CellType.Mine).Count || _gridManager.GetCellsByState(CellState.Cover).Count != 0)
        {
            return false;
        }
        foreach (Cell mineCell in _gridManager.GetCellsByType(CellType.Mine))
        {
            if (mineCell.currentState != CellState.Flag)
            {
                return false;
            }
        }
        return true;
    }

    private void RoomCompleted()
    {
        GameManager.Instance.FloorManager.currentRoom.ChangeRoomSate(RoomState.Complete);
        GameManager.visualManager.PlayRoomCompletionFeedbacks();
        npcList.Clear();
    }
    
}

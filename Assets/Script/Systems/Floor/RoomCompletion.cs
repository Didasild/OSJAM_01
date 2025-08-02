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
    private bool rooomFirstTimeUnlocked;
    
    private GridManager _gridManager;
    private FloorManager _floorManager;
    
    public void Init(GridManager gridManager)
    {
        _floorManager = GameManager.Instance.FloorManager;
        _gridManager = gridManager;
        npcList = new List<Cell>();
    }
    
    public void CheckRoomCompletion(RoomCompletionConditions roomConditions, RoomCompletionConditions roomUnlockedConditions)
    {
        if (GameManager.Instance.GridManager.isGeneratingRoom)
        {
            return;
        }
        
        if (GameManager.Instance.FloorManager.currentRoom.currentRoomState == RoomState.Complete)
        {
            return;
        }
                
        if (CheckCondition(roomUnlockedConditions) == false)
        {
            rooomFirstTimeUnlocked = false;
            return;
        }

        if (_floorManager.currentRoom.currentRoomState != RoomState.Complete)
        {
            if (CheckCondition(roomConditions))
            {
                RoomCompleted();
            }
        }
        
        if (rooomFirstTimeUnlocked == false)
        {
            RoomUnlocked();
        }
    }
    
    private bool CheckCondition(RoomCompletionConditions conditions)
    {
        if (conditions == RoomCompletionConditions.Default)
        {
            return FlaggedAllMineCondition();
        }
        
        if (conditions.HasFlag(RoomCompletionConditions.FlaggedAllMine) && !FlaggedAllMineCondition())
        {
            return false;
        }

        if (conditions.HasFlag(RoomCompletionConditions.NoActiveNpc) && !NoActiveNpcCondition())
        {
            return false;
        }
        
        //OTHER CONDITIONS
        
        return true;
    }

    #region CONDITIONDS
    private bool NoActiveNpcCondition()
    {
        npcList = _gridManager.GridInfos.GetCellsByType(CellType.Npc);
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
        return true;
    }

    private bool FlaggedAllMineCondition()
    {
        if (_gridManager.GridInfos.GetCellsByState(CellState.Flag).Count != _gridManager.GridInfos.GetCellsByType(CellType.Mine).Count || _gridManager.GridInfos.GetCellsByState(CellState.Cover).Count != 0)
        {
            return false;
        }
        foreach (Cell mineCell in _gridManager.GridInfos.GetCellsByType(CellType.Mine))
        {
            if (mineCell.currentState != CellState.Flag)
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    private void RoomUnlocked()
    {
        rooomFirstTimeUnlocked = true;
        if (GameManager.Instance.FloorManager.currentRoom.currentRoomState != RoomState.Complete)
        {
            GameManager.Instance.FloorManager.currentRoom.ChangeRoomSate(RoomState.StartedUnlock);
        }

        GameManager.VisualManager.centralFeedbackController.RooUnlockFeedback(GameManager.Instance.FloorManager
            .currentRoom);
    }

    private void RoomCompleted()
    {
        _floorManager.currentRoom.ChangeRoomSate(RoomState.Complete);
        npcList.Clear();
        RoomUnlocked();
    }
    
}

using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    #region FIELDS
    public NpcDialogsSettings NpcDialogsSettings;
    public List<string> currentDialogSequence;
    public DialogUtils.NPCState _currentNpcState;
    public Vector2Int currentPosition;
    private Cell currentCell;
    #endregion FIELDS

    public void Init(NpcDialogsSettings associatedNpcDialogsSettings, Vector2Int npcPosition)
    {
        NpcDialogsSettings = associatedNpcDialogsSettings;
        currentPosition = npcPosition;
        _currentNpcState = NpcDialogsSettings.baseNPCState;
        currentDialogSequence = NpcDialogsSettings.GetDialogSequence(_currentNpcState);
    }
    
    #region STATE
    public void ChangeNpcState(DialogUtils.NPCState newState)
    {
        _currentNpcState = newState;

        switch (_currentNpcState)
        {
            case DialogUtils.NPCState.None:
                NoneNpcState();
                break;
            case DialogUtils.NPCState.Active:
                ActiveNpcState();
                break;
            case DialogUtils.NPCState.Inactive:
                InactiveNpcState();
                break;
            case DialogUtils.NPCState.WaitingForTrigger:
                WaitingForTriggerState();
                break;
        }
        currentDialogSequence = NpcDialogsSettings.GetDialogSequence(_currentNpcState);
        if (currentCell != null)
        {
            SetCellVisual(currentCell);
        }
    }

    private void NoneNpcState()
    {

    }

    private void ActiveNpcState()
    {

    }

    private void InactiveNpcState()
    {
        GameManager.Instance.GridManager.RoomCompletion.CheckRoomCompletion(GameManager.Instance.FloorManager.currentRoom.roomConditions, GameManager.Instance.FloorManager.currentRoom.roomUnlockConditions);
    }

    private void WaitingForTriggerState()
    {
        
    }
    #endregion STATE

    #region CELL
    public void SetCellVisual(Cell associatedCell)
    {
        if (currentCell == null)
        {
            currentCell = associatedCell;
        }
        associatedCell.typeVisual.sprite = GameManager.visualManager.GetNpcStateVisual(_currentNpcState);
    }
    #endregion CELL

    public void UpdateDialogSettings(NpcDialogsSettings npcDialogsSettings)
    {
        Debug.Log("ALLO" + currentDialogSequence);
        NpcDialogsSettings = npcDialogsSettings;
        currentDialogSequence = NpcDialogsSettings.GetDialogSequence(_currentNpcState);

    }
}

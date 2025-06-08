
using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    #region FIELDS
    public NPCSettings npcSettings;
    public List<string> currentDialogSequence;
    public DialogUtils.NPCState _currentNpcState;
    private Cell _currentCell;
    #endregion FIELDS

    public void Init(NPCSettings associatedNpcSettings, Cell associatedCell)
    {
        _currentCell = associatedCell;
        npcSettings = associatedNpcSettings;
        _currentNpcState = npcSettings.baseNPCState;
        currentDialogSequence = npcSettings.GetDialogSequence(_currentNpcState);
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
        currentDialogSequence = npcSettings.GetDialogSequence(_currentNpcState);
        _currentCell.typeVisual.sprite = GameManager.visualManager.GetNpcStateVisual(_currentNpcState);
    }

    private void NoneNpcState()
    {

    }

    private void ActiveNpcState()
    {

    }

    private void InactiveNpcState()
    {
        GameManager.Instance.GridManager.RoomCompletion.CheckRoomCompletion(GameManager.Instance.FloorManager.currentRoom.roomConditions);
    }

    private void WaitingForTriggerState()
    {
        
    }
    #endregion STATE
}

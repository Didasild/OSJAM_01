using System.Collections.Generic;

public class NPC
{
    #region FIELDS
    public NpcDialogsSettings NpcDialogsSettings;
    public List<string> currentDialogSequence;
    public DialogUtils.NPCState _currentNpcState;
    private Cell _currentCell;
    #endregion FIELDS

    public void Init(NpcDialogsSettings associatedNpcDialogsSettings, Cell associatedCell)
    {
        _currentCell = associatedCell;
        NpcDialogsSettings = associatedNpcDialogsSettings;
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


using System.Collections.Generic;

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
            case DialogUtils.NPCState.none:
                NoneNpcState();
                break;
            case DialogUtils.NPCState.active:
                ActiveNpcState();
                break;
            case DialogUtils.NPCState.inactive:
                InactiveNpcState();
                break;
            case DialogUtils.NPCState.waitingForTrigger:
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
        
    }

    private void WaitingForTriggerState()
    {
        
    }
    #endregion STATE
}

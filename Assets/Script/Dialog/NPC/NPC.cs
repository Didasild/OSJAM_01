
using System.Collections.Generic;

public class NPC
{
    #region FIELDS
    public DialogUtils.NPCState currentNPCState;
    public NPCSettings npcSettings;
    public List<string> CurrentDialogSequence;

    #endregion FIELDS

    public void Init(NPCSettings associatedNpcSettings)
    {
        npcSettings = associatedNpcSettings;
        currentNPCState = npcSettings.baseNPCState;
        CurrentDialogSequence = npcSettings.GetDialogSequence(currentNPCState);
    }
    
    #region STATE
    public void ChangeNpcState(DialogUtils.NPCState newState)
    {
        currentNPCState = newState;

        switch (currentNPCState)
        {
            case DialogUtils.NPCState.none:
                NoneNpcState();
                break;
            case DialogUtils.NPCState.active:
                ActiveNpcState();
                break;
            case DialogUtils.NPCState.inactive:
                
                break;
            case DialogUtils.NPCState.waitingForTrigger:
                
                break;
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
        
    }
    #endregion STATE
}

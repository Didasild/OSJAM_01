using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DialogSettings", menuName = "MineCrawler/DialogSettings")]
public class NpcDialogsSettings : ScriptableObject
{
    List<DialogUtils.DialogStates> dialogStates;
    public NpcSettings npcSettings;
    public DialogUtils.NPCState baseNPCState;
    public List<DialogUtils.DialogPullStates> dialogPulls;
    
    private DialogPool _selectedPool;

    public List<string> GetDialogSequence(DialogUtils.NPCState npcState)
    {
        foreach (DialogUtils.DialogPullStates pull in dialogPulls)
        {
            if (pull.RelatedNpcState == npcState)
            {
                return pull.dialogPool.GetSentences();
            }
        }
        Debug.LogWarning($"No DialogPull found for state: {npcState}");
        return null;
    }
}

using System.Collections.Generic;
using Script.Scriptable.NPC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DialogSettings", menuName = "Limits/DialogSettings")]
public class NpcDialogsSettings : ScriptableObject
{
    public string notes;
    List<DialogUtils.DialogStates> dialogStates;
    public NpcSettings npcSettings;
    public DialogUtils.NPCState baseNPCState = DialogUtils.NPCState.Active;
    public List<DialogUtils.DialogPullStates> dialogPools;
    
    private DialogPool _selectedPool;

    public List<string> GetDialogSequence(DialogUtils.NPCState npcState)
    {
        foreach (DialogUtils.DialogPullStates pull in dialogPools)
        {
            if (pull.RelatedNpcState == npcState)
            {
                return pull.dialogPool.GetSentences();
            }
        }
        Debug.Log($"No DialogPool found for state: {npcState}");
        return null;
    }
}

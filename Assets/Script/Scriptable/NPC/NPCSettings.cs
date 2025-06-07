using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NPC", menuName = "MineCrawler/NPC")]
public class NPCSettings : ScriptableObject
{
    List<DialogUtils.DialogStates> dialogStates;
    public string npcName;
    public string npcDescription;
    public DialogUtils.NPCState baseNPCState;
    public List<DialogUtils.DialogPullStates> dialogPulls;
    
    private DialogPull _selectedPull;
    [HideInInspector] public Image npcDialogBoxImage;

    public List<string> GetDialogSequence(DialogUtils.NPCState npcState)
    {
        foreach (DialogUtils.DialogPullStates pull in dialogPulls)
        {
            if (pull.RelatedNpcState == npcState)
            {
                return pull.DialogPull.GetSentences();
            }
        }
        Debug.LogWarning($"No DialogPull found for state: {npcState}");
        return null; // Ou une séquence par défaut si tu préfères
    }
}

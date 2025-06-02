using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NPC", menuName = "MineCrawler/NPC")]
public class NPCSettings : ScriptableObject
{
    List<DialogUtils.DialogStates> dialogStates;
    public string npcName;
    public string npcDescription;
    public DialogSequence selectedSequence;
    public DialogUtils.NPCState baseNPCState;
    
    [HideInInspector] public Image npcDialogBoxImage;
    
    public DialogSequence baseDialogSequence;

    public DialogSequence GetDialogSequence(DialogUtils.NPCState npcState)
    {
        selectedSequence = baseDialogSequence;
        return selectedSequence;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NPC", menuName = "MineCrawler/NPC")]
public class NPCSettings : ScriptableObject
{
    [System.Serializable]
    public struct DialogStates
    {
        NPCState npcState;
        DialogSequence dialogSequence;
    }
    List<DialogStates> dialogStates;
    public enum NPCState
    {
        active,
        inactive,
        waitingForTrigger,
    }
    
    public string npcName;
    public string npcDescription;
    
    [HideInInspector] public Sprite npcGridCustomIcon;
    [HideInInspector] public Image npcDialogBoxImage;
    
    public DialogSequence baseDialogSequence;

    public DialogSequence GetDialogSequence()
    {
        DialogSequence sequence = CreateInstance<DialogSequence>();
        sequence = baseDialogSequence;
        return sequence;
    }
}

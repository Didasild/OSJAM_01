using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NPC", menuName = "MineCrawler/NPC")]
public class NPCSettings : ScriptableObject
{
    public string npcName;
    public string npcDescription;
    
    [HideInInspector] public Sprite npcGridCustomIcon;
    [HideInInspector] public Image npcDialogBoxImage;
    
    public DialogSequence baseDialogSequence;
}

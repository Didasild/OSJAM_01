using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NPC", menuName = "MineCrawler/NPC")]
public class NpcSettings : ScriptableObject
{
    public string npcName;
    public string npcDescription;
    public Sprite npcImage;
}

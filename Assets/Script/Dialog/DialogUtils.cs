using UnityEngine;

public class DialogUtils
{
    //A deplacer un script plus macro
    [System.Serializable]
    public struct DialogStates
    {
        NPCState npcState;
        DialogSequence dialogSequence;
    }
    [System.Serializable]
    public struct NpcData
    {
        public Vector2Int npcPosition;
        public NPCSettings npcSettings;
    }
    public enum NPCState
    {
        none,
        active,
        inactive,
        waitingForTrigger,
    }
}

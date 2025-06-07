using System.Collections.Generic;
using UnityEngine;

public class DialogUtils
{
    //A deplacer un script plus macro
    [System.Serializable]
    public struct DialogStates
    {
        NPCState npcState;
        global::DialogPull _dialogPull;
    }
    
    [System.Serializable]
    public struct NpcData
    {
        public Vector2Int npcPosition;
        public NPCSettings npcSettings;
    }
    
    [System.Serializable]
    public struct DialogPullStates
    {
        public NPCState RelatedNpcState;
        public DialogPull DialogPull;
    }
    public enum NPCState
    {
        None = 0,
        Active = 1,
        Inactive = 2,
        WaitingForTrigger = 3,
    }
    
    
}

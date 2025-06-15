using UnityEngine;

public class DialogUtils
{
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
        public NpcDialogsSettings npcDialogsSettings;
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
    
    public static NpcDialogsSettings GetNpcSettings(Vector2Int cellPosition)
    {
        foreach (NpcData npcData in GameManager.Instance.currentRoomSettings.npcDatas)
        {
            if (npcData.npcPosition == cellPosition)
            {
                return npcData.npcDialogsSettings;
            }
        }
        Debug.LogError("NPC settings not found");
        return null;
    }
}

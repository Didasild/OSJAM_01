using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FloorVisualScriptingUtils : MonoBehaviour
{
    private FloorManager _floorManager;
    private Health _health;
    public PlayableDirector playableDirector;

    public void Init(FloorManager floorManager, Health health)
    {
        _floorManager = floorManager;
        _health = health;
    }
    
    public void VSIncreaseMaxHealth(int increment)
    {
        Debug.Log("IncrementHealth");
        _health.IncreaseMaxHealth(increment);
    }
    
    public RoomData GetRoomDataFromPosition(Vector2Int roomPosition)
    {
        return _floorManager.GetRoomDataFromPosition(roomPosition);
    }

    public NPC GetNpcFromDialogSettings(RoomData roomData, NpcDialogsSettings npcDialogsSettings)
    {
        return roomData.GetNpcFromDialogSettings(npcDialogsSettings);
    }

    public void PlayRTC(TimelineAsset timelineAsset)
    {
        playableDirector.playableAsset = timelineAsset;
        playableDirector.Play();
    }

    public void GoToNextFloor()
    {
        GameManager.Instance.GoToNextFloor();
    }
    
}

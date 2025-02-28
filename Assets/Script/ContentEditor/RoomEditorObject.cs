using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RoomEditorObject : MonoBehaviour
{
    private Vector2Int _roomPosition;
    private SpriteRenderer _spriteRenderer;
    private FloorEditor _floorEditor;
    private RoomSettings _roomSettings;
    private VisualManager _visualManager;
    private RoomState _roomState;

    public void Init(FloorEditor floorEditor, RoomSettings roomSettings, Vector2Int roomPosition, RoomState roomState)
    {
        _floorEditor = floorEditor;
        _visualManager = floorEditor.visualManager;
        _roomSettings = roomSettings;
        _roomPosition = roomPosition;
        _roomState = roomState;
        name = $"Room {roomPosition}";
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        _spriteRenderer.sprite = _visualManager.GetRoomStateVisual(_roomState);
    }
}

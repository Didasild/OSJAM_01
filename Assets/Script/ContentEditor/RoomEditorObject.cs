using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RoomEditorObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private FloorEditor _floorEditor;

    public void Init(FloorEditor floorEditor, Sprite baseSprite)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _floorEditor = floorEditor;
        UpdateVisual(baseSprite);
    }

    public void UpdateVisual(Sprite sprite)
    {
        
    }
    [Button]
    private void addRoomTop()
    {
        
    }
    [Button]
    private void addRoomLeft()
    {
        
    }
    [Button]
    private void addRoomRight()
    {
        
    }
    [Button]
    private void addRoomBot()
    {
        
    }
    
    [Button]
    private void RemoveRoom()
    {
        
    }
}

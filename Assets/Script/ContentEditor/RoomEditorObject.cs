using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RoomEditorObject : MonoBehaviour
{
    public bool isSelected;
    private Vector2Int _roomPosition;
    public RoomState roomState;
    private SpriteRenderer _spriteRenderer;
    private FloorEditor _floorEditor;
    private RoomSettings _roomSettings;
    private VisualManager _visualManager;


    private void OnValidate()
    {
        name = $"Room {_roomPosition}";
        UpdateVisual();
    }
    private void OnEnable()
    {
        // S'abonne à l'événement de changement de sélection
        Selection.selectionChanged += UpdateSelectionState;
    }
    private void OnDisable()
    {
        // Se désabonne de l'événement de changement de sélection
        Selection.selectionChanged -= UpdateSelectionState;
    }
    public void Init(FloorEditor floorEditor, RoomSettings roomSettings, Vector2Int roomPosition, RoomState newRoomState)
    {
        _floorEditor = floorEditor;
        _visualManager = floorEditor.visualManager;
        _roomSettings = roomSettings;
        _roomPosition = roomPosition;
        roomState = newRoomState;
        name = $"Room {roomPosition}";
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (_spriteRenderer == null || _visualManager == null)
        {
            return;
        }
        _spriteRenderer.sprite = _visualManager.GetRoomStateVisual(roomState);
    }
    
    private void UpdateSelectionState()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        isSelected = false;
        foreach (GameObject selected in selectedObjects)
        {
            // Vérifie si l'objet n'est pas nul et correspond au GameObject actuel
            if (selected != null && selected == gameObject)
            {
                isSelected = true;
                _floorEditor.selectedRoomEditorObject = this;
                break; // Sort de la boucle dès que l'objet est trouvé
            }
        }
    }
}

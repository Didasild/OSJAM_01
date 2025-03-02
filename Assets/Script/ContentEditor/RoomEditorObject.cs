using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RoomEditorObject : MonoBehaviour
{
    public bool isSelected;
    public Vector2Int roomPosition;
    public RoomState roomState;
    public RoomSettings roomSettings;
    public GameObject selectedVisual;
    public GameObject isStartRoomVisual;
    private SpriteRenderer _spriteRenderer;
    private FloorEditor _floorEditor;
    private VisualManager _visualManager;

    private void OnValidate()
    {
        name = $"Room {roomPosition}";
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
        this.roomSettings = roomSettings;
        this.roomPosition = roomPosition;
        roomState = newRoomState;
        name = $"Room {roomPosition}";
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
        HideAllDescendants(transform);
    }
    private void HideAllDescendants(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.hideFlags = HideFlags.HideInHierarchy; // Rend l'objet non sélectionnable
            HideAllDescendants(child); // Appel récursif pour les enfants de cet enfant
        }
    }

    private void UpdateVisual()
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
                break;
            }
        }
        _floorEditor.UpdateSelectedVisual();
    }
}

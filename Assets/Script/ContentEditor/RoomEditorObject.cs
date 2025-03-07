using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RoomEditorObject : MonoBehaviour
{
    public bool isStartRoom;
    public Vector2Int roomPosition;
    public RoomState roomState;
    public RoomSettings roomSettings;
    public RoomCompletionCondition roomCondition;
    
    [Header("DEBUG / SETUP")]
    public bool showSetupElements = false;
    [ShowIf("showSetupElements")] public GameObject selectedVisual;
    [ShowIf("showSetupElements")] public GameObject isStartRoomVisual;
    [ShowIf("showSetupElements")] public GameObject noRoomSettingsVisual;
    
    private bool isSelected;
    private SpriteRenderer _spriteRenderer;
    private FloorEditor _floorEditor;
    private VisualManager _visualManager;

    private void OnValidate()
    {
        name = $"Room {roomPosition}";
        UpdatePosition();
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
    public void Init(FloorEditor floorEditor, RoomSettings roomSettings, Vector2Int roomPosition, RoomState newRoomState, bool isStartRoom)
    {
        _floorEditor = floorEditor;
        _visualManager = floorEditor.visualManager;
        this.roomSettings = roomSettings;
        this.roomPosition = roomPosition;
        this.isStartRoom = isStartRoom;
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

    public void UpdatePosition()
    {
        transform.position = new Vector3(roomPosition.x, roomPosition.y, transform.position.z);
    }

    private void UpdateVisual()
    {
        if (_spriteRenderer == null || _visualManager == null)
        {
            return;
        }
        noRoomSettingsVisual.SetActive(roomSettings == null);
        
        _spriteRenderer.sprite = _visualManager.GetRoomStateVisual(roomState);
        
        isStartRoomVisual.SetActive(isStartRoom);
    }
    
    private void UpdateSelectionState()
    {
        _floorEditor.selectedRoomEditorObjects = new List<RoomEditorObject>();
        _floorEditor.selectedObjects = Selection.gameObjects;

        isSelected = false;

        foreach (GameObject selected in _floorEditor.selectedObjects)
        {
            RoomEditorObject roomEditorObject = selected.GetComponent<RoomEditorObject>();

            if (roomEditorObject != null)
            {
                _floorEditor.selectedRoomEditorObjects.Add(roomEditorObject);

                if (selected == this.gameObject)
                {
                    isSelected = true;
                }
            }
        }
        _floorEditor.UpdateSelectedVisual();
    }
}

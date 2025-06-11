#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
[ExecuteInEditMode]
public class FloorEditor : MonoBehaviour
{
    #region VARIABLES
    [Header("____LOAD")]
    public FloorSettings floorToLoad;
    private List<FloorSettings.LoadedRoomData> roomDatasToSave;
    
    [Header("____GENERATION")] 
    public GameObject[] selectedObjects;
    public List<RoomEditorObject> selectedRoomEditorObjects;
    public RoomSettings roomSettingsToLoad;
    public RoomState roomStateToLoad;
    public bool isStartRoom;
    public List<RoomEditorObject> roomEditorObjects = new List<RoomEditorObject>();
    
    [Header("____SAVE")]
    public FloorSettings floorToSave;
    public Chapters chapter;
    public FloorType floorType;
    public int floorID;
    public VolumeProfile floorVolumeProfile;
    
    [Header("____SETUP")]
    public RoomEditor roomEditor;
    public VisualEditorManager visualEditorManager;
    public RoomEditorObject roomEditorObjectPrefab;
    public float roomOffset;

    // PRIVATE
    private string _defaultSaveFolder;
    private string _scriptableName;
    #endregion VARIABLES
    
    #region LOAD FLOOR
    private void LoadFloor(FloorSettings floorToGenerate)
    {
        ClearFloor();
        if (floorToGenerate is null)
        {
            Debug.LogWarning("Need Floor To Load");
            return;
        }
        roomDatasToSave = floorToGenerate.loadedRoomDatas;

        for (int i = 0; i < roomDatasToSave.Count; i++)
        {
            InstantiateRoomFromSettings(roomDatasToSave, i);
        }
        floorToSave = floorToGenerate;
    }
    private void InstantiateRoomFromSettings(List<FloorSettings.LoadedRoomData> roomDatas, int i)
    {
        GenerateRoom(roomDatas[i].roomPosition, InitRoomPosition(roomDatas[i].roomPosition), roomDatas[i].initRoomSettings, roomDatas[i].currentRoomState, roomDatas[i].startRoom);
    }
    #endregion LOAD FLOOR

    #region GENERATION
    public void GenerateFirstRoom()
    {
        ClearFloor();
        GenerateRoom(Vector2Int.zero, Vector3.zero, roomSettingsToLoad, roomStateToLoad, isStartRoom);
    }

    public void GenerateNeighborRoom(string direction)
    {
        Vector2Int roomNeighborOffset = Vector2Int.zero;
        switch (direction)
        {
            case "up":
                roomNeighborOffset = Vector2Int.up;
                break;
            case "down":
                roomNeighborOffset = Vector2Int.down;
                break;
            case "left":
                roomNeighborOffset = Vector2Int.left;
                break;
            case "right":
                roomNeighborOffset = Vector2Int.right;
                break;
        }

        foreach (RoomEditorObject selectedRoomObject in selectedRoomEditorObjects)
        {
            GenerateRoom(selectedRoomObject.roomPosition + roomNeighborOffset, InitRoomPosition(selectedRoomObject.roomPosition + roomNeighborOffset), roomSettingsToLoad, roomStateToLoad, isStartRoom);
        }

        roomSettingsToLoad = null;
    }
    #endregion

    #region SAVE FUNCTIONS
    public void CreateFloorScriptable()
    {
        //Set le nom
        _defaultSaveFolder = "Assets/Resources/Chapters/" + chapter;
        _scriptableName = chapter + "_Floor" + "_L_" + floorType + "_" + floorID.ToString("D2");
        
        //Crée l'instance
        string path = EditorUtility.SaveFilePanelInProject("Save Floor", _scriptableName, "asset", "Message test", _defaultSaveFolder);
        if (string.IsNullOrEmpty(path))
            return;
        FloorSettings newFloorSettings = ScriptableObject.CreateInstance<FloorSettings>();
        
        SetFloorSettings(newFloorSettings);
        
        // Sauvegarde l'instance dans le fichier spécifié
        AssetDatabase.CreateAsset(newFloorSettings, path);
        AssetDatabase.SaveAssets();
        
        // Met l'objet nouvellement créé en surbrillance dans le Project
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newFloorSettings;
        floorToSave = newFloorSettings;
        EditorUtility.SetDirty(newFloorSettings);

        Debug.Log($"ScriptableObject created at {path}");
    }

    public void UpdateExistingFloorScriptable()
    {
        SetFloorSettings(floorToSave);
        EditorUtility.FocusProjectWindow();
        EditorUtility.SetDirty(floorToSave);
    }

    private void SetFloorSettings(FloorSettings floorSettings)
    {
        floorSettings.proceduralFloor = false;
        floorSettings.loadedRoomDatas = new List<FloorSettings.LoadedRoomData>();
        floorSettings.floorBaseVolumeProfile = floorVolumeProfile;

        foreach (RoomEditorObject roomEditorObject in roomEditorObjects)
        {
            FloorSettings.LoadedRoomData newRoomData;
            
            newRoomData.startRoom = roomEditorObject.isStartRoom;
            newRoomData.initRoomSettings = roomEditorObject.roomSettings;
            newRoomData.currentRoomState = roomEditorObject.roomState;
            newRoomData.roomPosition = roomEditorObject.roomPosition;
            newRoomData.roomCondition = roomEditorObject.roomCondition;
            
            floorSettings.loadedRoomDatas.Add(newRoomData);
        }
    }
    #endregion SAVE FUNCTIONS

    #region METHODS
    private void GenerateRoom(Vector2Int roomGridPosition, Vector3 roomWorldPosition, RoomSettings roomSettings, RoomState roomState, bool isStartRoom = false)
    {
        if (!CheckIfPositionIsAvailable(roomGridPosition))
        {
            Debug.LogWarning("Une Room existe déjà a cette position");
            return;
        }
        
        RoomEditorObject roomObject = Instantiate(roomEditorObjectPrefab, roomWorldPosition , Quaternion.identity, transform);
        roomObject.Init(this, roomSettings, roomGridPosition, roomState, isStartRoom);
        roomEditorObjects.Add(roomObject);
        
        selectedRoomEditorObjects = new List<RoomEditorObject>();
        selectedRoomEditorObjects.Add(roomObject);
        
        UpdateSelectedVisual();
    }
    
    private Vector3 InitRoomPosition(Vector2Int roomPosition)
    {
        return new Vector3(roomPosition.x * roomOffset, roomPosition.y * roomOffset, 0);
    }

    public bool CheckIfPositionIsAvailable(Vector2Int roomPosition)
    {
        foreach (RoomEditorObject room in roomEditorObjects) // roomEditorObjects est ta liste
        {
            if (room.roomPosition == roomPosition)
            {
                return false; // La position est déjà prise
            }
        }

        return true; // La position est libre
    }
    
    public void ClearFloor()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        roomEditorObjects = new List<RoomEditorObject>();
        roomDatasToSave = new List<FloorSettings.LoadedRoomData>();
        selectedRoomEditorObjects = new List<RoomEditorObject>();
    }
    
    public void RemoveSelectedRooms()
    {
        foreach (RoomEditorObject selectedRoomObject in selectedRoomEditorObjects)
        {
            roomEditorObjects.Remove(selectedRoomObject);
            DestroyImmediate(selectedRoomObject.gameObject);
            selectedRoomEditorObjects = new List<RoomEditorObject>();
        }
    }

    public void MoveSelectedRooms(Vector2Int roomDirection)
    {
        foreach (RoomEditorObject selectedRoomEditorObject in selectedRoomEditorObjects)
        {
            if (CheckIfPositionIsAvailable(selectedRoomEditorObject.roomPosition + roomDirection))
            {
                selectedRoomEditorObject.roomPosition += roomDirection;
                selectedRoomEditorObject.UpdatePosition();
            }
            else
            {
                Debug.LogWarning("Can't move on an existing room");
            }
        }
    }
    
    public void UpdateSelectedVisual()
    {
        foreach (RoomEditorObject roomEditorObject in roomEditorObjects)
        {
            roomEditorObject.selectedVisual.SetActive(false);
        }

        if (selectedRoomEditorObjects != null)
        {
            foreach (RoomEditorObject selectedRoomEditorObject in selectedRoomEditorObjects)
            {
                selectedRoomEditorObject.selectedVisual.SetActive(true);
            }
        }
    }
    #endregion METHODS

    #region FUNCTIONS
    public void SwitchToRoomEditor(RoomSettings roomSettingsToOpen)
    {
        if (roomSettingsToOpen != null)
        {
            if (roomSettingsToOpen.proceduralRoom)
            {
                Debug.LogWarning("Can't Open Procedural Room in Editor");
                return;
            }
            roomEditor.roomSettingsToLoad = roomSettingsToOpen;
            roomEditor.LoadEditorRoom();
        }
        else
        {
            roomEditor.roomSize = new Vector2Int(10, 10);
            roomEditor.GenerateEditorRoom();
        }
        roomEditor.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void SwitchToFloorEditor()
    {
        roomEditor.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    #endregion FUNCTIONS
    #region DEBUG
    public void LoadFloor()
    {
        LoadFloor(floorToLoad);
    }
    #endregion DEBUG
}
#endif

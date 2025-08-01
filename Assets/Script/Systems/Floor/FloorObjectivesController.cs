using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class FloorObjectivesController : MonoBehaviour
{
    private FloorManager _floorManager;
    private List<RoomData> _currentObjectivesRooms;
    private List<Vector2Int> _currentObjectivesRoomPositions;
    [HideInInspector] public int _currentObjectivesIndex;
    private ScriptMachine _scriptMachine;
    
    public event Action<int> OnObjectiveIndexChanged;
    
    public void Init(FloorManager floorManager)
    {
        _floorManager = floorManager;
        _scriptMachine = GetComponent<ScriptMachine>();
    }

    public void SetFloorObjectives()
    {
        _currentObjectivesIndex = 0;
        if (_floorManager.currentFloorSetting.proceduralFloor)
        {
            _currentObjectivesRoomPositions = new List<Vector2Int>();
            _currentObjectivesRooms = new List<RoomData>();
            return;
        }
        
        _currentObjectivesRoomPositions = GetObjectivesRoomPositions(_currentObjectivesIndex);
        _currentObjectivesRooms = GetObjectivesRooms(_floorManager.roomList);
        UpdateObjectives(_currentObjectivesRooms);
        SetNewGraph(_floorManager.currentFloorSetting.objectiveGraph);
    }

    #region METHODS
    public void CheckObjectiveCompletion()
    {
        foreach (RoomData currentObjectivesRoom in _currentObjectivesRooms)
        {
            if (currentObjectivesRoom.isObjective)
            {
                return;
            }
        }
        
        _currentObjectivesIndex += 1;

        //Send a message to subscribers that the index change
        //OnObjectiveIndexChanged?.Invoke(_currentObjectivesIndex);
        
        //Debug.Log("OBJECTIVE COMPLETE");
        // Déclenche un event Visual Scripting (nom libre, ici "ObjectiveAdvanced")
        CustomEvent.Trigger(gameObject, "ObjectiveIncrement", _currentObjectivesIndex);

        _currentObjectivesRoomPositions = GetObjectivesRoomPositions(_currentObjectivesIndex);
        _currentObjectivesRooms = GetObjectivesRooms(_floorManager.roomList);
        UpdateObjectives(_currentObjectivesRooms);
    }

    private void UpdateObjectives(List<RoomData> roomDatas)
    {
        if (roomDatas.Count <= 0)
        {
            return;
        }
        foreach (RoomData roomData in roomDatas)
        {
            roomData.UpdateObjective();
        }
        GameManager.VisualManager.minimapVisual.OpenMinimap();
        _floorManager.minimap.FocusOnSelectedRoom(roomDatas[0]);
    }

    private void ResetObjectives()
    {
        _currentObjectivesIndex = 0;
    }
    #endregion METHODS

    #region UTILS
    private List<RoomData> GetObjectivesRooms(List<RoomData> floorRoomList)
    {
        List<RoomData> filteredRooms = new List<RoomData>();

        var positionSet = new HashSet<Vector2Int>(_currentObjectivesRoomPositions);
        foreach (RoomData room in floorRoomList)
        {
            if (positionSet.Contains(room.roomPosition))
            {
                filteredRooms.Add(room);
            }
        }
        return filteredRooms;
    }

    private List<Vector2Int> GetObjectivesRoomPositions(int objectiveIndex)
    {
        if (objectiveIndex < 0 || objectiveIndex >= _floorManager.currentFloorSetting.floorObjectivesList.Count)
        {
            Debug.Log("Liste of objectives completed");
            return new List<Vector2Int>();
        }
        return _floorManager.currentFloorSetting.floorObjectivesList[objectiveIndex].objectivesPositions;
    }
    
    private void SetNewGraph(ScriptGraphAsset newGraph)
    {
        if (_floorManager.currentFloorSetting.objectiveGraph != null)
        {
            _scriptMachine.enabled = false;
            _scriptMachine.SetGraph(newGraph);
            _scriptMachine.enabled = true;
        }
        Debug.LogWarning("No Objective flow in floor settings");
    }
    #endregion UTILS


}

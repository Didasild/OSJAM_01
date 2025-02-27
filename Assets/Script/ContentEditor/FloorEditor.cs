using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class FloorEditor : MonoBehaviour
{
    public FloorSettings floorToLoad;
    public RoomEditorObject RoomEditorObjectPrefab;
    
    public List<FloorSettings.loadedRoomDatas> loadedRoomData;

    [Button]
    private void LoadFloor()
    {
        
    }
}

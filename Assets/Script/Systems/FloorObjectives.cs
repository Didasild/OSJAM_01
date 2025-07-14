using UnityEngine;

public class FloorObjectives : MonoBehaviour
{
    private FloorManager _floorManager;
    
    public void Init(FloorManager floorManager)
    {
        _floorManager = floorManager;
    }
}

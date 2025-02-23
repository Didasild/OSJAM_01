using NaughtyAttributes;
using UnityEngine;

public class TransformOffset : MonoBehaviour
{
    public bool oneWayOffset = true;
    
    [ShowIf("oneWayOffset")] 
    public bool verticalOffset = false;
    [Range(-1f, 1f)]
    public float primaryOffSetValue;
    [Range(-1f, 1f)]
    public float secondaryOffSetValue;

    [SerializeField] private float offsetUnit = 6.4f;
    private Vector3 _basePosition;

    private void Start()
    {
        _basePosition = transform.localPosition;
    }
    private void Update()
    {
        float primaryOffset = primaryOffSetValue * offsetUnit;
        float secondaryOffset = secondaryOffSetValue * offsetUnit;
        
        if (oneWayOffset)
        {
            if (verticalOffset)
            {
                transform.localPosition = new Vector3(_basePosition.x, _basePosition.y + primaryOffset, 0);
            }
            else
            {
                transform.localPosition = new Vector3(_basePosition.x + primaryOffset, _basePosition.y, 0);
            }
        }
        else
        {
           transform.localPosition = new Vector3(_basePosition.x + primaryOffset, _basePosition.y + secondaryOffset, 0);
        }
    }

    public void ResetOffset()
    {
        primaryOffSetValue = 0f;
        secondaryOffSetValue = 0f;
        transform.localPosition = Vector3.zero;
    }
}

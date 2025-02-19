using UnityEngine;

public class TransformOffset : MonoBehaviour
{
    public bool verticalOffset = false;
    [Range(-1f, 1f)]
    public float offSetValue;

    private const float OffsetUnit = 6.4f;
    private Vector3 _basePosition;

    private void Start()
    {
        _basePosition = transform.localPosition;
    }
    private void Update()
    {
        float offset = offSetValue * OffsetUnit;

        if (verticalOffset)
        {
            transform.localPosition = new Vector3(_basePosition.x, _basePosition.y + offset, _basePosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(_basePosition.x + offset, _basePosition.y, 0);
        }
    }
}

using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class ShakeCamController : MonoBehaviour
{
    private Vector3 _originalPosition;

    public void Init()
    {
        _originalPosition = transform.position;
    }

    public void littleShakeCamera()
    {
        ShakeCamera(0.2f, 0.3f);
    }

    public void midShakeCamera()
    {
        ShakeCamera(0.5f, 0.5f);
    }
    
    public void BigShakeCamera()
    {
        ShakeCamera(0.5f, 1f);
    }

    private void ShakeCamera(float intensity, float duration)
    {
        transform.DOShakePosition(duration, new Vector3(intensity, intensity, 0), 20)
            .OnComplete(() => transform.position = _originalPosition); // Remet la caméra à sa position d'origine après le shake
    }
}

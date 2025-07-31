using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public enum ShakeType
{
    little,
    medium,
    big,
}

public class ShakeCamController : MonoBehaviour
{
    private Vector3 _originalPosition;

    public void Init()
    {
        _originalPosition = transform.position;
    }

    public void LittleShakeCamera(float duration = 0.5f)
    {
        ShakeCamera(0.2f, duration);
    }

    public void MidShakeCamera(float duration = 0.5f)
    {
        ShakeCamera(0.35f, duration);
    }
    
    public void BigShakeCamera(float duration = 0.5f)
    {
        ShakeCamera(0.5f, duration);
    }

    private void ShakeCamera(float intensity, float duration)
    {
        transform.DOShakePosition(duration, new Vector3(intensity, intensity, 0), 20)
            .OnComplete(() => transform.position = _originalPosition); // Remet la caméra à sa position d'origine après le shake
    }
}

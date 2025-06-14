using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour
{
    public float transitionDuration = 0.6f;
    
    [ReadOnly] public Material _material;
    private static readonly int ColorTransition = Shader.PropertyToID("_ColorTransition");
    private static readonly int AlphaTransition = Shader.PropertyToID("_AlphaTransition");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    
    
    public void InitMaterial(Sprite texture)
    {
        _material = GetComponent<Graphic>().materialForRendering;
        GetComponent<Image>().sprite = texture;
    }
    
    public void StartTransition(bool isAppear = true)
    {
        float endPoint = isAppear ? 1f : 0f;
        if (isAppear)
        {
            _material.SetFloat(ColorTransition, 0);
            _material.SetFloat(AlphaTransition, 0);
        }
        FadeProperty(ColorTransition, endPoint);
        FadeProperty(AlphaTransition, endPoint);
    }

    private void FadeProperty(int property, float endPoint)
    {
        DOTween.To(() => _material.GetFloat(property),
                x => _material.SetFloat(property, x),
                endPoint, transitionDuration)
            .SetEase(Ease.InOutQuint);
    }
}

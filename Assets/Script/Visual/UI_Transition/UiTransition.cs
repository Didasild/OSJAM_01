using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiTransition : MonoBehaviour
{
    [Header("FIELDS")]
    public float transitionDuration;

    [SerializeField] private float startAnimation = 0.8f;
    [SerializeField] private float endAnimation = 1f;
    private string property = "_AnimationTransition";
    
    [Header("MATERIALS")]
    [SerializeField] private Material mainBaseMaterial;
    [SerializeField] private Material mainAnimatedMaterial;
    [SerializeField] private Material secondaryBaseMaterial;
    [SerializeField] private Material secondaryAnimatedMaterial;
    
    [Header("LISTE UI ELEMENTS")]
    public List<Image> mainShaderImages;
    public List<Image> secondaryShaderImages;

    public void StartTransition(bool isAppear = true)
    {
        foreach (Image image in mainShaderImages)
        {
            AnimateTransition(image, mainBaseMaterial, mainAnimatedMaterial, isAppear);
        }

        foreach (Image image in secondaryShaderImages)
        {
            AnimateTransition(image, secondaryBaseMaterial, secondaryAnimatedMaterial, isAppear);
        }
    }
    
    private void AnimateTransition(Image image, Material baseMaterial, Material animatedMaterial, bool isAppear = true)
    {
        float startpoint = 0;
        float endpoint = 0;
        if (isAppear)
        {
            startpoint = startAnimation;
            endpoint = endAnimation;
        }
        else
        {
            startpoint = endAnimation;
            endpoint = startAnimation;
        }
        image.material = animatedMaterial;
        animatedMaterial.SetFloat(property, startpoint);
        
        DOTween.To(() => animatedMaterial.GetFloat(property),
                x => animatedMaterial.SetFloat(property, x),
                endpoint, transitionDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Restore base material
                image.material = baseMaterial;
            });
    }
}

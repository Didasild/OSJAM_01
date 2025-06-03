using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiTransition : MonoBehaviour
{
    [Header("FIELDS")]
    public float transitionDuration;
    [SerializeField] private string property = "_AnimationTransition";
    
    [Header("MATERIALS")]
    [SerializeField] private Material mainBaseMaterial;
    [SerializeField] private Material mainAnimatedMaterial;
    [SerializeField] private Material secondaryBaseMaterial;
    [SerializeField] private Material secondaryAnimatedMaterial;
    
    [Header("LISTE UI ELEMENTS")]
    public List<Image> mainShaderImages;
    public List<Image> secondaryShaderImages;

    public void StartTransition()
    {
        foreach (Image image in mainShaderImages)
        {
            AnimateTransition(image, mainBaseMaterial, mainAnimatedMaterial, property, transitionDuration);
        }

        foreach (Image image in secondaryShaderImages)
        {
            AnimateTransition(image, secondaryBaseMaterial, secondaryAnimatedMaterial, property, transitionDuration);
        }
    }
    
    private void AnimateTransition(Image image, Material baseMaterial, Material animatedMaterial, string propertyName, float animationDuration = 1f)
    {
        image.material = animatedMaterial;
        animatedMaterial.SetFloat(propertyName, 0f);

        // Animate property from 0 to 2
        DOTween.To(() => animatedMaterial.GetFloat(propertyName),
                x => animatedMaterial.SetFloat(propertyName, x),
                1.10f, animationDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Restore base material
                image.material = baseMaterial;
            });
    }
}

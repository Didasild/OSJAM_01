using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiTransition : MonoBehaviour
{
    public void AnimateTransition(Image image, Material baseMaterial, Material animatedMaterial, string propertyName, float animationDuration = 1f)
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

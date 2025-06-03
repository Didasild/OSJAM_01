using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPropertyBlock : MonoBehaviour
{
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material animatedMaterial;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private string propertyName = "_AnimationTransition";
    
    private Image _image;

    public void Init()
    {
        _image = GetComponent<Image>();
        _image.material = baseMaterial;
    }

    public void AnimateTransition()
    {
        if (_image == null || animatedMaterial == null || baseMaterial == null)
        {
            Debug.LogWarning("Missing reference for material or image");
            return;
        }
        
        _image.material = animatedMaterial;
        animatedMaterial.SetFloat(propertyName, 0f);

        // Animate property from 0 to 2
        DOTween.To(() => animatedMaterial.GetFloat(propertyName),
                x => animatedMaterial.SetFloat(propertyName, x),
                2f, animationDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Restore base material
                _image.material = baseMaterial;
            });
    }
}

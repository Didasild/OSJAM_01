using DG.Tweening;
using UnityEngine;

public class TransitionPropertyBlock : MonoBehaviour
{
    private MaterialPropertyBlock _propertyBlock;
    private Renderer _renderer;

    [SerializeField] private string propertyName = "_AnimationTransition";
    [SerializeField] private float animationSpeed = 1f;
    private float animationValue = 0f;

    public void Init()
    {
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void AnimateTransition()
    {
        animationValue = 0f;

        DOTween.To(() => animationValue, x => {
            animationValue = x;

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(propertyName, animationValue);
            _renderer.SetPropertyBlock(_propertyBlock);

        }, 2f, 2f).SetEase(Ease.Linear);
    }
}

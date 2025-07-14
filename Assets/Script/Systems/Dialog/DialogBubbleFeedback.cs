using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DialogBubbleFeedback : MonoBehaviour
{
    private DialogVisual _dialogVisual;
    private RectTransform _dialogBubbleFeedback;
    public float resetSpeed = 50f;
    public float sizeIncrement = 5f;
    public float maxSize = 600f;
    private Vector2 rectSize;
    private bool isShrinking;
    
    public void Init(DialogVisual dialog)
    {
        _dialogVisual = dialog;
        _dialogBubbleFeedback = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (_dialogBubbleFeedback.sizeDelta.x > 0)
        {
            Vector2 size = _dialogBubbleFeedback.sizeDelta;
            size.x -= 1 * (resetSpeed * Time.deltaTime);

            // Clamp à zéro pour éviter valeurs négatives
            if (size.x <= 0f)
            {
                size = new Vector2(0,31f);
            }

            _dialogBubbleFeedback.sizeDelta = size;
        }
    }

    public void IncreaseBubbleSize()
    {
        rectSize = _dialogBubbleFeedback.sizeDelta;
        if (rectSize.x <= maxSize)
        {
            rectSize = _dialogBubbleFeedback.sizeDelta;
            rectSize.x += sizeIncrement;
            _dialogBubbleFeedback.sizeDelta = rectSize;
        }
    }
    
    public void ResetBubbleSize()
    {
        _dialogBubbleFeedback.sizeDelta = new Vector2(0f, 31f);
    }
}

using TMPro;
using UnityEngine;

public class DialogVisual : MonoBehaviour
{
    #region FIELDS
    public TMP_Text characterName;
    public TMP_Text dialogText;
    #endregion FIELDS
    
    private Dialog _dialog;
    [SerializeField] private DialogBubbleFeedback _dialogBubbleFeedback;
    
    public DialogBubbleFeedback DialogBubbleFeedback => _dialogBubbleFeedback;
    
    public void Init(Dialog dialog)
    {
        _dialog = dialog;
        _dialogBubbleFeedback.Init(this);
    }
    
}

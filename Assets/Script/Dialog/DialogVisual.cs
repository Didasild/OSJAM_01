using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogVisual : MonoBehaviour
{
    [Header("REFERENCES")]
    #region FIELDS
    public TMP_Text characterName;
    public TMP_Text dialogText;
    #endregion FIELDS
    
    private Dialog _dialog;
    [SerializeField] private DialogBubbleFeedback _dialogBubbleFeedback;
    public DialogBubbleFeedback DialogBubbleFeedback => _dialogBubbleFeedback;
    
    [ReadOnly] public UiTransition uiTransition;
    
    public void Init(Dialog dialog)
    {
        _dialog = dialog;
        _dialogBubbleFeedback.Init(this);
        uiTransition = gameObject.GetComponent<UiTransition>();
    }
    
    public void DialogApparition()
    {
        uiTransition.StartTransition();
    }

    public void DialogDisparition()
    {
        uiTransition.StartTransition(false);
    }
}

using System.Collections.Generic;
using DG.Tweening;
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
    public GameObject dialogContainer;
    public GameObject dialogArrow;
    public GameObject portraitBox;
    public GameObject portraitMask;
    #endregion FIELDS
    
    private Dialog _dialog;
    [SerializeField] private DialogBubbleFeedback _dialogBubbleFeedback;
    public DialogBubbleFeedback DialogBubbleFeedback => _dialogBubbleFeedback;
    
    [ReadOnly] public UiTransition uiDialogBoxTransition;
    private UiTransition uiPortraitTransition;
    private PortraitController _portraitController;
    
    public void Init(Dialog dialog)
    {
        _dialog = dialog;
        _dialogBubbleFeedback.Init(this);
        
        portraitBox.SetActive(false);
        _portraitController = portraitBox.GetComponentInChildren<PortraitController>();
        
        uiDialogBoxTransition = gameObject.GetComponent<UiTransition>();
        uiPortraitTransition = portraitBox.GetComponent<UiTransition>();
    }
    
    public void DialogApparition(Sprite npcTexture = null)
    {
        portraitBox.SetActive(false);
        uiDialogBoxTransition.StartTransition();
        DOVirtual.DelayedCall(uiDialogBoxTransition.transitionDuration, () => PortraitApparition(npcTexture));
    }

    public void DialogDisparition()
    {
        uiDialogBoxTransition.StartTransition(false);
        _portraitController.StartTransition(false);
        DOVirtual.DelayedCall(uiDialogBoxTransition.transitionDuration/1.5f, ClearDialogBox);
        DOVirtual.DelayedCall(uiDialogBoxTransition.transitionDuration, PortraitDisparition);
    }

    private void PortraitApparition(Sprite npcTexture)
    {
        portraitBox.SetActive(true);
        portraitMask.SetActive(true);
        _portraitController.InitMaterial(npcTexture);
        
        uiPortraitTransition.StartTransition();
        _portraitController.StartTransition();
    }

    private void PortraitDisparition()
    {
        portraitMask.SetActive(false);
        uiPortraitTransition.StartTransition(false);
        DOVirtual.DelayedCall(uiPortraitTransition.transitionDuration, () => portraitBox.SetActive(false));
    }
    
    public void UpdateCharacterName(string newName)
    {
        characterName.text = newName;
    }
    
    public void UpdateDialogText(string newText)
    {
        if (newText != null)
        {
            dialogText.text = newText;
        }
        else
        {
            Debug.LogError("Dialog is null");
        }
    }
    
    public void ClearDialogBox()
    {
        dialogContainer.SetActive(false);
        dialogArrow.SetActive(false);
        dialogText.text = "";
        characterName.text = "";
        DialogBubbleFeedback.ResetBubbleSize();
    }
}

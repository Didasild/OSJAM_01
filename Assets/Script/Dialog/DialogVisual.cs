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
    public float transitionDuration;
    #endregion FIELDS
    
    private Dialog _dialog;
    [SerializeField] private DialogBubbleFeedback _dialogBubbleFeedback;
    
    [Header("MATERIALS")]
    [SerializeField] private Material mainBaseMaterial;
    [SerializeField] private Material mainAnimatedMaterial;
    [SerializeField] private Material secondaryBaseMaterial;
    [SerializeField] private Material secondaryAnimatedMaterial;

    [SerializeField] private string property = "_AnimationTransition";

    [Header("LISTE UI ELEMENTS")]
    public List<Image> mainShaderImages;
    public List<Image> secondaryShaderImages;
    public DialogBubbleFeedback DialogBubbleFeedback => _dialogBubbleFeedback;
    
    private UiTransition _uiTransition;
    
    public void Init(Dialog dialog)
    {
        _dialog = dialog;
        _dialogBubbleFeedback.Init(this);
        _uiTransition = GameManager.visualManager.UITransition;
    }

    [Button]
    public void DialogApparition()
    {
        foreach (Image image in mainShaderImages)
        {
            _uiTransition.AnimateTransition(image, mainBaseMaterial, mainAnimatedMaterial, property, transitionDuration);
        }

        foreach (Image image in secondaryShaderImages)
        {
            _uiTransition.AnimateTransition(image, secondaryBaseMaterial, secondaryAnimatedMaterial, property, transitionDuration);
        }
    }
}

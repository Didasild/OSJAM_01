using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogVisual : MonoBehaviour
{
    #region FIELDS
    public TMP_Text characterName;
    public TMP_Text dialogText;
    #endregion FIELDS
    
    private Dialog _dialog;
    [SerializeField] private DialogBubbleFeedback _dialogBubbleFeedback;
    [SerializeField] private TransitionPropertyBlock _propertyBlock;
    
    [Header("MATERIALS")]
    [SerializeField] private Material mainBaseMaterial;
    [SerializeField] private Material mainAnimatedMaterial;
    [SerializeField] private Material secondaryBaseMaterial;
    [SerializeField] private Material secondaryAnimatedMaterial;

    [Header("LISTE UI ELEMENTS")]
    public List<Image> mainShaderObjects;
    public List<Image> secondaryShaderObjects;
    public DialogBubbleFeedback DialogBubbleFeedback => _dialogBubbleFeedback;
    
    public void Init(Dialog dialog)
    {
        _dialog = dialog;
        _dialogBubbleFeedback.Init(this);
        _propertyBlock.Init();
    }

    [Button]
    private void DialogApparition()
    {
        _propertyBlock.AnimateTransition();
    }
    
}

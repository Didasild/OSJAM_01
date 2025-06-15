using System.Collections.Generic;
using DG.Tweening;
using Febucci.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dialog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region FIELDS
    private GameManager _gameManager;
    [SerializeField] private DialogVisual _dialogVisual;
    private RoomSettings _currentRoomSettings;
    private List<string> _currentDialogSequence;
    private int currentSequenceIndex;
    private bool dialogStarted;
    private NPC _currentNPC;
    public TextAnimatorPlayer _textAnimatorPlayer;
    
    public DialogVisual DialogVisual => _dialogVisual;
    #endregion
    
    public void Init(GameManager manager)
    {
        _gameManager = manager;

        if (_dialogVisual == null)
        {
            _dialogVisual = GetComponent<DialogVisual>();
        }
        _dialogVisual.Init(this);
        
        _dialogVisual.ClearDialogBox();
    }

    #region METHODS
    public void StartDialogSequence(NPC npc)
    {
        _currentNPC = npc;
        
        _dialogVisual.ClearDialogBox();
        
        _dialogVisual.dialogContainer.SetActive(true);
        _dialogVisual.DialogApparition(npc.NpcDialogsSettings.npcSettings.npcImage);

        DOVirtual.DelayedCall(_dialogVisual.uiDialogBoxTransition.transitionDuration/1.5f, () =>
        {
            DisplayDialogSequence(npc);
        });
    }

    private void DisplayDialogSequence(NPC npc)
    {
        _currentDialogSequence = npc.currentDialogSequence;
        NpcDialogsSettings npcDialogsSettings = npc.NpcDialogsSettings;
        
        _dialogVisual.UpdateCharacterName(npcDialogsSettings.npcSettings.npcName);
        
        if (_currentDialogSequence.Count > 0)
        {
            currentSequenceIndex = 0;
            dialogStarted = true;
            _dialogVisual.UpdateDialogText(_currentDialogSequence[0]);
        }
        else
        {
            Debug.LogError("No sentences in the DialogPull:" );
        }
    }

    private void GoToNextSentence()
    {
        if (_currentDialogSequence != null)
        {
            currentSequenceIndex++;
            if (currentSequenceIndex < _currentDialogSequence.Count)
            {
                _dialogVisual.UpdateDialogText(_currentDialogSequence[currentSequenceIndex]);
                _dialogVisual.dialogArrow.SetActive(false);
            }
            else
            {
                EndDialog();
            }
        }
        else
        {
            _dialogVisual.ClearDialogBox();
            Debug.Log("DialogPull is null");
        }
    }

    private void EndDialog()
    {
        dialogStarted = false;
        currentSequenceIndex = 0;
        
        //A DEVELOPPER QUAND NECESSAIRE ET PLACE AILLEURS POTENTIELLEMENT
        _currentNPC.ChangeNpcState(DialogUtils.NPCState.Inactive);
        _dialogVisual.DialogDisparition();
        DOVirtual.DelayedCall(_dialogVisual.uiDialogBoxTransition.transitionDuration/1.5f, () =>
        {
            _dialogVisual.ClearDialogBox();
        });
    }
    #endregion METHODS
    
    #region POINTER
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dialogStarted)
        {
            TooltipController.ShowTooltip("NEXT.");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipController.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_textAnimatorPlayer.IsPlaying)
        {
            _textAnimatorPlayer.SkipTypewriter();
        }
        else if (dialogStarted)
        {
            GoToNextSentence();
        }
    }
    #endregion POINTER

}

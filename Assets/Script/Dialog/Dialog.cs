using System.Collections.Generic;
using DG.Tweening;
using Febucci.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Dialog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region FIELDS
    private GameManager _gameManager;
    public GameObject dialogContainer;
    public GameObject dialogArrow;
    [SerializeField] private DialogVisual _dialogVisual;
    private RoomSettings _currentRoomSettings;
    private List<string> _currentDialogSequence;
    private int currentSequenceIndex;
    private bool dialogStarted;
    private NPC _currentNPC;
    public TextAnimatorPlayer textAnimatorPlayer;
    #endregion
    
    public void Init(GameManager manager)
    {
        _gameManager = manager;

        if (_dialogVisual == null)
        {
            _dialogVisual = GetComponent<DialogVisual>();
        }
        _dialogVisual.Init(this);
        
        ClearDialogBox();
    }

    #region METHODS
    public void StartDialogSequence(NPC npc)
    {
        ClearDialogBox();
        dialogContainer.SetActive(true);
        _dialogVisual.DialogApparition(npc.NpcDialogsSettings.npcSettings.npcImage);
        _currentNPC = npc;
        DOVirtual.DelayedCall(_dialogVisual.uiDialogBoxTransition.transitionDuration/1.5f, () =>
        {
            DisplayDialogSequence(npc);
        });
    }

    private void DisplayDialogSequence(NPC npc)
    {
        NpcDialogsSettings npcDialogsSettings = npc.NpcDialogsSettings;
        _currentDialogSequence = npc.currentDialogSequence;
        UpdateCharacterName(npcDialogsSettings.npcSettings.npcName);
        if (_currentDialogSequence.Count > 0)
        {
            currentSequenceIndex = 0;
            dialogStarted = true;
            UpdateDialogText(_currentDialogSequence[0]);
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
                UpdateDialogText(_currentDialogSequence[currentSequenceIndex]);
                dialogArrow.SetActive(false);
            }
            else
            {
                EndDialog();
            }
        }
        else
        {
            ClearDialogBox();
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
            ClearDialogBox();
        });
    }
    #endregion METHODS
    
    #region VISUAL METHODS TO MOVE

    //A BOUGER DANS DIALOG VISUAL
    private void UpdateCharacterName(string characterName)
    {
        _dialogVisual.characterName.text = characterName;
    }

    private void UpdateDialogText(string dialogText)
    {
        if (dialogText != null)
        {
            _dialogVisual.dialogText.text = dialogText;
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
        _dialogVisual.dialogText.text = "";
        _dialogVisual.characterName.text = "";
        _dialogVisual.DialogBubbleFeedback.ResetBubbleSize();
    }
    #endregion VISUAL METHODS
    
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
        if (textAnimatorPlayer.IsPlaying)
        {
            textAnimatorPlayer.SkipTypewriter();
        }
        else if (dialogStarted)
        {
            GoToNextSentence();
        }
    }
    #endregion POINTER

}

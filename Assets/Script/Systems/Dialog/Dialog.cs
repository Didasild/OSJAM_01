using System.Collections.Generic;
using DG.Tweening;
using Febucci.UI;
using Script.Scriptable.NPC;
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
    private bool _dialogStarted;
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

    #region NPC METHODS
    public void StartNpcDialogSequence(NPC npc)
    {
        if (_dialogStarted)
        {
            return;
        }
        _dialogStarted = true;
        _currentNPC = npc;
        
        _dialogVisual.ClearDialogBox();
        
        _dialogVisual.dialogContainer.SetActive(true);
        _dialogVisual.DialogApparition(npc.NpcDialogsSettings.npcSettings.npcImage);
        
        DOVirtual.DelayedCall(_dialogVisual.uiDialogBoxTransition.transitionDuration/1.5f, () =>
        {
            DisplayNpcDialogSequence(npc);
        });
    }

    public void StartEventDialogSequence(NpcDialogsSettings eventDialogsSettings)
    {
        _dialogStarted = true;
        
        _dialogVisual.ClearDialogBox();
        
        _dialogVisual.dialogContainer.SetActive(true);
        _dialogVisual.DialogApparition(eventDialogsSettings.npcSettings.npcImage);
        
        DOVirtual.DelayedCall(_dialogVisual.uiDialogBoxTransition.transitionDuration/1.5f, () =>
        {
            DisplayEventDialogSequence(eventDialogsSettings);
        });
    }

    private void DisplayNpcDialogSequence(NPC npc)
    {
        NpcDialogsSettings npcDialogsSettings = npc.NpcDialogsSettings;
        _currentDialogSequence = npc.currentDialogSequence;
        DisplayDialogSequenceInternal(npcDialogsSettings);
    }
    
    private void DisplayEventDialogSequence(NpcDialogsSettings eventDialogsSettings)
    {
        _currentDialogSequence = eventDialogsSettings.GetDialogSequence(DialogUtils.NPCState.Active);
        DisplayDialogSequenceInternal(eventDialogsSettings);
    }

    private void DisplayDialogSequenceInternal(NpcDialogsSettings dialogsSettings)
    {
        _dialogVisual.UpdateCharacterName(dialogsSettings.npcSettings.npcName);
        
        if (_currentDialogSequence.Count > 0)
        {
            currentSequenceIndex = 0;

            _dialogVisual.UpdateDialogText(_currentDialogSequence[0]);
        }
        else
        {
            _dialogStarted = false;
            Debug.LogError("No sentences in the DialogPool: " + _currentDialogSequence);
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
            Debug.Log("DialogPool is null");
        }
    }

    private void EndDialog()
    {
        _dialogStarted = false;
        currentSequenceIndex = 0;
        
        //A DEVELOPPER QUAND NECESSAIRE ET PLACE AILLEURS POTENTIELLEMENT
        if (_currentNPC._currentNpcState != DialogUtils.NPCState.Inactive)
        {
            _currentNPC.ChangeNpcState(DialogUtils.NPCState.Inactive);
        }
        
        _dialogVisual.DialogDisparition();
    }
    #endregion NPC METHODS
    
    #region POINTER
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_dialogStarted)
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
        else if (_dialogStarted)
        {
            GoToNextSentence();
        }
    }
    #endregion POINTER

}

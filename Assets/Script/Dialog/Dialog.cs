using System.Collections.Generic;
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
        _currentNPC = npc;
        NPCSettings npcSettings = npc.npcSettings;
        _currentDialogSequence = npcSettings.GetDialogSequence(npc.currentNPCState);
        UpdateCharacterName(npcSettings.npcName);
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
            }
            else
            {
                //A DEVELOPPER QUAND NECESSAIRE
                _currentNPC.ChangeNpcState(DialogUtils.NPCState.inactive);
                
                ClearDialogBox();
                dialogStarted = false;
                currentSequenceIndex = 0;
            }
        }
        else
        {
            ClearDialogBox();
            Debug.Log("DialogPull is null");
        }
    }

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
        _dialogVisual.dialogText.text = "";
        _dialogVisual.characterName.text = "";
        _dialogVisual.DialogBubbleFeedback.ResetBubbleSize();
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
        if (dialogStarted)
        {
            GoToNextSentence();
        }
    }
    #endregion POINTER

}

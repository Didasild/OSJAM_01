using UnityEngine;
using UnityEngine.EventSystems;

public class Dialog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region FIELDS
    private GameManager _gameManager;
    [SerializeField] private DialogVisual _dialogVisual;
    private RoomSettings _currentRoomSettings;
    private DialogSequence _currentDialogSequence;
    private int currentSequenceIndex;
    private bool dialogStarted;
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
    public void StartDialogSequence(NPCSettings npcSettings)
    {
        _currentDialogSequence = npcSettings.GetDialogSequence();
        UpdateCharacterName(npcSettings.npcName);
        if (_currentDialogSequence.sequence.Count > 0)
        {
            currentSequenceIndex = 0;
            dialogStarted = true;
            UpdateDialogText(_currentDialogSequence.sequence[0]);
        }
        else
        {
            Debug.LogError("No sequence in the DialogSequence: " + _currentDialogSequence.name);
        }
    }

    private void GoToNextSentence()
    {
        if (_currentDialogSequence != null)
        {
            currentSequenceIndex++;
            if (currentSequenceIndex < _currentDialogSequence.sequence.Count)
            {
                UpdateDialogText(_currentDialogSequence.sequence[currentSequenceIndex]);
            }
            else
            {
                ClearDialogBox();
                dialogStarted = false;
                currentSequenceIndex = 0;
            }
        }
        else
        {
            ClearDialogBox();
            Debug.Log("DialogSequence is null");
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

    private void ClearDialogBox()
    {
        _dialogVisual.dialogText.text = "";
        _dialogVisual.characterName.text = "";
    }
    #endregion METHODS
    
    #region POINTER
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

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

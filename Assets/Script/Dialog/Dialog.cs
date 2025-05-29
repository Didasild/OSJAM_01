using UnityEngine;

public class Dialog : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private DialogVisual _dialogVisual;
    private RoomSettings _currentRoomSettings;
    private DialogSequence _currentDialogSequence;
    
    public void Init(GameManager manager)
    {
        _gameManager = manager;
        _dialogVisual.Init(this);
        ClearDialogBox();
    }

    public void StartDialogSequence(NPCSettings npcSettings)
    {
        _currentDialogSequence = npcSettings.GetDialogSequence();
        UpdateCharacterName(npcSettings.npcName);
        UpdateDialogText(_currentDialogSequence.sequence[0]);
    }

    public void GoToNextSentence()
    {
        if (_currentDialogSequence != null)
        {
            
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
}

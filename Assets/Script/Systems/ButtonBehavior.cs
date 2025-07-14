using UnityEngine;


public class ButtonBehavior : MonoBehaviour
{
    public RoomDirection buttonDirection;
    public void OnButtonClick()
    {
        GameManager.Instance.FloorManager.ChangeRoomDirection(buttonDirection);
        TooltipController.HideTooltip();
    }
}

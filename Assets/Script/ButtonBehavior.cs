using UnityEngine;


public class ButtonBehavior : MonoBehaviour
{
    public RoomDirection buttonDirection;
    public void OnButtonClick()
    {
        GameManager.Instance.floorManager.ChangeRoomDirection(buttonDirection);
        TooltipSystem.HideTooltip();
    }
}

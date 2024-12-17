using System;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    public RoomDirection buttonDirection;
    public void OnButtonClick()
    {
        GameManager.Instance.dungeonManager.ChangeRoomDirection(buttonDirection);
    }
}

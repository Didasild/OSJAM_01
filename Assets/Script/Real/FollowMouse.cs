using System;
using NaughtyAttributes;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public Player player;
    public bool followX;
    [ShowIf("followX")] public Vector2 minMaxXPos;
    public bool followY;
    [ShowIf("followY")] public Vector2 minMaxYPos;
    private Vector2 currentMousePos;
    // Update is called once per frame

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    void Update()
    {
        currentMousePos = player.MousePosition;
        
        // Appliquer les contraintes
        float newX = followX ? Mathf.Clamp(currentMousePos.x, minMaxXPos.x, minMaxXPos.y) : transform.position.x;
        float newY = followY ? Mathf.Clamp(currentMousePos.y, minMaxYPos.x, minMaxYPos.y) : transform.position.y;

        // Appliquer la nouvelle position
        transform.position = new Vector2(newX, newY);
    }
}

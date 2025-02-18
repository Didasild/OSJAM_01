using UnityEngine;

public class SetInactiveOnStart : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}

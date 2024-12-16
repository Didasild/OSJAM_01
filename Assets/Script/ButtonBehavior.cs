using System;
using UnityEngine;
public enum ButtonDirection
    {
        Right,
        Left,
        Up,
        Down
    }
public class ButtonBehavior : MonoBehaviour
{
    public ButtonDirection buttonDirection;

    public void Start()
    {
        DefineDirection(name);
    }
    
    public void DefineDirection(String direction)
    {
        // Convertir la chaîne de caractères en ButtonDirection (en tenant compte de la casse)
        if (Enum.TryParse(direction, true, out ButtonDirection parsedDirection))
        {
            buttonDirection = parsedDirection;
        }
        else
        {
            Debug.LogWarning($"Direction '{direction}' invalide pour {gameObject.name}. Vérifiez que le nom correspond à une valeur de ButtonDirection.");
        }
    }

    public void OnuttonClick()
    {
        
    }
}

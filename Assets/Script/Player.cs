using UnityEngine;

public class Player : MonoBehaviour
{
    private Cell cellOver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Convertit la position de la souris en coordonnées du monde
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Effectue un raycast à la position de la souris
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Affiche le nom de l'objet détecté
            Debug.Log("Cellule détectée : " + hit.collider.gameObject.name);

            // Optionnel : Appelle une méthode sur le script Cell attaché
            cellOver = hit.collider.GetComponent<Cell>();
            if (cellOver != null)
            {
                Debug.Log("État actuel de la cellule : " + cellOver.currentState);
            }
        }
        else
        {
            Debug.Log("Aucune cellule détectée");
        }

        // Clique sur le bouton gauche
        if (Input.GetMouseButtonUp(0))
        {
            if (hit.collider !=null)
            {
                if (cellOver.currentState == Cell.Cellstate.Cover)
                {
                    ChangeCellState();
                }
            }
            else
            {
                Debug.Log("Aucune Cellule a cliquer");
            }
        }
    }

    private void ChangeCellState()
    {
        cellOver.ChangeState(Cell.Cellstate.Reveal);
    }

}

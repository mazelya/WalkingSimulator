using UnityEngine;

public class Up : MonoBehaviour
{
    [Header("Réglages de l'Ascenseur")]
    public float liftDistance = 5f; // Distance entre le bas et le haut
    public float liftSpeed = 3f;    // Vitesse de déplacement

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 targetPosition;

    private bool isMoving = false;
    private bool isAtTop = false; // Permet de savoir si l'ascenseur est en haut ou en bas

    private void Awake()
    {
        // On initialise les deux positions possibles
        startPosition = transform.position;
        endPosition = transform.position + Vector3.up * liftDistance;

        // Au début, l'ascenseur est en bas, donc la première cible sera le haut
        targetPosition = endPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        // On ne déclenche le mouvement que si le joueur entre et que l'ascenseur est immobile
        if (other.CompareTag("Player") && !isMoving)
        {
            // On détermine la nouvelle cible selon la position actuelle
            if (isAtTop)
            {
                targetPosition = startPosition; // On redescend
            }
            else
            {
                targetPosition = endPosition; // On monte
            }

            isMoving = true;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            // Déplacement fluide vers la cible
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                liftSpeed * Time.deltaTime
            );

            // Vérification si la destination est atteinte
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isMoving = false;
                // On inverse l'état pour le prochain passage
                isAtTop = !isAtTop;
            }
        }
    }
}
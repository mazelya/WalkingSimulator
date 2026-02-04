using UnityEngine;

public class Up : MonoBehaviour
{
    [Header("Réglages du mouvement")]
    public float liftDistance = 3.8f; // La distance exacte à parcourir
    public float liftSpeed = 2f;    // La vitesse du mouvement

    private Vector3 targetPosition;
    private bool shouldMove = false;

    private void OnTriggerEnter(Collider other)
    {
        // On vérifie si c'est le joueur et si on n'a pas encore bougé
        if (other.CompareTag("Player") && !shouldMove)
        {
            // On définit la position finale (Position actuelle + distance vers le haut)
            targetPosition = transform.position + Vector3.up * liftDistance;
            shouldMove = true;
        }
    }

    private void Update()
    {
        if (shouldMove)
        {
            // Déplacement par transformation de la location vers la cible
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                liftSpeed * Time.deltaTime
            );

            // On arrête de bouger une fois la cible atteinte
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                shouldMove = false;
            }
        }
    }
}
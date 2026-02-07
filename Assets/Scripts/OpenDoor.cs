using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    // On définit les directions possibles
    public enum DoorDirection { Droite, Gauche, Haut, Bas }

    [Header("Réglages de la porte")]
    public DoorDirection directionChoisie = DoorDirection.Droite;
    public float liftDistance = 1f;
    public float liftSpeed = 0.5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;

    private bool isMoving = false;

    private void Awake()
    {
        closedPosition = transform.position;

        // On calcule la position d'ouverture selon le choix dans l'inspecteur
        Vector3 directionVector = Vector3.right; // Par défaut

        switch (directionChoisie)
        {
            case DoorDirection.Droite:
                directionVector = transform.right;
                break;
            case DoorDirection.Gauche:
                directionVector = -transform.right;
                break;
            case DoorDirection.Haut:
                directionVector = transform.up;
                break;
            case DoorDirection.Bas:
                directionVector = -transform.up;
                break;
        }

        openPosition = closedPosition + (directionVector * liftDistance);
        targetPosition = closedPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPosition = openPosition;
            isMoving = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPosition = closedPosition;
            isMoving = true;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                liftSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isMoving = false;
            }
        }
    }
}
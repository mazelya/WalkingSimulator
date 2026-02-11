using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Réglages de la rotation")]
    [Tooltip("L'angle d'ouverture (ex: 90 ou -90)")]
    public float openAngle = 90f;
    public float rotationSpeed = 3f;

    [Header("Condition de déblocage")]
    public int requiredPeopleCount = 9;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion targetRotation;
    private bool isMoving = false;

    private void Awake()
    {
        // 1. On mémorise la rotation de départ
        closedRotation = transform.rotation;

        // 2. On utilise Vector3.up (l'axe vertical absolu du monde) 
        // pour être sûr que la porte ne "tombe" pas vers l'avant.
        openRotation = Quaternion.AngleAxis(openAngle, Vector3.up) * closedRotation;

        targetRotation = closedRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DialogueManager.Instance.GetMetCount() >= requiredPeopleCount)
            {
                targetRotation = openRotation;
                isMoving = true;
            }
            else
            {
                Debug.Log("Accès refusé : Discutez avec tout le monde !");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetRotation = closedRotation;
            isMoving = true;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isMoving = false;
            }
        }
    }
}
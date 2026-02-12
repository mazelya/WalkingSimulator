using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Réglages de la rotation")]
    public float openAngle = 90f;
    public float rotationSpeed = 3f;

    [Header("Condition de déblocage")]
    public int requiredPeopleCount = 9;

    [Header("Audio")]
    [Tooltip("Le son qui se joue quand la porte s'ouvre")]
    public AudioClip openSound;
    [Range(0f, 1f)] public float volume = 1f;
    private AudioSource audioSource;
    private bool hasPlayedSound = false; // Pour ne pas rejouer le son en boucle

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion targetRotation;
    private bool isMoving = false;

    private void Awake()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.AngleAxis(openAngle, Vector3.up) * closedRotation;
        targetRotation = closedRotation;

        // Configuration automatique de l'AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // Son 3D
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DialogueManager.Instance.GetMetCount() >= requiredPeopleCount)
            {
                targetRotation = openRotation;
                isMoving = true;

                // --- JOUER LE SON ---
                if (openSound != null && !hasPlayedSound)
                {
                    audioSource.PlayOneShot(openSound, volume);
                    hasPlayedSound = true; // On marque comme joué pour cette ouverture
                }
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
            hasPlayedSound = false; // On réinitialise pour que ça rejoue au prochain passage si besoin
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
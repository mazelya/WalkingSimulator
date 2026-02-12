using UnityEngine;

public class Up : MonoBehaviour
{
    [Header("Réglages de l'Ascenseur")]
    public float liftDistance = 5f;
    public float liftSpeed = 3f;

    [Header("Émetteurs Audio (3D)")]
    [Tooltip("Glisser ici l'objet contenant l'AudioSource du décollage")]
    public AudioSource sourceTakeoff;  // L'émetteur pour la montée

    [Tooltip("Glisser ici l'objet contenant l'AudioSource du stationnaire")]
    public AudioSource sourceHover;    // L'émetteur pour le haut

    [Tooltip("Glisser ici l'objet contenant l'AudioSource de l'atterrissage")]
    public AudioSource sourceLanding;  // L'émetteur pour la descente

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 targetPosition;

    private bool isMoving = false;
    private bool isAtTop = false;

    private void Awake()
    {
        startPosition = transform.position;
        endPosition = transform.position + Vector3.up * liftDistance;
        targetPosition = endPosition;

        // Sécurité : on s'assure que tout est coupé au lancement du jeu
        StopAllSounds();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            if (isAtTop)
            {
                // CAS 1 : Descente
                targetPosition = startPosition;
                // On active l'émetteur "Atterrissage"
                SwitchToAudioSource(sourceLanding);
            }
            else
            {
                // CAS 2 : Montée
                targetPosition = endPosition;
                // On active l'émetteur "Décollage"
                SwitchToAudioSource(sourceTakeoff);
            }

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

            // Vérification de l'arrivée (avec une marge très fine)
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isMoving = false;
                isAtTop = !isAtTop;

                // GESTION DES ÉTATS SONORES À L'ARRIVÉE
                if (isAtTop)
                {
                    // Arrivé EN HAUT -> On passe au bruit stationnaire
                    SwitchToAudioSource(sourceHover);
                }
                else
                {
                    // Arrivé EN BAS -> On coupe tout
                    StopAllSounds();
                }
            }
        }
    }

    // --- Gestion des Audio Sources Multiples ---

    /// <summary>
    /// Active l'AudioSource demandé et coupe tous les autres.
    /// </summary>
    private void SwitchToAudioSource(AudioSource activeSource)
    {
        // 1. On arrête les autres sons pour éviter la cacophonie
        if (sourceTakeoff != activeSource) sourceTakeoff.Stop();
        if (sourceHover != activeSource) sourceHover.Stop();
        if (sourceLanding != activeSource) sourceLanding.Stop();

        // 2. On lance le son désiré s'il n'est pas déjà en train de jouer
        if (activeSource != null && !activeSource.isPlaying)
        {
            activeSource.Play();
        }
    }

    /// <summary>
    /// Arrête tous les sons du drone.
    /// </summary>
    private void StopAllSounds()
    {
        if (sourceTakeoff != null) sourceTakeoff.Stop();
        if (sourceHover != null) sourceHover.Stop();
        if (sourceLanding != null) sourceLanding.Stop();
    }
}
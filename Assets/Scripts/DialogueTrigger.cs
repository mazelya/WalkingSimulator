using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConditionalDialogue
{
    public string requiredNpcID;
    public DialogueLine[] conversation;
}

public class DialogueTrigger : MonoBehaviour
{
    public string npcID;
    public string npcName;

    [Header("Dialogues")]
    public DialogueLine[] defaultConversation;

    [Header("Conditions Spéciales")]
    public List<ConditionalDialogue> specializedDialogues;

    [Header("Réglages de répétition")]
    public bool playOnlyOnce = false;
    private bool hasPlayedDefault = false;

    [Header("Réglages Rotation")]
    public float rotationSpeed = 5f;

    [Header("Audio")]
    public AudioClip dialogueVoice; // GLISSE TA MUSIQUE/GIBBERISH ICI
    [Range(0f, 1f)] public float volume = 1f; // Règle le volume ici
    private AudioSource audioSource;

    private bool isChatting = false;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform playerTransform;
    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();

        // --- AJOUT AUDIO ---
        // On récupère ou on crée l'AudioSource automatiquement
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // On empêche le son de se lancer au démarrage du jeu
        audioSource.loop = true; // On met en boucle pour faire un effet de parole continue
        audioSource.spatialBlend = 1.0f; // 1.0 = Son 3D (vient du NPC), 0.0 = Son 2D (dans la tête)
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        DialogueLine[] conversationToPlay = null;

        foreach (var cond in specializedDialogues)
        {
            if (DialogueManager.Instance.HasMetNpc(cond.requiredNpcID))
            {
                conversationToPlay = cond.conversation;
                break;
            }
        }

        if (conversationToPlay == null)
        {
            if (playOnlyOnce && hasPlayedDefault) return;
            conversationToPlay = defaultConversation;
            hasPlayedDefault = true;
        }

        isChatting = true;
        DialogueManager.Instance.MarkNpcAsMet(npcID);

        // --- LANCEMENT AUDIO ---
        if (dialogueVoice != null && audioSource != null)
        {
            audioSource.clip = dialogueVoice;
            audioSource.volume = volume;
            audioSource.Play();
        }

        // ARRÊT DU PNJ
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.updateRotation = false; // Désactive la rotation auto du NavMesh
        }

        // FORCE L'ANIMATION IDLE
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
        }

        StopAllCoroutines();
        StartCoroutine(LookAtPlayer());

        DialogueManager.Instance.ShowDialogue(conversationToPlay, npcName);
    }

    private void Update()
    {
        if (isChatting)
        {
            // 1. On vérifie si on clique pour passer au texte suivant
            if (Input.GetMouseButtonDown(0))
            {
                DialogueManager.Instance.DisplayNextLine();
            }

            // 2. SÉCURITÉ : Si le DialogueManager a fermé la box (fin du texte), on libère le PNJ
            // Cela permet de reprendre l'animation même si on est encore dans le collider
            if (!DialogueManager.Instance.dialogueBox.activeSelf)
            {
                EndDialogue();
            }
        }
    }

    private void EndDialogue()
    {
        if (!isChatting) return; // Évite de répéter si déjà fini

        // --- ARRÊT AUDIO ---
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        isChatting = false;
        DialogueManager.Instance.HideDialogue();

        // REPRISE DU PNJ
        if (agent != null)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
        }

        // REPRISE DE L'ANIMATION PRINCIPALE (Marche)
        if (anim != null)
        {
            anim.SetFloat("Speed", 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndDialogue();
        }
    }

    private IEnumerator LookAtPlayer()
    {
        while (isChatting && playerTransform != null)
        {
            float originalX = transform.rotation.eulerAngles.x;
            float originalZ = transform.rotation.eulerAngles.z;
            Vector3 targetPoint = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            Vector3 direction = targetPoint - transform.position;

            if (direction.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                transform.rotation = Quaternion.Euler(originalX, newRotation.eulerAngles.y, originalZ);
            }
            yield return null;
        }
    }
}

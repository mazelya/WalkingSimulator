using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public string npcID;
    public string npcName;
    public DialogueLine[] conversation;

    [Header("Réglages de répétition")]
    public bool playOnlyOnce = false; // Option pour jouer une seule fois
    private bool hasPlayed = false;   // Mémorise si déjà joué

    [Header("Réglages Rotation")]
    public float rotationSpeed = 5f;

    private bool isChatting = false;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform playerTransform;
    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // --- LOGIQUE DE MÉMOIRE GLOBALE ---
            if (playOnlyOnce && DialogueManager.Instance.HasMetNpc(npcID))
            {
                return; // On a déjà parlé à ce PNJ précis, on ignore.
            }
            // ----------------------------------

            playerTransform = other.transform;
            StartDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndDialogue();
        }
    }

    private void StartDialogue()
    {
        isChatting = true;
        hasPlayed = true; // On marque comme joué dès le début du dialogue

        // On enregistre ce PNJ dans le registre global
        DialogueManager.Instance.MarkNpcAsMet(npcID);

        // 1. Arrêter la navigation
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // 2. Arrêter l'animation (Passage en Idle)
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
        }

        // 3. Lancer la rotation
        StopAllCoroutines();
        StartCoroutine(LookAtPlayer());

        DialogueManager.Instance.ShowDialogue(conversation, npcName);
    }

    private void EndDialogue()
    {
        isChatting = false;
        DialogueManager.Instance.HideDialogue();

        if (agent != null) agent.isStopped = false;
        if (anim != null) anim.SetFloat("Speed", 1f);
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

    private void Update()
    {
        if (isChatting && Input.GetMouseButtonDown(0))
        {
            DialogueManager.Instance.DisplayNextLine();
        }
    }
}
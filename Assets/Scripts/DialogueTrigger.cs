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
    private bool hasPlayedDefault = false; // On mémorise si le texte par défaut a été dit

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
            // On ne bloque plus ici, on laisse StartDialogue décider
            playerTransform = other.transform;
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        DialogueLine[] conversationToPlay = null;

        // 1. On cherche d'abord si une condition spéciale est remplie
        foreach (var cond in specializedDialogues)
        {
            if (DialogueManager.Instance.HasMetNpc(cond.requiredNpcID))
            {
                conversationToPlay = cond.conversation;
                break;
            }
        }

        // 2. Si aucune condition n'est remplie, on gère le dialogue par défaut
        if (conversationToPlay == null)
        {
            // Si on a déjà joué le dialogue par défaut ET que playOnlyOnce est coché : on stoppe.
            if (playOnlyOnce && hasPlayedDefault)
            {
                return;
            }

            conversationToPlay = defaultConversation;
            hasPlayedDefault = true; // On marque que le texte par défaut a été utilisé
        }

        // 3. Lancement effectif du dialogue choisi
        isChatting = true;
        DialogueManager.Instance.MarkNpcAsMet(npcID);

        if (agent != null) { agent.isStopped = true; agent.velocity = Vector3.zero; }
        if (anim != null) anim.SetFloat("Speed", 0f);

        StopAllCoroutines();
        StartCoroutine(LookAtPlayer());

        DialogueManager.Instance.ShowDialogue(conversationToPlay, npcName);
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndDialogue();
        }
    }
}
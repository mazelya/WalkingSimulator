using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public string npcName = "Guide";
    public DialogueLine[] conversation;

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

        // 1. Arrêter la navigation
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // 2. Arrêter l'animation (Passage en Idle)
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f); // Assure-toi que "Speed" est le nom du paramètre dans ton Animator
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

        if (agent != null) agent.isStopped = false; // Relance le NavMesh
        if (anim != null) anim.SetFloat("Speed", 1f); // Relance l'animation de marche
    }

    private IEnumerator LookAtPlayer()
    {
        while (isChatting && playerTransform != null)
        {
            // 1. On mémorise ton inclinaison X actuelle pour ne pas la perdre
            float originalX = transform.rotation.eulerAngles.x;
            float originalZ = transform.rotation.eulerAngles.z;

            // 2. On définit le point cible à la même hauteur que le PNJ
            Vector3 targetPoint = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);

            Vector3 direction = targetPoint - transform.position;

            if (direction.sqrMagnitude > 0.1f)
            {
                // 3. On calcule la rotation vers le joueur
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // 4. On lisse la rotation
                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                // 5. APPLICATION : On garde l'inclinaison X et Z d'origine
                // On ne change QUE l'axe Y (la direction vers laquelle il fait face)
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
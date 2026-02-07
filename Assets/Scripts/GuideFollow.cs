using UnityEngine;
using UnityEngine.AI;

public class GuideFollow : MonoBehaviour
{
    [Header("Réglages Suivi")]
    public Transform playerTransform;
    public float stoppingDistance = 3f;
    public float rotationSpeed = 5f;

    [Header("État")]
    public bool isFollowing = false; // Par défaut, il ne suit pas

    private NavMeshAgent agent;
    private Animator anim;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
    }

    void Update()
    {
        // Si le premier contact n'a pas eu lieu, on ne fait rien
        if (!isFollowing || playerTransform == null || agent == null || !agent.isOnNavMesh)
        {
            // On s'assure que l'animation de marche est à 0 s'il ne suit pas
            if (anim != null) anim.SetFloat("Speed", 0);
            return;
        }

        // 1. Destination = Joueur
        agent.SetDestination(playerTransform.position);

        // 2. Animations
        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        // 3. Regarder le joueur à l'arrêt
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            RotateTowardsPlayer();
        }
    }

    // Cette fonction est appelée par Unity quand le joueur entre dans le Collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isFollowing)
            {
                isFollowing = true;
                Debug.Log("Le guide commence à vous suivre !");
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float originalX = transform.rotation.eulerAngles.x;
            float originalZ = transform.rotation.eulerAngles.z;

            Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(originalX, newRot.eulerAngles.y, originalZ);
        }
    }
}
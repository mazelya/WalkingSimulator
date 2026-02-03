using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction lookAction;

    private Vector2 moveAmt;
    private Vector2 lookAmt;
    private Rigidbody rb;

    [Header("Movement (Joystick Gauche)")]
    public float WalkSpeed = 5f;

    [Header("Look (Joystick Droit)")]
    public Transform firstPersonCamera;
    public float RotateSpeed = 100f;
    public float LookUpDownSpeed = 80f;
    public float MinLookAngle = -80f;
    public float MaxLookAngle = 80f;

    private float xRotation = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Empêche la physique de faire tomber ou tourner le perso tout seul
        rb.freezeRotation = true;

        // On va chercher les actions spécifiquement par leur nom
        // Assure-toi que dans ton Input Action Asset, les noms sont exactement "Move" et "Look"
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    private void OnEnable() => InputSystem.actions.Enable();
    private void OnDisable() => InputSystem.actions.Disable();

    private void Update()
    {
        // 1. On lit les valeurs séparément
        moveAmt = moveAction.ReadValue<Vector2>();
        lookAmt = lookAction.ReadValue<Vector2>();

        // 2. On gère la rotation de la caméra ICI uniquement avec lookAmt (Stick Droit)
        HandleRotation();
    }

    private void FixedUpdate()
    {
        // 3. On gère le déplacement ICI uniquement avec moveAmt (Stick Gauche)
        HandleMovement();
    }

    private void HandleMovement()
    {
        // On calcule la direction (Seulement X et Z, pas de Y)
        Vector3 moveDirection = (transform.forward * moveAmt.y) + (transform.right * moveAmt.x);

        // Déplacement physique
        rb.MovePosition(rb.position + moveDirection * WalkSpeed * Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {
        // --- ROTATION HORIZONTALE (Tourner le corps du joueur) ---
        // On utilise lookAmt.x (Stick Droit Horizontal)
        float bodyRotation = lookAmt.x * RotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * bodyRotation);

        // --- ROTATION VERTICALE (Lever/Baisser la tête) ---
        // On utilise lookAmt.y (Stick Droit Vertical)
        xRotation -= lookAmt.y * LookUpDownSpeed * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, MinLookAngle, MaxLookAngle);

        // On applique uniquement à la caméra enfant
        firstPersonCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
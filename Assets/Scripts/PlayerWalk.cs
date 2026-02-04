using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Références")]
    public InputActionAsset InputActions;
    public Transform PlayerCamera;

    [Header("Réglages Mouvement (Joystick Gauche)")]
    public float WalkSpeed = 5f;

    [Header("Réglages Caméra (Joystick Droit)")]
    public float Sensitivity = 15f;
    public float MaxLookAngle = 80f;

    private InputAction moveAction;
    private InputAction lookAction;
    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation = 0f;

    private void Awake()
    {
        var playerMap = InputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Bloque TOUTE rotation physique
    }

    private void OnEnable() => InputActions.Enable();
    private void OnDisable() => InputActions.Disable();

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        // On traite la rotation dans l'Update pour la fluidité visuelle
        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // JOYSICK GAUCHE : Déplacement pur
        // On calcule un vecteur qui contient l'avant/arrière ET la gauche/droite
        Vector3 moveDirection = (transform.forward * moveInput.y) + (transform.right * moveInput.x);

        // Application du mouvement sur le Rigidbody
        rb.MovePosition(rb.position + moveDirection * WalkSpeed * Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {
        // JOYSTICK DROIT : Rotation pure

        // Rotation horizontale (Corps + Caméra)
        float horizontalRot = lookInput.x * Sensitivity * Time.deltaTime;
        transform.Rotate(0, horizontalRot, 0);

        // Rotation verticale (Caméra uniquement)
        verticalRotation -= lookInput.y * Sensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -MaxLookAngle, MaxLookAngle);

        if (PlayerCamera != null)
        {
            PlayerCamera.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }
}
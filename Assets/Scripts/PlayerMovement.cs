using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float mouseSensitivity = 100f; 

    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform cameraTransform;
    public AudioSource footsteps;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private bool movingLastFrame = false;

    private Vector3 knockbackVelocity = Vector3.zero;
    private float knockbackDecay = 5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity / 100f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity / 100f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move((velocity + knockbackVelocity) * Time.deltaTime);

        if (move.magnitude >= 0.001 && !movingLastFrame)
        {
            footsteps.Play();
        }
        else if (move.magnitude <= 0.001 && movingLastFrame)
        {
            footsteps.Stop();
        }
        if (move.magnitude <= 0.001) movingLastFrame = false;
        else movingLastFrame = true;        

        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
    }
    
    public void ApplyKnockback(Vector3 direction, float force)
    {
        direction.Normalize();
        direction.y = 1f;
        knockbackVelocity += direction * force;
    }
}
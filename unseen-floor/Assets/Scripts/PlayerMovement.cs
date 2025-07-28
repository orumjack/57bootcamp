// PlayerMovement.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] private float walkSpeed   = 3f;
    [SerializeField] private float sprintSpeed = 5.5f;

    public float WalkSpeed   => walkSpeed;
    public bool  IsRunning   { get; private set; }
    [Header("Mouse")]
    [SerializeField] private float lookSpeed = 2f;

    public float CurrentSpeed { get; private set; }   // <-- read by HeadBob / Audio
    public Vector3 Velocity   { get; private set; }   //  (magnitude & grounded)

    private CharacterController cc;
    private Transform cam;
    private float pitch;

    private void Awake ()
    {
        cc  = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>().transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update ()
    {
        HandleLook();
        HandleMove();
    }

    private void HandleLook ()
    {
        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(0f, mx, 0f);
        pitch = Mathf.Clamp(pitch - my, -80f, 80f);
        cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMove ()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.right * h + transform.forward * v).normalized;
        bool sprintKey  = Input.GetKey(KeyCode.LeftShift) && v > 0f;
        IsRunning       = sprintKey; 
        CurrentSpeed    = IsRunning ? sprintSpeed : walkSpeed;

        cc.SimpleMove(dir * CurrentSpeed);
        Velocity = cc.velocity; // world-space, includes gravity
    }
}

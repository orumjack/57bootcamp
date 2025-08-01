using UnityEngine;

/// <summary>
/// First-person controller: WASD movement with optional sprint and mouse look.
/// Other systems read <see cref="CurrentSpeed"/>, <see cref="Velocity"/>,
/// and <see cref="IsRunning"/>.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move Speeds")]
    [SerializeField] private float walkSpeed   = 3f;
    [SerializeField] private float sprintSpeed = 5.5f;

    [Header("Mouse Look")]
    [SerializeField] private float lookSpeed   = 2f;

    /* ───────── public read-only ── */
    public float WalkSpeed    => walkSpeed;
    public bool  IsRunning    { get; private set; }
    public float CurrentSpeed { get; private set; }
    public Vector3 Velocity   { get; private set; }   // world-space

    /* ───────── private fields ─── */
    CharacterController _cc;
    Transform           _cam;
    float               _pitch;

    /* ───────── life-cycle ─────── */
    void Awake()
    {
        _cc  = GetComponent<CharacterController>();
        _cam = GetComponentInChildren<Camera>().transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleLook();
        HandleMove();
    }

    /* ───────── input handlers ─── */
    void HandleLook()
    {
        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(0f, mx, 0f);

        _pitch = Mathf.Clamp(_pitch - my, -80f, 80f);
        _cam.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.right * h + transform.forward * v).normalized;

        IsRunning    = Input.GetKey(KeyCode.LeftShift) && v > 0f;
        CurrentSpeed = IsRunning ? sprintSpeed : walkSpeed;

        _cc.SimpleMove(dir * CurrentSpeed);
        Velocity = _cc.velocity;   // includes gravity
    }
}

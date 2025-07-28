using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class HeadBob : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] private float amplitudeWalk   = 0.03f;
    [SerializeField] private float amplitudeSprint = 0.06f;
    [SerializeField] private float frequencyWalk   = 6f;
    [SerializeField] private float frequencySprint = 9f;

    private PlayerMovement mover;
    private Transform cam;
    private Vector3  startLocalPos;
    private float    bobTimer;

    private void Awake ()
    {
        mover = GetComponent<PlayerMovement>();
        cam   = GetComponentInChildren<Camera>().transform;
        startLocalPos = cam.localPosition;
    }

    private void LateUpdate ()
    {
        if (mover.Velocity.sqrMagnitude > 0.1f)
        {
            // choose profile
            bool running = mover.IsRunning;      // ← clean & explicit
            float amp = running ? amplitudeSprint : amplitudeWalk;
            float freq = running ? frequencySprint : frequencyWalk;

            bobTimer += Time.deltaTime * freq;

            Vector3 offset = new Vector3
            (
                Mathf.Sin(bobTimer) * amp,
                Mathf.Cos(bobTimer * 2f) * amp,
                0f
            );

            cam.localPosition = startLocalPos + offset;
        }
        else
        {
            // return smoothly
            bobTimer = 0f;
            cam.localPosition = Vector3.Lerp
            (
                cam.localPosition,
                startLocalPos,
                Time.deltaTime * 5f
            );
        }
    }
}

using UnityEngine;

/// <summary>
/// Simple sine-wave head-bob for walking and sprinting.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[DisallowMultipleComponent]
public class HeadBob : MonoBehaviour
{
    [Header("Amplitude")]
    [SerializeField] private float amplitudeWalk   = 0.03f;
    [SerializeField] private float amplitudeSprint = 0.06f;

    [Header("Frequency (cycles/sec)")]
    [SerializeField] private float frequencyWalk   = 6f;
    [SerializeField] private float frequencySprint = 9f;

    /* ───────── private fields ─── */
    PlayerMovement _mover;
    Transform      _cam;
    Vector3        _startLocalPos;
    float          _bobTimer;

    /* ───────── life-cycle ─────── */
    void Awake()
    {
        _mover          = GetComponent<PlayerMovement>();
        _cam            = GetComponentInChildren<Camera>().transform;
        _startLocalPos  = _cam.localPosition;
    }

    /* ───────── after movement ─── */
    void LateUpdate()
    {
        if (_mover.Velocity.sqrMagnitude > 0.1f)
        {
            bool   running = _mover.IsRunning;
            float  amp     = running ? amplitudeSprint : amplitudeWalk;
            float  freq    = running ? frequencySprint : frequencyWalk;

            _bobTimer += Time.deltaTime * freq;

            Vector3 offset = new(
                Mathf.Sin(_bobTimer)       * amp,
                Mathf.Cos(_bobTimer * 2f)  * amp,
                0f);

            _cam.localPosition = _startLocalPos + offset;
        }
        else
        {
            _bobTimer = 0f;
            _cam.localPosition = Vector3.Lerp(
                _cam.localPosition,
                _startLocalPos,
                Time.deltaTime * 5f);
        }
    }
}

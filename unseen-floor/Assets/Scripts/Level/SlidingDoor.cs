using System.Collections;
using UnityEngine;

/// <summary>
/// Two-wing sliding door: the upper wing moves +Y, the lower wing –Y.<br/>
/// Distance can be auto-calculated from each wing’s Renderer height or set
/// manually; each wing has its own speed.
/// </summary>
[DisallowMultipleComponent]
public class SlidingDoor : MonoBehaviour
{
    [Header("Wing Transforms")]
    [Tooltip("Wing that moves upward (+Y)")]
    [SerializeField] private Transform upperWing;

    [Tooltip("Wing that moves downward (–Y)")]
    [SerializeField] private Transform lowerWing;

    [Header("Travel distance")]
    [Tooltip("If true, each wing travels by its own mesh height")]
    [SerializeField] private bool autoDistance = true;

    [Min(0f)]
    [Tooltip("Manual distance used when Auto Distance is false")]
    [SerializeField] private float manualDistance = 2f;

    [Header("Move speeds (units/sec)")]
    [SerializeField] private float upperSpeed = 3f;
    [SerializeField] private float lowerSpeed = 2f;

    /* ───────── private state ───── */
    Vector3 _upperClosed, _lowerClosed;
    Vector3 _upperOpen,   _lowerOpen;
    Coroutine _moveRoutine;

    /* ───────── life-cycle ─────── */
    void Awake()
    {
        float dist = autoDistance
            ? Mathf.Max(GetHeight(upperWing), GetHeight(lowerWing))
            : manualDistance;

        _upperClosed = upperWing.localPosition;
        _lowerClosed = lowerWing.localPosition;

        _upperOpen   = _upperClosed + Vector3.up * dist;
        _lowerOpen   = _lowerClosed - Vector3.up * dist;
    }

    /* ───────── public API ─────── */
    public void Open()  => MoveWings(_upperOpen,   _lowerOpen);
    public void Close() => MoveWings(_upperClosed, _lowerClosed);

    /* ───────── helpers ────────── */
    static float GetHeight(Transform t) =>
        t.TryGetComponent<Renderer>(out var r) ? r.bounds.size.y : 1f;

    void MoveWings(Vector3 uTarget, Vector3 lTarget)
    {
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveRoutine(uTarget, lTarget));
    }

    IEnumerator MoveRoutine(Vector3 uTarget, Vector3 lTarget)
    {
        while ((upperWing.localPosition - uTarget).sqrMagnitude > 0.0001f)
        {
            upperWing.localPosition = Vector3.MoveTowards(
                upperWing.localPosition, uTarget, upperSpeed * Time.deltaTime);

            lowerWing.localPosition = Vector3.MoveTowards(
                lowerWing.localPosition, lTarget, lowerSpeed * Time.deltaTime);

            yield return null;
        }
        _moveRoutine = null;
    }
}

using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Assign the two wing objects")]
    [SerializeField] private Transform upperWing;   // moves +Y
    [SerializeField] private Transform lowerWing;   // moves -Y

    [Header("Distance (auto if true)")]
    public bool   autoDistance = true;
    [Min(0f)] public float manualDistance = 2f;     // used if autoDistance = false

    [Header("Speeds (units / sec)")]
    public float upperSpeed = 3f;
    public float lowerSpeed = 2f;

    /* ───────── private ───────── */
    Vector3 upperClosed, lowerClosed;
    Vector3 upperOpen,   lowerOpen;
    Coroutine routine;

    void Awake()
    {
        float dist = autoDistance
            ? Mathf.Max(GetHeight(upperWing), GetHeight(lowerWing))
            : manualDistance;

        upperClosed = upperWing.localPosition;
        lowerClosed = lowerWing.localPosition;

        upperOpen   = upperClosed + Vector3.up * dist;
        lowerOpen   = lowerClosed - Vector3.up * dist;
    }

    static float GetHeight(Transform t) =>
        t.TryGetComponent<Renderer>(out var r)
            ? r.bounds.size.y
            : 1f;   // fallback

    /* ───────── API ───────── */
    public void Open()  => MoveWings(upperOpen,   lowerOpen);
    public void Close() => MoveWings(upperClosed, lowerClosed);

    void MoveWings(Vector3 uTarget, Vector3 lTarget)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveRoutine(uTarget, lTarget));
    }

    IEnumerator MoveRoutine(Vector3 uTarget, Vector3 lTarget)
    {
        while ((upperWing.localPosition - uTarget).sqrMagnitude > 0.001f)
        {
            upperWing.localPosition = Vector3.MoveTowards(
                upperWing.localPosition, uTarget, upperSpeed * Time.deltaTime);

            lowerWing.localPosition = Vector3.MoveTowards(
                lowerWing.localPosition, lTarget, lowerSpeed * Time.deltaTime);

            yield return null;
        }
        routine = null;
    }
}

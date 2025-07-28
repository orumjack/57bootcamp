using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to the Box-Collider trigger that lives in Capsule B’s doorway.
/// • Anchor_A  → doorway centre in Capsule A (child of Capsule A)
/// • Anchor_B  → identical doorway centre in Capsule B (child of Capsule B)
///
/// Both anchors must share identical *local* position inside their capsules,
/// but their Y-rotation can differ (0° vs 90°, etc.).  This script copies the
/// player’s local offset into Capsule A **and** sets yaw to Anchor_A’s yaw
/// while keeping the original pitch.
/// </summary>
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class CapsuleTeleporter : MonoBehaviour
{
    [Header("Doorway Anchors (drag from Hierarchy)")]
    [SerializeField] private Transform anchorA;   // doorway in Capsule A
    [SerializeField] private Transform anchorB;   // doorway in Capsule B

    private bool coolingDown;

    private void OnTriggerEnter(Collider other)
    {
        if (coolingDown)                 return;  // already mid-warp
        if (!other.CompareTag("Player")) return;

        StartCoroutine(WarpCoroutine(other.transform));
    }

    private IEnumerator WarpCoroutine(Transform playerRoot)
    {
        coolingDown = true;

        // cache comps once
        var mover = playerRoot.GetComponent<PlayerMovement>();
        var cc    = playerRoot.GetComponent<CharacterController>();

        /*── 1. pause movement one physics tick ────────────────────*/
        if (mover) mover.enabled = false;
        if (cc)    cc.enabled    = false;
        yield return new WaitForFixedUpdate();

        /*── 2. POSITION: copy local offset B → A ──────────────────*/
        Vector3 localPos = anchorB.InverseTransformPoint(playerRoot.position);
        playerRoot.position = anchorA.TransformPoint(localPos);

        /*── 2b. ORIENTATION: yaw = Anchor_A, keep original pitch ──*/
        float pitch = playerRoot.eulerAngles.x;                 // look-up value
        float yaw   = anchorA.eulerAngles.y;                    // corridor heading
        playerRoot.rotation = Quaternion.Euler(pitch, yaw, 0f); // no roll

        /*── 3. (optional) trigger doors / anomalies / counters ───*/
        // DoorManager.Instance.OnLoopComplete();
        // AnomalyManager.Instance.StartNewLoop();

        /*── 4. resume control next physics tick ──────────────────*/
        yield return new WaitForFixedUpdate();
        if (cc)    cc.enabled    = true;
        if (mover) mover.enabled = true;

        /*── 5. debounce trigger for 0.5 s ────────────────────────*/
        yield return new WaitForSeconds(0.5f);
        coolingDown = false;
    }
}

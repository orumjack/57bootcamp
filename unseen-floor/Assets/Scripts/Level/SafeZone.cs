using UnityEngine;

/// <summary>
/// Trigger on each capsule doorway that pauses the timer, scores the last
/// corridor decision, and closes/opens doors as needed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SafeZone : MonoBehaviour
{
    public enum CapsuleType { A, B }

    [Header("Capsule Settings")]
    [SerializeField] private CapsuleType capsule = CapsuleType.A;

    /* ───────── enter capsule ───── */
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var gs  = GameSession.I;
        var dec = DecisionManager.I;

        /* pause timer & flag that a Bank/Risk choice is expected */
        gs.PauseTimer();
        gs.EnterSafeZone();

        /* close Door B immediately when stepping into Capsule B */
        if (capsule == CapsuleType.B)
            DoorManager.I.CloseDoorB();

        /* evaluate corridor decision */
        bool correct = true;   // default when player skipped a click

        if (dec.DecisionIsMade)
        {
            correct = dec.PlayerChoseAnomaly ==
                      AnomalyManager.Instance.HasAnomalyThisLoop;

            if (!correct)
                gs.OnWrongDecision();   // instant pot→bank deduction
        }

        gs.SetDecisionResult(correct);

        /* re-arm click gate for the next corridor */
        dec.ResetForNewLoop();
    }

    /* ───────── leave capsule A ─── */
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (capsule == CapsuleType.A)
        {
            DoorManager.I.CloseDoorA();
            GameSession.I.BeginLoopTimer();   // resume countdown
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SafeZone : MonoBehaviour
{
    public enum CapsuleType { A, B }
    [SerializeField] private CapsuleType capsule = CapsuleType.A;

    /*───────────────────────────────*/
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var gm  = GameSession.I;
        var dec = DecisionManager.Instance;

        /* 1 ▸ pause countdown, mark safe zone */
        gm.PauseTimer();
        gm.EnterSafeZone();

        /* 2 ▸ B-capsule door closes as soon as you step in */
        if (capsule == CapsuleType.B)
            DoorManager.I.CloseDoorB();

        /* 3 ▸ evaluate the corridor decision */
        bool correct;
        if (dec.DecisionIsMade)
        {
            correct = dec.PlayerChoseAnomaly ==
                      AnomalyManager.Instance.HasAnomalyThisLoop;

            if (!correct)
                gm.OnWrongDecision();        // ← immediate pot-to-bank deduction
        }
        else
        {
            correct = true;                  // no click = neutral success
        }

        gm.SetDecisionResult(correct);

        /* 4 ▸ re-arm decision gate for next corridor */
        dec.ResetForNewLoop();
        // Door A closes & timer restarts in OnTriggerExit (Capsule A).
    }

    /*───────────────────────────────*/
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (capsule == CapsuleType.A)
        {
            DoorManager.I.CloseDoorA();
            GameSession.I.BeginLoopTimer();   // corridor timer resumes
        }
    }
}

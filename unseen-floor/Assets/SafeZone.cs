using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var gm  = GameSession.I;
        var dec = DecisionManager.Instance;

        gm.PauseTimer();          // stop countdown inside capsule
        gm.EnterSafeZone();       // AwaitingChoice = true

        /* evaluate last corridor decision (or default-success if none) */
        bool correct;
        if (dec.DecisionIsMade)
        {
            correct = dec.PlayerChoseAnomaly ==
                      AnomalyManager.Instance.HasAnomalyThisLoop;
        }
        else
        {
            correct = true;       // no click this loop → treat as neutral success
        }

        gm.SetDecisionResult(correct);

        dec.ResetForNewLoop();    // re-arm decision gate for next corridor
        // Doors & gm.BeginLoopTimer() will run after the player picks Bank/Risk.
    }
}

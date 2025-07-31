using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var gm  = GameSession.I;
        var dec = DecisionManager.Instance;

        gm.PauseTimer();        // stop countdown while in capsule
        gm.EnterSafeZone();     // AwaitingChoice = true

        /* evaluate the corridor decision */
        if (dec.DecisionIsMade)
        {
            bool correct = dec.PlayerChoseAnomaly ==
                           AnomalyManager.Instance.HasAnomalyThisLoop;

            gm.SetDecisionResult(correct);   // <- SINGLE new call
            // (No ApplySuccess / ResetPotOnly here anymore)
        }

        dec.ResetForNewLoop();  // ready input gate for next corridor
        // Doors & BeginLoopTimer are triggered by Bank / Risk choice.
    }
}

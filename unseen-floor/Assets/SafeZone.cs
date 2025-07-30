using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var gm  = GameSession.I;
        var dec = DecisionManager.Instance;

        gm.PauseTimer();                    // pause countdown in Safe Zone

        /* evaluate last corridor decision */
        if (dec.DecisionIsMade)
        {
            bool correct = dec.PlayerChoseAnomaly ==
                           AnomalyManager.Instance.HasAnomalyThisLoop;

            if (correct)
            {
                gm.ApplySuccess();          // boost pot, shorten timer
                // <<< Patch D will handle Bank/Risk choice here >>>
            }
            else
            {
                gm.ResetPotOnly();          // pot lost, bank preserved
            }
        }

        /* prepare for next corridor run */
        dec.ResetForNewLoop();

        // Door open/close & gm.ResumeTimer() will be handled
        // when the player exits the capsule (next patch).
    }
}

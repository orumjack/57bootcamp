using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ▶ Patch E-1: reset all Bank/Risk props when entering any Capsule
        foreach (var choice in FindObjectsOfType<ChoiceInteractable>())
            choice.gameObject.SetActive(true);

        var gm  = GameSession.I;
        var dec = DecisionManager.Instance;

        gm.PauseTimer();           // pause countdown in Safe Zone
        gm.EnterSafeZone();        // flag AwaitingChoice & keep timer paused

        /* evaluate last corridor decision */
        if (dec.DecisionIsMade)
        {
            bool correct = dec.PlayerChoseAnomaly ==
                           AnomalyManager.Instance.HasAnomalyThisLoop;

            // remember the result for later Bank/Risk choice
            gm.SetDecisionResult(correct);
        }

        /* prepare for next corridor run */
        dec.ResetForNewLoop();

        // Door open/close & gm.BeginLoopTimer() happen later,
        // once the player actually chooses Bank or Risk.
    }
}

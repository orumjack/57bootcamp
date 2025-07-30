using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChoiceInteractable : MonoBehaviour
{
    public enum ChoiceType { Bank, Risk }
    [SerializeField] private ChoiceType type;

    [Tooltip("Optional glow/light shown when this choice is available.")]
    [SerializeField] private GameObject highlight;

    private bool playerInRange;

    /* ───────────────────────────────────────────────────────── */

    private void Update()
    {
        // nothing to do: player far away or no choice needed
        if (!playerInRange || !GameSession.I.AwaitingChoice) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (type)
            {
                case ChoiceType.Bank:
                    GameSession.I.ChooseBank();        // pot → bank
                    break;

                case ChoiceType.Risk:
                    GameSession.I.OnChooseRisk();      // keep / shrink timer
                    break;
            }

            DoorManager.I.OpenCapsuleDoor();           // let player exit
            GameSession.I.BeginLoopTimer();            // resume countdown
            DecisionManager.Instance.ResetForNewLoop();

            // deactivate highlight & input until next safe-zone arrival
            if (highlight) highlight.SetActive(false);
            playerInRange = false;
        }
    }

    /* ───────────────────────────────────────────────────────── */

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        if (GameSession.I.AwaitingChoice && highlight)
            highlight.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;

        if (highlight) highlight.SetActive(false);
    }
}

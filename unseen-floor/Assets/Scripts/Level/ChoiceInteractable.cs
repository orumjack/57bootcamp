using UnityEngine;

/// <summary>
/// Interactable pedestal that lets the player press **E** to choose Bank or Risk
/// once inside a capsule safe zone.
/// </summary>
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class ChoiceInteractable : MonoBehaviour
{
    public enum ChoiceType { Bank, Risk }

    [Header("Choice Settings")]
    [SerializeField] private ChoiceType type = ChoiceType.Bank;

    [Tooltip("Optional glow/light shown while the choice is available")]
    [SerializeField] private GameObject highlight;

    /* ───────── private state ─── */
    bool _playerInRange;

    /* ───────── frame tick ─────── */
    void Update()
    {
        if (!_playerInRange || !GameSession.I.AwaitingChoice) return;
        if (!Input.GetKeyDown(KeyCode.E))                 return;

        switch (type)
        {
            case ChoiceType.Bank: GameSession.I.ChooseBank();  break;
            case ChoiceType.Risk: GameSession.I.OnChooseRisk(); break;
        }

        DoorManager.I.OpenDoorA();      // let player exit capsule A
        GameSession.I.BeginLoopTimer(); // resume corridor countdown
        DecisionManager.I.ResetForNewLoop();

        if (highlight) highlight.SetActive(false);
        _playerInRange = false;
    }

    /* ───────── trigger events ─── */
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = true;

        if (GameSession.I.AwaitingChoice && highlight)
            highlight.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;

        if (highlight) highlight.SetActive(false);
    }
}

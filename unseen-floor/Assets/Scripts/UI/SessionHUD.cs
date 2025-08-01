using UnityEngine;
using TMPro;

/// <summary>
/// Heads-up display that shows current Bank, Pot, and timer values every frame.
/// Drag three <see cref="TMP_Text"/> objects into the Inspector slots.
/// </summary>
[DisallowMultipleComponent]
public class SessionHUD : MonoBehaviour
{
    [Header("Text References")]
    [Tooltip("Displays Total Banked")]
    [SerializeField] private TMP_Text bankedTxt;

    [Tooltip("Displays Current Pot")]
    [SerializeField] private TMP_Text potTxt;

    [Tooltip("Displays Countdown Timer")]
    [SerializeField] private TMP_Text timerTxt;

    /* ───────── private fields ─── */
    GameSession _gs;

    /* ───────── life-cycle ─────── */
    void Start()
    {
        _gs = GameSession.I;

        if (!_gs)
            Debug.LogError("[SessionHUD] GameSession singleton not found.");

        if (!bankedTxt || !potTxt || !timerTxt)
            Debug.LogError("[SessionHUD] One or more Text references missing.");
    }

    /* ───────── frame tick ─────── */
    void Update()
    {
        if (!_gs) return;

        bankedTxt.text = $"Banked:  ${_gs.TotalBanked}";
        potTxt.text    = $"Pot:     ${_gs.CurrentPayout}";
        timerTxt.text  = $"Timer:   {_gs.TimerSec}s";
    }
}

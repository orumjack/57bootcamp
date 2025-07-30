using UnityEngine;
using TMPro;                // remove if using legacy Text
using UnityEngine.UI;       // needed only for legacy Text

public class SessionHUD : MonoBehaviour
{
    [Header("Drag the three Text objects here")]
    [SerializeField] private TMP_Text bankedTxt;
    [SerializeField] private TMP_Text potTxt;
    [SerializeField] private TMP_Text timerTxt;

    private GameSession gs;

    private void Start()    // ← changed from Awake()
    {
        gs = GameSession.I;
        if (gs == null) Debug.LogError("SessionHUD: GameSession singleton not found!");
        if (!bankedTxt || !potTxt || !timerTxt)
            Debug.LogError("SessionHUD: One or more Text references not assigned.");
    }

    private void Update()
    {
        if (!gs) return;   // graceful fail if singleton missing

        bankedTxt.text = $"Banked:  ${gs.TotalBanked}";
        potTxt.text    = $"Pot:     ${gs.CurrentPayout}";
        timerTxt.text  = $"Timer:   {gs.TimerSec}s";
    }
}

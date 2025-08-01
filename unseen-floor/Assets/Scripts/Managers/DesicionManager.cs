using UnityEngine;

/// <summary>
/// Captures the single LMB/RMB corridor decision each loop.<br/>
/// • LMB → claim “no anomaly” • RMB → claim “anomaly”<br/>
/// Ignores clicks inside capsules or after a decision is made.
/// </summary>
[DisallowMultipleComponent]
public class DecisionManager : MonoBehaviour
{
    /* ───────── singleton ───────── */
    public static DecisionManager I        { get; private set; }
    public static DecisionManager Instance => I;   // legacy alias

    /* ───────── public read-only ── */
    public bool DecisionIsMade     => _decisionMade;
    public bool PlayerChoseAnomaly { get; private set; }   // true = RMB

    /* ───────── private fields ─── */
    private bool _decisionMade;

    /* ───────── life-cycle ─────── */
    private void Awake ()
    {
        if (I && I != this) { Destroy(this); return; }
        I = this;
    }

    /* ───────── per-frame input ── */
    private void Update ()
    {
        if (!GameSession.I.IsTimerRunning || _decisionMade) return;

        bool lmb = Input.GetMouseButtonDown(0);
        bool rmb = Input.GetMouseButtonDown(1);
        if (!lmb && !rmb) return;

        PlayerChoseAnomaly = rmb;
        _decisionMade      = true;

        GameSession.I.PauseTimer();
        DoorManager.I.OpenDoorB();

        Debug.Log($"<color=cyan>[Decision]</color> {(rmb ? "ANOMALY" : "CLEAN")} chosen");
    }

    /* ───────── reset each loop ─ */
    public void ResetForNewLoop ()
    {
        _decisionMade      = false;
        PlayerChoseAnomaly = false;
    }
}

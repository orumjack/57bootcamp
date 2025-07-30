using UnityEngine;

[DisallowMultipleComponent]
public class DecisionManager : MonoBehaviour
{
    /*──────────────── singleton ───────────────*/
    public static DecisionManager Instance { get; private set; }

    /*──────────────── public read-only API ─────*/
    public bool DecisionIsMade     => decisionMade;      // SafeZone queries
    public bool PlayerChoseAnomaly { get; private set; } // true = RMB, false = LMB

    /*──────────────── private state ────────────*/
    private bool decisionMade;                          // gate → only one click / loop

    /*──────────────── life-cycle ───────────────*/
    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    private void Update()
    {
        if (decisionMade) return;                      // already decided this loop

        /*──── read player input ────*/
        bool lmb = Input.GetMouseButtonDown(0);        // “clean” claim
        bool rmb = Input.GetMouseButtonDown(1);        // “anomaly” claim
        if (!lmb && !rmb) return;                      // no click this frame

        /*──── store decision ───────*/
        PlayerChoseAnomaly = rmb;                      // RMB = anomaly, LMB = clean
        decisionMade       = true;

        /*──── immediate debug log (optional) ──*/
        bool correct = PlayerChoseAnomaly == AnomalyManager.Instance.HasAnomalyThisLoop;
        Debug.Log(correct ? "<color=green>✓ Decision recorded</color>"
                          : "<color=yellow>✗ Decision recorded</color>");
        /*  Final scoring happens in SafeZone.OnTriggerEnter */
    }

    /*──────────────── called by SafeZone / Teleporter ─*/
    public void ResetForNewLoop()
    {
        decisionMade       = false;
        PlayerChoseAnomaly = false;
    }
}

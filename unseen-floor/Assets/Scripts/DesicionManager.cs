using UnityEngine;

[DisallowMultipleComponent]
public class DecisionManager : MonoBehaviour
{
    public static DecisionManager Instance { get; private set; }

    public bool DecisionIsMade     => decisionMade;
    public bool PlayerChoseAnomaly { get; private set; }

    private bool decisionMade;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    private void Update()
    {
        /* 1 ▸ ignore input unless corridor timer is running */
        if (!GameSession.I.IsTimerRunning) return;
        if (decisionMade)                 return;

        bool lmb = Input.GetMouseButtonDown(0);     // claim: no anomaly
        bool rmb = Input.GetMouseButtonDown(1);     // claim: anomaly
        if (!lmb && !rmb) return;

        /* 2 ▸ record decision */
        PlayerChoseAnomaly = rmb;
        decisionMade       = true;

        /* 3 ▸ stop timer for the remainder of this loop */
        GameSession.I.PauseTimer();
        DoorManager.I.OpenDoorB();

        /* 4 ▸ optional debug echo */
        Debug.Log($"Decision recorded: {(rmb ? "ANOMALY" : "CLEAN")}");
    }

    public void ResetForNewLoop()
    {
        decisionMade       = false;
        PlayerChoseAnomaly = false;
    }
}

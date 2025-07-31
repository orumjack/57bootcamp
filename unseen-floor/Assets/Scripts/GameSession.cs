using UnityEngine;

/// <summary>
/// Run-wide economy (bank, pot, streak) + per-loop countdown.
/// Exposed read-only properties feed the HUD and gameplay checks.
/// </summary>
public class GameSession : MonoBehaviour
{
    /* ───────── singleton ───────── */
    public static GameSession I { get; private set; }

    [Header("Tunables (drag LoopParams asset here)")]
    [SerializeField] private LoopParams p;

    /* ───────── public read-only ─── */
    public int TotalBanked { get; private set; }
    public int CurrentPayout { get; private set; }
    public int Streak { get; private set; }
    public int TimerSec => Mathf.CeilToInt(timer);

    public bool AwaitingChoice { get; private set; }
    public bool LastDecisionCorrect { get; private set; }

    /* ───────── private fields ───── */
    private float timer;
    private bool timerRunning;
    private bool prizeSpawned;

    /* ───────── life-cycle ───────── */
    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        ResetToBase();          // base pot / timer / streak
    }

    /* ───────── public API ───────── */

    #region Safe-zone & decision flow
    public void EnterSafeZone()
    {
        timerRunning = false;
        AwaitingChoice = true;
    }

    public void SetDecisionResult(bool correct)
    {
        LastDecisionCorrect = correct;
    }

    public void ChooseBank()
    {
        if (!AwaitingChoice) return;

        TotalBanked += CurrentPayout;
        ResetToBase();
        AwaitingChoice = false;

        CheckWin();
    }

    public void OnChooseRisk()
    {
        if (!AwaitingChoice) return;

        if (LastDecisionCorrect) ApplySuccess();
        else OnWrongDecision();

        AwaitingChoice = false;
        CheckWin();
    }
    #endregion

    #region Economy helpers
    /// <summary>Resets pot, timer, streak to starting values.</summary>
    public void ResetToBase()
    {
        CurrentPayout = p.BASE_PAYOUT;
        timer = p.START_TIMER_SEC;
        timerRunning = true;
        Streak = 0;
    }

    /// <summary>Wrong corridor guess.</summary>
    public void OnWrongDecision()
    {
        if (p.BANK_CAN_GO_NEGATIVE)
            TotalBanked -= CurrentPayout;            // can dip below zero
        else
            TotalBanked = Mathf.Max(0, TotalBanked - CurrentPayout);
    }

    /// <summary>Successful risk loop: grow pot & shrink timer.</summary>
    public void ApplySuccess()
    {
        Streak++;

        CurrentPayout = Mathf.FloorToInt(
            CurrentPayout * p.MULTIPLIER_MEAN *
            Random.Range(1f - p.MULTIPLIER_VARIANCE,
                         1f + p.MULTIPLIER_VARIANCE));

        timer = Mathf.CeilToInt(
            timer * p.TIMER_MULTIPLIER - p.TIMER_FLAT_DECREMENT);

        timerRunning = true;
    }

    public void ResetPotOnly()
    {
        CurrentPayout = p.BASE_PAYOUT;
        Streak = 0;
        timerRunning = false;          // paused until corridor restarts
    }
    #endregion

    #region Timer control
    public void BeginLoopTimer() => timerRunning = true;
    public void PauseTimer() => timerRunning = false;

    public void TickTimer(float delta)
    {
        if (!timerRunning) return;

        timer = Mathf.Max(0f, timer - delta);

        if (timer <= 0f)
        {
            timerRunning = false;
            Debug.Log("<color=red>✖ Timer expired — YOU LOSE</color>");
            // TODO: lose overlay or scene reset
        }
    }
    #endregion

    #region Win check (simple log)

    private bool winAnnounced;


    private void CheckWin()
    {
        if (winAnnounced) return;
        if (TotalBanked < p.TARGET_SCORE) return;

        Debug.Log("<color=lime>■ YOU WIN — target score reached!</color>");
        winAnnounced = true;
        // Optionally pause the game:
        // Time.timeScale = 0;
    }

    #endregion
}

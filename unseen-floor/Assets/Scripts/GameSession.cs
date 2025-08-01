using UnityEngine;

/// <summary>
/// Run-wide economy (bank, pot, streak) + per-loop countdown timer.
/// </summary>
public class GameSession : MonoBehaviour
{
    /* ───────── singleton ───────── */
    public static GameSession I { get; private set; }

    [Header("Tunables (drag LoopParams asset here)")]
    [SerializeField] private LoopParams p;

    /* ───────── public read-only ─── */
    public int  TotalBanked   { get; private set; }
    public int  CurrentPayout { get; private set; }
    public int  Streak        { get; private set; }
    public int  TimerSec      => Mathf.CeilToInt(timer);

    public bool AwaitingChoice      { get; private set; }
    public bool LastDecisionCorrect { get; private set; }
    public bool IsTimerRunning      => timerRunning;

    /* ───────── private fields ───── */
    float timer;
    bool  timerRunning;
    bool  winAnnounced;

    /* ───────── life-cycle ───────── */
    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        ResetToBase();
    }

    /* ───────── safe-zone flow ───── */
    public void EnterSafeZone()
    {
        timerRunning  = false;
        AwaitingChoice = true;
    }

    public void SetDecisionResult(bool correct) => LastDecisionCorrect = correct;

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

        if (LastDecisionCorrect)
            ApplySuccess();       // grow pot & shrink timer
        /* else: deduction already handled in SafeZone.OnTriggerEnter() */

        AwaitingChoice = false;
        CheckWin();
    }

    /* ───────── economy helpers ─── */
    public void ResetToBase()
    {
        CurrentPayout = p.BASE_PAYOUT;
        timer         = p.START_TIMER_SEC;
        timerRunning  = true;
        Streak        = 0;
    }

    /// <summary>Wrong corridor guess or timer expiry.</summary>
    public void OnWrongDecision()
    {
        if (p.BANK_CAN_GO_NEGATIVE)
            TotalBanked -= CurrentPayout;
        else
            TotalBanked = Mathf.Max(0, TotalBanked - CurrentPayout);

        ResetPotOnly();
    }

    public void ApplySuccess()
    {
        Streak++;

        CurrentPayout = Mathf.FloorToInt(
            CurrentPayout * p.MULTIPLIER_MEAN *
            Random.Range(1f - p.MULTIPLIER_VARIANCE,
                         1f + p.MULTIPLIER_VARIANCE));

        timer = Mathf.CeilToInt(
            timer * p.TIMER_MULTIPLIER - p.TIMER_FLAT_DECREMENT);
    }

    void ResetPotOnly()
    {
        CurrentPayout = p.BASE_PAYOUT;
        Streak        = 0;
        timerRunning  = false;         // resumes when Door A closes
    }

    /* ───────── timer control ───── */
    public void BeginLoopTimer() => timerRunning = true;
    public void PauseTimer()     => timerRunning = false;

    public void TickTimer(float delta)
    {
        if (!timerRunning) return;

        timer = Mathf.Max(0f, timer - delta);

        if (timer <= 0f)
        {
            timerRunning = false;
            Debug.Log("<color=red>✖ Timer expired — treated as wrong decision</color>");

            OnWrongDecision();        // single deduction & reset
            DoorManager.I.OpenDoorB(); // open B to prevent soft-lock
        }
    }

    /* ───────── win check ───────── */
    void CheckWin()
    {
        if (winAnnounced)                  return;
        if (TotalBanked < p.TARGET_SCORE)  return;

        winAnnounced = true;
        Debug.Log("<color=lime>■ YOU WIN — target score reached!</color>");
        // Time.timeScale = 0;  // uncomment to pause the game
    }
}

using UnityEngine;

/// <summary>
/// Handles the entire run-state economy (Bank, Pot, Streak) and the
/// per-loop countdown timer.
/// </summary>
public class GameSession : MonoBehaviour
{
    /* ───────── singleton ───────── */
    public static GameSession I { get; private set; }

    [Header("Design-time parameters")]
    [Tooltip("ScriptableObject that defines payouts, timer maths, etc.")]
    [SerializeField] private LoopParams p;

    /* ───────── public read-only ─── */
    public int  TotalBanked   { get; private set; }
    public int  CurrentPayout { get; private set; }
    public int  Streak        { get; private set; }
    public int  TimerSec      => Mathf.CeilToInt(_timer);

    public bool AwaitingChoice      { get; private set; }
    public bool LastDecisionCorrect { get; private set; }
    public bool IsTimerRunning      => _timerRunning;

    /* ───────── private fields ───── */
    float _timer;
    bool  _timerRunning;
    bool  _winAnnounced;

    /* ───────── life-cycle ──────── */
    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        ResetToBase();
    }

    /* ───────── safe-zone flow ──── */
    public void EnterSafeZone()
    {
        _timerRunning  = false;
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

        if (LastDecisionCorrect) ApplySuccess();   // grow pot & shrink timer
        // else: wrong decision already handled in SafeZone

        AwaitingChoice = false;
        CheckWin();
    }

    /* ───────── economy helpers ─── */
    public void ResetToBase()
    {
        CurrentPayout = p.BASE_PAYOUT;
        _timer        = p.START_TIMER_SEC;
        _timerRunning = true;
        Streak        = 0;
    }

    /// <summary>Wrong corridor guess OR timer expiry.</summary>
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

        _timer = Mathf.CeilToInt(
            _timer * p.TIMER_MULTIPLIER - p.TIMER_FLAT_DECREMENT);
    }

    void ResetPotOnly()          // private helper
    {
        CurrentPayout = p.BASE_PAYOUT;
        Streak        = 0;
        _timerRunning = false;   // resumes when Door A closes
    }

    /* ───────── timer control ───── */
    public void BeginLoopTimer() => _timerRunning = true;
    public void PauseTimer()     => _timerRunning = false;

    public void TickTimer(float delta)
    {
        if (!_timerRunning) return;

        _timer = Mathf.Max(0f, _timer - delta);

        if (_timer <= 0f)
        {
            _timerRunning = false;
            Debug.Log("<color=red>[Timer] Expired — treated as wrong decision</color>");

            OnWrongDecision();           // single deduction & reset
            DoorManager.I.OpenDoorB();   // open B to prevent soft-lock
        }
    }

    /* ───────── win check ───────── */
    void CheckWin()
    {
        if (_winAnnounced)                 return;
        if (TotalBanked < p.TARGET_SCORE)  return;

        _winAnnounced = true;
        Debug.Log("<color=lime>[Economy] ■ YOU WIN — target score reached!</color>");
        // Time.timeScale = 0;  // uncomment to pause the game
    }

    /* ───────── frame tick ─────── */
    void Update() => TickTimer(Time.deltaTime);
}

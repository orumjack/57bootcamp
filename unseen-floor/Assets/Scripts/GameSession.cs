using UnityEngine;

/// <summary>
/// Holds the run-wide economy (bank, pot, streak) and the per-loop countdown timer.
/// Exposed read-only properties feed the HUD and gameplay checks.
/// </summary>
public class GameSession : MonoBehaviour
{
    /*──────── singleton convenience ─────────*/
    public static GameSession I { get; private set; }

    [Header("Tunables (drag LoopParams asset here)")]
    [SerializeField] private LoopParams p;

    /*──────── public read-only state ────────*/
    public int TotalBanked { get; private set; }
    public int CurrentPayout { get; private set; }
    public int Streak { get; private set; }
    public int TimerSec => Mathf.CeilToInt(timer);   // HUD reads this

    /*──────── private timer fields ─────────*/
    private float timer;          // float seconds for smooth ticking
    private bool timerRunning;   // gate so we can pause during capsules

    /*──────────────── life-cycle ───────────*/
    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        ResetToBase();            // initialise pot / timer / streak
    }

    /*──────────────── core API ─────────────*/

    /// <summary>Resets pot, timer and streak to starting values.</summary>
    public void ResetToBase()
    {
        CurrentPayout = p.BASE_PAYOUT;
        timer = p.START_TIMER_SEC;
        timerRunning = true;                 // timer counts as soon as run starts
        Streak = 0;
    }

    /// <summary>Call after a successful risk-decision.</summary>
    public void ApplySuccess()
    {
        Streak++;

        // Increase pot with random multiplier
        CurrentPayout = Mathf.FloorToInt(
            CurrentPayout * p.MULTIPLIER_MEAN *
            Random.Range(1f - p.MULTIPLIER_VARIANCE,
                         1f + p.MULTIPLIER_VARIANCE));

        // Shrink timer for next loop
        timer = Mathf.CeilToInt(
            timer * p.TIMER_MULTIPLIER - p.TIMER_FLAT_DECREMENT);

        timerRunning = true;      // keep countdown active
    }

    /// <summary>Current probability of success for next risk roll.</summary>
    public float CurrentSuccessProb()
    {
        int extra = Mathf.Max(0, Streak - p.SUCCESS_PROB_DROP_THRESHOLD);
        return p.SUCCESS_PROB_START - extra * p.SUCCESS_PROB_DROP_PER_STREAK;
    }

    /// <summary>Move pot to bank and reset economy.</summary>
    public void BankPot()
    {
        TotalBanked += CurrentPayout;
        ResetToBase();
    }

    public void ResetPotOnly()
    {
        CurrentPayout = p.BASE_PAYOUT;
        Streak = 0;
        timerRunning = false;          // stop countdown in safe zone
    }

    /*──────── timer control ────────────────*/

    /// <summary>Called by CapsuleTeleporter when the new loop starts.</summary>
    public void BeginLoopTimer() => timerRunning = true;

    /*──────── external timer gates ───────*/
    public void PauseTimer() => timerRunning = false;
    public void ResumeTimer() => timerRunning = true;


    /// <summary>Tick down the countdown; call once per frame.</summary>
    public void TickTimer(float delta)
    {
        if (!timerRunning) return;

        timer = Mathf.Max(0f, timer - delta);

        if (timer <= 0f)
        {
            timerRunning = false;
            Debug.Log("<color=red>✖ Timer expired — YOU LOSE</color>");
            // TODO: trigger lose overlay or reset scene
        }
    }

    public bool AwaitingChoice { get; private set; }   // exposed for HUD if needed

    public void EnterSafeZone()
    {
        timerRunning = false;
        AwaitingChoice = true;
    }

    public void ChooseBank()
    {
        if (!AwaitingChoice) return;
        TotalBanked += CurrentPayout;
        ResetToBase();                 // timer = START_TIMER_SEC
        AwaitingChoice = false;
    }

    public void ChooseRisk()
    {
        if (!AwaitingChoice) return;
        ApplySuccess();                // increases pot, shrinks timer
        AwaitingChoice = false;
    }
    
    /*  add to fields */
public bool LastDecisionCorrect { get; private set; }   // read by interactable

/* called by SafeZone AFTER scoring anomaly */
public void SetDecisionResult(bool correct)
{
    LastDecisionCorrect = correct;
}

/* helper used by ChoiceInteractable */
public void OnChooseRisk()
{
    if (LastDecisionCorrect) ApplySuccess();   // shrink timer, grow pot
    else ResetPotOnly();                       // pot lost, timer full
}

/* OnChooseBank already exists as ChooseBank() */


}

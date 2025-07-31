using UnityEngine;

[CreateAssetMenu(menuName = "Loop Runner/Params")]
public class LoopParams : ScriptableObject
{
    public int BASE_PAYOUT = 10;
    public float MULTIPLIER_MEAN = 1.7f;
    public float MULTIPLIER_VARIANCE = 0.15f;
    public int START_TIMER_SEC = 30;
    public float TIMER_MULTIPLIER = 0.8f;
    public int TIMER_FLAT_DECREMENT = 1;
    public float SUCCESS_PROB_START = 0.9f;
    public int SUCCESS_PROB_DROP_THRESHOLD = 3;
    public float SUCCESS_PROB_DROP_PER_STREAK = 0.05f;
    public int TARGET_SCORE = 1000;
    public bool  DEDUCT_BANK_ON_FAIL = true;   // toggle in Inspector
    public bool BANK_CAN_GO_NEGATIVE = true;

}

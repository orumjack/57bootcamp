using UnityEngine;

[RequireComponent(typeof(GameSession))]
public class TimerTicker : MonoBehaviour
{
    private GameSession gs;

    private void Awake() => gs = GetComponent<GameSession>();

    private void Update()
    {
        gs.TickTimer(Time.deltaTime);
    }
}

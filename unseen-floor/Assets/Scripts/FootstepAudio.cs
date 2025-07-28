using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] walkLoops;   // 6-s ambience of footsteps
    [SerializeField] private AudioClip[] runLoops;

    private PlayerMovement mover;
    private AudioSource    src;
    private bool           wasMoving;

    private void Awake ()
    {
        mover = GetComponent<PlayerMovement>();
        src   = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = true;                   // we want the clip to cycle
    }

    private void Update ()
    {
        bool moving = mover.Velocity.sqrMagnitude > 0.1f;

        // 1. Started or stopped moving?
        if (moving && !wasMoving)           PlayLoop();
        if (!moving &&  wasMoving)          src.Stop();

        // 2. Still moving but switched walk↔run?
        if (moving && wasMoving && StateChanged())     PlayLoop();

        wasMoving = moving;
    }

    // ----------------- helpers -----------------

    void PlayLoop ()
    {
        AudioClip[] bank = mover.IsRunning ? runLoops : walkLoops;
        if (bank.Length == 0) return;

        src.clip = bank[Random.Range(0, bank.Length)];
        src.Play();
    }

    bool StateChanged ()
        => mover.IsRunning != (src.clip != null && System.Array.IndexOf(runLoops, src.clip) >= 0);
}

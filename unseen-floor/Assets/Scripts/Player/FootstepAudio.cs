using UnityEngine;

/// <summary>
/// Loops a walking or running ambience clip while the player is in motion.
/// Walk and run loops are swapped seamlessly when the sprint key is held.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class FootstepAudio : MonoBehaviour
{
    [Header("Loop Clips (drop WAV/OGG)")]
    [Tooltip("Ambient loop played while walking")]
    [SerializeField] private AudioClip[] walkLoops;

    [Tooltip("Ambient loop played while sprinting")]
    [SerializeField] private AudioClip[] runLoops;

    /* ───────── private fields ─── */
    PlayerMovement _mover;
    AudioSource    _src;
    bool           _wasMoving;

    /* ───────── life-cycle ─────── */
    void Awake()
    {
        _mover          = GetComponent<PlayerMovement>();
        _src            = GetComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop        = true;
    }

    /* ───────── frame tick ─────── */
    void Update()
    {
        bool moving = _mover.Velocity.sqrMagnitude > 0.1f;

        if (moving && !_wasMoving) PlayLoop();     // start
        if (!moving && _wasMoving)  _src.Stop();   // stop

        // running ↔ walking toggle
        if (moving && _wasMoving && StateChanged())
            PlayLoop();

        _wasMoving = moving;
    }

    /* ───────── helpers ────────── */
    void PlayLoop()
    {
        AudioClip[] bank = _mover.IsRunning ? runLoops : walkLoops;
        if (bank.Length == 0) return;

        _src.clip = bank[Random.Range(0, bank.Length)];
        _src.Play();
    }

    bool StateChanged()
    {
        bool clipIsRun = _src.clip && System.Array.IndexOf(runLoops, _src.clip) >= 0;
        return _mover.IsRunning != clipIsRun;
    }
}

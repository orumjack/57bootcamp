using UnityEngine;

[DisallowMultipleComponent]
public class FlipProp : MonoBehaviour
{
    /*──────── twin children ───────*/
    [Header("Twin Children (set once)")]
    [SerializeField] private GameObject normalChild;   // active by default
    [SerializeField] private GameObject anomalyChild;  // inactive by default

    /*──────── designer knobs ──────*/
    [Header("Designer Tweaks")]
    [Range(0, 2)] public int  tier         = 0;
    [Min(1)]     public int  weight       = 1;
    public string           group         = "";
    [Min(0)]     public int  cooldownBase = 2;

    /*──────── runtime state ───────*/
    [HideInInspector] public int cooldown;

    /*──────── api ─────────────────*/
    public void SetAnomaly(bool on)
    {
        normalChild.SetActive(!on);
        anomalyChild.SetActive(on);
    }

    public bool IsAnomalyActive => anomalyChild.activeSelf;   // ← NEW
}

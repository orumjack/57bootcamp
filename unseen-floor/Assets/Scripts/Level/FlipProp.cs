using UnityEngine;

/// <summary>
/// Swappable corridor prop with a normal and anomaly appearance.
/// </summary>
[DisallowMultipleComponent]
public class FlipProp : MonoBehaviour
{
    /* ───────── twin children ──── */
    [Header("Child References")]
    [Tooltip("Active in normal state")]
    [SerializeField] private GameObject normalChild;

    [Tooltip("Active in anomaly state")]
    [SerializeField] private GameObject anomalyChild;

    /* ───────── designer knobs ─── */
    [Header("Designer Tweaks")]
    [Range(0, 2)]  public int    tier         = 0;   // Difficulty tier (unused in current anomaly logic)
    [Min(1)]       public int    weight       = 1;   // Selection weight (unused; all anomalies equal chance now)
    public         string        group        = "";  // Group identifier (unused in current logic)
    [Min(0)]       public int    cooldownBase = 2;   // Base cooldown (unused; anomalies can repeat each loop)

    /* ───────── runtime state ─── */
    [HideInInspector] public int cooldown;           // (unused in simplified logic)

    /* ───────── public API ────── */
    public void SetAnomaly(bool on)
    {
        // Toggle between normal and anomaly appearance
        normalChild.SetActive(!on);
        anomalyChild.SetActive(on);
    }

    public bool IsAnomalyActive => anomalyChild.activeSelf;
}

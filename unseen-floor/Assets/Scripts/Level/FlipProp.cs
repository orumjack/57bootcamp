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
    [Range(0, 2)]  public int    tier         = 0;
    [Min(1)]       public int    weight       = 1;
    public         string        group        = "";
    [Min(0)]       public int    cooldownBase = 2;

    /* ───────── runtime state ─── */
    [HideInInspector] public int cooldown;

    /* ───────── public API ────── */
    public void SetAnomaly(bool on)
    {
        normalChild.SetActive(!on);
        anomalyChild.SetActive(on);
    }

    public bool IsAnomalyActive => anomalyChild.activeSelf;
}

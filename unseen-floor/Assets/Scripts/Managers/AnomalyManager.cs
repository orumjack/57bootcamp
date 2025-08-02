using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AnomalyManager : MonoBehaviour
{
    /* ──────── singleton ─────────── */
    public static AnomalyManager Instance { get; private set; }

    /* ──────── inspector knobs ───── */
    [Tooltip("0–1 chance that THIS loop spawns an anomaly (e.g. 0.4 = 40 %)")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyChance = 0.40f;   // 40%

    [Tooltip("How many props to flip when an anomaly is triggered this loop")]
    [SerializeField] private int anomaliesPerLoop = 1;

    [Tooltip("Correct loops before next difficulty tier (unused in simplified logic)")]
    [SerializeField] private int loopsPerTier = 3;

    /* ──────── runtime data ──────── */
    private readonly List<FlipProp> allProps = new();
    private string lastGroup   = "";   // (unused in simplified logic)
    private int    loopIndex   = 0;

    /* Public read-only flag used by decision logic */
    public bool HasAnomalyThisLoop { get; private set; }

    /* ──────── life-cycle ────────── */
    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(this); return; }
        Instance = this;

        // Populate list of all FlipProp instances in the scene (including inactive ones)
        allProps.AddRange(FindObjectsOfType<FlipProp>(true));
    }

    /* ──────── called by teleporter ─ */
    public void StartNewLoop()
    {
        loopIndex++;

        // int currentTier = Mathf.Clamp(loopIndex / loopsPerTier, 0, 2);
        // (Tier progression is not used in the simplified anomaly selection logic)

        /* 1 ▸ Reset visuals & tick down cooldowns */
        foreach (var p in allProps)
        {
            // p.cooldown = Mathf.Max(0, p.cooldown - 1);  // Cooldown disabled (anomalies can repeat each loop)
            p.SetAnomaly(false);                           // Ensure all props are in normal state at start of loop
        }

        /* 2 ▸ Determine if this loop should have an anomaly */
        HasAnomalyThisLoop = Random.value < anomalyChance;
        if (loopIndex == 1)
        {
            // Ensure the first loop is always safe (no anomalies)
            HasAnomalyThisLoop = false;
        }
        if (!HasAnomalyThisLoop) return;  // No anomaly this loop, exit early

        /* 3 ▸ Build the candidate pool of possible anomaly props */
        var pool = new List<FlipProp>();
        // int totalWeight = 0;  // Total weight for weighted random (not used anymore)

        foreach (var p in allProps)
        {
            // if (p.tier > currentTier) continue;    // Disable tier gating (all anomalies considered equally)
            // if (p.cooldown > 0)       continue;    // Disable cooldown (allow repeats)
            // if (p.group == lastGroup) continue;    // Disable group restriction (allow same group repeats)

            pool.Add(p);
            // totalWeight += p.weight;  // Weight not used; all anomalies have equal chance
        }

        // If no candidates available (should not happen in simplified logic unless pool is empty)
        if (pool.Count == 0)
        {
            HasAnomalyThisLoop = false;
            return;
        }

        /* 4 ▸ Randomly pick and activate anomalies */
        for (int n = 0; n < anomaliesPerLoop && pool.Count > 0; n++)
        {
            // Weighted random selection (not used in simplified logic):
            // int roll = Random.Range(1, totalWeight + 1);
            // FlipProp chosen = null;
            // foreach (var p in pool)
            // {
            //     roll -= p.weight;
            //     if (roll <= 0) { chosen = p; break; }
            // }
            // if (chosen == null) break;

            // New selection: choose a random prop from the pool with equal probability
            FlipProp chosen = pool[Random.Range(0, pool.Count)];

            // Activate the chosen prop's anomaly state
            chosen.SetAnomaly(true);

            // Set cooldown and group tracking (disabled in simplified logic):
            // chosen.cooldown = chosen.cooldownBase;
            // lastGroup = chosen.group;

            // Remove the chosen prop from the pool to avoid picking it again this loop
            pool.Remove(chosen);
            // If weight were used, we'd subtract chosen.weight from totalWeight here (not needed now)
        }
    }

    /* ──────── helper for multi-prop queries (optional) ─ */
    public bool AnyAnomalyActive()
    {
        foreach (var p in allProps)
        {
            if (p.IsAnomalyActive) return true;
        }
        return false;
    }
}

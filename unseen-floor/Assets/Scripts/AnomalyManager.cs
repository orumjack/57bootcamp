using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AnomalyManager : MonoBehaviour
{
    /*──────── singleton ───────────*/
    public static AnomalyManager Instance { get; private set; }

    /*──────── inspector knobs ─────*/
    [Tooltip("0–1 chance that THIS loop spawns anomalies (e.g. 0.4 = 40 %)")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyChance = 0.40f;   // 40 %

    [Tooltip("How many props to flip when anomalyChance succeeds")]
    [SerializeField] private int anomaliesPerLoop = 1;

    [Tooltip("Correct loops before next difficulty tier")]
    [SerializeField] private int loopsPerTier = 3;

    /*──────── runtime data ────────*/
    private readonly List<FlipProp> allProps = new();
    private string lastGroup   = "";
    private int    loopIndex   = 0;

    /* public read-only flag used by decision logic */
    public bool HasAnomalyThisLoop { get; private set; }

    /*──────── life-cycle ──────────*/
    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(this); return; }
        Instance = this;

        allProps.AddRange(FindObjectsOfType<FlipProp>(true));
    }

    /*──────── called by teleporter ─*/
    public void StartNewLoop()
    {
        loopIndex++;
        int currentTier = Mathf.Clamp(loopIndex / loopsPerTier, 0, 2);

        /* 1 ▸ reset visuals & tick cooldowns */
        foreach (var p in allProps)
        {
            p.cooldown = Mathf.Max(0, p.cooldown - 1);
            p.SetAnomaly(false);
        }

        /* 2 ▸ roll the 60/40 clean-vs-anomaly */
        HasAnomalyThisLoop = Random.value < anomalyChance;
        if (!HasAnomalyThisLoop) return;                 // 60 % clean loop

        /* 3 ▸ build candidate pool */
        var pool = new List<FlipProp>();
        int totalWeight = 0;

        foreach (var p in allProps)
        {
            if (p.tier  > currentTier)  continue;
            if (p.cooldown > 0)         continue;
            if (p.group == lastGroup)   continue;

            pool.Add(p);
            totalWeight += p.weight;
        }

        if (pool.Count == 0) { HasAnomalyThisLoop = false; return; }

        /* 4 ▸ pick & activate anomalies */
        for (int n = 0; n < anomaliesPerLoop && pool.Count > 0; n++)
        {
            int roll = Random.Range(1, totalWeight + 1);
            FlipProp chosen = null;

            foreach (var p in pool)
            {
                roll -= p.weight;
                if (roll <= 0) { chosen = p; break; }
            }

            if (chosen == null) break;

            chosen.SetAnomaly(true);
            chosen.cooldown = chosen.cooldownBase;
            lastGroup = chosen.group;

            totalWeight -= chosen.weight;
            pool.Remove(chosen);
        }
    }

    /*──────── helper for multi-prop queries (optional) ─
    public bool AnyAnomalyActive()
    {
        foreach (var p in allProps)
            if (p.IsAnomalyActive) return true;
        return false;
    }*/
}

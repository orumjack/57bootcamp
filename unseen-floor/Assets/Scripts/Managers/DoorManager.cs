using UnityEngine;

/// <summary>
/// Central access point for the two sliding capsule doors.
/// </summary>
[DisallowMultipleComponent]
public class DoorManager : MonoBehaviour
{
    /* ───────── singleton ───────── */
    public static DoorManager I { get; private set; }

    [Header("Door Components")]
    [SerializeField] private SlidingDoor doorA;
    [SerializeField] private SlidingDoor doorB;

    /* ───────── life-cycle ─────── */
    void Awake()
    {
        if (I && I != this) { Destroy(this); return; }
        I = this;
    }

    /* ───────── door A (capsule A) ─ */
    public void OpenDoorA()  => doorA.Open();   // after Bank/Risk
    public void CloseDoorA() => doorA.Close();  // on trigger exit

    /* ───────── door B (capsule B) ─ */
    public void OpenDoorB()  => doorB.Open();   // after LMB/RMB decision
    public void CloseDoorB() => doorB.Close();  // on trigger enter
}

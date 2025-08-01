using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static DoorManager I { get; private set; }

    [Header("Reference the two door components")]
    [SerializeField] private SlidingDoor doorA;
    [SerializeField] private SlidingDoor doorB;

    void Awake() => I = this;

    /* called after Bank / Risk inside Capsule A */
    public void OpenDoorA()  => doorA.Open();
    /* called when player exits Trigger of Capsule A */
    public void CloseDoorA() => doorA.Close();

    /* called immediately after LMB/RMB decision */
    public void OpenDoorB()  => doorB.Open();
    /* called when player enters Trigger of Capsule B */
    public void CloseDoorB() => doorB.Close();
}

using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static DoorManager I;
    private void Awake() => I = this;

    public void OpenCapsuleDoor()
    {
        // TODO: slide animation; for now just disable collider
    }
}


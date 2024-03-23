using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Door door;

    public void SwitchGetsHit()
    {
        door.OpenDoor();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Door door;
    public GameObjectManager gameObjectManager;

    public void SwitchGetsHit()
    {
        if (door != null)
        {
            door.OpenDoor();
        }

        if (gameObjectManager != null)
        {
            gameObjectManager.EnableObjects();
        }
    }
}

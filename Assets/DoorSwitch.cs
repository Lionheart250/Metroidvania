using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Door door;
    public GameObjectManager gameObjectManager;
    public IllusionaryWall illusionaryWall;
    public MoneyTree moneyTree;
    public ManaTree manaTree;

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

        if (illusionaryWall != null)
        {
            illusionaryWall.DisableIllusionObjects();

        }

        if (moneyTree != null)
        {
            moneyTree.SpawnCoins();

        }

        if (manaTree != null)
        {
            manaTree.AddMana();

        }
    }
}

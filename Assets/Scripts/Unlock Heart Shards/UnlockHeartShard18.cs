using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockHeartShard18  : MonoBehaviour
{

    bool used;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerController.Instance.unlockedHeartShard18)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
{
    if (_collision.CompareTag("Player") && !used)
    {
        used = true;
       

        // Invoke the UnlockAndDestroy method after 3 seconds
        Invoke("UnlockAndDestroy", 3f);
    }
}

private void UnlockAndDestroy()
    {
    PlayerController.Instance.unlockedHeartShard18 = true;
    SaveData.Instance.SavePlayerData();
    gameObject.SetActive(false); // Deactivate the game object
    Destroy(gameObject);
    }


}

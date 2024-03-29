using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTree : MonoBehaviour
{
    public GameObject coinPrefab;
    public int numberOfCoins = 1;
    public int spawnLimit = 3; // Number of times to spawn coins before destroying the MoneyTree
    private int spawnCount = 0; // Counter for the number of times coins have been spawned
    public List<GameObject> objectsToManage = new List<GameObject>();

    public string doorID;

    private bool doorOpen; // Use a private variable to track door state

    private void Start()
    {
        SaveData.Instance.LoadEnvironmentData();
        SaveData.Instance.GetDoorState(doorID);
        // Check if the door should be open based on saved state
        if (SaveData.Instance.GetDoorState(doorID))
        {
            doorOpen = true;
            DisableMoneyObjects();
        }
        else
        {
            doorOpen = false;
            EnableMoneyObjects();
        }
    }

    public void EnableMoneyObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(true);
        }
        SaveData.Instance.SetDoorState(doorID, false); // Set door state to closed in save data
        SaveData.Instance.SaveEnvironmentData();
        doorOpen = true; // Set door state to open
    }

    public void DisableMoneyObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(false);
        }
        SaveData.Instance.SetDoorState(doorID, true); // Set door state to open in save data
        SaveData.Instance.SaveEnvironmentData();
        doorOpen = false; // Set door state to closed
    }

    public void SpawnCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            Rigidbody2D coinRigidbody = coin.GetComponent<Rigidbody2D>();
            if (coinRigidbody == null)
            {
                coinRigidbody = coin.AddComponent<Rigidbody2D>();
            }

            // Add a random upward force to the coin
            coinRigidbody.AddForce(Vector2.up * Random.Range(50f, 100f), ForceMode2D.Impulse);

            // Add a random sideways force to the coin
            Vector2 sidewaysForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            coinRigidbody.AddForce(sidewaysForce * Random.Range(1f, 3f), ForceMode2D.Impulse);
        }

        spawnCount++;

        if (spawnCount >= spawnLimit)
        {
            Destroy(gameObject);
            doorOpen = true;
            DisableMoneyObjects();
        }
    }
}

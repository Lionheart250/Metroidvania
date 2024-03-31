using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTree : MonoBehaviour
{
    public GameObject coinPrefab;
    public int numberOfCoins = 1;
    public int spawnLimit = 3; // Number of times to spawn coins before destroying the MoneyTree
    public int spawnCount = 0; // Counter for the number of times coins have been spawned
    public List<GameObject> objectsToManage = new List<GameObject>();

    public string SpawnID;
    private void Start()
    {
        LoadState();
    }

    public void EnableMoneyObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(true);
        }
        SaveState();
    }

    public void DisableMoneyObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(false);
        }
        SaveState();
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

        SaveState(); // Save the spawn count after spawning coins

        if (spawnCount >= spawnLimit)
        {
            DisableMoneyObjects();
        }
    }

    // Save and load the state of the money tree
    private void SaveState()
    {
        SaveData.Instance.SaveSpawnCount(SpawnID, spawnCount);
        SaveData.Instance.SaveEnvironmentData();
    }

    private void LoadState()
    {
        SaveData.Instance.LoadEnvironmentData();
        spawnCount = SaveData.Instance.GetSpawnCount(SpawnID);
        if (spawnCount >= spawnLimit)
        {
            DisableMoneyObjects();
        }
        else
        {
            EnableMoneyObjects();
        }
    }
}

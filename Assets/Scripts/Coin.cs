using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // Adjust as needed
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CurrencyManager.Instance.AddCurrency(coinValue);
            Destroy(gameObject); // Destroy the coin after collecting
        }
    }
}

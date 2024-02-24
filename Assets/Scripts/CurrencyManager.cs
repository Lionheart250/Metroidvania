using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int currencyAmount = 0;
    public TextMeshProUGUI currencyText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        UpdateCurrencyText();
    }

    public void AddCurrency(int amount)
    {
        currencyAmount += amount;
        UpdateCurrencyText();
    }

    private void UpdateCurrencyText()
    {
        if (currencyText != null)
        {
            currencyText.text = "Currency: " + currencyAmount.ToString();
        }
    }
}

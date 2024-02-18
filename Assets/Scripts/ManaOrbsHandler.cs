using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaOrbsHandler : MonoBehaviour
{
    public bool usedMana;
    public List<GameObject> manaOrbs;
    public List<Image> orbFills;

    public float countDown = 3f;
    float totalManaPool;

    // Start is called before the first frame update
    void Start()
    {
        InitializeManaOrbs();
    }

    // Update is called once per frame
    void Update()
    {
        InitializeManaOrbs();
        CashInMana();
    }

    void InitializeManaOrbs()
    {
        for (int i = 0; i < manaOrbs.Count; i++)
        {
            if (manaOrbs[i] != null)
            {
                manaOrbs[i].SetActive(i < PlayerController.Instance.manaOrbs);
            }
        }
    }

    public void UpdateMana(float _manaGainFrom)
    {
        for (int i = 0; i < orbFills.Count; i++)
        {
            if (i < manaOrbs.Count && manaOrbs[i] != null && manaOrbs[i].activeInHierarchy && orbFills[i] != null)
            {
                orbFills[i].fillAmount += _manaGainFrom;
                break;
            }
        }
    }

    void CashInMana()
    {
        if (usedMana && PlayerController.Instance.Mana <= 1)
        {
            countDown -= Time.deltaTime;
        }

        if (countDown <= 0)
        {
            usedMana = false;
            countDown = 3;

            totalManaPool = (orbFills[0].fillAmount + orbFills[1].fillAmount + orbFills[2].fillAmount) * 0.33f;
            float manaNeeded = 1 - PlayerController.Instance.Mana;

            if (manaNeeded > 0)
            {
                if (totalManaPool >= manaNeeded)
                {
                    PlayerController.Instance.Mana += manaNeeded;
                    for (int i = 0; i < orbFills.Count; i++)
                    {
                        if (orbFills[i] != null)
                        {
                            orbFills[i].fillAmount = 0;
                        }
                    }

                    float addBackTotal = (totalManaPool - manaNeeded) / 0.33f;
                    while (addBackTotal > 0)
                    {
                        UpdateMana(addBackTotal);
                        addBackTotal -= 1;
                    }
                }
                else
                {
                    PlayerController.Instance.Mana += totalManaPool;
                    for (int i = 0; i < orbFills.Count; i++)
                    {
                        if (orbFills[i] != null)
                        {
                            orbFills[i].fillAmount = 0;
                        }
                    }
                }
            }
        }
    }
}

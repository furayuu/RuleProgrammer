using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 10;

    private int currentHP;
    
    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("Base HP : " + currentHP);

        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver");

        //‰æ–Ê‚Ì‘JˆÚ‚È‚Ç
    }
}

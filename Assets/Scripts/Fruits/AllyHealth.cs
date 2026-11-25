
using UnityEngine;

public class AllHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    // HP‚ğ‰ñ•œ
    public void Heal(int amount)
    {
        currentHealth += amount;

        // Å‘åHP‚ğ’´‚¦‚È‚¢‚æ‚¤‚É‚·‚é
        currentHealth = Mathf.Min(
            currentHealth,
            maxHealth
        );

        Debug.Log("Ally HP: " + currentHealth);
    }

    // ƒ_ƒ[ƒW‚ğó‚¯‚é
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        currentHealth = Mathf.Max(
            currentHealth,
            0
        );

        Debug.Log("Ally HP: " + currentHealth);
    }
}


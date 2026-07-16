using UnityEngine;
using TMPro;
using System.Data.SqlTypes;

public class GameManager : MonoBehaviour
{
    public float roundTime = 30f;   
    private float timer;

    public int currentRound = 1;
    public int maxRound = 5;
    public int Money = 0;

    public TMP_Text timerText;
    public TMP_Text roundText;
    public TMP_Text moneyText;

    public EnemySpawner enemySpawner;

    private bool gameFinished = false;

    void Start()
    {
        timer = roundTime;

        roundText.text = "Round " + currentRound;

        enemySpawner.roundDuration = roundTime;

        enemySpawner.StartRound(currentRound);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        timerText.text = Mathf.Ceil(Mathf.Max(timer, 0)).ToString();

        if (timer <= 0)
        {
            NextRound();
        }

        if (gameFinished)
            return;

        HaveMoney();
    }

    void NextRound()
    {
        if (currentRound >= maxRound)
        {
            gameFinished = true;
            Debug.Log("Stage Clear!");
            return;
        }

        currentRound++;

        roundText.text = "Round " + currentRound;

        timer = roundTime;

        enemySpawner.roundDuration = roundTime;

        enemySpawner.StartRound(currentRound);
    }

    void HaveMoney()
    {
        moneyText.text = "Money " + Money;
    }
}
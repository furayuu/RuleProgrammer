using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float roundTime = 30f;   // 1ラウンドの時間
    private float timer;

    public int currentRound = 1;

    public TMP_Text timerText;
    public TMP_Text roundText;

    public EnemySpawner enemySpawner;

    void Start()
    {
        timer = roundTime;

        roundText.text = "Round " + currentRound;

        // ラウンド1開始
        enemySpawner.StartRound(currentRound);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        timerText.text = Mathf.Ceil(timer).ToString();

        if (timer <= 0)
        {
            NextRound();
        }
    }

    void NextRound()
    {
        currentRound++;

        roundText.text = "Round " + currentRound;

        timer = roundTime;

        enemySpawner.StartRound(currentRound);
    }
}
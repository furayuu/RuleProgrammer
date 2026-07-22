using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ----------------------------
    // Singleton
    // ----------------------------
    public static GameManager Instance { get; private set; }

    [Header("Round")]
    public float roundTime = 30f;
    private float timer;

    public int currentRound = 1;
    public int maxRound = 5;

    [Header("Money")]
    public int Money = 0;

    public TMP_Text timerText;
    public TMP_Text roundText;
    public TMP_Text moneyText;

    public EnemySpawner enemySpawner;

    private bool gameFinished = false;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        timer = roundTime;

        roundText.text = "Round " + currentRound;

        enemySpawner.roundDuration = roundTime;

        enemySpawner.StartRound(currentRound);

        UpdateMoneyUI();
    }

    void Update()
    {
        if (gameFinished)
            return;

        timer -= Time.deltaTime;

        timerText.text = Mathf.Ceil(Mathf.Max(timer, 0)).ToString();

        if (timer <= 0)
        {
            NextRound();
        }
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

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money " + Money;
        }
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (Money < amount)
            return false;

        Money -= amount;
        UpdateMoneyUI();

        return true;
    }
}
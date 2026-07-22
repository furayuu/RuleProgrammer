    using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("UI")]
    public GameObject buildPanel;

    [Header("Ally Prefabs")]
    public GameObject appleAllyPrefab;

    [Header("Cost")]
    public int appleCost = 50;

    private BuildPoint currentBuildPoint;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (buildPanel != null)
            buildPanel.SetActive(false);
    }
    void Update()
    {
        if (buildPanel == null || !buildPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseBuildMenu();
        }
    }

    public void OpenBuildMenu(BuildPoint point)
    {
        Debug.Log("OpenBuildMenu");

        if (point == null)
            return;

        if (point.hasAlly)
        {
            Debug.Log("This platform already has an Ally.");
            return;
        }

        currentBuildPoint = point;

        Debug.Log(buildPanel);

        buildPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void CloseBuildMenu()
    {
        if (buildPanel != null)
            buildPanel.SetActive(false);

        currentBuildPoint = null;

        Time.timeScale = 1f;
    }

    public void BuildApple()
    {
        if (currentBuildPoint == null)
            return;

        if (appleAllyPrefab == null)
        {
            Debug.LogError("Apple Ally Prefab is Missing!");
            return;
        }

        if (!GameManager.Instance.SpendGold(appleCost))
        {
            Debug.Log("Not enough gold");
            return;
        }

        GameObject ally = Instantiate(
            appleAllyPrefab,
            currentBuildPoint.buildPosition.position,
            Quaternion.identity);

        if (ally != null)
        {
            currentBuildPoint.currentAlly = ally;
            currentBuildPoint.hasAlly = true;

            AllyHealth health = ally.GetComponent<AllyHealth>();

            if (health != null)
            {
                health.ownerBuildPoint = currentBuildPoint;
            }
        }

        CloseBuildMenu();
    }
}
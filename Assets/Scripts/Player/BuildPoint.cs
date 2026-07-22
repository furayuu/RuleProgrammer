using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    [Header("Build Position")]
    public Transform buildPosition;

    [HideInInspector]
    public bool hasAlly = false;

    [HideInInspector]
    public GameObject currentAlly;

    private bool playerInside = false;

    void Update()
    {
        if (!playerInside)
            return;

        if (hasAlly)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(gameObject.name + " Press E");
            BuildManager.Instance.OpenBuildMenu(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log(gameObject.name + " Enter");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
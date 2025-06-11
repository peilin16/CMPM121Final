using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject restartUI;
    public bool isDisplay = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.uiManager = this;
        EventBus.Instance.OnPlayerDeath += HandlePlayerDeath;
    }
    public void HandlePlayerDeath(GameObject player)
    {
        if (isDisplay)
            return;
        isDisplay = true;
        restartUI.SetActive(true);
        RestartUI restartUIScript = restartUI.GetComponent<RestartUI>();
        if (restartUIScript != null)
        {
            restartUIScript.HandleData();
        }
        else
        {
            Debug.LogError("RestartUI component not found on the restartUI GameObject!");
        }
    }
    void Update()
    {
        if(!isDisplay && GameManager.Instance.state == GameManager.GameState.VICTORY)
        {
            isDisplay = true;
            restartUI.SetActive(true);
            RestartUI restartUIScript = restartUI.GetComponent<RestartUI>();
            restartUIScript.HandleData();
        }
    }

}

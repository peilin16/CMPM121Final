using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject restartUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.Instance.OnPlayerDeath += HandlePlayerDeath;
    }
    public void HandlePlayerDeath(GameObject player)
    {
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


}

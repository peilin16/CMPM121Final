using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    
    public Button startButton;
    
    public Button settingsButton;

    public Button makerBtn;

    public string settingLevelName;

    public GameObject tips;
    
    public EnemySpawner spawner;
    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClick);
        settingsButton.onClick.AddListener(OnSettingsClick);
        makerBtn.onClick.AddListener(OnMakerClick);
        EventCenter.AddListener<string>(EventDefine.ChoosenLevelName,ChoosenLevelName);
        GameManager.Instance.StartPanel = this.gameObject;
    }

    void ChoosenLevelName(string levelName)
    {
        settingLevelName = levelName;
    }
    
    

    private void OnMakerClick()
    {
        EventCenter.Broadcast(EventDefine.ShowMakerPanel);
    }

    private void OnSettingsClick()
    {
       EventCenter.Broadcast(EventDefine.ShowSettingPanel);
    }

    private void OnStartClick()
    {
        if (GameManager.Instance.playerSpriteManager.currentIconIndex != -1 )
        {
            GameManager.Instance.StartGame();
            // spawner.StartLevel(settingLevelName);
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(ShowTips());
        }
        GameManager.Instance.uiManager.isDisplay = false;
    }

    IEnumerator ShowTips()
    {
        tips.SetActive(true);
        
        yield return new WaitForSeconds(3.5f);
        tips.SetActive(false);
    }


    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventDefine.ChoosenLevelName,ChoosenLevelName);
    }
}

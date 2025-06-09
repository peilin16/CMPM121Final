using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HUD : MonoBehaviour
{

    public Button restartBtn;

    private void Awake()
    {
        restartBtn.onClick.AddListener(OnClickRestartBtn);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            EventCenter.Broadcast(EventDefine.ShowSpellSelectorPanel);
        }
        if (Input.GetKey(KeyCode.J))
        {
            EventCenter.Broadcast(EventDefine.ShowRelicSelectorPanel);
        }
    }

    private void OnClickRestartBtn()
    {
        //TODO restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class MakerPanel : MonoBehaviour
{

    public Button closeBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(OnCloseClick);
        EventCenter.AddListener(EventDefine.ShowMakerPanel,ShowMakerPanel);
    }

    private void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     gameObject.SetActive(false);   
    }

    void ShowMakerPanel()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowMakerPanel,ShowMakerPanel);
    }
}

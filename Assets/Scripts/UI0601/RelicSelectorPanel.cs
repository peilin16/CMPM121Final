using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RelicSelectorPanel : MonoBehaviour
{
    
    public PlayerController player;
    [Header("UI Elements")] 
    public Button chooseRelicButton;
    public Color chosenColor;
    
    [Header("Relic")] 
    public List<Button> relicList = new List<Button>();
    public Text relicInfoText;
    public List<Relic> displayRelic = new List<Relic>();
    
    public int currentRelicIndex;
    private void Awake()
    {
        chooseRelicButton.onClick.AddListener(OnChooseRelic);
        relicList[0].onClick.AddListener(()=>OnRelicBtnClick(0));
        relicList[1].onClick.AddListener(()=>OnRelicBtnClick(1));
        relicList[2].onClick.AddListener(()=>OnRelicBtnClick(2));
        EventCenter.AddListener(EventDefine.ShowRelicSelectorPanel,ShowRelicSelectorPanel);
    }

    
    /// <summary>
    /// Select the currently selected Relic
    /// </summary>
    private void OnChooseRelic()
    {
        if (currentRelicIndex < 0 || currentRelicIndex >=  displayRelic.Count)
        {
            Debug.LogError("Invalid relic selection index.");
            return;
        }
        player.TakeRelic(displayRelic[currentRelicIndex]);
        // Relic selected = displayRelic[index];
        //GameManager.Instance.relicInventory.AddRelic(selected); 
        foreach(var i in player.carriedRelic)
        {
            Debug.Log("relic:" + i.name + "count:" +i.triggerDescription);
        }
        displayRelic.Clear();
        gameObject.SetActive(false);
    }

    private void Start()
    {
       gameObject.SetActive(false);
    }
    

    public void ShowRelicSelectorPanel()
    {
        Debug.Log("Show");
        RandomRelics();
        gameObject.SetActive(true);
    }


    void RandomRelics()
    {
        displayRelic.Clear();

        // Get full relic list from RelicManager
        var allRelics = GameManager.Instance.relicManager.GetAllRelics(); 
        var carried = player.carriedRelic;

        // Filter out already carried relics
        List<Relic> available = new List<Relic>();
        foreach (var relic in allRelics)
        {
            if (!player.carriedRelic.Contains(relic))
            {
                available.Add(relic);
            }
        }

        // Shuffle and take 3 unique ones
        for (int i = 0; i < 3 && available.Count > 0; i++)
        {

            int index = Random.Range(0, available.Count);
            //Debug.Log("index:" + index + " count:" + available.Count);
            var r = available[index];
            //Debug.Log("name:" + r.name);
            //Debug.Log("name:" + r.iconIndex);
            available.RemoveAt(index);
            displayRelic.Add(r);
            
            relicList[i].transform.Find("sprite").GetComponent<Image>().sprite =  GameManager.Instance.relicIconManager.Get(r.iconIndex);
            relicList[i].transform.Find("index").GetComponent<Text>().text =  r.name;
        }
    }
    
    string FormatRelicText(Relic relic)
    {
        return $"<b>Trigger:</b> {relic.triggerDescription}\n<b>Effect:</b> {relic.effectDescription}";
    }

    void OnRelicBtnClick(int index)
    {
        ReRelicColor();
        relicList[index].GetComponent<Image>().color = chosenColor;
        relicInfoText.text = FormatRelicText(displayRelic[index]);
        
        currentRelicIndex = index;
    }
    
    void ReRelicColor()
    {
        foreach (Button relicBtn in relicList)
        {
            relicBtn.GetComponent<Image>().color = Color.white;
        }
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowRelicSelectorPanel,ShowRelicSelectorPanel);
    }
}

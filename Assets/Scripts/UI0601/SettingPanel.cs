using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public SpriteView spriteView1;
    public SpriteView spriteView2;
    public SpriteView spriteView3;

    public Text infoText;


    public PlayerController playerController;

    [Header("UI Elements")] public Color chosenColor;
    public Button closeBtn;
    [Header("Spell")] 
    public List<Button> spellList = new List<Button>();
    public Text spellInfoText;
    public GameObject tips;
    
    private List<Spell> generatedSpells = new List<Spell>();
    private Spell generatedSpell;
    public SpellUIContainer playerSpellUIs;
    public int currentSpellIndex;

    [Header("Character")] 
    private Button mageBtn;
    private Button warlockBtn;
    private Button battlemageBtn;


    private void Awake()
    {
        mageBtn = spriteView1.GetComponent<Button>();
        warlockBtn = spriteView2.GetComponent<Button>();
        battlemageBtn = spriteView3.GetComponent<Button>();

        closeBtn.onClick.AddListener(OnCloseClick);

    
        spellList[0].onClick.AddListener(()=>OnSpellBtnClick(0));
        spellList[1].onClick.AddListener(()=>OnSpellBtnClick(1));
        spellList[2].onClick.AddListener(()=>OnSpellBtnClick(2));
        
        mageBtn.onClick.AddListener(OnMageClick);
        warlockBtn.onClick.AddListener(OnWarlockClick);
        battlemageBtn.onClick.AddListener(OnBattlemageClick);

        EventCenter.AddListener(EventDefine.ShowSettingPanel, ShowSettingPanel);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        gameObject.SetActive(false);
    }

    void ShowSettingPanel()
    {
        gameObject.SetActive(true);
        tips.SetActive(false);
        Apply(spriteView1.gameObject, "mage", GameManager.Instance.playerSpriteManager.Get(0));
        Apply(spriteView2.gameObject, "warlock", GameManager.Instance.playerSpriteManager.Get(1));
        Apply(spriteView3.gameObject, "battlemage", GameManager.Instance.playerSpriteManager.Get(2));

       
    }

    public void Apply(GameObject characterObj, string label, Sprite sprite)
    {
        characterObj.transform.Find("sprite").GetComponent<Image>().sprite = sprite;
        characterObj.transform.Find("index").GetComponent<Text>().text = label;
    }

    private void OnCloseClick()
    {
        // EventCenter.Broadcast(EventDefine.ChoosenLevelName, choosenLevelName);
        
        if (currentSpellIndex < 0 || currentSpellIndex >= generatedSpells.Count)
        {
            Debug.LogWarning("Invalid spell index selected.");
            return;
        }
        
        // playerController.player.spellcaster.spells.Clear();
        playerController.player.spellcaster.spells.Add(generatedSpells[currentSpellIndex]);
        
        playerSpellUIs.spellUIs[playerController.spellNum].SetActive(true);
        playerSpellUIs.spellUIs[playerController.spellNum].GetComponent<SpellUI>().SetSpell(generatedSpells[currentSpellIndex]);

        playerController.spellNum += 1;
        
        // playerSpellUIs.spellUIs[0].SetActive(true);
        // playerSpellUIs.spellUIs[0].GetComponent<SpellUI>().SetSpell(generatedSpells[index]);
        
        
        
        gameObject.SetActive(false);
    }


    void RandomSpell()
    {
        generatedSpells.Clear();
        for (int i = 0; i < 3; i++)
        {
            Spell spell = GameManager.Instance.spellBuilder.MakeRandomSpell(playerController.player.spellcaster);
            generatedSpells.Add(spell);
            spellList[i].transform.Find("rewardSpellSprite").GetComponent<SpellUI>().SetSpell(spell);
            spellList[i].transform.Find("index").GetComponent<Text>().text = spell.GetName();
           
        }
    }
    
    IEnumerator ShowTips()
    {
        tips.SetActive(true);
        
        yield return new WaitForSeconds(3.5f);
        tips.SetActive(false);
    }


    void OnSpellBtnClick(int index)
    {

        if (GameManager.Instance.playerSpriteManager.currentIconIndex == -1)
        {
            StartCoroutine(ShowTips());
            return;
        }
        
        ReSpellColor();
        spellList[index].GetComponent<Image>().color = chosenColor;
        Debug.Log("OnSpellBtnClick   " + index);
        Debug.Log("generatedSpells   " + generatedSpells.Count);
        string modText = "";
        int modCount = generatedSpells[index].modifierSpells.Count;

        if (modCount > 0)
        {
            modText += $"\n<b>Modifiers:</b> {modCount}\n";
            foreach (var mod in generatedSpells[index].modifierSpells)
            {
                modText += $"- {mod.name}\n";
            }
        }
        else
        {
            modText += "\n<b>Modifiers:</b> None";
        }

        spellInfoText.text =
            $"<b>{generatedSpells[index].GetName()}</b>\n" +
            $"{generatedSpells[index].GetDescription()}\n\n" +
            $"<b>Damage:</b> {generatedSpells[index].GetDamage()}\n" +
            $"<b>Mana Cost:</b> {generatedSpells[index].GetManaCost()}\n" +
            $"<b>Cooldown:</b> {generatedSpells[index].GetCooldown():0.00}s" +
            modText;
        
        currentSpellIndex = index;
        
        
      

    }


    void ReSpellColor()
    {
        foreach (Button spellBtn in spellList)
        {
            spellBtn.GetComponent<Image>().color = Color.white;
        }
    }

    private void OnBattlemageClick()
    {
        ReCharacterColor();
        DisplayInfo("battlemage");
        battlemageBtn.GetComponent<Image>().color = chosenColor;
    }

    private void OnWarlockClick()
    {
        ReCharacterColor();
        DisplayInfo("warlock");
        warlockBtn.GetComponent<Image>().color = chosenColor;
    }

    private void OnMageClick()
    {
        ReCharacterColor();
        DisplayInfo("mage");
        mageBtn.GetComponent<Image>().color = chosenColor;
    }


    void ReCharacterColor()
    {
        mageBtn.GetComponent<Image>().color = Color.white;
        warlockBtn.GetComponent<Image>().color = Color.white;
        battlemageBtn.GetComponent<Image>().color = Color.white;
    }

    bool isRandomSpell = false;
    public void Chosen(int index)
    {
        GameManager.Instance.playerSpriteManager.currentIconIndex = index;
        playerController.loadCharacter(index);
        if (!isRandomSpell)
        {
            RandomSpell();
            isRandomSpell = true;
        }
       
    }

    public void DisplayInfo(string chosenLevelName)
    {
        TextAsset jsonText = Resources.Load<TextAsset>("classes");
        if (jsonText == null)
        {
            Debug.LogError("classes.json not found in Resources!");
            return;
        }

        var root = Newtonsoft.Json.Linq.JObject.Parse(jsonText.text);

        // Get current wave and default spell power context
        int wave = GameManager.Instance.currentWave;

        string Format(JObject data)
        {
            string health = data["health"]?.ToString();
            string mana = data["mana"]?.ToString();
            string manaReg = data["mana_regeneration"]?.ToString();
            string spellPower = data["spellpower"]?.ToString();
            string speed = data["speed"]?.ToString();

            string result = "";
            result += $"Health: {RPNCalculator.EvaluateFloat(health, wave)}\n";
            result += $"Mana: {RPNCalculator.EvaluateFloat(mana, wave)}\n";
            result += $"Mana Regen: {RPNCalculator.EvaluateFloat(manaReg, wave)}\n";
            result += $"Spell Power: {RPNCalculator.EvaluateFloat(spellPower, wave)}\n";
            result += $"Speed: {RPNCalculator.EvaluateFloat(speed, wave)}\n";
            return result;
        }

        // ChImfomation1.text = Format((JObject)root[chosenLevelName]);
        infoText.text = Format((JObject)root[chosenLevelName]);
        // ChImfomation2.text = Format((JObject)root["warlock"]);
        // ChImfomation3.text = Format((JObject)root["battlemage"]);
    }


    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowSettingPanel, ShowSettingPanel);
    }
}
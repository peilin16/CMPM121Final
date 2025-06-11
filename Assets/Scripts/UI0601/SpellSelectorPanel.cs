using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpellSelectorPanel : MonoBehaviour
{
    public PlayerController playerController;
    [Header("UI Elements")] 
    public Button chooseSpellButton;
    public Color chosenColor;
    
    [Header("Spell")] 
    public List<Button> spellList = new List<Button>();
    public Text spellInfoText;
   
    
    private List<Spell> generatedSpells = new List<Spell>();
    private Spell generatedSpell;
    public SpellUIContainer playerSpellUIs;
    public int currentSpellIndex;


    private void Awake()
    {
        chooseSpellButton.onClick.AddListener(OnChooseButtonClick);
        
        spellList[0].onClick.AddListener(()=>OnSpellBtnClick(0));
        spellList[1].onClick.AddListener(()=>OnSpellBtnClick(1));
        spellList[2].onClick.AddListener(()=>OnSpellBtnClick(2));

        EventCenter.AddListener(EventDefine.ShowSpellSelectorPanel,ShowSpellSelectorPanel);
    }

    private void Start()
    {
        
       gameObject.SetActive(false);
    }


    void ShowSpellSelectorPanel()
    {

        //Test TODO 
       /* GameManager.Instance.playerSpriteManager.currentIconIndex = 0;
        playerController.loadCharacter(0);*/
        
        gameObject.SetActive(true);
        RandomSpell();
    }

        
    /// <summary>
    /// Select the currently selected Spell
    /// </summary>
    private void OnChooseButtonClick()
    {
        
        if (currentSpellIndex < 0 || currentSpellIndex >= generatedSpells.Count)
        {
            Debug.LogWarning("Invalid spell index selected.");
            return;
        }
        playerController.player.spellcaster.spells.Add(generatedSpells[currentSpellIndex]);
        
        playerSpellUIs.spellUIs[playerController.spellNum].SetActive(true);
        playerSpellUIs.spellUIs[playerController.spellNum].GetComponent<SpellUI>().SetSpell(generatedSpells[currentSpellIndex]);

        playerController.spellNum += 1;
        Debug.LogWarning(playerController.spellNum);
        gameObject.SetActive(false);
    }

    void ReSpellColor()
    {
        foreach (Button spellBtn in spellList)
        {
            spellBtn.GetComponent<Image>().color = Color.white;
        }
    }
    
    void OnSpellBtnClick(int index)
    {

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

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowSpellSelectorPanel,ShowSpellSelectorPanel);
    }
}

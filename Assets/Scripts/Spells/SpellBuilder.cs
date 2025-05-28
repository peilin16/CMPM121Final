using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;


public class SpellBuilder 
{
    private Dictionary<string, SpellData> spellDB = new Dictionary<string, SpellData>();
    private Dictionary<string, ModifierSpell> modifierSpellDB = new Dictionary<string, ModifierSpell>();
    public SpellBuilder()
    {
        LoadAllSpells();
    }
    public void LoadAllSpells()
    {
        if (spellDB.Count > 0 && modifierSpellDB.Count > 0)
            return;

        TextAsset jsonText = Resources.Load<TextAsset>("spells");
        if (jsonText == null)
        {
            Debug.LogError("spells.json not found in Resources!");
            return;
        }

        JObject root = JObject.Parse(jsonText.text);

        LoadBaseSpells(root);
        LoadModifiedSpells(root);
    }
    private void LoadBaseSpells(JObject root)
    {
        foreach (var pair in root)
        {
            try
            {
                JObject obj = (JObject)pair.Value;

                // If it doesn't have spell-modifying keys, treat as base spell
                bool isModifier =
                    obj["damage_multiplier"] != null ||
                    obj["mana_multiplier"] != null ||
                    obj["mana_adder"] != null ||
                    obj["speed_multiplier"] != null ||
                    obj["cooldown_multiplier"] != null ||
                    obj["projectile_trajectory"] != null;

                if (!isModifier)
                {
                    SpellData data = obj.ToObject<SpellData>();
                    spellDB[pair.Key] = data;
                }
            }
            catch
            {
                Debug.LogWarning($"Failed to parse base spell: {pair.Key}");
            }
        }
    }

    private void LoadModifiedSpells(JObject root)
    {
        foreach (var pair in root)
        {
            JObject obj = (JObject)pair.Value;
            string name = obj["name"]?.ToString();
            string desc = obj["description"]?.ToString();

            try
            {
                switch (name)
                {
                    case "damage-amplified":
                        modifierSpellDB[pair.Key] = new DamageAmpModifier(obj);
                        break;

                    case "speed-amplified":
                        modifierSpellDB[pair.Key] = new SpeedAmpModifier(obj);
                        break;

                    case "doubled":
                        modifierSpellDB[pair.Key] = new DoublerModifier(obj);
                        break;

                    case "split":
                        modifierSpellDB[pair.Key] = new SplitterModifier(obj);
                        break;

                    case "chaotic":
                        modifierSpellDB[pair.Key] = new ChaosModifier(obj);
                        break;

                    case "homing":
                        modifierSpellDB[pair.Key] = new HomingModifier(obj);
                        break;

                    case "mana-amplified":
                        modifierSpellDB[pair.Key] = new ManaAmpModifier(obj);
                        break;

                    case "waver":
                        modifierSpellDB[pair.Key] = new WaverModifier(obj);
                        break;

                    /*default:
                        Debug.LogWarning($"[LoadModifiedSpells] Unknown modifier name: {name}");
                        break;*/
                }
            }
            catch
            {
                Debug.LogWarning($"[LoadModifiedSpells] Failed to parse modifier spell: {pair.Key}");
            }
        }
    }






    public Spell MakeRandomSpell(SpellCaster owner)
    {
        /*if(spellDB == null && modifierSpellDB == null)
            LoadAllSpells(); */

        float roll = Random.value;

        // 50% chance to return a modifier spell (recursive wrap), 60% to return a base spell
        if (roll < 0.7f)
        {
            // Get random modifier
            List<string> modKeys = new List<string>(modifierSpellDB.Keys);
            string modKey = modKeys[Random.Range(0, modKeys.Count)];
            ModifierSpell modifier = modifierSpellDB[modKey];

            
            // Recursively wrap a base or another modified spell
            Spell inner = MakeRandomSpell(owner);
            inner.modifierSpells.Add(modifier);
            Debug.Log( " spell:"+ inner.data.name + " damage:" + inner.GetDamage() + " cooldown:" + inner.GetCooldown() + " speed:"+inner.GetSpeed()+" mod name:" + modifier.name );
            return inner;
        }
        else
        {
            // Get random base spell
            List<string> baseKeys = new List<string>(spellDB.Keys);
            string baseKey = baseKeys[Random.Range(0, baseKeys.Count)];
            SpellData baseData = spellDB[baseKey];
            return BuildSpell(owner, baseData);
        }
    }


    public Spell Build(SpellCaster owner, string key)
    {
        if (spellDB == null)
            LoadAllSpells();

        if (!spellDB.ContainsKey(key))
        {
            Debug.LogWarning($"Spell '{key}' not found. Using default.");
            return new Spell(owner, new SpellData
            {
                name = "Default",
                icon = 0,
                base_damage = 10,
                base_mana_cost = 5,
                base_cooldown = 1f,
                projectile = new ProjectileData { sprite = "0", trajectory = "straight", base_speed = 8f }
            });
        }

        return BuildSpell(owner, spellDB[key]);
    }
    public Spell BuildSpell(SpellCaster owner, SpellData data)
    {
        int wave = GameManager.Instance.currentWave;
        
        float power = owner.spellPower;

        data.base_damage = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.damage.amount, wave, power));

        data.base_mana_cost = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.mana_cost, wave, power));
        data.base_cooldown = RPNCalculator.EvaluateFloat(data.cooldown, wave, power);

        if (!string.IsNullOrEmpty(data.secondary_damage))
            data.base_secondary_damage = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.secondary_damage, wave, power));

        if (!string.IsNullOrEmpty(data.N))
            data.N_value = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.N, wave, power));

        if (data.projectile != null)
        {
            data.projectile.base_speed = RPNCalculator.EvaluateFloat(data.projectile.speed, wave, power);
            if (!string.IsNullOrEmpty(data.projectile.lifetime))
                data.projectile.base_lifetime = RPNCalculator.EvaluateFloat(data.projectile.lifetime, wave, power);
        }

        if (data.secondary_projectile != null)
        {
            data.secondary_projectile.base_speed = RPNCalculator.EvaluateFloat(data.secondary_projectile.speed, wave, power);
            if (!string.IsNullOrEmpty(data.secondary_projectile.lifetime))
                data.secondary_projectile.base_lifetime = RPNCalculator.EvaluateFloat(data.secondary_projectile.lifetime, wave, power);
        }


        // if require the different effect spell
        if (data.name == "Arcane Spray")
            return new ArcaneSpraySpell(owner, data);
        
        return new Spell(owner, data);
    }
    
}

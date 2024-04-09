using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateSpell : MonoBehaviour
{
    public UI_Manager ui_mg;

    public Spell newSpell;
    
    public int enhancmentTokens = 10;
    
    public TextMeshProUGUI enhancmentTokensCounter;
    public TMP_Dropdown elementDropdown, spellTypeDropdown;

    public ButtonStatChange dmgButton, atkSpeedButton;
    public Element[] elements;
    Element currentElement;
    public void CreateNewSpell()
    {
        newSpell = new Spell();
        currentElement = elements[0];
        enhancmentTokens = 10;
        enhancmentTokensCounter.text = enhancmentTokens.ToString();
        dmgButton.value = 0;
        dmgButton.counter.text = dmgButton.value.ToString();
        atkSpeedButton.value = 0;
        atkSpeedButton.counter.text = atkSpeedButton.value.ToString();
    }
    public void FinalizeSpell()
    {
        if (currentElement == null) return;
        if (enhancmentTokens > 0) return;
        newSpell.element = currentElement;
        newSpell.damage = currentElement.dmgBase;
        newSpell.attackSpeed = currentElement.atkSpeedBase;

        newSpell.damage += dmgButton.value;
        newSpell.attackSpeed += atkSpeedButton.value;

        FindObjectOfType<CastSpells>().spell = newSpell;

        ui_mg.SpellMenuEnterExit();
    }
    void Start()
    {
        elementDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(elementDropdown); });

        List<TMP_Dropdown.OptionData> optionsData = new List<TMP_Dropdown.OptionData>();
        foreach (Element ele in elements)
        {
            optionsData.Add(new TMP_Dropdown.OptionData(ele.name));
        }
        elementDropdown.AddOptions(optionsData);
    }
    void DropdownValueChanged(TMP_Dropdown change)
    {
        currentElement = elements[change.value];
    }
}

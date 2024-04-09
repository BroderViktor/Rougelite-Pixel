using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonStatChange : MonoBehaviour, IPointerClickHandler
{
    public CreateSpell spellMenu;
    public float value;
    public TextMeshProUGUI counter;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (spellMenu.enhancmentTokens > 0) 
            { 
                value++;
                spellMenu.enhancmentTokens--;

                counter.text = value.ToString();
                spellMenu.enhancmentTokensCounter.text = spellMenu.enhancmentTokens.ToString();
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (value > 0) 
            { 
                value--;
                spellMenu.enhancmentTokens++;

                counter.text = value.ToString();
                spellMenu.enhancmentTokensCounter.text = spellMenu.enhancmentTokens.ToString();
            }
        }
    }

}

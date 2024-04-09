using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMainScript : MonoBehaviour
{
    ActiveSpellManager spellManager;
    CastTypes caster;

    private void Start()
    {
        spellManager = FindObjectOfType<ActiveSpellManager>();
        caster = FindObjectOfType<CastTypes>();
    }
    public void CastSpell()
    {
        //Lag spellet, if eller en kode i magicspell
        GameObject projectile = caster.castProjectile();
        projectile.GetComponent<testProjScript>().spellManager = spellManager;
        spellManager.activeSpells.Add(projectile);
        //Generer en linje med påvirkning

        //Final effect
    }
}

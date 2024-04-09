using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MagicSpell : ScriptableObject
{
    public CastTypes caster;

    public int dmg;
    public int projSpeed;
    
    public GameObject CastType()
    {
        return caster.castProjectile();
    }
    public void hitEffects()
    {

    }
    public void pathEffects(Cell[] hitCells)
    {
        //foreach block in hit, apply patheffect som er refereanse til void
        
    }
    public void finalEffects()
    {

    }
}

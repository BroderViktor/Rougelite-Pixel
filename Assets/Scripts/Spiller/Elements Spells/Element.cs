using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : ScriptableObject
{
    public Material mat;
    public float dmgBase = 10, atkSpeedBase = 1;
    public virtual void SpecialEffect(Cell cell)
    {

    }
    public virtual void BlockChanger()
    {

    }
}

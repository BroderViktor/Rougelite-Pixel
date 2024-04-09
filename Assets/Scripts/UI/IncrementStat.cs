using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncrementStat : MonoBehaviour
{
    public StatTracker[] counters;
    public void IncreaseCount(int num)
    {
        counters[num].value++;
        counters[num].counter.text = counters[num].value.ToString();
    }
    public void DecreaseCount(int num)
    {
        counters[num].value--;
        counters[num].counter.text = counters[num].value.ToString();
    }
}
[System.Serializable]
public class StatTracker
{
    public TextMeshProUGUI counter;
    public int value;
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSTracker : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Update()
    {
        int currentFps = (int)(1f / Time.unscaledDeltaTime);
        text.text = "FPS: " + currentFps;
    }
}

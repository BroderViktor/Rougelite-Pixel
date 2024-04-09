using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public Camera cam;

    public List<backgroundObject> backgroundElements = new List<backgroundObject>();

    //float offset;
    Vector2 camBounds;
    private void Start()
    {
        camBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        for (int i = 0; i < backgroundElements.Count; i++)
        {
            backgroundElements[i].bgBounds = backgroundElements[i].bg.GetComponent<SpriteRenderer>().bounds.size;
            backgroundElements[i].defaultOffset = backgroundElements[i].bgBounds * i - backgroundElements[i].bgBounds;
        }
    }
    private void Update()
    {
        int i = 0;
        foreach (backgroundObject bg in backgroundElements)
        {
            Vector2 pos = cam.transform.position * bg.speed;
            bg.bg.transform.position = new Vector2(pos.x + bg.offset.x + bg.defaultOffset.x, cam.transform.position.y);

            if (bg.bg.transform.position.x + bg.bgBounds.x / 2 < 
                cam.transform.position.x - camBounds.x + bg.defaultOffset.x)
            {
                bg.offset.x += bg.bgBounds.x;
            }
            else if (bg.bg.transform.position.x - bg.bgBounds.x / 2 >
                cam.transform.position.x + camBounds.x + bg.defaultOffset.x)
            {
                bg.offset.x -= bg.bgBounds.x;
            }
            i++;
        }
    }
}
[System.Serializable] 
public class backgroundObject
{
    public GameObject bg;
    public float speed;
    public Vector2 bgBounds, defaultOffset, offset;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraFollow : NetworkBehaviour
{
    [SerializeField] float interpolateValue, PPU;

    [System.NonSerialized] public PlayerControll localSpiller;

    void FixedUpdate()
    {
        if (localSpiller == null) return;

        Vector3 SplrPosV3 = new Vector3(localSpiller.transform.position.x, localSpiller.transform.position.y - 0.25f, -10);
        Vector3 thisPosV3 = new Vector3(transform.position.x, transform.position.y, -10);

        Vector3 Pos = Vector3.Lerp(SplrPosV3, thisPosV3, interpolateValue);

        transform.position = RoundToPixel(Pos, true);
    }
    public Vector3 RoundToPixel(Vector3 position, bool cam)
    {
        float UPP = 1/PPU;
        if (UPP == 0.0f)
            return position;

        Vector3 result;
        result.x = Mathf.Round(position.x / UPP) * UPP;
        result.y = Mathf.Round(position.y / UPP) * UPP;
        result.z = cam ? -10 : 0;

        return result;
    }
}

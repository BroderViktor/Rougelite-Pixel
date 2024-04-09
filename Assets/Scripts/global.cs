using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class global
{
    public static bool userInput = true;

    public static int PPU, chunkWidth, chunkHeight, chunkLoadedX, chunkLoadedY;

    public static Vector2Int chunkOffset;

    public static Cell_Info C_I;

    public static Camera cam;


    public static Vector2Int ConvertWorldToMapPos(Vector2 point)
    {
        Vector2 mapPos = point * global.PPU;

        Vector2Int mapPosInt =
            new Vector2Int(
                (int)(mapPos.x),
                (int)(mapPos.y));

        Vector2Int chunkOffsetPixel = new Vector2Int(
            (int)(global.chunkOffset.x * global.chunkWidth),
            (int)(global.chunkOffset.y * global.chunkHeight));

        mapPosInt -= chunkOffsetPixel;

        mapPosInt += new Vector2Int((chunkLoadedX - 1) / 2 * (int)global.chunkWidth, (chunkLoadedY - 1) / 2 * (int)global.chunkHeight);

        return mapPosInt;
    }
}

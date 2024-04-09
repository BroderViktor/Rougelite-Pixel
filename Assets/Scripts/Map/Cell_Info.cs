using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell_Info : MonoBehaviour
{
    public byte Stone, Grass, PlantRoot, Air, Water, Dirt, plant1, plant2, plant3;

    public BlockType[] blockArray;
    
    public Vector2Int[] WaterMoveDir;

    public Material[] mats;

    [System.NonSerialized] public byte renderedTypes;
    public void InitializeCellInfo()
    {
        global.C_I = this;
        for (int i = 1; i < blockArray.Length; i++)
        {
            if (blockArray[i].renderedInChunk)
            {
                blockArray[i].rendererNum = renderedTypes;

                renderedTypes++;
            }
        }
        mats = new Material[renderedTypes];
        int x = 0;
        for (int i = 1; i < blockArray.Length; i++)
        {
            if (blockArray[i].renderedInChunk)
            {
                mats[x] = blockArray[i].mat;
                x++;
            }
        }
    }
}
[System.Serializable]
public class BlockType
{
    public string name;

    public byte numerator, rendererNum;
    public bool solid, renderedInChunk, destroyable;

    public Material mat;
}
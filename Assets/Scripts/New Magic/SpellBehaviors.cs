using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellBehaviors
{
    public static Cell_Info C_I;
    public static MapGen mG;
    public static ActiveSpellManager spellManager;

    public static Cell destroyBlock(Vector2Int pos, byte blockNum) //Bytte ut med en data holder class
    {
        int chunkID;
        Vector2Int chunkPos;
        BlockType type = C_I.blockArray[mG.ReturnValueOfPoint(pos, out chunkID, out chunkPos)];
        
        CreateChunkMesh chunk = mG.activeChunks[chunkID].GetComponent<CreateChunkMesh>();

        int nodeNum = chunkPos.y * chunk.width + chunkPos.x;

        Cell block = chunk.map[nodeNum];

        if (type.destroyable && type.numerator != 0) //<-- destroyable kan være en option
        {
            block.type = C_I.Air;

            if (!spellManager.chunksList.Contains(chunk)) spellManager.chunksList.Add(chunk);
        }

        return block;
    }
    public static Cell createBlock(Vector2Int pos, byte blockNum)
    {
        int chunkID;
        Vector2Int chunkPos;
        BlockType type = C_I.blockArray[mG.ReturnValueOfPoint(pos, out chunkID, out chunkPos)];

        CreateChunkMesh chunk = mG.activeChunks[chunkID].GetComponent<CreateChunkMesh>();

        int nodeNum = chunkPos.y * chunk.width + chunkPos.x;

        Cell block = chunk.map[nodeNum];

        if (!type.destroyable && type.numerator != 0) //<-- destroyable kan være en option
        {
            block.type = blockNum;

            if (!spellManager.chunksList.Contains(chunk)) spellManager.chunksList.Add(chunk);
        }

        return block;
    }
}

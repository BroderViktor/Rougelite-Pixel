using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
    public MapGen mapgen;
    public bool generate;

    byte[][] ruleList = new byte[30][];
    private void Start()
    {
        ruleList[0] = new byte[] { 0, 0, 1, 3, 1, 0, 2, 0, 2, 0, 4, 2, 3, 2, 0, 1, 0, 1, 0, 4 };
        ruleList[1] = new byte[] { 0,0,1,3,1,0,2,0,2,0,4,3,0,1,0,1,0,4,3,2,2,0,1,0,1,0,4,1,3,2,0,2,0,2,0,4 };
        ruleList[2] = new byte[] { 1, 3, 0, 2, 0, 4, 2, 0, 3, 1, 0, 4 };
        ruleList[3] = new byte[] { 0, 0, 3, 1, 4, 0, 0, 0, 0, 3, 2, 4, 0, 0};
    }
    public Plant GeneratePlant(byte[] seed, int loops, int rule, short chunkID, byte type, Vector2Int pos, Vector2Int dir)
    {
        return new Plant(GenerateTreePaths(seed, loops, rule), chunkID, type, pos, dir);
    }
    List<byte> GenerateTreePaths(byte[] seed, int loops, int rule)
    {
        List<byte> treePaths = new List<byte>();
        treePaths.AddRange(seed);

        for (int i = 0; i < loops; i++) treePaths = generateBraches(treePaths, rule);

        return treePaths;
    }
    List<byte> generateBraches(List<byte> seed, int rule)
    {
        List<byte> result = new List<byte>();

        foreach (byte branch in seed)
        {
            if (branch == 0) //Regel hvis det er forover (0)
            {
                result.AddRange(ruleList[rule]);
            }
            else result.Add(branch); //Ingen regel for dette tallet
        }

        return result;
    }
    public void CreatePlant(List<byte> treePaths, short currentChunkID, byte plantType, Vector2Int pos, Vector2Int dir)
    {
        List<Vector2Int> savedPositions = new List<Vector2Int>();
        List<short> savedChunks = new List<short>();

        foreach (byte path in treePaths)
        {
            short chunkID = -1;
            Vector2Int newPos = Vector2Int.zero;
            if      (path == 0)
            {
                createPixel(pos + new Vector2Int(0, dir.y), currentChunkID, plantType, out chunkID, out newPos);

                pos = newPos;
                currentChunkID = chunkID;
            }
            else if (path == 1)
            {
                createPixel(pos + new Vector2Int(dir.x, dir.y), currentChunkID, plantType, out chunkID, out newPos);

                pos = newPos;
                currentChunkID = chunkID;
            }
            else if (path == 2)
            {
                createPixel(pos + new Vector2Int(dir.x * -1, dir.y), currentChunkID, plantType, out chunkID, out newPos);

                pos = newPos;
                currentChunkID = chunkID;
            }
            else if (path == 3) //Lagrer
            {
                savedPositions.Add(pos);
                savedChunks.Add(currentChunkID);
            }
            else if (path == 4) //Popper
            {
                pos = savedPositions[0];
                currentChunkID = savedChunks[0];
                savedPositions.RemoveAt(0);
                savedChunks.RemoveAt(0);
            }
        }
    }
    void createPixel(Vector2Int pos, short currentChunkID, byte type, out short chunkID, out Vector2Int newPos)
    {
        chunkID = 0;
        newPos = Vector2Int.zero;

        for (int w = 0; w >= 0; w--)
        {
            for (int h = 0; h >= 0; h--)
            {
                Cell pixel = ReturnValueOfPoint(pos + new Vector2Int(0, 0), currentChunkID, out chunkID, out newPos);

                if (pixel != null) if (pixel.type == mapgen.C_I.Air)
                {
                    pixel.type = type;
                }
            }
        }
    }
    Cell ReturnValueOfPoint(Vector2Int point, int currentChunkID, out short chunkID, out Vector2Int newPos)
    {
        //NOTE: Returnerer null hvis det er utenfor en chunk
        int x = point.x;
        int y = point.y;

        if (currentChunkID < 0 || currentChunkID > (mapgen.loadSize.x * 2 + 1) * (mapgen.loadSize.y * 2 + 1) - 1)   
        { 
            if (!(x > global.chunkWidth - 1) || !(x < 0) || !(y > global.chunkHeight - 1) || !(y < 0))
            {
                newPos = point;
                chunkID = (short)currentChunkID;
                return null;
            }

        }

        if (x > global.chunkWidth - 1 || x < 0 || y > global.chunkHeight - 1 || y < 0)
        {
            Vector2Int chunkDir = new Vector2Int(0, 0);

            if (x > global.chunkWidth - 1) { x = 0; chunkDir += new Vector2Int(1, 0); }
            else if (x < 0) { x = global.chunkWidth - 1; chunkDir += new Vector2Int(-1, 0); }
            if (y > global.chunkHeight - 1) { y = 0; chunkDir += new Vector2Int(0, 1); }
            else if (y < 0) { y = global.chunkHeight - 1; chunkDir += new Vector2Int(0, -1); }

            newPos = new Vector2Int(x, y);
            
            chunkID = (short)(currentChunkID + (chunkDir.y * (mapgen.loadSize.x * 2 + 1) + chunkDir.x));
            
            if (chunkID < 0) { return null; }
            else if (chunkID > (mapgen.loadSize.x * 2 + 1) * (mapgen.loadSize.y * 2 + 1) - 1) 
            { 
                //chunkID = (short)((mapgen.loadSize.x * 2 + 1) * (mapgen.loadSize.y * 2 + 1) - 1); 
                return null; 
            }
            
            CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();

            return chunk.map[y * chunk.width + x];
        }
        else
        {
            chunkID = (short)currentChunkID;
            CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();
            
            newPos = new Vector2Int(x, y);

            return chunk.map[y * chunk.width + x];
        }
    }
}

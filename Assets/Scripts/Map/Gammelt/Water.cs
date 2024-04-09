using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    /*public MapGen mapgen;

    public CreateChunkMesh currentChunk;
    public int currentChunkID;

    public Vector2Int posInChunk;

    public float UpdateRate;

    public bool sleepMode;

    public void StartMoving()
    {
        sleepMode = false;
        StartCoroutine("UpdatePos");
    }
    public void movePos()
    {
        Vector2Int[] directions = WaterMoveLocations();

        for (int x = 0; x < directions.Length; x++)
        {
            if (FindNewMove(posInChunk, directions[x])) return;
        }
    }
    bool FindNewMove(Vector2Int pos, Vector2Int direction)
    {
        short chunkID;
        Vector2Int newPos;
        if (ReturnValueOfPoint(pos + direction, out chunkID, out newPos) == 0)
        {
            Move(newPos, direction, chunkID);
            return true;
        }
        return false;
    }
    void Move(Vector2Int pos, Vector2 dir, short chunkID)
    {
        currentChunk.map[posInChunk.y * currentChunk.width + posInChunk.x] = 0;

        currentChunk = mapgen.chunkArray[chunkID].GetComponent<CreateChunkMesh>();
        currentChunkID = chunkID;

        transform.position += new Vector3(dir.x, dir.y) / 32;
        posInChunk = pos;

        currentChunk.map[posInChunk.y * currentChunk.width + posInChunk.x] = 2;
    }
    void CheckSleepMode()
    {
        Vector2Int[] directions = WaterMoveLocations();

        for (int x = 0; x < directions.Length; x++)
        {
            if (CheckIfAir(posInChunk, directions[x])) return;

            if (x == directions.Length - 1) { StopCoroutine("UpdatePos"); sleepMode = true; }
        }
    }
    bool CheckIfAir(Vector2Int pos, Vector2Int direction)
    {
        short chunkID;
        Vector2Int newPos;

        if (ReturnValueOfPoint(pos + direction, out chunkID, out newPos) == 0)
        {
            return true;
        }
        return false;
    }
    int ReturnValueOfPoint(Vector2Int point, out short chunkID, out Vector2Int newPos)
    {
        //NOTES: Returnerer 255 hvis det er utenfor en chunk

        int x = point.x;
        int y = point.y;
        
        if (x > 79 || x < 0 || y > 79 || y < 0)
        {
            Vector2Int chunkDir = new Vector2Int(0, 0);

            if (x > 79) { x = 0; chunkDir += new Vector2Int(1, 0); }
            else if (x < 0) { x = 79; chunkDir += new Vector2Int(-1, 0); }
            if (y > 79) { y = 0; chunkDir += new Vector2Int(0, 1); }
            else if (y < 0) { y = 79; chunkDir += new Vector2Int(0, -1); }

            newPos = new Vector2Int(x, y);

            chunkID = (short)(currentChunkID + chunkDir.y * mapgen.width + chunkDir.x);
            if (chunkID < 0 || chunkID > mapgen.height * mapgen.width - 1) { return 255; }

            CreateChunkMesh chunk = mapgen.chunkArray[chunkID].GetComponent<CreateChunkMesh>();

            return chunk.map[y * chunk.width + x];
        }
        else
        {
            chunkID = (short)currentChunkID;
            newPos = new Vector2Int(x, y);

            return currentChunk.map[y * currentChunk.width + x];
        }
    }
    IEnumerator UpdatePos()
    {
        for (; ; )
        {
            movePos();
            yield return new WaitForSeconds(UpdateRate);
        }
    }
    Vector2Int[] WaterMoveLocations()
    {
        Vector2Int[] directions = new Vector2Int[5];

        //directions[0] = new Vector2Int(0, -2);
        directions[0] = new Vector2Int(0, -1);
        directions[1] = new Vector2Int(1, -1);
        directions[2] = new Vector2Int(-1, -1);
        directions[3] = new Vector2Int(1, 0);
        directions[4] = new Vector2Int(-1, 0);

        return directions;
    }*/
}

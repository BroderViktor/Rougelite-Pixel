using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public Vector2Int pos;
    //public Vector2Int chunkOffset;
    //public Vector2 velocity;
    

    public byte type;

    /*public short managerObjectID;
    public short arrayPos;*/

    //public int chunkID;
    public CreateChunkMesh chunk;
    public bool sleepMode = false;


    public Cell(Vector2Int _pos, CreateChunkMesh _chunk, byte _type)
    {
        pos = _pos;
        chunk = _chunk;
        type = _type;
    }
    /*public Vector2Int[] WaterMoveLocations()
    {
        Vector2Int[] directions = new Vector2Int[5];

        /*directions[0] = new Vector2Int(Mathf.RoundToInt(velocity.x), Mathf.RoundToInt(velocity.y));
        if (directions[0] == Vector2Int.zero) velocity = new Vector2(0, -1);
        directions[0] = new Vector2Int( 0,  -1);
        directions[1] = new Vector2Int( 1,  -1);
        directions[2] = new Vector2Int(-1,  -1);
        directions[3] = new Vector2Int( 1,   0);
        directions[4] = new Vector2Int(-1,   0);

        return directions;
    }*/
    /*public Vector2Int[] GenerateMoveLocations()
    {
        
        if (velocity != Vector2.zero)
        {
            Vector2Int[] locations = new Vector2Int[5];
            locations[0] = new Vector2Int((int)velocity.x, (int)velocity.y);

            for (int locNum = 1; locNum < 5; locNum++)
            {
                if (velocity.x == 0)
                {
                    locations.
                }
                else if (velocity.y == 0)
                {
                    return null;
                }
            }
        }
        else
        {
            return null;
        }
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Plant
{
    public List<byte> branchPaths;
    public short chunkID;
    public byte type;
    public Vector2Int pos;
    public Vector2Int dir;
    public Plant(List<byte> _paths, short _chunkID, byte _type, Vector2Int _pos, Vector2Int _dir)
    {
        branchPaths = _paths;
        chunkID = _chunkID;
        pos = _pos;
        dir = _dir;
        type = _type;
    }
}
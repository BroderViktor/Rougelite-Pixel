using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MapGen : MonoBehaviour
{
    [SerializeField] NoiseGenerator nG;
    [SerializeField] GameObject chunkPrefab;

    public Cell_Info C_I;

    public Vector2Int loadSize;

    public bool outlineOn;
    public float upperValue, lowerValue;

    //[System.NonSerialized] public int pixelsPerUnit;
    //[System.NonSerialized] public float chunkWidth, chunkHeight;

    [System.NonSerialized] public GameObject[] chunkArray;
    [System.NonSerialized] public GameObject[] activeChunks;

    private void Start()
    {
        global.PPU = chunkPrefab.GetComponent<CreateChunkMesh>().pixelsPerUnit;
        global.chunkWidth = chunkPrefab.GetComponent<CreateChunkMesh>().width;
        global.chunkHeight = chunkPrefab.GetComponent<CreateChunkMesh>().height;
        global.chunkLoadedX = loadSize.x * 2 + 1;
        global.chunkLoadedY = loadSize.y * 2 + 1;

        C_I.InitializeCellInfo();

        chunkArray = new GameObject[global.chunkLoadedX * global.chunkLoadedY];
        GenerateMap();
    }
    public int ReturnValueOfPoint(Vector2Int point, out int chunkID, out Vector2Int newPos)
    {
        //NOTES: Returnerer 255 hvis det er utenfor en chunk

        Vector2Int pixelPos = point;

        int chunkX = pixelPos.x / global.chunkWidth;
        int chunkY = pixelPos.y / global.chunkHeight;


        if (chunkX > loadSize.x * 2 + 1 || chunkX < 0 || chunkY > loadSize.y * 2 + 1 || chunkY < 0)
        {
            chunkID = 0;
            newPos = Vector2Int.zero;
            return 0; 
        }

        chunkID = chunkY * (loadSize.x * 2 + 1) + chunkX;

        CreateChunkMesh chunk;
    
        try
        {
            chunk = activeChunks[chunkID].GetComponent<CreateChunkMesh>();
        }
        catch (System.Exception)
        {
            Debug.Log("chunkID" + chunkID);
            Debug.Log("point" + point);
            Debug.Log("LOadsize" + (loadSize.x) * (int)global.chunkWidth + " " + (loadSize.y) * (int)global.chunkWidth);

            Debug.Log("chunkXY" + chunkX + " + " + chunkY);

            throw;
        }

        Vector2Int newPixelPos = new Vector2Int(
            pixelPos.x - chunkX * global.chunkWidth,
            pixelPos.y - chunkY * global.chunkHeight);


        if (newPixelPos.x > global.chunkWidth - 1 || newPixelPos.x < 0
         || newPixelPos.y > global.chunkHeight - 1 || newPixelPos.y < 0)
        {
            Vector2Int chunkDir = new Vector2Int(0, 0);

            if (newPixelPos.x > (int)global.chunkWidth - 1) { newPixelPos.x = 0; }
            else if (newPixelPos.x < 0) { newPixelPos.x = (int)global.chunkWidth - 1; }
            if (newPixelPos.y > (int)global.chunkHeight - 1) { newPixelPos.y = 0; }
            else if (newPixelPos.y < 0) { newPixelPos.y = (int)global.chunkHeight - 1; }

            newPos = newPixelPos;

            chunkID = (short)(chunkID + chunkDir.y * global.chunkLoadedX + chunkDir.x);
            if (chunkID < 0 || chunkID > global.chunkLoadedX * global.chunkLoadedY - 1) { return 255; }

            chunk = activeChunks[chunkID].GetComponent<CreateChunkMesh>();
            return chunk.map[newPos.y * chunk.width + newPos.x].type;
        }
        else
        {
            newPos = new Vector2Int(newPixelPos.x, newPixelPos.y);

            return chunk.map[newPixelPos.y * chunk.width + newPixelPos.x].type;
        }
    }
    public void GenerateMap()
    {
        //Vector3 offset = new Vector3(5, 5); //Hvor den nederste chunken er
        for (float y = 0; y < global.chunkLoadedY; y++)
        {
            for (float x = 0; x < global.chunkLoadedX; x++)
            {
                CreateChunk(x, y);
            }
        }
    }
    public GameObject CreateChunk(float x, float y) 
    {
        GameObject chunk = Instantiate(chunkPrefab, this.transform);
        
        //chunk.GetComponent<CreateChunkMesh>().nG = nG;
        //chunk.GetComponent<CreateChunkMesh>().ID = (int)(y * (loadSize.x * 2 + 1) + x);
        chunk.GetComponent<MeshRenderer>().materials = C_I.mats;

        chunkArray[(int)y * global.chunkLoadedX + (int)x] = chunk;
        
        chunk.SetActive(false);
        
        return chunk;
    }
}

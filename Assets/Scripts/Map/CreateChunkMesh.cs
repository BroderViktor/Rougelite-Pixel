using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]

public class CreateChunkMesh : MonoBehaviour
{
    #region Vars
    public Vector2 Pos;

    public NoiseGenerator nG;

    public int width, height, ID;
    public short arrayPos;
    public byte pixelsPerUnit;

    public List<Plant> plants = new List<Plant>();

    [SerializeField] PolygonCollider2D polyCollider;
    [SerializeField] Texture2D textureAtlas;

    /*[SerializeField] CreateMesh meshCreator;
    [SerializeField] TraceOutlines outline;*/

    [SerializeField] int textureResolution;

    Cell_Info C_I;
    PlantGenerator plnt_Gen;
    
    bool foundVar;

    public Cell[] map;

    public bool isUsable = false, isActive = false;

    public int x, y;
    public int cellNUmber; 
    #endregion

    void Start()
    {
        //GenerateChunk();
        C_I = FindObjectOfType<Cell_Info>();
        plnt_Gen = FindObjectOfType<PlantGenerator>();
        //meshCreator.C_I = C_I;
    }

    /*public void GenerateChunk()
    {
        map = GenerateChunkMap();
        GenerateMesh();
    }*/
    /*public void GenerateMesh()
    {
        meshCreator.GenerateMesh(map, 0);
        outline.TraceMapOutline(map);
        //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        //timer.Start();
        //meshCreator.GenerateMesh(map, width, height, pixelsPerUnit);
        //timer.Stop();
        //Debug.Log("Meshgen: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        //timer.Restart();
        //if (collider) outline.CreatePolygonCollider(map, width, height, polyCollider, pixelsPerUnit);
        //timer.Stop();
        //Debug.Log("collider: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
    }*/

    public Cell[] GenerateChunkMap()
    {
        if (!foundVar)
        {
            C_I = FindObjectOfType<Cell_Info>();
            
            plnt_Gen = FindObjectOfType<PlantGenerator>();
            foundVar = true;
        }

        Cell[] chunkMap = new Cell[width * height];


        byte[] noiseMap = nG.GenerateNoiseMap(Pos.x, Pos.y);

        plants = new List<Plant>();

        System.Random rng = new System.Random((int)(Pos.x * Pos.y * nG.seed));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte nodeType = noiseMap[y * height + x];

                if (nodeType == C_I.Grass)
                {
                    if (y < height - 1)
                    {
                        if (noiseMap[(y + 1) * height + x] == C_I.Air)
                        {
                            int num = rng.Next(0, 120);
                            if (num > 22 && num < 26)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, rng.Next(2, 3), 0, arrayPos, C_I.plant1, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, 1)));
                            }
                            else if (num <= 22)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, 1, 1, arrayPos, C_I.plant2, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2)*2-1, 1)));
                            }
                            else if (num > 118)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, 3, 2, arrayPos, C_I.plant3, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2)*2-1, 1)));
                            }
                        }
                    }
                    if (y > 0)
                    {
                        if (noiseMap[(y - 1) * height + x] == C_I.Air)
                        {
                            if (rng.Next(0, 25) == 0)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, rng.Next(1, 3), 3, arrayPos, C_I.plant1, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, -1)));
                            }
                        } 
                    }
                }
                //Vector2Int chunkPos = new Vector2Int((int)(transform.position.x * 32), (int)(transform.position.y * 32));

                chunkMap[y * width + x] = new Cell(new Vector2Int(x, y), this, nodeType);
            }
        }
       
        return chunkMap;
    }
}

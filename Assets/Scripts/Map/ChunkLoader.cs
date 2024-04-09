using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public List<GameObject> useableChunks = new List<GameObject>();

    MapGen mpgn;
    PlantGenerator plnt_Gen;
    ThreadedChunkGen chunkGen;
    Cell_Info C_I;
    NoiseGenerator NG;

    Vector2Int spillerPos = Vector2Int.zero;
    
    float chunkSize;

    public void Start()
    {
        /*System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        timer.Start();*/
        mpgn = FindObjectOfType<MapGen>();
        plnt_Gen = FindObjectOfType<PlantGenerator>();
        chunkGen = FindObjectOfType<ThreadedChunkGen>();
        C_I = FindObjectOfType<Cell_Info>();
        NG = FindObjectOfType<NoiseGenerator>();

        mpgn.activeChunks = new GameObject[(1 + mpgn.loadSize.x * 2) * (1 + mpgn.loadSize.y * 2)];

        foreach (GameObject chunk in mpgn.chunkArray)
        {
            useableChunks.Add(chunk);
        }
        chunkSize = (float)global.chunkWidth / global.PPU;

        spillerPos = new Vector2Int((int)(transform.position.x / chunkSize), (int)(transform.position.y / chunkSize));
        if (transform.position.x < 0) spillerPos += new Vector2Int(-1, 0);
        //if (transform.position.y < 0) spillerPos += new Vector2Int(0, -1);
       
        for (int x = -mpgn.loadSize.x; x < mpgn.loadSize.x + 1; x++)
        {
            for (int y = -mpgn.loadSize.y; y < mpgn.loadSize.y + 1; y++)
            {
                LoadChunk(x, y, spillerPos);
            }
        }
        //foreach (CreateChunkMesh chunk in chunks)
        //{
        //    //Debug.Log(chunk.arrayPos + "#####################################");

        //    foreach (Plant plant in chunk.plants)
        //    {
        //        plntGen.CreatePlant(plant.branchPaths, plant.chunkID, plant.type, plant.pos, plant.dir);
        //    }

        //    chunk.GenerateMesh();
        //}
        /*timer.Stop();
        Debug.Log("Time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));*/
        //StartCoroutine("checkMove");
    }
    private void Update()
    {
        //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        //timer.Start();
        Vector2Int pos = new Vector2Int((int)(transform.position.x / chunkSize), (int)(transform.position.y / chunkSize));
        if (transform.position.x < 0) pos += new Vector2Int(-1, 0);
        //if (transform.position.y < 0) pos += new Vector2Int(0, -1);

        if (pos != spillerPos)
        {


            global.chunkOffset = pos;
            Vector2Int heading = pos - spillerPos; //Finner retningen

            SortChunks();

            if (heading.x > 0)
            {
                for (int x = -mpgn.loadSize.x + (mpgn.loadSize.x * 2 - heading.x); x < mpgn.loadSize.x; x++)
                {
                    for (int y = -mpgn.loadSize.y; y < mpgn.loadSize.y + 1; y++)
                    {
                        LoadChunk(x + 1, y, pos);
                    }
                }
            }
            if (heading.y > 0)
            {
                for (int x = -mpgn.loadSize.x; x < mpgn.loadSize.x + 1; x++)
                {
                    for (int y = -mpgn.loadSize.y + (mpgn.loadSize.y * 2 - heading.y); y < mpgn.loadSize.y; y++)
                    {
                        LoadChunk(x, y + 1, pos);
                    }
                }
            }
            if (heading.x < 0)
            {
                for (int x = mpgn.loadSize.x - (mpgn.loadSize.x * 2 + heading.x); x > -mpgn.loadSize.x; x--)
                {
                    for (int y = -mpgn.loadSize.y; y < mpgn.loadSize.y + 1; y++)
                    {
                        LoadChunk(x - 1, y, pos);
                    }
                }
            }
            if (heading.y < 0)
            {
                for (int x = -mpgn.loadSize.x; x < mpgn.loadSize.x + 1; x++)
                {
                    for (int y = mpgn.loadSize.y - (mpgn.loadSize.y * 2 + heading.y); y > -mpgn.loadSize.y; y--)
                    {
                        LoadChunk(x, y - 1, pos);
                    }
                }
            }

            //foreach (CreateChunkMesh chunk in chunks)
            //{
            //    foreach (Plant plant in chunk.plants)
            //    {
            //        plntGen.CreatePlant(plant.branchPaths, plant.chunkID, plant.type, plant.pos, plant.dir);
            //    }

            //    chunk.GenerateMesh();
            //}

            spillerPos = pos;

            
            
        }
        //timer.Stop();
        //Debug.Log("Moved Chunks: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
    }
    void SortChunks()
    {
        List<GameObject> chunks = new List<GameObject>();
        chunks.AddRange(mpgn.activeChunks);

        mpgn.activeChunks = new GameObject[(1 + mpgn.loadSize.x * 2) * (1 + mpgn.loadSize.y * 2)];
        
        foreach (GameObject chunk in chunks)
        {
            if (chunk != null)
            {
                MoveArrayPos(chunk);
            }
        }
    }
    void LoadChunk(float x, float y, Vector2Int intSpillerPos)
    {
        /*System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        t.Start();*/
        Vector3 pos = new Vector3(chunkSize * (x + intSpillerPos.x), chunkSize * (y + intSpillerPos.y)); //Posisjon i world coords
        
        GameObject chunk;

        if (useableChunks.Count == 0)
        {
            chunk = mpgn.CreateChunk(0, 0);
        }
        else
        {
            chunk = useableChunks[0];
        }
        //Debug.Log("1:  " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        
        chunk.transform.position = pos;
        chunk.GetComponent<CreateChunkMesh>().Pos = pos;
        //Debug.Log("2:   " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        MoveArrayPos(chunk);
        //Debug.Log("3:   " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        GenerateChunk(chunk.GetComponent<CreateChunkMesh>(), true);

        chunk.SetActive(true);
        //Debug.Log("4: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        //t.Stop();
        //.Log("Chunk Gen" + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
    }
    public void GenerateChunk(CreateChunkMesh chunk, bool generateNewMap)
    {
        if (generateNewMap) chunkGen.RequestMapData(MapDataRecived, chunk);
        else if (!generateNewMap) chunkGen.RequestMeshData(MeshDataRecived, chunk); 
    }
    void MapDataRecived(MapData data)
    {
        data.chunk.map = CreateMap(data.noiseMap, data.chunk);
        //foreach (Cell s in data.chunk.map) Debug.Log(data.chunk.ID + "   " + s.type);
        chunkGen.RequestMeshData(MeshDataRecived, data.chunk);
    }
    Cell[] CreateMap(byte[] noiseMap, CreateChunkMesh chunk)
    {
        Cell[] map = new Cell[noiseMap.Length];

        chunk.plants = new List<Plant>();

        System.Random rng = new System.Random((int)(chunk.Pos.x * chunk.Pos.y * NG.seed * chunk.arrayPos));

        for (int y = 0; y < global.chunkHeight; y++)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                byte nodeType = noiseMap[y * global.chunkHeight + x];
                
                if (nodeType == C_I.Grass)
                {
                    if (y < global.chunkHeight - 1)
                    {
                        if (noiseMap[(y + 1) * global.chunkHeight + x] == C_I.Air)
                        {
                            int num = rng.Next(0, 120);
                            if (num > 22 && num < 26)
                            {
                                chunk.plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, rng.Next(2, 3), 0, chunk.arrayPos, C_I.plant1, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, 1)));
                            }
                            else if (num <= 22)
                            {
                                chunk.plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, 1, 1, chunk.arrayPos, C_I.plant2, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, 1)));
                            }
                            else if (num > 118)
                            {
                                chunk.plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, 3, 2, chunk.arrayPos, C_I.plant3, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, 1)));
                            }
                        }
                    }
                    if (y > 0)
                    {
                        if (noiseMap[(y - 1) * global.chunkHeight + x] == C_I.Air)
                        {
                            if (rng.Next(0, 25) == 0)
                            {
                                chunk.plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, rng.Next(1, 3), 3, chunk.arrayPos, C_I.plant1, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, -1)));
                            }
                        }
                    }
                }
                //Vector2Int chunkPos = new Vector2Int((int)(transform.position.x * 32), (int)(transform.position.y * 32));
                //if (chunk.arrayPos == 4)Debug.Log("noise: " + nodeType);
                map[y * global.chunkWidth + x] = new Cell(new Vector2Int(x, y), chunk, nodeType);
            }
        }

        return map;
    }
    void MeshDataRecived(MeshData data)
    {
        data.chunk.GetComponent<MeshFilter>().sharedMesh = CreateMesh(data.vertArray, data.trisArrays);
        
        SetPolygonCollider(data);
    }
    void SetPolygonCollider(MeshData data)
    {
        PolygonCollider2D collider = data.chunk.GetComponent<PolygonCollider2D>();

        collider.enabled = false;
        collider.pathCount = 0;
        collider.pathCount = data.outlinePoints.Count; //Setter kollisjonsblokken til å ha plass til så mange deler

        int pathIndex = 0;
        foreach (Vector2[] corners in data.outlinePoints) //Går gjennom lista av omrisser
        {
            collider.SetPath(pathIndex, corners); //Setter kollisjonsblokkene til å bruke omrissene
            pathIndex++;
        }
        collider.enabled = true;
    }
    Mesh CreateMesh(Vector3[] vertArray, int[][] trisArrays)
    {
        Mesh mesh = new Mesh();

        mesh.subMeshCount = C_I.renderedTypes;

        mesh.vertices = vertArray;

        for (int i = 0; i < trisArrays.Length; i++)
        {
            mesh.SetTriangles(trisArrays[i], i);
        }

        return mesh;
    }
    void UnloadChunk(GameObject chunk)
    {
        useableChunks.Add(chunk);
        chunk.SetActive(false);
        chunk.GetComponent<CreateChunkMesh>().arrayPos = 0;
        //chunk.GetComponent<CreateChunkMesh>().map = new Cell[global.chunkWidth * global.chunkHeight];
    }
    void MoveArrayPos(GameObject chunk)
    {
        Vector2Int intPos =
            new Vector2Int(Mathf.RoundToInt(chunk.transform.position.x / chunkSize), Mathf.RoundToInt(chunk.transform.position.y / chunkSize));

        Vector2Int AdjustedPos = intPos - global.chunkOffset + mpgn.loadSize;

        short arrayPos = (short)(AdjustedPos.y * (1 + mpgn.loadSize.y * 2) + AdjustedPos.x);

        if (AdjustedPos.x > (mpgn.loadSize.x * 2) || AdjustedPos.x < 0 
          ||AdjustedPos.y > (mpgn.loadSize.y * 2) || AdjustedPos.y < 0)
        {
            UnloadChunk(chunk);

            return;
        }

        if (mpgn.activeChunks[arrayPos] != null)
        {
            UnloadChunk(mpgn.activeChunks[arrayPos]);
        }
        chunk.GetComponent<CreateChunkMesh>().arrayPos = arrayPos;
        mpgn.activeChunks[arrayPos] = chunk;

        useableChunks.Remove(chunk);
    }
}

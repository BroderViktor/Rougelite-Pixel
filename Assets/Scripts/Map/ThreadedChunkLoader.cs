using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class ThreadedChunkLoader : MonoBehaviour
{
    public NoiseGenerator NG;
    public MapGen MP;
    public CreateMesh CM;
    public Cell_Info C_I;
    public PlantGenerator plnt_Gen;
    public GameObject chunkinstanteate;
    private void Start()
    {
        CreateChunkMesh[] chunks = new CreateChunkMesh[13];
        for (int i = 0; i < chunks.Length; i++)
        {
            var f = Instantiate(chunkinstanteate).GetComponent<CreateChunkMesh>();
            f.nG = NG;
            f.Pos = new Vector2(-10, -20);
            chunks[i] = f;
        }
        //GenerateChunks(chunks);
        GenerateChunksThreaded(chunks);
        //Debug.Log("HEi");
        //GenerateChunks2(chunks);
        
    }
    public async Task GenerateChunks(CreateChunkMesh[] chunks)
    {
        Task<byte[]>[] tasks = new Task<byte[]>[chunks.Length];

        System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        t.Start();
        for (int x = 0; x < chunks.Length; x++)
        {
            Vector2 pos = new Vector2(chunks[x].Pos.x, chunks[x].Pos.y);
            short arrayPos = chunks[x].arrayPos;
            tasks[x] = Task<byte[]>.Factory.StartNew(() => NG.GenerateNoiseMap(pos.x, pos.y));
        }
        //Debug.Log("beforeresult: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

        byte[][] result = await Task.WhenAll(tasks);

        //t.Stop();
        //Debug.Log("full time: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

        for (int x = 0; x < chunks.Length; x++)
        {
            Vector2 pos = new Vector2(chunks[x].Pos.x, chunks[x].Pos.y);
            short arrayPos = chunks[x].arrayPos;
            chunks[x].map = GenerateChunkMap(pos, arrayPos, result[x]);
        }
        foreach (CreateChunkMesh chunk in chunks)
        {
            //chunk.GenerateMesh();
        }
        //t.Restart();

        //Task<data>[] meshTask = new Task<data>[chunks.Length];
        //for (int x = 0; x < chunks.Length; x++)
        //{
        //    Cell[] map = chunks[x].map;
        //    meshTask[x] = Task<data>.Factory.StartNew(() => GenerateMeshData(map));
        //}
        ////Debug.Log("before finish: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        //data[] meshresult = await Task.WhenAll(meshTask);
        ////t.Stop();
        ////Debug.Log("data: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        ////t.Restart();
        //for (int x = 0; x < chunks.Length; x++)
        //{
        //    data d = meshresult[x];
        //    Mesh mesh = new Mesh();

        //    mesh.Clear();

        //    mesh.subMeshCount = C_I.renderedTypes;

        //    mesh.vertices = d.vert;

        //    for (int i = 0; i < d.trisArrays.Length; i++)
        //    {
        //        mesh.SetTriangles(d.trisArrays[i], i);
        //    }
        //    //mesh.uv = uv;
        //    //mesh.normals = normals;
        //    //mesh.RecalculateNormals();

        //    chunks[x].GetComponent<MeshFilter>().sharedMesh = mesh;
        //}
        
        t.Stop();
        Debug.Log("meshgen: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
    }
    public void GenerateChunksThreaded(CreateChunkMesh[] chunks)
    {
        System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        t.Start();
        Thread[] threads = new Thread[chunks.Length];
        byte[] map = new byte[0];
        Vector2 e = Vector2.zero;
        Thread thread = new Thread(new ThreadStart(() => NG.GenerateNoiseMapThreaded(e.x, e.y, map)));
        thread.Start();
        thread.Join();
        Debug.Log(map.Length);
        /*for (int i = 0; i < chunks.Length; i++)
        {
            CreateChunkMesh chunk = chunks[i];
            Vector2 pos = new Vector2(chunk.Pos.x, chunk.Pos.y);


            threads[i] = new Thread(new ThreadStart(() => NG.GenerateNoiseMapThreaded(pos.x, pos.y, map[i])));
            threads[i].Start();
            threads[i].Join();
            Debug.Log(map[i].Length);
        }*/
        t.Stop();
        Debug.Log("full time: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

    }

    public void GenerateChunks2(CreateChunkMesh[] chunks)
    {
        System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        t.Start();
        foreach (CreateChunkMesh chunk in chunks)
        {
            chunk.GetComponent<CreateChunkMesh>().map = chunk.GetComponent<CreateChunkMesh>().GenerateChunkMap();
            //chunk.GenerateMesh();
        }
        t.Stop();
        Debug.Log("full time: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

    }
    //data Meshtest()
    //{
    //    data d = new data(new Vector3[] { new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 0) }, new int[] { 0, 1, 2 });

    //    return d;
    //}
    public data GenerateMeshData(Cell[] map)
    {
        System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch t2 = new System.Diagnostics.Stopwatch();
        t.Start();
        Vector3[] vertArray = new Vector3[global.chunkWidth * global.chunkHeight * 4];
        //Vector2[] uv = new Vector2[vertArray.Length];
        //Vector3[] normals = new Vector3[vertArray.Length];
        t2.Start();
        int[][] triangleArrays = new int[C_I.renderedTypes][];
        t2.Stop();
        Debug.Log("createArray: " + t2.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        for (byte i = 0; i < triangleArrays.Length; i++)
        {
            t2.Restart();
            triangleArrays[i] = new int[global.chunkWidth * global.chunkHeight * 2 * 3];
            t2.Stop();
            Debug.Log("createSubArray: " + t2.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        }


        for (int y = 0; y < global.chunkHeight; y++)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {

                if (C_I.blockArray[map[y * global.chunkWidth + x].type].renderedInChunk)
                {
                    int squareIndex = (y * global.chunkWidth + x) * 4; //For hver firkant skal det være 4 vertices

                    vertArray[squareIndex + 0] = new Vector3((float)x / global.PPU, (float)y / global.PPU);
                    vertArray[squareIndex + 1] = new Vector3(((float)x + 1) / global.PPU, (float)y / global.PPU);
                    vertArray[squareIndex + 2] = new Vector3((float)x / global.PPU, ((float)y + 1) / global.PPU);
                    vertArray[squareIndex + 3] = new Vector3(((float)x + 1) / global.PPU, ((float)y + 1) / global.PPU);

                    //normals[squareIndex + 0] = new Vector3(Random.Range(0, 1) * 2 - 1f, Random.Range(0, 1) * 2 - 1f, 0;
                    //normals[squareIndex + 1] = new Vector3(Random.Range(0, 1) * 2 - 1f, Random.Range(0, 1) * 2 - 1f, Random.Range(0, 1) * 2 - 1f);
                    //normals[squareIndex + 2] = new Vector3(Random.Range(0, 1) * 2 - 1f, Random.Range(0, 1) * 2 - 1f, Random.Range(0, 1) * 2 - 1f);
                    //normals[squareIndex + 3] = new Vector3(Random.Range(0, 1) * 2 - 1f, Random.Range(0, 1) * 2 - 1f, Random.Range(0, 1) * 2 - 1f);

                    /*Vector3 normalDir = new Vector3(Random.Range(-normalStrenght, normalStrenght), Random.Range(-normalStrenght, normalStrenght), 1);
                    
                    normals[squareIndex + 0] = normalDir;
                    normals[squareIndex + 1] = normalDir;
                    normals[squareIndex + 2] = normalDir;
                    normals[squareIndex + 3] = normalDir;*/


                    /*uv[squareIndex + 0] = new Vector2(0, 0);
                    uv[squareIndex + 1] = new Vector2(1, 0);
                    uv[squareIndex + 2] = new Vector2(0, 1);
                    uv[squareIndex + 0] = new Vector2(1, 1);*/
                }

            }
        }

        for (int y = 0; y < global.chunkHeight; y++)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                BlockType blokk = C_I.blockArray[map[y * global.chunkWidth + x].type];
                if (blokk.renderedInChunk)
                {
                    int squareIndex = y * global.chunkWidth + x;

                    int triangleOffset = squareIndex * 6;
                    int squareOffset = squareIndex * 4;

                    triangleArrays[blokk.rendererNum][triangleOffset + 0] = squareOffset + 0;
                    triangleArrays[blokk.rendererNum][triangleOffset + 1] = squareOffset + 2;
                    triangleArrays[blokk.rendererNum][triangleOffset + 2] = squareOffset + 1;

                    triangleArrays[blokk.rendererNum][triangleOffset + 3] = squareOffset + 1;
                    triangleArrays[blokk.rendererNum][triangleOffset + 4] = squareOffset + 2;
                    triangleArrays[blokk.rendererNum][triangleOffset + 5] = squareOffset + 3;
                }
            }
        }

        data d = new data(vertArray, triangleArrays);
        t.Stop();
        Debug.Log("meshdata: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        return d; 

    }
    public struct data
    {
        public readonly Vector3[] vert;
        public readonly int[][] trisArrays;
        public data(Vector3[] _vert, int[][] _tris)
        {
            vert = _vert;
            trisArrays = _tris;
        }
    }
    public Cell[] GenerateChunkMap(Vector2 Pos, short arrayPos, byte[] noiseMap)
    {
        Cell[] chunkMap = new Cell[global.chunkWidth * global.chunkHeight];

        //plants = new List<Plant>();

        System.Random rng = new System.Random((int)(Pos.x * Pos.y * NG.seed));
        for (int y = 0; y < global.chunkHeight; y++)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                byte nodeType = noiseMap[y * global.chunkWidth + x];
#if false
                if (nodeType == C_I.Grass)
                {
                    if (y < global.chunkHeight - 1)
                    {
                        if (noiseMap[(y + 1) * global.chunkWidth + x] == C_I.Air)
                        {
                            int num = rng.Next(0, 120);
                            if (num > 22 && num < 26)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, rng.Next(2, 3), 0, arrayPos, C_I.plant1, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, 1)));
                            }
                            else if (num <= 22)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, 1, 1, arrayPos, C_I.plant2, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, 1)));
                            }
                            else if (num > 118)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, 3, 2, arrayPos, C_I.plant3, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, 1)));
                            }
                        }
                    }
                    if (y > 0)
                    {
                        if (noiseMap[(y - 1) * global.chunkWidth + x] == C_I.Air)
                        {
                            if (rng.Next(0, 25) == 0)
                            {
                                plants.Add(plnt_Gen.GeneratePlant(new byte[] { 0 }, rng.Next(1, 3), 3, arrayPos, C_I.plant1, new Vector2Int(x, y), new Vector2Int(rng.Next(0, 2) * 2 - 1, -1)));
                            }
                        }
                    }
                } 
#endif
                //Vector2Int chunkPos = new Vector2Int((int)(transform.position.x * 32), (int)(transform.position.y * 32));
                
                chunkMap[y * global.chunkWidth + x] = new Cell(new Vector2Int(x, y), MP.chunkArray[arrayPos].GetComponent<CreateChunkMesh>(), nodeType);

            }
        }
        return chunkMap;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : MonoBehaviour
{
    public Cell_Info C_I;
    bool foundCellInfo = false;

    public void GenerateWaterMesh(Cell[] map, int PPU)
    {
        Vector3[] vertArray = new Vector3[map.Length * 4];
        //Vector2[] uv = new Vector2[vertArray.Length];

        int[] triangleArray = new int[map.Length * 2 * 3];

        int x = 0;
        foreach (Cell cell in map)
        {
            int squareIndex = x * 4; //For hver firkant skal det være 4 vertice

                /*Debug.Log("pos"  + cell.pos);
                Debug.Log("chunk" + cell.chunk.arrayPos);
                Debug.Log("offset" + cell.chunkOffset);*/
            Vector2Int chunkOffset = new Vector2Int(
                (int)(cell.chunk.transform.position.x * PPU), 
                (int)(cell.chunk.transform.position.y * PPU));

            Vector2 cellWorldPos = cell.pos + chunkOffset;

            vertArray[squareIndex + 0] = new Vector3(cellWorldPos.x,        cellWorldPos.y      ) / PPU;
            vertArray[squareIndex + 1] = new Vector3(cellWorldPos.x + 1,    cellWorldPos.y      ) / PPU;
            vertArray[squareIndex + 2] = new Vector3(cellWorldPos.x,        cellWorldPos.y + 1  ) / PPU;
            vertArray[squareIndex + 3] = new Vector3(cellWorldPos.x + 1,    cellWorldPos.y + 1  ) / PPU;

            /*uv[squareIndex + 0] = new Vector2(0, 0);
            uv[squareIndex + 1] = new Vector2(1, 0);
            uv[squareIndex + 2] = new Vector2(0, 1);
            uv[squareIndex + 0] = new Vector2(1, 1);*/
            x++;
        }
        int y = 0;
        foreach (Cell cell in map)
        {
            int squareIndex = y;

            int triangleOffset = squareIndex * 6;
            int squareOffset = squareIndex * 4;

            triangleArray[triangleOffset + 0] = squareOffset + 0;
            triangleArray[triangleOffset + 1] = squareOffset + 2;
            triangleArray[triangleOffset + 2] = squareOffset + 1;

            triangleArray[triangleOffset + 3] = squareOffset + 1;
            triangleArray[triangleOffset + 4] = squareOffset + 2;
            triangleArray[triangleOffset + 5] = squareOffset + 3;
            y++;
        }

        Mesh mesh = new Mesh();

        mesh.Clear();

        mesh.vertices = vertArray;
        mesh.triangles = triangleArray;
        //mesh.uv = uv;

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
    public void GenerateMesh(Cell[] map, float normalStrenght)
    {
        if (!foundCellInfo) { C_I = FindObjectOfType<Cell_Info>(); foundCellInfo = true; }
        
        Vector3[] vertArray = new Vector3[global.chunkWidth * global.chunkHeight * 4];
        //Vector2[] uv = new Vector2[vertArray.Length];
        //Vector3[] normals = new Vector3[vertArray.Length];
        System.Diagnostics.Stopwatch t2 = new System.Diagnostics.Stopwatch();
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

        Mesh mesh = new Mesh();
        
        mesh.Clear();

        mesh.subMeshCount = C_I.renderedTypes;

        mesh.vertices = vertArray;

        for (int i = 0; i < triangleArrays.Length; i++)
        {
            mesh.SetTriangles(triangleArrays[i], i);
        }
        //mesh.uv = uv;
        //mesh.normals = normals;
        //mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
    public void GenerateMeshData(Cell[] map, out Vector3[] vertArray, out int[][] triangleArrays)
    {
        vertArray = new Vector3[global.chunkWidth * global.chunkHeight * 4];

        triangleArrays = new int[C_I.renderedTypes][];

        for (byte i = 0; i < triangleArrays.Length; i++)
        {
            triangleArrays[i] = new int[global.chunkWidth * global.chunkHeight * 2 * 3];
        }
        int count = 0;
        for (int y = 0; y < global.chunkHeight; y++)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                if (map[y * global.chunkWidth + x].type != 3) count++;
                if (C_I.blockArray[map[y * global.chunkWidth + x].type].renderedInChunk)
                {
                    int squareIndex = (y * global.chunkWidth + x) * 4; //For hver firkant skal det være 4 vertices

                    vertArray[squareIndex + 0] = new Vector3((float)x / global.PPU, (float)y / global.PPU);
                    vertArray[squareIndex + 1] = new Vector3(((float)x + 1) / global.PPU, (float)y / global.PPU);
                    vertArray[squareIndex + 2] = new Vector3((float)x / global.PPU, ((float)y + 1) / global.PPU);
                    vertArray[squareIndex + 3] = new Vector3(((float)x + 1) / global.PPU, ((float)y + 1) / global.PPU);
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
    }
}

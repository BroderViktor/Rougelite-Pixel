using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulateWorld : MonoBehaviour
{
    public LayerMask destructible;
    public ChunkLoader chunkLoader;

    [SerializeField] MapGen mapgen;
    [SerializeField] GameObject pixel;
    [SerializeField] GameObject waterChunk;
    [SerializeField] GameObject rubleEffect;

    SubstanceManager waterManager;
    Cell_Info C_I;
    GameObject currentSubstanceRenderer;

    public float rubleCooldown;
    bool canMakeRuble = true;

    private void Start()
    {
        mapgen = FindObjectOfType<MapGen>();
        C_I = mapgen.C_I;
        waterManager = FindObjectOfType<SubstanceManager>();
    }
    public void DestroyBlock(Vector2 point, float size)
    {
        Vector2Int mapPosInt = global.ConvertWorldToMapPos(point);

        //mapPosInt += new Vector2Int(() * (int)global.chunkWidth, (mapgen.loadSize.y) * (int)global.chunkWidth);

        int destroyedBlocksNum = 0;
        byte destroyedType = 0;

        int radSqured = (int)(size / 2) * (int)(size / 2) + (int)(size / 2);
        
        List<CreateChunkMesh> chunksList = new List<CreateChunkMesh>();
        
        for (float y = -size / 2; y < size / 2 + 1; y++)
        {
            for (float x = -size / 2; x < size / 2 + 1; x++)
            {
                float radius = (x * x + y * y);
                if (radius < radSqured && radius > -radSqured)
                {
                    int chunkID;
                    Vector2Int pos;

                    if (C_I.blockArray[mapgen.ReturnValueOfPoint(mapPosInt + new Vector2Int((int)x, (int)y), out chunkID, out pos)].destroyable)
                    {
                        CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();

                        int nodeNum = pos.y * chunk.width + pos.x;

                        Cell block = chunk.map[nodeNum];

                        if (Random.Range(0, 3) == 0) destroyedType = block.type;
                        destroyedBlocksNum++;

                        block.type = C_I.Air;
                        
                        if (!chunksList.Contains(chunk)) chunksList.Add(chunk);
                    }
                }
                
            }
        }


        foreach (CreateChunkMesh chunk in chunksList)
        {
            chunkLoader.GetComponent<ChunkLoader>().GenerateChunk(chunk, false);   
        }

        if (canMakeRuble && destroyedBlocksNum > 0)
        {
            CreateRuble(point, destroyedBlocksNum, size, destroyedType);
        }
    }
    public void DestroyPixels(Vector2Int[] positions)
    {
        int destroyedBlocksNum = 0;
        byte destroyedType = 0;

        List<CreateChunkMesh> chunksList = new List<CreateChunkMesh>();
        foreach (Vector2Int mapPosInt in positions)
        { 
            int chunkID;
            Vector2Int pos;

            if (C_I.blockArray[mapgen.ReturnValueOfPoint(mapPosInt, out chunkID, out pos)].destroyable)
            {
                CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();

                int nodeNum = pos.y * chunk.width + pos.x;

                Cell block = chunk.map[nodeNum];

                if (Random.Range(0, 3) == 0) destroyedType = block.type;
                destroyedBlocksNum++;

                block.type = C_I.Air;

                if (!chunksList.Contains(chunk)) chunksList.Add(chunk);
            }
        }
        foreach (CreateChunkMesh chunk in chunksList)
        {
            chunkLoader.GetComponent<ChunkLoader>().GenerateChunk(chunk, false);
        }

        /*if (canMakeRuble && destroyedBlocksNum > 0)
        {
            CreateRuble(point, destroyedBlocksNum, size, destroyedType);
        }*/
    }
    public void CreateBlocks(Vector2 point, float size, byte type)
    {
        Cell[] cellArray = new Cell[(int)(size * size) * 4];
        CreateChunkMesh[] chunksArray = new CreateChunkMesh[15];

        if (type == C_I.Water)
        {
            if (currentSubstanceRenderer == null || currentSubstanceRenderer.GetComponent<SimulatedSubstanceManager>().numMembers + size * size > (int)(size * size) * 5)
            {
                currentSubstanceRenderer = Instantiate(waterChunk);
                currentSubstanceRenderer.GetComponent<SimulatedSubstanceManager>().waterMap = new Cell[(int)(size * size) * 5];
                currentSubstanceRenderer.GetComponent<SimulatedSubstanceManager>().mapgen = mapgen;
                currentSubstanceRenderer.transform.position = Vector3.zero;
                currentSubstanceRenderer.transform.parent = waterManager.transform;

                currentSubstanceRenderer.GetComponent<SimulatedSubstanceManager>().arrayNum = waterManager.waterNum;

                waterManager.SubSimMeshList.Add(currentSubstanceRenderer.GetComponent<SimulatedSubstanceManager>());
                waterManager.waterNum++;

                StartCoroutine("NewSubSimManagerTimer");
            }
        }

        Vector2 mapPos = point * global.PPU;
        
        Vector2Int mapPosInt = 
            new Vector2Int(
                (int)(mapPos.x),
                (int)(mapPos.y));

        Vector2Int chunkOffsetPixel = new Vector2Int(
            (int)(global.chunkOffset.x * global.chunkWidth),
            (int)(global.chunkOffset.y * global.chunkHeight));

        mapPosInt -= chunkOffsetPixel;

        mapPosInt += new Vector2Int((mapgen.loadSize.x) * (int)global.chunkWidth, (mapgen.loadSize.y) * (int)global.chunkHeight);
        
        Debug.Log(mapPosInt);

        int num = 0;

        int radSqured = (int)(size / 2) * (int)(size / 2) + (int)(size / 2);
        for (float y = -size / 2; y < size / 2 + 1; y++)
        {
            for (float x = -size / 2; x < size / 2 + 1; x++)
            {
                float radius = (x * x + y * y);
                if (radius < radSqured && radius > -radSqured)
                {
                    int chunkID;
                    Vector2Int pos;
                    
                    if (mapgen.ReturnValueOfPoint(mapPosInt + new Vector2Int((int)x, (int)y), out chunkID, out pos) == C_I.Air)
                    {
                        CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();

                        int nodeNum = pos.y * chunk.width + pos.x;

                        Cell cell = chunk.map[nodeNum];

                        cell.type = type;

                        if (C_I.blockArray[type].solid)
                        {
                            for (int i = 0; i < chunksArray.Length; i++)
                            {
                                if (chunksArray[i] == null) chunksArray[i] = chunk;
                                if (chunksArray[i] == null) Debug.Log(chunk);
                                else if (chunksArray[i] == chunk) i = chunksArray.Length;
                            }
                        }
                        else if (type == C_I.Water) { cellArray[num] = cell; num++; }
                    }
                }
            }
        }

        if (C_I.blockArray[type].solid)
        {
            foreach (CreateChunkMesh chunk in chunksArray)
            {
                if (chunk == null) return;
                chunkLoader.GetComponent<ChunkLoader>().GenerateChunk(chunk, false);
            }
        }

        if (cellArray.Length != 0 && type == C_I.Water) currentSubstanceRenderer.GetComponent<SimulatedSubstanceManager>().AddMembers(cellArray, num);
    }
    IEnumerator NewSubSimManagerTimer()
    {
        yield return new WaitForSeconds(10);
        currentSubstanceRenderer = null;
    }
    IEnumerator NewRubleTimer()
    {
        yield return new WaitForSeconds(rubleCooldown);
        canMakeRuble = true;
    }
    void CreateRuble(Vector2 point, int destroyedBlocksNum, float size, byte type)
    {
        GameObject rubleEffectClone = Instantiate(rubleEffect, point, Quaternion.identity);
        ParticleSystem.Burst burst = rubleEffectClone.GetComponent<ParticleSystem>().emission.GetBurst(0);
        burst.minCount = (short)(destroyedBlocksNum / 20);
        burst.maxCount = (short)(destroyedBlocksNum / 2);
        rubleEffectClone.GetComponent<ParticleSystem>().emission.SetBurst(0, burst);

        ParticleSystem.ShapeModule shape = rubleEffectClone.GetComponent<ParticleSystem>().shape; 
        shape.radius = size / global.PPU / 2;

        ParticleSystemRenderer renderer = rubleEffectClone.GetComponent<ParticleSystemRenderer>();

        renderer.material = C_I.blockArray[type].mat;

        Destroy(rubleEffectClone, 0.75f);
        canMakeRuble = false;
        StartCoroutine("NewRubleTimer");
    }
    public Vector3 returnMouseOverPoint(Vector2 point, out CreateChunkMesh chunk)
    {
        Vector2 mapPos = point * global.PPU;

        Vector2Int mapPosInt =
            new Vector2Int(
                Mathf.RoundToInt(mapPos.x),
                Mathf.RoundToInt(mapPos.y));

        Vector2Int chunkOffsetPixel = new Vector2Int(
            (int)(global.chunkOffset.x * global.chunkWidth),
            (int)(global.chunkOffset.y * global.chunkWidth));

        mapPosInt -= chunkOffsetPixel;

        mapPosInt += new Vector2Int((mapgen.loadSize.x) * (int)global.chunkWidth, (mapgen.loadSize.y) * (int)global.chunkWidth);

        int chunkID;
        Vector2Int pos;
        int v = mapgen.ReturnValueOfPoint(mapPosInt, out chunkID, out pos);
        chunk = mapgen.chunkArray[chunkID].GetComponent<CreateChunkMesh>();

        Vector3 result = new Vector3(pos.x, pos.y, v);
        return result;
    }
}

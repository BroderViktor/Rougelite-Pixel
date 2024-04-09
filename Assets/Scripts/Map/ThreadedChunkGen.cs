using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class ThreadedChunkGen : MonoBehaviour
{
    public NoiseGenerator NG;
    public CreateMesh meshGen;
    public TraceOutlines traceOutline;

    Queue<ThreadResultInfo<MapData>> ThreadResultMapData = new Queue<ThreadResultInfo<MapData>>();
    Queue<ThreadResultInfo<MeshData>> ThreadResultMeshData = new Queue<ThreadResultInfo<MeshData>>();
    private void Update()
    {
        //System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        //t.Start();

        if (ThreadResultMapData.Count > 0)
        {
            for (int i = 0; i < ThreadResultMapData.Count; i++)
            {
                lock (ThreadResultMeshData)
                {
                    ThreadResultInfo<MapData> threadResult = ThreadResultMapData.Dequeue();
                    threadResult.callback(threadResult.parameter);
                }
            }
        }
        if (ThreadResultMeshData.Count > 0)
        {
            for (int i = 0; i < ThreadResultMeshData.Count; i++)
            {
                lock (ThreadResultMeshData)
                {
                    ThreadResultInfo<MeshData> threadResult = ThreadResultMeshData.Dequeue();
                    threadResult.callback(threadResult.parameter);
                }
            }
        }
        //t.Stop();
        //Debug.Log("Tid i millisekunder: " + t.ElapsedMilliseconds);
    }
    public void RequestMapData(Action<MapData> callback, CreateChunkMesh chunk)
    {
        ThreadPool.QueueUserWorkItem((object state) => GenerateChunkDataThread(callback, chunk));
    }
    void GenerateChunkDataThread(Action<MapData> callback, CreateChunkMesh chunk)
    {
        MapData mapData = new MapData(NG.GenerateNoiseMap(chunk.Pos.x, chunk.Pos.y), chunk);
        //if (chunk.arrayPos == 4) foreach (byte b in mapData.noiseMap) Debug.Log("chunk: " + chunk.arrayPos +" noise: " + b);
        ThreadResultInfo<MapData> threadResult = new ThreadResultInfo<MapData>(callback, mapData);
        lock (ThreadResultMapData) {
            ThreadResultMapData.Enqueue(threadResult);
        };
    }
    public void RequestMeshData(Action<MeshData> callback, CreateChunkMesh chunk)
    {
        ThreadPool.QueueUserWorkItem((object state) => GenerateMeshDataThread(callback, chunk));
    }
    void GenerateMeshDataThread(Action<MeshData> callback, CreateChunkMesh chunk)
    {
        MeshData chunkData = GenerateChunkData(chunk);
        ThreadResultInfo<MeshData> threadResult = new ThreadResultInfo<MeshData>(callback, chunkData);
        lock (ThreadResultMeshData)
        {
            ThreadResultMeshData.Enqueue(threadResult);
        };
    }
    MeshData GenerateChunkData(CreateChunkMesh chunk)
    {
        Vector3[] vertArray;
        int[][] trisArrays;

        meshGen.GenerateMeshData(chunk.map, out vertArray, out trisArrays);

        List<Vector2[]> outline = traceOutline.TraceMapOutline(chunk.map);
        return new MeshData(vertArray, trisArrays, outline, chunk);
    }
    struct ThreadResultInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;
        public ThreadResultInfo(Action<T> _callback, T _parameter)
        {
            callback = _callback;
            parameter = _parameter;
        }
    }
}
public struct MapData
{
    public readonly byte[] noiseMap;
    public readonly CreateChunkMesh chunk;
    public MapData(byte[] _noiseMap, CreateChunkMesh _chunk)
    {
        noiseMap = _noiseMap;
        chunk = _chunk;
    }
}
public struct MeshData
{
    public readonly Vector3[] vertArray;
    public readonly int[][] trisArrays;

    public readonly List<Vector2[]> outlinePoints;

    public readonly CreateChunkMesh chunk;
    public MeshData(Vector3[] _vertArray, int[][] _trisArrays, List<Vector2[]> _outline, CreateChunkMesh _chunk)
    {
        vertArray = _vertArray;
        trisArrays = _trisArrays;
        chunk = _chunk;
        outlinePoints = _outline;
    }
}
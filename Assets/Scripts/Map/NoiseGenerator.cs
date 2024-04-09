using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] MapGen mG;

    public bool generateMap, randomSeed;
    public float seed;

    public float scale;
    public int numOctaves;
    public float ampMod, freqMod;
    public bool round;

    Vector2[] randomOctaveOffset;

    public float[] thresholds;
    public byte[] thresholdValues;

    [System.NonSerialized] public System.Random rng;

    void Start()
    {
        if (randomSeed) seed = Random.Range(0, 1000000);
        rng = new System.Random((int)seed);
        randomOctaveOffset = new Vector2[numOctaves];
        for (int i = 0; i < numOctaves; i++)
        {
            randomOctaveOffset[i] = new Vector2(rng.Next(-100000, 100000), rng.Next(-100000, 100000));
        }
    }
    public byte[] GenerateNoiseMap(float _offsetX, float _offsetY)
    {
        System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        t.Start();
        float[] map = new float[global.chunkWidth * global.chunkHeight];

        for (int x = 0; x < global.chunkWidth; x++)
        {
            for (int y = 0; y < global.chunkHeight; y++)
            {
                float heightMultiplier = 1;
                if (y + _offsetY * global.PPU > -400) heightMultiplier = Mathf.Lerp(0, 1, (y + _offsetY * global.PPU) / -100);

                float amplitude = 1;
                float frequency = 1;

                for (int i = 0; i < numOctaves; i++)
                {
                    float valuex = (x + _offsetX * global.PPU) / scale * frequency + randomOctaveOffset[i].x;
                    float valuey = (y + _offsetY * global.PPU) / scale * frequency + randomOctaveOffset[i].y;

                    float noiseValue = Mathf.PerlinNoise(valuex, valuey) * amplitude * heightMultiplier;

                    map[y * global.chunkWidth + x] += noiseValue;

                    amplitude *= ampMod;
                    frequency *= freqMod;
                }
            }
        }
        byte[] byteMap = new byte[global.chunkWidth * global.chunkHeight];
        if (generateMap)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                for (int y = 0; y < global.chunkWidth; y++)
                {
                    //map[y * _width + x] = Mathf.InverseLerp(minValue, maxValue, map[y * _width + x] + 0.1f);
                    //if (round) byteMap[y * _width + x] = (byte)Mathf.RoundToInt(map[y * _width + x] - 0.1f);
                    for (int i = 0; i < thresholds.Length; i++)
                    {
                        if (map[y * global.chunkWidth + x] <= thresholds[i])
                        {
                            byteMap[y * global.chunkWidth + x] = thresholdValues[i];
                        }
                    }
                }
            }
        }
        else for (int i = 0; i < byteMap.Length; i++) byteMap[i] = 3;
        t.Stop();
        //Debug.Log("full time: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        return byteMap;
    }
    public void GenerateNoise2MapThread2ed()
    {
    }
    public void GenerateNoiseMapThreaded(float _offsetX, float _offsetY, byte[] mapResult)
    {
        System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        t.Start();
        float[] map = new float[global.chunkWidth * global.chunkHeight];

        for (int x = 0; x < global.chunkWidth; x++)
        {
            for (int y = 0; y < global.chunkHeight; y++)
            {
                float heightMultiplier = 1;
                if (y + _offsetY * global.PPU > -400) heightMultiplier = Mathf.Lerp(0, 1, (y + _offsetY * global.PPU) / -100);

                float amplitude = 1;
                float frequency = 1;

                for (int i = 0; i < numOctaves; i++)
                {
                    float valuex = (x + _offsetX * global.PPU) / scale * frequency + randomOctaveOffset[i].x;
                    float valuey = (y + _offsetY * global.PPU) / scale * frequency + randomOctaveOffset[i].y;

                    float noiseValue = Mathf.PerlinNoise(valuex, valuey) * amplitude * heightMultiplier;

                    map[y * global.chunkWidth + x] += noiseValue;

                    amplitude *= ampMod;
                    frequency *= freqMod;
                }
            }
        }
        byte[] byteMap = new byte[global.chunkWidth * global.chunkHeight];
        if (generateMap)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                for (int y = 0; y < global.chunkWidth; y++)
                {
                    //map[y * _width + x] = Mathf.InverseLerp(minValue, maxValue, map[y * _width + x] + 0.1f);
                    //if (round) byteMap[y * _width + x] = (byte)Mathf.RoundToInt(map[y * _width + x] - 0.1f);
                    for (int i = 0; i < thresholds.Length; i++)
                    {
                        if (map[y * global.chunkWidth + x] <= thresholds[i])
                        {
                            byteMap[y * global.chunkWidth + x] = thresholdValues[i];
                        }
                    }
                }
            }
        }
        else for (int i = 0; i < byteMap.Length; i++) byteMap[i] = 3;
        t.Stop();
        //Debug.Log("full time: " + t.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        mapResult = byteMap;
    }
}

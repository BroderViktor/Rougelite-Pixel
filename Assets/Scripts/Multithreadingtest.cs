using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Multithreadingtest : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertArray;
    int[] triangleArray;
    bool ehi;
    void Start()
    {
        mesh = new Mesh();
        Thread CreateMesh = new Thread(GenMesh);
        CreateMesh.Start();
        Thread Cretmsh = new Thread(GenMesh);
        Cretmsh.Start();
        CreateMesh.Join();
        Cretmsh.Join();
        mesh.vertices = vertArray;
        mesh.triangles = triangleArray;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void GenMesh()
    {
        vertArray = new Vector3[4];
        vertArray[0] = new Vector3(0,   0);
        vertArray[1] = new Vector3(1,   0);
        vertArray[2] = new Vector3(0,   1);
        vertArray[3] = new Vector3(1,   1);

        triangleArray = new int[6];
        triangleArray[0] = 0;
        triangleArray[1] = 2;
        triangleArray[2] = 1;

        triangleArray[3] = 1;
        triangleArray[4] = 2;
        triangleArray[5] = 3;

        int x = global.PPU;
    }

}

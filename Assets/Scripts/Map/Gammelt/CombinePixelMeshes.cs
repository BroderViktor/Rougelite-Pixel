using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinePixelMeshes : MonoBehaviour
{

    void Start()
    {
        MeshFilter[] pixelMeshes = GetComponentsInChildren<MeshFilter>(); 
        CombineInstance[] combinedMesh = new CombineInstance[pixelMeshes.Length]; 

        for (int x = 0; x < pixelMeshes.Length; x++)
        {
            combinedMesh[x].mesh = pixelMeshes[x].sharedMesh;
            combinedMesh[x].transform = pixelMeshes[x].transform.localToWorldMatrix;
            pixelMeshes[x].gameObject.SetActive(false);
        }
        Debug.Log(combinedMesh.Length);
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combinedMesh);
        transform.gameObject.SetActive(true);
    }

}

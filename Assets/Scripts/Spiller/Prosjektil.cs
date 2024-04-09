using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prosjektil : MonoBehaviour
{
    public int size = 12, Speed;

    public bool DestroyBlocks = true, CreateBlocks = false;
    
    ManipulateWorld maniWorld;

    private void Start()
    {
        maniWorld = FindObjectOfType<ManipulateWorld>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        LayerMask layer = collision.gameObject.layer;

        if (maniWorld.destructible == (maniWorld.destructible | 1 << layer))
        {
            //if (DestroyBlocks) maniWorld.DestroyBlock(collision.GetContact(0).point, size);
            if (CreateBlocks) maniWorld.CreateBlocks(collision.GetContact(0).point, size, 2);
            Destroy(this.gameObject);
        }
    }
}

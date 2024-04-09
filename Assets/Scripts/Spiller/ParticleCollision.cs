using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public int DestructionArea;
    public GameObject Explosion;

    [System.NonSerialized] public ManipulateWorld mW;

    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();


    private void OnParticleCollision(GameObject other)
    {
        int numEvents = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
        foreach (ParticleCollisionEvent Event in collisionEvents)
        {
            mW.DestroyBlock(Event.intersection, DestructionArea);
            GameObject ExplosionClone = Instantiate(Explosion, Event.intersection, Quaternion.identity);
            Destroy(ExplosionClone, 0.4f);
            Destroy(this.gameObject);
        }
    }
}

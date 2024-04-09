using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testProjScript : MonoBehaviour
{
    public ActiveSpellManager spellManager;

    [System.NonSerialized] public float lifetimeRemainder;

    public float lifetime, speed, homingSpeed;

    void FixedUpdate()
    {
        //Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);

        //float Angle = Mathf.Atan2(transform.position.y - mousePos.y, transform.position.x - mousePos.x) * Mathf.Rad2Deg - 180;

        //Quaternion homingAngle = Quaternion.Euler(0, 0, Angle);

        //Quaternion rotation = Quaternion.Lerp(transform.rotation, homingAngle, homingSpeed);

        //transform.rotation = rotation;

        Vector3 nextMove = transform.position + transform.right * speed;
        
        spellManager.PathTransform(transform.position, nextMove);
        
        transform.position = nextMove;
    }
}

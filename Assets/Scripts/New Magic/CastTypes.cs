using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTypes : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject castProjectile()
    {
        Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);

        float Angle = Mathf.Atan2(transform.position.y - mousePos.y, transform.position.x - mousePos.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, Angle - 180));
        proj.GetComponent<testProjScript>().lifetimeRemainder = Time.time + proj.GetComponent<testProjScript>().lifetime;
        return proj;
    }
    public void beam()
    {

    }
}

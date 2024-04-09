using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSpells : MonoBehaviour
{
    public GameObject projectil;

    public bool ON;

    ManipulateWorld mW;

    Camera cam;

    PlayerControll PC;

    public Spell spell;

    public bool fireSpells = false;
    float nextFireTime;

    private void Start()
    {
        cam = Camera.main;
        mW = FindObjectOfType<ManipulateWorld>();
        PC = GetComponent<PlayerControll>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) fireSpells = !fireSpells;
        
        if (Input.GetMouseButton(0) && fireSpells && nextFireTime < Time.time && spell.attackSpeed != 0)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            float Angle = Mathf.Atan2(transform.position.y - mousePos.y, transform.position.x - mousePos.x) * Mathf.Rad2Deg;

            GameObject proj = Instantiate(projectil, transform.position, Quaternion.Euler(0, 0, Angle - 180));
            proj.GetComponent<ParticleCollision>().DestructionArea = Mathf.RoundToInt(spell.damage);
            proj.GetComponent<ParticleCollision>().mW = mW;
            nextFireTime = Time.time + 1 / spell.attackSpeed;
            //proj.GetComponent<Rigidbody2D>().velocity = -proj.transform.right * proj.Speed;
        }
    }
}

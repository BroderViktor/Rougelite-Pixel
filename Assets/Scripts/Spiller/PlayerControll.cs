using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControll : NetworkBehaviour
{
    [SerializeField] float speed, JumpPower, jumpDuration, airModifier, jumpingframes;
    [SerializeField] LayerMask Ground;
    [SerializeField] Vector2 checkSize;
    [SerializeField] Transform groundCheckPos1, headPos, pixelCheckR, pixelCheckL;
    [SerializeField] ManipulateWorld manipulateBlocks;
    [SerializeField] Prosjektil prosjektil;
    [SerializeField] int destructionSize, creationSize;
    
    public Vector3 blockType;
    public CreateChunkMesh chunk;
    public bool singleClick;

    [System.NonSerialized] public byte blockInteractionType = 255;
 
    Rigidbody2D rb;
    Cell_Info C_I;
    PlantGenerator plG;
    CastSpells spellCaster;

    float jumpTime, lastDirection;

    bool isGrounded, isTouchingRoof, continiousJump, sameJump, pushWall, clicked;

    MagicMainScript caster;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        manipulateBlocks = FindObjectOfType<ManipulateWorld>();
        manipulateBlocks.chunkLoader = GetComponent<ChunkLoader>();
        C_I = FindObjectOfType<Cell_Info>();
        spellCaster = GetComponent<CastSpells>();
        global.cam = Camera.main;

        plG = FindObjectOfType<PlantGenerator>();


        caster = FindObjectOfType<MagicMainScript>();
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        FindObjectOfType<CameraFollow>().localSpiller = this;
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyUp(KeyCode.Space) && global.userInput) { continiousJump = false; sameJump = false; }
        
        if (Physics2D.OverlapBox(groundCheckPos1.position, checkSize, 0, Ground)) { isGrounded = true; StopCoroutine("jumpFrames"); }
        else if (!continiousJump) StartCoroutine("jumpFrames");
        else isGrounded = false;

        if (Input.GetKey(KeyCode.Space) && global.userInput)
        {
            if (isGrounded && !sameJump)
            {
                sameJump = true;
                continiousJump = true;
                jumpTime = jumpDuration + Time.time;
                rb.velocity = new Vector2(rb.velocity.x, JumpPower);
            }
            else if (jumpTime > Time.time && continiousJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpPower);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0) && global.userInput) blockInteractionType = 255;
        if (Input.GetKeyDown(KeyCode.Alpha1) && global.userInput) blockInteractionType = 254;
        if (Input.GetKeyDown(KeyCode.Alpha2) && global.userInput) blockInteractionType = C_I.Stone;
        if (Input.GetKeyDown(KeyCode.Alpha3) && global.userInput) blockInteractionType = C_I.Grass;
        if (Input.GetKeyDown(KeyCode.Alpha4) && global.userInput) blockInteractionType = C_I.Water;
        
        //if (Input.GetKeyDown(KeyCode.F)) plG.GenerateTree(new byte[] { 0 }, 2, 40, Vector2Int.zero);
        //if (Input.GetKeyDown(KeyCode.Alpha5)) blockInteractionType = 0;
        
        if (Input.GetMouseButton(0) && !spellCaster.fireSpells && global.userInput)
        {
            if (clicked != false) return;
            //caster.CastSpell();



            ///*Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);
            //float Angle = Mathf.Atan2(transform.position.y - mousePos.y, transform.position.x - mousePos.x) * Mathf.Rad2Deg;

            //Prosjektil proj = Instantiate(prosjektil, transform.position, Quaternion.Euler(0, 0, Angle));

            //proj.GetComponent<Rigidbody2D>().velocity = -proj.transform.right * proj.Speed;*/
            if (blockInteractionType == 254)
            {
                Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);
                manipulateBlocks.DestroyBlock(mousePos, destructionSize);
            }
            else if (blockInteractionType == C_I.Stone || blockInteractionType == C_I.Grass)
            {
                Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);
                manipulateBlocks.CreateBlocks(mousePos, creationSize, blockInteractionType);
            }
            else if (blockInteractionType == C_I.Water)
            {
                Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);
                manipulateBlocks.CreateBlocks(mousePos, creationSize, blockInteractionType);
            }
            if (singleClick == true) clicked = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            clicked = false;
        }
        //CreateChunkMesh chunkID;
        //blockType = manipulateBlocks.returnMouseOverPoint(global.cam.ScreenToWorldPoint(Input.mousePosition), out chunkID);
        //chunk = chunkID;
    }
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        float adSpeed = rb.velocity.x != Input.GetAxisRaw("Horizontal") * speed && !isGrounded ?
            airModifier * speed : speed;

        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * adSpeed, rb.velocity.y);
        if (Input.GetAxisRaw("Horizontal") != 0) lastDirection = Input.GetAxisRaw("Horizontal");

        isTouchingRoof = Physics2D.OverlapBox(headPos.position, checkSize, 0, Ground);

        if (isTouchingRoof && rb.velocity.y > 0) continiousJump = false;
        if (!isGrounded && rb.velocity.y > 0 && !continiousJump) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 1.5f);
        if (rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 1.2f);
        //if (pushWall) transform.position -= new Vector3(0.005f * lastDirection, 0, 0);
        //transform.position = global.cam..RoundToPixel(transform.position, false);

        if (rb.velocity.x < 0)
        {
            RaycastHit2D ground = Physics2D.Raycast(pixelCheckL.position, Vector3.left, 0.03f, Ground);
            RaycastHit2D tooHigh = Physics2D.Raycast(pixelCheckL.position + new Vector3(0,0.0625f), Vector3.left, 0.03f, Ground);
            if (ground && !tooHigh && isGrounded) transform.position += Vector3.up / 28;
        }
        else if (rb.velocity.x > 0)
        {
            RaycastHit2D ground = Physics2D.Raycast(pixelCheckR.position, Vector3.right, 0.03f, Ground);
            RaycastHit2D tooHigh = Physics2D.Raycast(pixelCheckR.position + new Vector3(0, 0.0625f), Vector3.right, 0.03f, Ground);
            if (ground && !tooHigh && isGrounded) transform.position += Vector3.up / 28;
        }
    }
    IEnumerator jumpFrames()
    {
        yield return new WaitForSeconds(jumpingframes);
        isGrounded = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(pixelCheckL.position, new Vector3(-0.03f, 0, 0));
        Gizmos.DrawRay(pixelCheckR.position, new Vector3(0.03f, 0, 0));

        Gizmos.DrawWireCube(groundCheckPos1.position, checkSize);
        Gizmos.DrawWireCube(headPos.position, checkSize);
    }

}

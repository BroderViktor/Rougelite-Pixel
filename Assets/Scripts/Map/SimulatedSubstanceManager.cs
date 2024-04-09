using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedSubstanceManager : MonoBehaviour
{
    public MapGen mapgen;
    public Cell[] waterMap;
    public Cell[] waterMapShortend;
    public CreateMesh meshGen;

    Cell_Info C_I;

    public float UpdateRate;

    public int numMembers = 0;

    public short arrayNum;

    private void Start()
    {
        C_I = mapgen.C_I;
        StartCoroutine("UpdatePos");
    }
    IEnumerator UpdatePos()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(UpdateRate);
            movePos();
        }
    }
    public void movePos()
    {
        short num = 0;

        //Debug.Log(      "Moving------------------------------------------");
        foreach (Cell cell in waterMapShortend)
        {
            //Debug.Log(num + "--------------------------------------------");
            if (cell.sleepMode == false && cell != null) //Kanskje endre til if velocity == 0 så sette den til -1 
            {
                FindNewMove(cell, num);
            }
            num++;
        }
        meshGen.GenerateWaterMesh(waterMapShortend, global.PPU);
    }
    void FindNewMove(Cell cell, short num)
    {
        Vector2Int[] directions = C_I.WaterMoveDir;
        Vector2Int pos = cell.pos;
        int multiplier = Random.Range(0, 2);
        if (multiplier == 0) multiplier = -1;
        
        for (int x = 0; x < directions.Length; x++)
        {
            Vector2Int dir = new Vector2Int(directions[x].x * multiplier, directions[x].y);

            short chunkID;
            Vector2Int newPos;

            int cellValue = ReturnValueOfPoint(pos + dir, cell.chunk.arrayPos, out chunkID, out newPos);
            //int waterNearby = 0;

            /*Debug.Log("KAN BEVEGE SEG==================" + (cellValue == C_I.Air));
            Debug.Log("cellValue" + cellValue);
            Debug.Log("Retning = " + directions[x] + " newPos " + newPos + " chunkID " + chunkID);*/

            if (cellValue == C_I.Air)
            {
                Move(newPos, chunkID, cell, num);

                /*for (int i = 0; i < directions.Length; i++)
                {
                    Vector2Int revDir = new Vector2Int(directions[i].x, directions[i].y * -1);
                    int newCellValue = ReturnValueOfPoint(pos + revDir, cell.chunk.arrayPos, out chunkID, out newPos);
                    if (newCellValue == 2) 
                    {
                        CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();
                        chunk.map[newPos.y * chunk.width + newPos.x].sleepMode = false;
                    }
                }*/
                


                return;
            }

            /*else
            {
                if (cellValue == 2) waterNearby++;
                if (waterNearby == directions.Length - 1)
                {
                    cell.sleepMode = true;
                }
            }*/
        }
        //cell.velocity /= 2;
    }

    int ReturnValueOfPoint(Vector2Int point, int currentChunkID, out short chunkID, out Vector2Int newPos)
    {
        //NOTES: Returnerer 255 hvis det er utenfor en chunk

        int x = point.x;
        int y = point.y;

        if (x > global.chunkWidth - 1 || x < 0 || y > global.chunkHeight - 1 || y < 0)
        {

            Vector2Int chunkDir = new Vector2Int(0, 0);

            if (x > global.chunkWidth - 1) { x = 0;  chunkDir += new Vector2Int(1,  0); }
            else if (x < 0)         { x = (int)global.chunkWidth - 1; chunkDir += new Vector2Int(-1, 0); }
            if (y > global.chunkHeight - 1)     { y = 0;  chunkDir += new Vector2Int(0,  1); }
            else if (y < 0) { y = (int)global.chunkHeight - 1; chunkDir += new Vector2Int(0, -1); }
            
            newPos = new Vector2Int(x, y);

            chunkID = (short)(currentChunkID + chunkDir.y * (mapgen.loadSize.x * 2 + 1) + chunkDir.x);
            if (chunkID < 0 || chunkID > (mapgen.loadSize.x * 2 + 1) * (mapgen.loadSize.y * 2 + 1)) { /*Debug.LogError("Utenfor kart");*/ return 255; }

            CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();
            
            return chunk.map[y * chunk.width + x].type;
        }
        else
        {
            chunkID = (short)currentChunkID;
            CreateChunkMesh chunk = mapgen.activeChunks[chunkID].GetComponent<CreateChunkMesh>();

            newPos = new Vector2Int(x, y);

            return chunk.map[y * chunk.width + x].type;
        }
    }
    void Move(Vector2Int pos, short newChunkID, Cell cell, short num)
    {

        //Fjerner vannet sin gamle posisjon
        CreateChunkMesh chunk = mapgen.activeChunks[cell.chunk.arrayPos].GetComponent<CreateChunkMesh>(); //Chunken vann cellen er i

        Vector2Int oldPos = cell.pos; //Lager en referanse til vannets gamle posisjon

        cell.pos = pos; //Flytter posisjonen til vannet

        chunk.map[oldPos.y * chunk.width + oldPos.x] = new Cell(oldPos, chunk, C_I.Air); //Forige celle blir gjort om til luft

        chunk = mapgen.activeChunks[newChunkID].GetComponent<CreateChunkMesh>(); //Finner chunken vi er i

        chunk.map[pos.y * chunk.width + pos.x] = cell;

        cell.chunk = chunk;

        waterMapShortend[num] = cell;
        waterMap[num] = cell;
        //Fjerner vannet sin gamle posisjon
        /*CreateChunkMesh chunk = mapgen.activeChunks[cell.chunk.arrayPos].GetComponent<CreateChunkMesh>(); //Chunken vann cellen er i


        chunk.map[cell.pos.y * chunk.width + cell.pos.x].type = C_I.Air; //Forige vann celle blir tom for vann

        //Writer vannet sin nye posisjon

        chunk = mapgen.activeChunks[newChunkID].GetComponent<CreateChunkMesh>();
        
        Cell newCell = chunk.map[pos.y * chunk.width + pos.x];

        newCell.type = C_I.Water;
        newCell.chunk = chunk;

        waterMapShortend[num] = newCell;
        waterMap[num] = newCell;*/
    }
    public void AddMembers(Cell[] newMembers, int lenght)
    {
        newMembers = shortenWaterArray(newMembers, lenght);

        for (int x = numMembers; x < numMembers + newMembers.Length; x++)
        {
            waterMap[x] = newMembers[x - numMembers];
        }
        numMembers += newMembers.Length;
        waterMapShortend = shortenWaterArray(waterMap, numMembers);
        
        //Debug.Log("Added " + newMembers.Length + " new members to " + gameObject);

        meshGen.GenerateWaterMesh(waterMapShortend, global.PPU);
    }
    Cell[] shortenWaterArray(Cell[] array, int lenght)
    {
        Cell[] result = new Cell[lenght];
        for (int x = 0; x < lenght; x++)
        {
            result[x] = array[x];
        }
        return result;
    } 
}
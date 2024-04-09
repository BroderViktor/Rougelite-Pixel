using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSpellManager : MonoBehaviour
{
    public List<GameObject> activeSpells = new List<GameObject>();
    public List<GameObject> oldSpells = new List<GameObject>();

    public List<CreateChunkMesh> chunksList = new List<CreateChunkMesh>();

    public MapGen mG;
    public Cell_Info C_I;
    ChunkLoader chunkLoader;

    public delegate Cell PathEffect(Vector2Int pos, byte type = 0);

    PathEffect pathEffect;



    void Start()
    {
        mG = FindObjectOfType<MapGen>();
        C_I = FindObjectOfType<Cell_Info>();
        chunkLoader = FindObjectOfType<ChunkLoader>();
        SpellBehaviors.C_I = C_I;
        SpellBehaviors.mG = mG;
        SpellBehaviors.spellManager = this;

        pathEffect = SpellBehaviors.createBlock;
    }

    IEnumerator f()
    {
        yield return new WaitForSeconds(3f);

        System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
        t.Start();
        for (int x = 0; x < 30000; x++)
        {

        }
        t.Stop();
        Debug.Log("TIME: " + t.ElapsedMilliseconds);
    }
    private void Update()
    {
        foreach (GameObject spell in activeSpells)
        {
            if (Time.time > spell.GetComponent<testProjScript>().lifetimeRemainder)
            {
                oldSpells.Add(spell);
            }
        }
        foreach (CreateChunkMesh chunk in chunksList)
        {
            chunkLoader.GetComponent<ChunkLoader>().GenerateChunk(chunk, false);
        }
        foreach (GameObject spell in oldSpells)
        {
            activeSpells.Remove(spell);
            Destroy(spell);
        }
        chunksList.Clear();
        oldSpells.Clear();
    }
    public void FillPolygon(List<Cell>[] polygonLines, List<Vector2> points)
    {
        int largestY = 0;
        int smallestY = 0;

        bool setMin = false;
        foreach (Vector2 point in points)
        {
            Vector2Int pointInt = global.ConvertWorldToMapPos(point);

            if (!setMin) 
            {
                smallestY = pointInt.y;
                setMin = true;
            }

            largestY = pointInt.y > largestY ? pointInt.y : largestY;
            smallestY = pointInt.y < smallestY ? pointInt.y : smallestY;
        }
 
        int difference = largestY - smallestY + 1;

        List<Cell>[] heightScan = new List<Cell>[difference];
        for (int i = 0; i < difference; i++) heightScan[i] = new List<Cell>();
        
        foreach (List<Cell> line in polygonLines)
        {
            foreach (Cell cell in line)
            {
                int chunkY = cell.chunk.arrayPos / global.chunkLoadedY;
                int chunkX = cell.chunk.arrayPos - chunkY * global.chunkLoadedY;

                int cellposY = (cell.pos.y + (int)(chunkY * global.chunkHeight));
                int cellposX = (cell.pos.x + (int)(chunkX * global.chunkWidth));

                if (heightScan[largestY - cellposY].Count > 0)
                {
                    int placeToAdd = -1;
                    int counter = 0;
                    bool locked = false;
                    foreach (Cell cellAtHeight in heightScan[largestY - cellposY])
                    {
                        int chunkY2 = cellAtHeight.chunk.arrayPos / global.chunkLoadedY;
                        int chunkX2 = cellAtHeight.chunk.arrayPos - chunkY2 * global.chunkLoadedY;

                        int cell2posX = (cellAtHeight.pos.x + (int)(chunkX2 * global.chunkWidth));

                        if (cellposX < cell2posX && !locked)
                        {
                            placeToAdd = counter;
                            locked = true;
                        }
                        counter++;
                    }
                    if (placeToAdd == -1) heightScan[largestY - cellposY].Add(cell);
                    else heightScan[largestY - cellposY].Insert(placeToAdd, cell);
                }
                else heightScan[largestY - cellposY].Add(cell);
            }
        }
        int de = 0;
        foreach (List<Cell> heightLevel in heightScan)
        {
            if (heightLevel.Count > 2)
            {
                for (int x = 0; x < heightLevel.Count - 1; x++)
                {
                    Cell cell1 = heightLevel[x];

                    int chunkY1 = cell1.chunk.arrayPos / global.chunkLoadedY;
                    int chunkX1 = cell1.chunk.arrayPos - chunkY1 * global.chunkLoadedY;
                    int cellposX1 = (cell1.pos.x + (int)(chunkX1 * global.chunkWidth));
                    int cellposY1 = (cell1.pos.y + (int)(chunkY1 * global.chunkHeight));

                    List<Cell> cellsToRemove = new List<Cell>();
                    for (int y = 1; y < heightLevel.Count - x - 1; y++) //1 fordi det er en mer en den før, mark må være to siden den gå opp
                    {
                        Cell cellx = heightLevel[x + y];

                        int chunkYx = cellx.chunk.arrayPos / global.chunkLoadedY;
                        int chunkXx = cellx.chunk.arrayPos - chunkYx * global.chunkLoadedY;

                        int cellposXx = (cellx.pos.x + (int)(chunkXx * global.chunkWidth));

                        if (cellposX1 + y == cellposXx)
                        {
                            cellsToRemove.Add(heightLevel[x + y]);
                        }
                    }
                    foreach (Cell cellToRemove in cellsToRemove)
                    {
                        heightLevel.Remove(cellToRemove);
                    }
                }
            }
            de++;
        }
        foreach (List<Cell> heightLevel in heightScan)
        {
            if (heightLevel.Count > 0)
            {
                //if (heightLevel.Count % 2 != 0)
                //{
                //    for (int p = 0; p < heightLevel.Count; p++)
                //    {
                //        Cell cell1 = heightLevel[p];

                //        int chunkY1 = cell1.chunk.arrayPos / global.chunkLoadedY;
                //        int chunkX1 = cell1.chunk.arrayPos - chunkY1 * global.chunkLoadedY;
                //        int cellposY1 = (cell1.pos.y + (int)(chunkY1 * global.chunkHeight));
                //        int cellposX1 = (cell1.pos.x + (int)(chunkX1 * global.chunkWidth));

                //        Debug.Log("p = " + p + " Pos =  " + new Vector2Int(cellposX1, cellposY1));
                //    }
                //}
                for (int x = 0; x < heightLevel.Count - 1; x += 2)
                {

                    Cell cell1 = heightLevel[x];

                    int chunkY1 = cell1.chunk.arrayPos / global.chunkLoadedY;
                    int chunkX1 = cell1.chunk.arrayPos - chunkY1 * global.chunkLoadedY;
                    int cellposY1 = (cell1.pos.y + (int)(chunkY1 * global.chunkHeight));
                    int cellposX1 = (cell1.pos.x + (int)(chunkX1 * global.chunkWidth));

                    Vector2Int cell1Pos = new Vector2Int(cellposX1, cellposY1);


                    Cell cell2 = heightLevel[x + 1];

                    int chunkY2 = cell2.chunk.arrayPos / global.chunkLoadedY;
                    int chunkX2 = cell2.chunk.arrayPos - chunkY2 * global.chunkLoadedY;
                    int cellposY2 = (cell2.pos.y + (int)(chunkY2 * global.chunkHeight));
                    int cellposX2 = (cell2.pos.x + (int)(chunkX2 * global.chunkWidth));

                    Vector2Int cell2Pos = new Vector2Int(cellposX2, cellposY2);

                    PathTransform(Vector2.zero, Vector2.zero, cell1Pos, cell2Pos);
                }
            }
        }
    }
    public List<Cell> PathTransform(Vector2 prevPos, Vector2 pos, Vector2Int intprevPos = new Vector2Int(), Vector2Int intPos = new Vector2Int())
    {
        Vector2Int prevPosInt;
        Vector2Int posInt;

        if (intprevPos != Vector2Int.zero && intPos != Vector2Int.zero)
        {
            prevPosInt = intprevPos;
            posInt = intPos;
        }
        else
        {
            prevPosInt = global.ConvertWorldToMapPos(prevPos); 
            posInt = global.ConvertWorldToMapPos(pos);
        }

        Vector2Int posChange = prevPosInt - posInt;

        int xDir = posChange.x > 0 ? 1 : -1;
        int yDir = posChange.y > 0 ? 1 : -1;

        posChange = new Vector2Int(Mathf.Abs(posChange.x), Mathf.Abs(posChange.y));

        int large;
        int small;
        Vector2Int largeMulti;
        Vector2Int smallMulti;

        if (posChange.y > posChange.x)
        {
            large = posChange.y;
            small = posChange.x;
            largeMulti = new Vector2Int(0, 1);
            smallMulti = new Vector2Int(1, 0);
        }
        else
        {
            large = posChange.x;
            small = posChange.y; 
            largeMulti = new Vector2Int(1, 0);
            smallMulti = new Vector2Int(0, 1);
        }

        float climbFactor = (float)small / (float)large;
        //Debug.Log("small" + small + " Lrge: " + large);
        //Debug.Log("CLIMB: -------------: " + climbFactor);

        float smallTracker = 0;

        List<Cell> CellsInLine = new List<Cell>();

        for (int largeTracker = 0; largeTracker < large; largeTracker++)
        {
            Vector2Int largeValue = new Vector2Int(largeTracker, largeTracker) * largeMulti;
            Vector2Int smallValue = new Vector2Int(Mathf.RoundToInt(smallTracker), Mathf.RoundToInt(smallTracker)) * smallMulti;
            Vector2Int combinedValue = smallValue + largeValue;
            Vector2Int point = new Vector2Int(combinedValue.x * -xDir, combinedValue.y * -yDir);

            //Debug.Log("newpos: " + (prevPosInt + point) + "    point: " + point);
            
            CellsInLine.AddRange(BlockTransform(prevPosInt + point, 0));
            
            smallTracker += climbFactor;
        }
        return CellsInLine;
    }

    List<Cell> BlockTransform(Vector2Int pos, int size)
    {
        List<Cell> affectedCells = new List<Cell>();
        int radSqured = (int)(size / 2) * (int)(size / 2) + (int)(size / 2) + 1;

        for (float y = -size / 2; y < size / 2 + 1; y++)
        {
            for (float x = -size / 2; x < size / 2 + 1; x++)
            {
                float radius = (x * x + y * y);
                if (radius < radSqured && radius > -radSqured)
                {
                    //Debug.Log(pos + new Vector2Int((int)x, (int)y));
                    //EN ACTION SKAL SPILLES AV HER
                    affectedCells.Add(pathEffect(pos + new Vector2Int((int)x, (int)y), 1));
                }
            }
        }
        return affectedCells;
    }
}
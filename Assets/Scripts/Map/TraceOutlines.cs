using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceOutlines : MonoBehaviour
{
    public Cell_Info C_I;

    public List<Vector2[]> TraceMapOutline(Cell[] map)
    {
        byte[] points = GenerateNodeMap(map); //Definerer kartet og gir alle punktene verdier basert på hvor mange veggblokker som er nær
        byte[] bitmask = GenerateBitNodeMap(map);

        List<Vector2[]> outlinesList = new List<Vector2[]>(); //En liste av omrisser figuren
                                                             //Vi trenger fler fordi noen figurer har flere deler som ikke henger sammen

        bool[] visitedNodes = new bool[points.Length]; //Liste over alle punktene skal brukes til å sjekke om vi har vært på det punktet

        int nodeNum = 0;

        foreach (bool visitedNode in visitedNodes) //Går gjennom den listen og finner en blokk vi kan starte fra
        {
            if (visitedNode == false)
            {
                if (points[nodeNum] == 1 || points[nodeNum] == 3)
                {
                    outlinesList.Add(generateOutline(visitedNodes, nodeNum, global.PPU, points, bitmask)); //Lagger omrissen og legger det i lista
                }
            }
            nodeNum++;
        }
        return outlinesList;
    }
    Vector2[] generateOutline(bool[] visitedNodes, int nodeNum, int PPU, byte[] points, byte[] bitmask)
    {
        Vector2[] corners = new Vector2[(global.chunkHeight + 1) * (global.chunkWidth + 1)]; //Antall mulige hjørner

        int y = Mathf.FloorToInt(nodeNum / (global.chunkWidth + 1));
        Vector2Int currentNodePos = new Vector2Int(nodeNum - y * (global.chunkWidth + 1), y); //Setter posisjonen vi skal sjekke fra
        Vector2Int prevNodePos = Vector2Int.zero;

        Vector2Int startNodePos = new Vector2Int(10000000, 10000000); //Setter start posisjonen til et veldig høyt tall siden vi skal endre det senere

        int x = 0;
        int bx = 0;

        bool stopUpdate = false;
        bool cornerEnd = false;

        //Debug.Log("RUN START--------------------");
        while (!cornerEnd && bx < 500) //Mens det ikke er funnet det siste hjørnet
        {
            
            byte currentPoint = points[currentNodePos.y * (global.chunkWidth + 1) + currentNodePos.x]; //Punktet vi ser på
            byte currentBitPoint = bitmask[currentNodePos.y * (global.chunkWidth + 1) + currentNodePos.x]; //Punktet vi ser på
            /*Debug.Log("RUN@@@@@@@@@@@@@@@@@@@@@@@@@@: " + bx);
            Debug.Log("pos: " + (currentNodePos));*/
            if (currentPoint == 1 || currentPoint == 3 || currentBitPoint == 5 || currentBitPoint == 10) //Hvis det er et hjørne
            {
                corners[x] = ((Vector2)currentNodePos) / PPU; //Legg til i hjørne lista
                x++;
                
                if (startNodePos == currentNodePos) cornerEnd = true;//Sjekker om vi er på det siste punktet

                if (!stopUpdate) //Definerer det siste punktet
                {
                    startNodePos = currentNodePos;
                    stopUpdate = true;
                }
                else if (currentPoint == 1 || currentPoint == 3)
                {
                    
                    visitedNodes[currentNodePos.y * (global.chunkWidth + 1) + currentNodePos.x] = true;
                    //Definerer denne posisjonen som sjekket
                    //Gjør at den helst ikke vil dra hit igjen med mindre den må
                }

                

                Vector2Int posChange = CheckNodesAround //Sjekker blokker rundt
                    (currentNodePos.x, currentNodePos.y,
                    prevNodePos.x, prevNodePos.y,
                    bitmask, points);

                prevNodePos = currentNodePos;
                currentNodePos += posChange;  //Oppdaterer posisjonen
            }
            else if (currentPoint == 2) //Hvis det er en flat vegg
            {
                visitedNodes[currentNodePos.y * (global.chunkWidth + 1) + currentNodePos.x] = true;

                Vector2Int posChange = CheckNodesAround //Sjekker blokker rundt
                    (currentNodePos.x, currentNodePos.y,
                    prevNodePos.x, prevNodePos.y,
                    bitmask, points);
                //Debug.Log("OldPos_2 " + currentNodePos);
                //Debug.Log("PosChange_2 " + posChange);
                
                prevNodePos = currentNodePos;
                currentNodePos += posChange;    //Oppdaterer posisjonen

                cornerEnd = posChange == Vector2Int.zero;

                //Debug.Log("CurrentPos_2 " + currentNodePos);  
            }
            
            bx++;
            
            
            //Debug.Log("currentPoint: " + currentPoint);
        }
        corners = shortenArray(corners, x);

        return corners; //Gir tilbake en array med omrissen til figuren
    }
    byte[] GenerateNodeMap(Cell[] map)
    {
        byte[] nodeMap = new byte[(global.chunkHeight + 1) * (global.chunkWidth + 1)];
        for (int y = 0; y < global.chunkHeight; y++)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                if (C_I.blockArray[map[y * global.chunkWidth + x].type].solid)
                {
                    nodeMap[y * (global.chunkWidth + 1) + x + 0] += 1;
                    nodeMap[y * (global.chunkWidth + 1) + x + 1] += 1;
                    nodeMap[y * (global.chunkWidth + 1) + x + (global.chunkWidth + 1) + 0] += 1;
                    nodeMap[y * (global.chunkWidth + 1) + x + (global.chunkWidth + 1) + 1] += 1;
                }
            }
        }

        return nodeMap;
    }
    byte[] GenerateBitNodeMap(Cell[] map)
    {
        byte[] nodeMap = new byte[(global.chunkHeight + 1) * (global.chunkWidth + 1)];
        for (int y = 0; y < global.chunkHeight; y++)
        {
            for (int x = 0; x < global.chunkWidth; x++)
            {
                if (C_I.blockArray[map[y * global.chunkWidth + x].type].solid)
                {
                    nodeMap[y * (global.chunkWidth + 1) + x                  ] += 2;
                    nodeMap[y * (global.chunkWidth + 1) + x               + 1] += 1;
                    nodeMap[y * (global.chunkWidth + 1) + x + (global.chunkWidth + 1)    ] += 4;
                    nodeMap[y * (global.chunkWidth + 1) + x + (global.chunkWidth + 1) + 1] += 8;
                }
            }
        }
        return nodeMap;
    }
    int FindNodeType(int x, int y, int xChange, int yChange, byte[] points)
    {
        int pointer = (y + yChange) * (global.chunkWidth + 1) + x + xChange;
        if (pointer >= 0 && pointer <= points.Length - 1)
        {
            return points[pointer];
        }
        return 0;
    }
    Vector2Int[] FindPossibleMoveDirections(int x, int y, int prevX, int prevY, byte[] bitmask)
    {
        int pointer = y * (global.chunkWidth + 1) + x;

        BitArray bitArray = new BitArray(new byte[] { bitmask[pointer] }); //Gir en array med bitsa som lager dette tallet
        bool[] directions = new bool[4];
        Vector2Int[] dir = new Vector2Int[4];

        if (bitmask[pointer] == 5 || bitmask[pointer] == 10)
        {
            int prevPointer = (prevY) * (global.chunkWidth + 1) + (prevX);
            
            byte prevBitmask = getBitmaskCornerValue(bitmask[prevPointer], new Vector2Int(x - prevX, y - prevY));

            byte bitmapAdjusted = (byte)(bitmask[pointer] & prevBitmask);

            bitArray = new BitArray(new byte[] { bitmapAdjusted });

        }

        for (byte i = 0; i < 4; i++)
        {
            if (bitArray[i])
            {
                directions[i] = directions[i] ? false : true;
                if (i == 0) directions[3] = directions[3] ? false : true;
                else directions[i - 1] = directions[i - 1] ? false : true;
            }
        }

        dir[0] = directions[0] ? new Vector2Int(0, 1) : Vector2Int.zero;
        dir[1] = directions[1] ? new Vector2Int(1, 0) : Vector2Int.zero;
        dir[2] = directions[2] ? new Vector2Int(0, -1) : Vector2Int.zero;
        dir[3] = directions[3] ? new Vector2Int(-1, 0) : Vector2Int.zero;
        
        return dir;
    }
    byte getBitmaskCornerValue(byte bitmaskCorner, Vector2Int dir)
    {
        if (bitmaskCorner == 1)
        {
            if (dir.x == -1) return 2;
            else if (dir.y == 1) return 8;
        }
        else if (bitmaskCorner == 2)
        {
            if (dir.x == 1) return 1;
            else if (dir.y == 1) return 4;
        }
        else if (bitmaskCorner == 4)
        {
            if (dir.x == 1) return 8;
            else if (dir.y == -1) return 2;
        }
        else if (bitmaskCorner == 8)
        {
            if (dir.x == -1) return 4;
            else if (dir.y == -1) return 1;
        }
        else if (bitmaskCorner == 5)
        {
            if (dir.x < 0 || dir.y < 0) return 2;
            else if(dir.x > 0 || dir.y > 0) return 8;
        }
        else if (bitmaskCorner == 10)
        {
            if (dir.x > 0 || dir.y < 0) return 1;
            else if(dir.x < 0 || dir.y > 0) return 4;
        }
        return bitmaskCorner;
    }
    Vector2Int CheckNodesAround(int x, int y, int prevX, int prevY, byte[] bitmask, byte[] points)
    {
        int checkNode;

        bool[] dirLock = new bool[] { y == global.chunkHeight, x == global.chunkWidth, y == 0, x == 0 };

        Vector2Int[] possibleMoveDir = FindPossibleMoveDirections(x, y, prevX, prevY, bitmask);
 
        for (int i = 0; i < possibleMoveDir.Length; i++)
        {
            Vector2Int dir = possibleMoveDir[i];

            if (dir != Vector2Int.zero && !dirLock[i])
            {
                checkNode = FindNodeType(x, y, dir.x, dir.y, points);
                if (checkNode == 1 || checkNode == 2 || checkNode == 3)
                {
                    if (x + dir.x != prevX || y + dir.y != prevY) 
                        return dir;
                }
            }
        }
        for (int i = 0; i < possibleMoveDir.Length; i++)
        {
            Vector2Int dir = possibleMoveDir[i];

            if (dir != Vector2Int.zero && !dirLock[i])
            {
                checkNode = FindNodeType(x, y, dir.x, dir.y, points);

                if (checkNode == 1 || checkNode == 2 || checkNode == 3)
                {
                    return dir;
                }
            }
        }
        
        
        Debug.LogError("Finner ingen ny posisjon");
        return Vector2Int.zero;
    }
    Vector2[] shortenArray(Vector2[] array, int size)
    {
        Vector2[] result = new Vector2[size];
        for (int n = 0; n < size; n++)
        {
            result[n] = array[n];
        }
        return result;
    }
}

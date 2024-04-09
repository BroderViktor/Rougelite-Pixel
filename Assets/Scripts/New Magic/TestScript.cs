using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public ActiveSpellManager spellManager;

    List<Vector2> trianglePoints = new List<Vector2>();
    int trianglePointsNum;
    public float pointSpaceCooldown = 0.1f;
    float cooldown;

    void Update()
    {
        spikeCreator();
    }
    void spikeCreator()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);

            trianglePoints.Add(mousePos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);

            Vector2 basePos = trianglePoints[0];

            Vector2 vectorAngle = basePos - mousePos;

            float distance = Vector2.Distance(basePos, mousePos);

            Vector2 normal = new Vector2(vectorAngle.y / (6), vectorAngle.x / (6));

            trianglePoints[0] = new Vector2(normal.x * -1, normal.y) + basePos;
            trianglePoints.Add(new Vector2(normal.x, normal.y * -1) + basePos);
            trianglePoints.Add(mousePos);

            trianglePointsNum = 3;

            CreatePolygon();
        }
    }
    void CreatePolygonPoints()
    {
        if (Input.GetMouseButton(0))
        {
            if (cooldown > Time.time) return;
            cooldown = Time.time + pointSpaceCooldown;

            Vector2 mousePos = global.cam.ScreenToWorldPoint(Input.mousePosition);

            trianglePoints.Add(mousePos);

            trianglePointsNum++;

        }
        if (Input.GetMouseButtonDown(1) && trianglePointsNum > 1)
        {
            CreatePolygon();
        }
    }
    void CreatePolygon()
    {
        List<Cell>[] polygonLines = new List<Cell>[trianglePointsNum];

        for (int x = 0; x < trianglePointsNum; x++)
        {
            int x2 = x == trianglePointsNum - 1 ? 0 : x + 1;
            polygonLines[x] = spellManager.PathTransform(trianglePoints[x], trianglePoints[x2]);
        }

        spellManager.FillPolygon(polygonLines, trianglePoints);

        trianglePointsNum = 0;
        trianglePoints.Clear();
    }
}

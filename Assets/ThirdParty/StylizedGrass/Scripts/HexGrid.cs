using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexGrid
{
    public Vector3 OffsetPosition;
    public float OuterRadius;
    public float InnerRadius;
    public Vector3[] Corners;

    public HexGrid(float outerRadius,Vector3 gridPosition)
    {
        OffsetPosition = gridPosition;
        OuterRadius = outerRadius;
        InnerRadius = outerRadius * 0.866025404f;

        Corners = new Vector3[] {
                    new Vector3(0f, 0f, OuterRadius),
                    new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
                    new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
                    new Vector3(0f, 0f, -OuterRadius),
                    new Vector3(-InnerRadius, 0f, -0.5f * outerRadius),
                    new Vector3(-InnerRadius, 0f, 0.5f * outerRadius) };
    }

    public Vector3 HexToWorld(Vector2Int hex)
    {
        Vector3 position;
        position.x = (hex.x + hex.y * 0.5f - hex.y / 2) * (InnerRadius * 2f);
        position.y = 0f;
        position.z = hex.y * (OuterRadius * 1.5f);
        position += OffsetPosition;

        return position;
    }

    public Vector2Int WorldToHex(Vector3 position)
    {

        position -= OffsetPosition;
        position /= OuterRadius * Mathf.Sqrt(3);

        var temp = Mathf.Floor(position.x + Mathf.Sqrt(3) * position.z + 1);
        var q = Mathf.FloorToInt((Mathf.Floor(2 * position.x + 1) + temp) / 3);
        var r = Mathf.FloorToInt((temp + Mathf.Floor(-position.x + Mathf.Sqrt(3) * position.z + 1)) / 3);

        q -= Mathf.FloorToInt(r/2);
        if (r % 2 != 0)
            q -= 1;

        return new Vector2Int(q,r);
    }

    public void DrawHex(Vector3 position)
    {
        for (int i = 1; i < 6; i++)
        {
            Gizmos.DrawLine(Corners[i - 1] + position, Corners[i] + position);
        }
        Gizmos.DrawLine(Corners[0] + position, Corners[5] + position);
    }
}
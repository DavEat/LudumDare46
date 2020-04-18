using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line {

    const float verticalLineGradiant = 1e5f;

    float gradiant;
    float y_intercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    float gradiantPerpendicular;

    private bool approcheSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0)
            gradiantPerpendicular = verticalLineGradiant;
        else gradiantPerpendicular = dy / dx;

        if (gradiantPerpendicular == 0)
            gradiant = verticalLineGradiant;
        else gradiant = -1 / gradiantPerpendicular;

        y_intercept = pointOnLine.y - gradiant * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradiant);

        approcheSide = false;
        approcheSide = GetSide(pointPerpendicularToLine);
    }

    private bool GetSide(Vector2 p)
    {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approcheSide;
    }

    public float DistanceFromPoint(Vector2 p)
    {
        float yInterrceptPerpendicular = p.y - gradiantPerpendicular * p.x;
        float intersectX = (yInterrceptPerpendicular - y_intercept) / (gradiant - gradiantPerpendicular);
        float intersectY = gradiant * intersectX + y_intercept;

        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 lineDir = new Vector3(1, 0, gradiant).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine(lineCenter - lineDir * length / 2f, lineCenter + lineDir * length / 2);
    }
}

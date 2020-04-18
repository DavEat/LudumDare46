using System;
using UnityEngine;

public static class Utilities
{
    /// <summary>Make a rotation a value troward the center in 0,0 with the given angle</summary>
    /// <param name="value">vector to be rotate</param>
    /// <param name="angle">angle for the rotation must be 0/90/180/270/360</param>
    /// <returns>return the rotation of the value</returns>
    public static Vector2 Rotate(Vector2 value, float angle)
    {
        return Rotate(value, angle, Vector2Int.zero);
    }
    /// <summary>Make a rotation a value troward the center with the given angle</summary>
    /// <param name="value">vector to be rotate</param>
    /// <param name="angle">angle for the rotation must be 0/90/180/270/360</param>
    /// <param name="center">center of the rotation</param>
    /// <returns>return the rotation of the value</returns>
    public static Vector2 Rotate(Vector2 value, float angle, Vector2 center)
    {
        /*double r = (Math.PI / 180) * angle;

        double c = Math.Cos(r);
        double s = Math.Sin(r);

        int x = (int)(c * (value.x - center.x) - s * (value.y - center.y) + center.x);
        int y = (int)(s * (value.x - center.x) + c * (value.y - center.y) + center.y);

        return new Vector2Int(x, y);*/

        while (angle < 0)
            angle += 360;
        while (angle > 360)
            angle -= 360;

        float x = value.x;
        float y = value.y;
        float t;

        switch (angle)
        {
            case 90:
                t = y;
                y = -x;
                x = t;
                break;
            case 180:
                x = -x;
                y = -y;
                break;
            case 270:
                t = x;
                x = -y;
                y = t;
                break;
        }

        return new Vector2(x, y);
    }
    public static Vector2Int Rotate(Vector2Int value, float angle)
    {
        /*double r = (Math.PI / 180) * angle;

        double c = Math.Cos(r);
        double s = Math.Sin(r);

        int x = (int)(c * (value.x - center.x) - s * (value.y - center.y) + center.x);
        int y = (int)(s * (value.x - center.x) + c * (value.y - center.y) + center.y);

        return new Vector2Int(x, y);*/

        while (angle < 0)
            angle += 360;
        while (angle > 360)
            angle -= 360;

        int x = value.x;
        int y = value.y;
        int t;

        switch (angle)
        {
            case 90:
                t = y;
                y = -x;
                x = t;
                break;
            case 180:
                x = -x;
                y = -y;
                break;
            case 270:
                t = x;
                x = -y;
                y = t;
                break;
        }

        return new Vector2Int(x, y);
    }
}
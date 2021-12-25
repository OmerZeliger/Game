using System;
using UnityEngine;

public class Utils
{
    public static Direction intToHorzDir(int dir)
    {
        switch (dir)
        {
            case -1 : return Direction.LEFT;
            case 0 : return Direction.NONE;
            case 1 : return Direction.RIGHT;
        }

        throw new ArgumentException("Bad input lol");
    }

    public static Direction intToVertDir(int dir)
    {
        switch (dir)
        {
            case -1 : return Direction.DOWN;
            case 0 : return Direction.NONE;
            case 1 : return Direction.UP;
        }

        throw new ArgumentException("Bad input lol");
    }

    public static Vector2 dirToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.LEFT : return new Vector2(-1, 0);
            case Direction.RIGHT: return new Vector2(1, 0);
            case Direction.DOWN: return new Vector2(0, -1);
            case Direction.UP: return new Vector2(0, 1);
        }

        return new Vector2(0, 0);
    }

    public static Vector2 subtract(Vector2 first, Vector2 second)
    {
        return new Vector2(first.x - second.x, first.y - second.y);
    }
}

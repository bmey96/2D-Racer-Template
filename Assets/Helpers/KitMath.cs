using System.Collections.Generic;
using UnityEngine;

// <summary>
/// Math and geometry utility functions
/// </summary>
public static class KitMath
{
    /// <summary>
    /// Wraps a value within a specified range (exclusive upper bound)
    /// </summary>
    public static float Wrap(float value, float lowerRange, float upperRange)
    {
        if (value >= lowerRange && value < upperRange)
            return value;

        float rangeSize = upperRange - lowerRange;

        if (value >= upperRange)
        {
            float excess = value - lowerRange;
            float wrapped = excess % rangeSize;
            return lowerRange + wrapped;
        }
        else
        {
            float deficit = lowerRange - value;
            float wrapped = deficit % rangeSize;
            return upperRange - wrapped;
        }
    }

    /// <summary>
    /// Wraps a value within a specified range (inclusive upper bound)
    /// </summary>
    public static float WrapInclusive(float value, float lowerRange, float upperRange)
    {
        if (value >= lowerRange && value <= upperRange)
            return value;

        float rangeSize = upperRange - lowerRange + 1;

        if (value > upperRange)
        {
            float excess = value - lowerRange;
            float wrapped = excess % rangeSize;
            return lowerRange + wrapped;
        }
        else
        {
            float deficit = lowerRange - value;
            float wrapped = deficit % rangeSize;
            return upperRange - wrapped + 1;
        }
    }

    /// <summary>
    /// Converts a 2D direction vector to a rotation quaternion
    /// </summary>
    public static Quaternion DirectionToRotation2D(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Gets the shortest distance from a point to a line segment
    /// </summary>
    public static float GetShortestDistanceToLine(Vector2 pointToMeasure, Vector2 linePoint1, Vector2 linePoint2)
    {
        Vector2 lineDirection = linePoint2 - linePoint1;
        Vector2 pointVector = pointToMeasure - linePoint1;

        float crossProduct = Mathf.Abs(lineDirection.x * pointVector.y - lineDirection.y * pointVector.x);
        float lineLength = lineDirection.magnitude;

        return crossProduct / lineLength;
    }

    /// <summary>
    /// Gets turn direction using cross product (positive = left, negative = right)
    /// </summary>
    public static float GetTurnDirection(Vector2 currentDirection, Vector2 targetDirection)
    {
        return currentDirection.x * targetDirection.y - currentDirection.y * targetDirection.x;
    }
}
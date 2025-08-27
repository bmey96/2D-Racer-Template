using UnityEngine;

// <summary>
/// Unity-specific movement and transform utilities
/// </summary>
public static class KitMovement
{
    /// <summary>
    /// Smoothly moves a transform towards a target position
    /// </summary>
    public static void SmoothFollow(Transform follower, Transform target, float followSpeed, bool maintainZ = true)
    {
        Vector3 targetPos = target.position;
        if (maintainZ)
            targetPos.z = follower.position.z;

        follower.position = Vector3.Lerp(follower.position, targetPos, Time.deltaTime * followSpeed);
    }

    /// <summary>
    /// Smoothly moves a transform towards a target position (2D version)
    /// </summary>
    public static void SmoothFollow2D(Transform follower, Vector2 targetPosition, float followSpeed)
    {
        Vector2 currentPos = follower.position;
        Vector2 newPos = Vector2.Lerp(currentPos, targetPosition, Time.deltaTime * followSpeed);
        follower.position = new Vector3(newPos.x, newPos.y, follower.position.z);
    }

}
using UnityEngine;

public interface IRacer
{
    public float maxSpeed { get; }
    public float maxSpeedTime {  get; }
    public float steeringSpeed { get; }
    public Transform transf {  get; }

    public void Push(Vector2 unnormalizedDirection)
    {

    }
}

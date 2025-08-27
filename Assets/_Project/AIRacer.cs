using Unity.VisualScripting;
using UnityEngine;

public class AIRacer : MonoBehaviour, IRacer
{
    [SerializeField] private LayerMask _generalCollisionDetectionLayer;
    [SerializeField] private float _collisionRadius = .5f;
    [SerializeField] private RacerPath _racerPath;
    [SerializeField] private float _maxSpeedTime = 2;
    [SerializeField] private float _currentSpeed = 0;
    [SerializeField] private float _maxSpeedTimer = 0;
    [SerializeField] private int _currentPointIndex = 0;
    [SerializeField] private float _turnAngle = 0;
    [SerializeField] private float _similarity = 0;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turningSpeedMultiplier = .3f;
    [SerializeField] private float _steeringSpeed = 25;
    [SerializeField] private Vector2 _destinationDeviation;

    private bool _isAdjustingTurn = false;

    public float maxSpeed => _speed;

    public float maxSpeedTime => _maxSpeedTime;

    public float steeringSpeed => _steeringSpeed;

    public Transform transf => transform;

    private void HandleMovement()
    {
        Vector3 direction = ((_racerPath.points[_currentPointIndex].position + (Vector3)_destinationDeviation) - transform.position).normalized;
        Quaternion directionRotation = DirectionToRotation2D(direction);
        float angle = directionRotation.eulerAngles.z;

        float distance = (_racerPath.points[_currentPointIndex].position + (Vector3)_destinationDeviation - transform.up).magnitude;

        //_turnAngle = angle - 90;

        float similarity = Vector2.Dot(transform.up, direction);
        float cross = transform.up.x * direction.y - transform.up.y * direction.x;

        // if similarity is our of within .15 of 1 start adjusting 
        if (similarity < .95f && _isAdjustingTurn == false)
        {
            _isAdjustingTurn = true;
        }

        _maxSpeedTimer += Time.deltaTime;

        if (_maxSpeedTimer <= _maxSpeedTime)
        {
            _currentSpeed = (_maxSpeedTimer / _maxSpeedTime) * _speed;
        }
        else if (_maxSpeedTimer > _maxSpeedTime)
        {
            _currentSpeed = _speed;
        }

        // Adjust til its within .5
        if (_isAdjustingTurn)
        {

            float adjustedSpeed = Mathf.Lerp(0.2f, 1f, similarity) * _currentSpeed * _turningSpeedMultiplier; // Speed ranges from 0.2 to 1.0

            // Slow down while turning for better precision
            transform.position += transform.up * adjustedSpeed * Time.deltaTime; // Reduced speed

            float turnDirection = Mathf.Sign(cross);
            _turnAngle += turnDirection * Time.deltaTime * _steeringSpeed;

            _turnAngle = KitMath.Wrap(_turnAngle, -360, 360);

            if (similarity > .99f)
            {
                _isAdjustingTurn = false;
            }
        }
        else
        {




            // Full speed when aligned
            transform.position += transform.up * _currentSpeed * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(0, 0, _turnAngle);


        // When the car reaches point increment the index
        if (Vector2.Distance(transform.position, _racerPath.points[_currentPointIndex].position + (Vector3)_destinationDeviation) <= .2f)
        {
            _currentPointIndex++;

            if (_currentPointIndex >= _racerPath.points.Count)
            {
                _currentPointIndex = 0;
            }

        }
    }


    private Quaternion DirectionToRotation2D(Vector2 direction)
    {
        // For 2D objects in 3D space (rotating around Z-axis)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private float GetShortestDistanceToLine(Vector2 pointToMeasure, Vector2 linePoint1, Vector2 linePoint2)
    {
        Vector2 lineDirection = linePoint2 - linePoint1;
        Vector2 pointVector = pointToMeasure - linePoint1;

        float crossProduct = Mathf.Abs(lineDirection.x * pointVector.y - lineDirection.y * pointVector.x);
        float lineLength = lineDirection.magnitude;

        return crossProduct / lineLength;
    }

    private void HandleCheckPointClearing()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _collisionRadius, _generalCollisionDetectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            Checkpoint checkPoint = colliders[i].GetComponent<Checkpoint>();
            ReverseDetector reverseDetector = colliders[i].GetComponent<ReverseDetector>();
            if (checkPoint != null)
            {


                checkPoint.Clear(this);
            }
            else if (reverseDetector != null)
            {
                reverseDetector.Trigger(this);
            }
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleCheckPointClearing();
    }
}

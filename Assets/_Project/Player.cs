using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IRacer
{
    [SerializeField] private LayerMask _movementCollisionDetectionLayer;
    [SerializeField] private LayerMask _generalCollisionDetectionLayer;
    [SerializeField] private float _collisionRadius = .5f;
    private float _turnAngle = 0;
    [SerializeField] private float _maxSpeedTime = 2;
    [SerializeField] private float _maxSpeed = 15;
    [SerializeField] private float _currentSpeed = 0;

    [SerializeField] private float _maxSpeedTimer = 0;
    [SerializeField] private float _gas = 0;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _gasSound;

    [SerializeField] private Vector2 _pushForce = Vector2.zero;
    [SerializeField] private Transform _forwardCollisionPoint;
    [SerializeField] private Transform _forwardLeftCollisionPoint;
    [SerializeField] private Transform _forwardRightCollisionPoint;
    [SerializeField] private float _steeringSpeed = 70;

    public float maxSpeed => _maxSpeed;

    public float maxSpeedTime => _maxSpeedTime;

    public float steeringSpeed => _steeringSpeed;
    public Transform transf => transform;

    private void HandleMovement()
    {
        // Steering
        float turnInput = -Input.GetAxisRaw("Horizontal");

        _turnAngle += turnInput * Time.deltaTime * _steeringSpeed;

        transform.rotation = Quaternion.Euler(0, 0, _turnAngle);

        bool gasPressedThisFrame = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S));

        if (Mathf.Abs(_pushForce.sqrMagnitude) > 0)
        {
            _pushForce = Vector2.MoveTowards(_pushForce, Vector2.zero, Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {

            if (_gas != 1)
            {
                _maxSpeedTimer = 0;
            }

            _audioSource.clip = _gasSound;
            _audioSource.Play();


            _gas = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (_gas != -1)
            {
                _maxSpeedTimer = 0;
            }

            _audioSource.clip = _gasSound;
            _audioSource.Play();

            _gas = -1;
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {

            _audioSource.Stop();
        }

        // If gas is pressed
        if (gasPressedThisFrame)
        {


            // Handle acceleration build up towards max speed
            _maxSpeedTimer += Time.deltaTime;

            _maxSpeedTimer = Mathf.Clamp(_maxSpeedTimer, 0, _maxSpeedTime);



        }
        else
        {
            // Handle acceleration build up towards max speed
            _maxSpeedTimer -= Time.deltaTime;

            _maxSpeedTimer = Mathf.Clamp(_maxSpeedTimer, 0, _maxSpeedTime);

        }

        _currentSpeed = _maxSpeed * (_maxSpeedTimer / _maxSpeedTime);

        Vector3 direction = (transform.up * _gas).normalized;

        if (WillCollide(direction))
        {
            _currentSpeed = 0;
        }


        transform.position += (direction * Time.deltaTime * _currentSpeed);

        transform.position += (Vector3)_pushForce.normalized * _pushForce.magnitude * Time.deltaTime;


    }

    public void Push(Vector2 pushDirectionUnNormalized)
    {
        _pushForce = pushDirectionUnNormalized;
    }
    public bool WillCollide(Vector3 direction)
    {

        RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, _collisionRadius, _movementCollisionDetectionLayer);
        RaycastHit2D ray2 = Physics2D.Raycast(_forwardRightCollisionPoint.position, direction, .1f, _movementCollisionDetectionLayer);
        RaycastHit2D ray3 = Physics2D.Raycast(_forwardLeftCollisionPoint.position, direction, .1f, _movementCollisionDetectionLayer);


        if (ray.collider != null || ray2.collider != null || ray3.collider != null)
        {
            return true;
        }
        return false;
    }

    private void HandleCheckPointClearing()
    {
        // Option 1: Using OverlapCircleAll (recommended for this use case)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _collisionRadius, _generalCollisionDetectionLayer);

        // Option 2: Using CircleCastAll (if you specifically need casting)
        // Collider2D[] colliders = Physics2D.CircleCastAll(transform.position, _collisionRadius, Vector2.up, 0f, _raycastLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            Checkpoint checkPoint = colliders[i].GetComponent<Checkpoint>();
            ReverseDetector reverseDetector = colliders[i].GetComponent<ReverseDetector>();
            if (checkPoint != null)
            {


                checkPoint.Clear(this);
            }
            else if(reverseDetector != null)
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
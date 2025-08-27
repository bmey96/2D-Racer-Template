using UnityEngine;

public class Cam : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed;

    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, new Vector3(_target.position.x, _target.position.y, 0), Time.deltaTime * _followSpeed);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}

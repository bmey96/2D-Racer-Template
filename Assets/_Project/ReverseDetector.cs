using UnityEngine;

public class ReverseDetector : MonoBehaviour
{
    [SerializeField] private LapHandler _lapHandler;

    [SerializeField] private bool _detectReverse = true;

    private bool _triggered = false;

    private float _timeSinceTrigger = 0;
    

    public void Trigger(IRacer racer)
    {
        if(_timeSinceTrigger <= 0.05f) return;

        _timeSinceTrigger = 0;

        _lapHandler.SetReverseDetected(_detectReverse, racer);

        _triggered = true;
    }

    private void Update()
    {
        _timeSinceTrigger += Time.deltaTime;


    }
}

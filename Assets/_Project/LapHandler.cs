using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LapHandler : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private List<AIRacer> _bots;
    [SerializeField] private List<IRacer> _finishedRacers = new List<IRacer>();

    [SerializeField] private List<IRacer> _racers = new List<IRacer>();
    [SerializeField] private List<List<Checkpoint>> _checkPointsClearedTable = new List<List<Checkpoint>>();
    Dictionary<IRacer, int> _lapMap = new Dictionary<IRacer, int>();
    Dictionary<IRacer, int> _checkPointMap = new Dictionary<IRacer, int>();
    Dictionary<IRacer, bool> _reverseDetectedMap = new Dictionary<IRacer, bool>();

    [SerializeField] private List<Checkpoint> _checkPoints;
    [SerializeField] private List<Checkpoint> _checkPointsCleared = new List<Checkpoint>();
    [SerializeField] private bool _reverseDetected = false;
    [SerializeField] private int _expectedNextCheckPointIndex = 0;
    [SerializeField] private int _currentLap = 1;
    [SerializeField] private int _totalLaps = 3;

    public int totalRacers => _racers.Count;
    public int currentLap => _currentLap;
    public int totalLaps => _totalLaps;
    private void Start()
    {
        _checkPointsClearedTable = new List<List<Checkpoint>> { };
        _lapMap = new Dictionary<IRacer, int>();
        _racers = new List<IRacer>();
        _checkPointMap = new Dictionary<IRacer, int>();
        _reverseDetectedMap = new Dictionary<IRacer, bool>();

        for (int i = 0; i < _checkPoints.Count; i++)
        {
            _checkPoints[i].Initialize(this);
        }

        _racers.Add(_player);
        _lapMap[_player] = 1;
        _checkPointMap[_player] = 0;
        _reverseDetectedMap[_player] = false;
        _checkPointsClearedTable.Add(new List<Checkpoint>());

        for (int i = 0; i < _bots.Count; i++)
        {
            AIRacer bot = _bots[i];

            _racers.Add(bot);
            _lapMap[bot] = 1;
            _checkPointMap[bot] = 0;
            _reverseDetectedMap[bot] = false;
            _checkPointsClearedTable.Add(new List<Checkpoint>());
        }
    }

    public void SetReverseDetected(bool isReversing, IRacer racer)
    {
        int racerIndex = _racers.IndexOf(racer);

        _reverseDetectedMap[racer] = isReversing;

        _checkPointsClearedTable[racerIndex].Clear();

        _checkPointMap[racer] = 0;

        _checkPointsClearedTable[0] = new List<Checkpoint>();

        for(int i = 0; i < _checkPoints.Count; i++)
        {
            _checkPoints[i].ClearClearer(racer);
        }
    }

    // Gets a random racer that hast yet finished the race
    public IRacer GetRandomRacer()
    {
        List<IRacer> unfinishedRacers = new List<IRacer>();

        for (int i = 0; i < _racers.Count; i++)
        {
            if (!_finishedRacers.Contains(_racers[i]))
            {
                unfinishedRacers.Add(_racers[i]);
            }
        }

        int randomIndex = Random.Range(0, unfinishedRacers.Count - 1);

        return unfinishedRacers[randomIndex];
    }

    public int GetPlace(IRacer racer)
    {
        var racerProgress = new List<(IRacer racer, int lap, int nextCheckpoint, float distanceToNext)>();

        for (int i = 0; i < _racers.Count; i++)
        {
            int currentLap = _lapMap[_racers[i]];
            int nextCheckpointIndex = _checkPointMap[_racers[i]];
            Vector2 position = _racers[i].transf.position;

            float distanceToNextCheckpoint = 0f;

            // Handle valid checkpoint indices
            if (nextCheckpointIndex >= 0 && nextCheckpointIndex < _checkPoints.Count)
            {
                Vector2 nextCheckpointPos = _checkPoints[nextCheckpointIndex].transform.position;
                distanceToNextCheckpoint = Vector2.Distance(position, nextCheckpointPos);
            }
            else
            {
                // If checkpoint index is invalid, use a very large distance (worst position)
                distanceToNextCheckpoint = float.MaxValue;
                Debug.LogWarning($"Invalid checkpoint index {nextCheckpointIndex} for racer {_racers[i]}");
            }

            racerProgress.Add((_racers[i], currentLap, nextCheckpointIndex, distanceToNextCheckpoint));
        }

        // Sort by lap (higher better), then by checkpoint progress (higher better), then by distance to next (lower better)
        racerProgress.Sort((a, b) =>
        {
            if (a.lap != b.lap) return b.lap.CompareTo(a.lap);
            if (a.nextCheckpoint != b.nextCheckpoint) return b.nextCheckpoint.CompareTo(a.nextCheckpoint);
            return a.distanceToNext.CompareTo(b.distanceToNext);
        });

        // Find position of target racer
        for (int i = 0; i < racerProgress.Count; i++)
        {
            if (racerProgress[i].racer == racer)
                return i + 1;
        }

        return -1; // Racer not found
    }
    public int GetLap(IRacer racer)
    {
        return _lapMap[racer];
    }

    public bool ClearCheckPoint(Checkpoint checkpoint, IRacer racer)
    {
        int currentPosition = _checkPointMap[racer];
        int racerIndex = _racers.IndexOf(racer);


        int checkPointIndex = _checkPoints.IndexOf(checkpoint);
        int nextCheckPointIndex = _checkPointMap[racer];

        // Check that this checkpoint is the next one for the player
        if (checkPointIndex != nextCheckPointIndex)
        {
            return false;
        }

        // Check if the player has been flagged as reversing and if the checkpoint is the last
        if (_reverseDetectedMap[racer] && checkPointIndex >= _checkPoints.Count - 1)
        {
            return false;
        }

        if (_checkPointsCleared.Contains(checkpoint))
        {
            return false;
        }

        Debug.Log("Racer Index: " + racerIndex);
        Debug.Log("List size;" + _checkPointsClearedTable.Count);
        Debug.Log("Hit Checkpoint;" + checkPointIndex);
        Debug.Log("Next Checkpoint;" + checkPointIndex);

        _checkPointsClearedTable[racerIndex].Add(checkpoint);

        _checkPointMap[racer]++;

        if (AreListsEqual(_checkPointsClearedTable[racerIndex], _checkPoints))
        {
            OnFinishLap(racer);
        }

        return true;
    }

    public void UnclearCheckPoint(Checkpoint checkPoint)
    {
        _checkPointsCleared.Remove(checkPoint);

        checkPoint.gameObject.SetActive(true);
    }
    public void OnFinishLap(IRacer racer)
    {
        int racerIndex = _racers.IndexOf(racer);

        _lapMap[racer]++;
        _checkPointsClearedTable[racerIndex].Clear();
        _checkPointMap[racer] = 0; // Ensure it's exactly 0

        Debug.Log($"Lap finished - Racer: {racer}, New Lap: {_lapMap[racer]}, Reset Checkpoint: {_checkPointMap[racer]}");

        GameManager.Instance.OnLapFinished(this);

        for (int i = 0; i < _checkPoints.Count; i++)
        {
            _checkPoints[i].ClearClearer(racer);
        }

        if (_lapMap[racer] > _totalLaps)
        {
            _finishedRacers.Add(racer);
        }

        if (racer == _player && _lapMap[racer] > _totalLaps)
        {
            GameManager.Instance.OnGameWin(this);
        }
        else if (_lapMap[racer] > _totalLaps)
        {
            racer.transf.gameObject.SetActive(false);
        }
    }
    public static bool AreListsEqual<T>(List<T> list1, List<T> list2)
    {
        // Handle null cases
        if (list1 == null && list2 == null)
            return true;

        if (list1 == null || list2 == null)
            return false;

        // Check if lengths are different
        if (list1.Count != list2.Count)
            return false;

        // Compare each element using reference equality
        for (int i = 0; i < list1.Count; i++)
        {
            if (!ReferenceEquals(list1[i], list2[i]))
                return false;
        }

        return true;
    }

}

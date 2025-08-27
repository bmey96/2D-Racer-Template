using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private List<IRacer> _clearers = new List<IRacer>();

    private LapHandler _lapHandler;

    public void Initialize(LapHandler lapHandler)
    {
        _lapHandler = lapHandler;
    }

    public void Clear(IRacer racer)
    {
        if (_clearers.Contains(racer)) return;

        _lapHandler.ClearCheckPoint(this, racer);

        _clearers.Add(racer);
    }

    public void ClearClearer(IRacer racer)
    {
        _clearers.Remove(racer);
    }
}

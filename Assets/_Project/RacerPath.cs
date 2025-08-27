using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RacerPath : MonoBehaviour
{
    [SerializeField] private List<Transform> _points;

    public List<Transform> points => _points;


}

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Cam _camera;
    [SerializeField] private AIRacer _bot;
    [SerializeField] private TextMeshProUGUI _enemyLapText;
    [SerializeField] private LapHandler _lapHandler;
    [SerializeField] private TextMeshProUGUI _lapText;
    [SerializeField] private TextMeshProUGUI _positionText;
    [SerializeField] private Player _player;

    [SerializeField] private float _uiUpdateInterval;

    [SerializeField] private GameObject _standardScreen;
    [SerializeField] private GameObject _winScreen;

    private float _uiUpdateTimer = 0;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject gameManagerObject = new GameObject("GameManager");
                    _instance = gameManagerObject.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy this one
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set this as the instance
        _instance = this;

        // Optionally persist across scenes
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // Clear the instance reference when this object is destroyed
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void HandleUIUpdates()
    {
        _uiUpdateTimer += Time.deltaTime;

        if(_uiUpdateTimer >+_uiUpdateInterval)
        {
            _uiUpdateTimer = 0;

            _positionText.text = "Position " + _lapHandler.GetPlace(_player) + "st";

            _enemyLapText.text = "Enemy Lap" + _lapHandler.GetLap(_bot);
        }

    }

    public void OnLapFinished(LapHandler lapHandler)
    {
        int playerLap = lapHandler.GetLap(_player);

        _lapText.text = "Lap " + playerLap + "/" + lapHandler.totalLaps;


    }

    public void OnGameWin(LapHandler lapHandler)
    {
        Time.timeScale = 0;

        _standardScreen.SetActive(false);
        _winScreen.SetActive(true);

        //_camera.SetTarget(_lapHandler.GetRandomRacer().transf);
    }

    private void Update()
    {
        HandleUIUpdates();
    }
}
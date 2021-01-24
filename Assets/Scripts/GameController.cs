using System.Collections;
using System.Collections.Generic;
using FrameworkdHelper;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public SpawnManager SpawnManager;
    public Player Player;
    public Camera GameCamera;

    [SerializeField]
    private Transform[] _levels;

    [SerializeField]
    private Vector3 _levelStartPosition;

    [SerializeField]
    private Vector3 _startPlayerPosition;

    [SerializeField]
    private float _playerSpeed;

    public float PlayerSpeed
    {
        get
        {
            return _playerSpeed;
        }
    }

    [SerializeField]
    private float _enemySpeed;

    public float EnemySpeed
    {
        get
        {
            return _enemySpeed;
        }
    }

    [SerializeField]
    private float _bulletSpeed;

    public float BulletSpeed
    {
        get
        {
            return _bulletSpeed;
        }
    }

    [SerializeField]
    private float _playerRotateSpeed;

    public float PlayerRotateSpeed
    {
        get
        {
            return _playerRotateSpeed;
        }
    }

    [SerializeField] 
    private float _enemyAttackRange;

    public float EnemyAttackRange
    {
        get
        {
            return _enemyAttackRange;
        }
    }

    private int _levelIndex = 0;
    private const int LevelGap = 233;

    public bool IsPause { get; private set; } = true;

    protected override void Awake()
    {
        base.Awake();
        GameCamera.gameObject.SetActive(false);
    }

    private void ReOrderLevelInterval()
    {
        //Debug.Log("ReOrderLevelInterval" + _levelIndex);
        Vector3 LevelPos = _levels[_levelIndex].position;
        _levelIndex++;
        if (_levelIndex >= _levels.Length)
        {
            _levelIndex = 0;
        }
        _levels[_levelIndex].gameObject.SetActive(true);
        _levels[_levelIndex].position = LevelPos + new Vector3(0, 0, LevelGap);
    }

    public void PlayGame()
    {
        GameCamera.gameObject.SetActive(true);
        float delay = LevelGap / (_playerSpeed * 2);
        float reOrderPeriod = LevelGap / _playerSpeed;
        ResetLevelPosition();
        InvokeRepeating("ReOrderLevelInterval", delay, reOrderPeriod);
        Player.Setup(_startPlayerPosition);
        SpawnManager.StartGame();
        IsPause = false;
    }

    public void PauseGame()
    {
        IsPause = true;
    }

    public void ResumeGame()
    {
        IsPause = false;
    }

    public void GameOver()
    {
        IsPause = true;
        GUIManager.Instance.GameOver();
        CancelInvoke();
    }

    public void ResetLevelPosition()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i].position = _levelStartPosition + i * new Vector3(0, 0, LevelGap);
            _levels[i].gameObject.SetActive(true);
        }
    }
}

public enum ActionState { Run, Death, Walk, Eat }

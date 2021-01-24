using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private Transform[] _gunLocs;

    [SerializeField]
    private HandGun _weapon;

    [SerializeField]
    private CharacterController _controller;

    [SerializeField]
    private Transform _modelHolder;

    [SerializeField] 
    private Transform _bodyCamTransform;

    [SerializeField]
    private Animator _anim;

    [SerializeField]
    private bool _isImmortal;

    private static float Speed
    {
        get
        {
            return GameController.Instance.PlayerSpeed;
        }
    }

    private static SpawnManager SpawnManager
    {
        get
        {
            return GameController.Instance.SpawnManager;
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    private static Camera GameCamera
    {
        get
        {
            return GameController.Instance.GameCamera;
        }
    }

    private static float RotateSpeed
    {
        get
        {
            return GameController.Instance.PlayerRotateSpeed;
        }
    }
    
    public ActionState CurrentState
    {
        get
        {
            return _currentState;
        }
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnActionStateChanged();
            }
        }
    }

    private ActionState _currentState = ActionState.Run;

    private Dictionary<ActionState, string> _animationNames = new Dictionary<ActionState, string>()
    {
        {ActionState.Run, "run"},
        {ActionState.Death, "death"}
    };


    public void Setup()
    {
        _weapon.SetupWeapon();
        CurrentState = ActionState.Run;
    }

    public void ResetPlayerPosition(Vector3 position)
    {
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        _bodyCamTransform.localRotation = new Quaternion(0, 0, 0, 0);
        _modelHolder.localRotation = new Quaternion(0, 0, 0, 0);
        transform.position = position;
    }

    public Bullet Shoot(Vector3 target)
    {
        if (CurrentState == ActionState.Death)
        {
            return null;
        }
        Vector3 muzzlePos = 
            _gunLocs[Random.Range(0, _gunLocs.Length)].position;

        return _weapon.Shoot(
            SpawnManager, 
            1,
            muzzlePos,
            target);
    }
    
    private void Update()
    {
        if (GameController.Instance.IsPause)
        {
            return;
        }

        Moving();
        LookToNearestTarget();
    }

    private void Moving()
    {
        if (CurrentState == ActionState.Death)
        {
            return;
        }

        _controller.Move(transform.forward * Speed * Time.deltaTime);
    }

    private void LookToNearestTarget()
    {
        if (CurrentState == ActionState.Death)
        {
            return;
        }
        Enemy nearestEnemy = SpawnManager.GetNearestEnemy();
        if (nearestEnemy == null)
        {
            return;
        }
        Quaternion qt = Quaternion.LookRotation(nearestEnemy.Position - GameCamera.transform.position);
        Quaternion newRotate = Quaternion.Slerp(GameCamera.transform.rotation, qt, Time.deltaTime * RotateSpeed);
        GameCamera.transform.rotation = new Quaternion(0, newRotate.y, 0, newRotate.w);
        //GameCamera.transform.rotation = new Quaternion(0, Mathf.Clamp(newRotate.y, -0.75f, 0.75f), 0, newRotate.w);
    }

    public void Damaged()
    {
        
        Death();
    }

    private void Death()
    {
#if UNITY_EDITOR
        if (_isImmortal)
        {
            return;
        }
#endif
        if (CurrentState == ActionState.Death)
        {
            return;
        }
        CurrentState = ActionState.Death;
        StartCoroutine(StartGameOver());
    }

    private IEnumerator StartGameOver()
    {
        yield return GUIManager.WaitXSecond;
        GameController.Instance.GameOver();
    }

    private void OnActionStateChanged()
    {
        _anim.SetTrigger(_animationNames[CurrentState]);
    }

}

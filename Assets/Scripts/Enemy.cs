using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreGame
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] 
        private Transform _bodyLoc;

        private Rigidbody _rb;

        private static Player Player
        {
            get
            {
                return GameController.Instance.Player;
            }
        }
        
        private static float Speed
        {
            get
            {
                return GameController.Instance.EnemySpeed;
            }
        }

        private Vector3 _moveDirection
        {
            get
            {
                return (Player.Position - Position).normalized;
            }
        }

        public SpawnData SpawnData { get; private set; }

        public int HP { get; private set; }
        public bool IsCurrentTarget { get; set; }
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        private static SpawnManager SpawnManager
        {
            get
            {
                return GameController.Instance.SpawnManager;
            }
        }
        
        private Animator _anim;

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
                    _anim.SetTrigger(_animationNames[_currentState]);
                }
            }
        }

        private ActionState _currentState = ActionState.Walk;

        private Dictionary<ActionState, string> _animationNames = new Dictionary<ActionState, string>()
        {
            {ActionState.Walk, "run"},
            {ActionState.Eat, "eat"},
            {ActionState.Death, "death"}
        };

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _anim = GetComponent<Animator>();
        }

        public void SetupSpawnData(SpawnData data)
        {
            SpawnData = data;
            transform.position = SpawnData.SpawnPosition;
        }
        public virtual void OnDesSpawn()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            HP = 0;
        }

        public virtual void OnSpawn()
        {
            CurrentState = ActionState.Walk;
            gameObject.SetActive(true);
            HP = 1;
            //transform.position = SpawnData.SpawnPosition;
        }

        private void FixedUpdate()
        {
            if (GameController.Instance.IsPause)
            {
                return;
            }

            transform.LookAt(Player.transform);
            if (CurrentState == ActionState.Walk)
            {
                _rb.MovePosition(transform.position + (_moveDirection * Speed * Time.fixedDeltaTime));

                if (Vector3.Distance(Position, Player.Position) < GameController.Instance.EnemyAttackRange)
                {
                    DamagedPlayer();
                }
            }
        }

        public void OnPointerUp(BaseEventData baseEventData)
        {
            //PointerEventData pointerEventData = baseEventData as PointerEventData;

            //Vector3 targetPos = pointerEventData == null
            //    ? GetGroundPosition(pointerEventData.pressPosition)
            //    : transform.position;
            Bullet bullet = Player.Shoot(_bodyLoc.position);
            if (bullet != null)
            {
                bullet.OnHit -= OnDeath;
                bullet.OnHit += OnDeath;
            }
        }

        private void DamagedPlayer()
        {
            CurrentState = ActionState.Eat;
            Player.Damaged();
        }

        private void OnDeath()
        {
            ObjectToPool fx = SpawnManager.GetPoolObject("Explode");
            fx.transform.position = Position;
            fx.StartSelfDespawnAfter(1);
            SpawnManager.EnemyPools[SpawnData.PrefabIndex].Release(this);
            // Todo add death animation
        }

    }
}

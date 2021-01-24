using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame;

public class SpawnManager: MonoBehaviour
{
    [Header("Setup")]
    
    [SerializeField]
    private int _delaySpawnRandomEnemies = 5;
    

    [SerializeField]
    private Enemy[] _enemyPrefs;
    [SerializeField]
    private float _randomMinX = -77f;
    [SerializeField]
    private float _randomMaxX = -58f;
    [SerializeField]
    private float _randomDistanceMin = 45f;
    [SerializeField]
    private float _randomDistanceMax = 65f;
    [SerializeField]
    private ObjectToPool[] _poolObjects;
    
    public List<ObjectPool<Enemy>> EnemyPools = new List<ObjectPool<Enemy>>();
    public Dictionary<string, ObjectPool<ObjectToPool>> ObjectsPools = new Dictionary<string, ObjectPool<ObjectToPool>>();

    //private int _maxEnemyBullets = 15;
    private HashSet<Enemy> _activeEnemies = new HashSet<Enemy>();
    private List<ObjectToPool> _activeObjectToPools = new List<ObjectToPool>();
    private Queue<int> _queueEnemiesIndex = new Queue<int>();
    private const int _maxEnemies = 20;
    private WaitForSeconds _waitToSpawnRandomEnemies;
    private Transform _objectHolder;

    private static Player Player
    {
        get
        {
            return GameController.Instance.Player;
        }
    }


    public bool IsPlaying
    {
        get { return !GameController.Instance.IsPause; }
    }

    private void Awake()
    {
        if (_objectHolder == null)
        {
            _objectHolder = new GameObject("EditorEnemiesHolder").transform;
        }
        _queueEnemiesIndex = GenerateRandomQueue(_enemyPrefs.Length, 0, _enemyPrefs.Length);
        SetupEnemiesPool();
    }
    
    public void StartGame()
    {
        DespawnAllActiveObjectPools();
        StartSpawnRandom();
    }
    
    private void Update()
    {
    }

    public void DespawnAllActiveObjectPools()
    {
        List<Enemy> activeEnemies = new List<Enemy>(_activeEnemies);    
        //Debug.Log("activeEnemies " + activeEnemies.Count);
        //Debug.Log("activeBarels " + activeBarels.Count);
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            EnemyPools[activeEnemies[i].SpawnData.PrefabIndex].Release(activeEnemies[i]);
        }
        DespawnAllPoolObjects();
    }

    private void DespawnAllPoolObjects()
    {
        List<ObjectToPool> activeObjectToPools = new List<ObjectToPool>(_activeObjectToPools);

        for (int i = 0; i < activeObjectToPools.Count; i++)
        {
            ReleasePoolObject(activeObjectToPools[i]);
        }
    }
    
    private void SetupEnemiesPool()
    {
        if (EnemyPools.Count < 1)
        {
            for (int i = 0; i < _enemyPrefs.Length; i++)
            {
                ObjectPool<Enemy> childEnemyPool = new ObjectPool<Enemy>(capacity:_maxEnemies,
                    actionNew: enemyName => {
                        int index = 0;
                        for (int j = 0; j < _enemyPrefs.Length; j++)
                        {
                            if (enemyName == _enemyPrefs[j].name)
                            {
                                index = j;
                                break;
                            }
                        }
                        //Debug.Log("init enemy index:" + index);
                        Enemy enemy = Instantiate(_enemyPrefs[index]);
#if UNITY_EDITOR
                        enemy.transform.SetParent(_objectHolder);
#endif
                        if (!_activeEnemies.Contains(enemy))
                        {
                            _activeEnemies.Add(enemy);
                        }

                        return enemy;
                    },
                    actionOnGet: enemy => {
                        enemy.OnSpawn();
                        if (!_activeEnemies.Contains(enemy))
                        {
                            _activeEnemies.Add(enemy);
                        }
                    },
                    actionOnRelease: enemy => {
                        if (_activeEnemies.Contains(enemy))
                        {
                            _activeEnemies.Remove(enemy);
                        }
                        enemy.OnDesSpawn();
                    },
                    _enemyPrefs[i].name);
                if (!EnemyPools.Contains(childEnemyPool))
                {
                    EnemyPools.Add(childEnemyPool);
                }
            }
        }
    }
    
    public void CreateObjectPool(string keyName)
    {
        if (!ObjectsPools.ContainsKey(keyName))
        {
            ObjectPool<ObjectToPool> Pool = new ObjectPool<ObjectToPool>(5,
            (name) => {
                int index = 0;
                for (int j = 0; j < _poolObjects.Length; j++)
                {
                    if (name == _poolObjects[j].name)
                    {
                        index = j;
                        break;
                    }
                }
                ObjectToPool obj = Instantiate(_poolObjects[index]);
#if UNITY_EDITOR
                obj.transform.SetParent(_objectHolder);
#endif
                obj.name = name;
                if (!_activeObjectToPools.Contains(obj))
                {
                    _activeObjectToPools.Add(obj);
                }
                return obj;
            },
            (obj) => {
                obj.GetItem(obj);
                if (!_activeObjectToPools.Contains(obj))
                {
                    _activeObjectToPools.Add(obj);
                }                
            },
            (obj) => {
                if (_activeObjectToPools.Contains(obj))
                {
                    _activeObjectToPools.Remove(obj);
                }
                obj.ReleaseItem(obj);
            },
            keyName);
            ObjectsPools.Add(keyName, Pool);
        }        
    }

    public ObjectToPool GetPoolObject(string KeyName)
    {
        if (!ObjectsPools.ContainsKey(KeyName))
        {
            CreateObjectPool(KeyName);
        }
        return ObjectsPools[KeyName].Get();
    }

    public void ReleasePoolObject(ObjectToPool obj)
    {
        ObjectsPools[obj.name].Release(obj);
    }

    public void StartSpawnRandom()
    {
        _waitToSpawnRandomEnemies = new WaitForSeconds(_delaySpawnRandomEnemies);
        StartCoroutine(StartRandomSpawnEnemies());
    }

    private IEnumerator StartRandomSpawnEnemies()
    {
        while(IsPlaying)
        {
            //Debug.Log(Time.time);
            yield return _waitToSpawnRandomEnemies;
            // Todo: add condition to win or lose;
            SpawnRandomEnemies();
        }
    }

    private void SpawnRandomEnemies()
    {
        int randomIndex = _queueEnemiesIndex.Dequeue();
        //Debug.Log("randomIndex: " + randomIndex);
        Enemy enemy = EnemyPools[randomIndex].Get();
        SpawnData data = new SpawnData(randomIndex, GetRandomSpawnPosition());
        enemy.SetupSpawnData(data);
        _queueEnemiesIndex.Enqueue(randomIndex);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(
            Random.Range(_randomMinX, _randomMaxX),
            0,
            Player.Position.z + Random.Range(_randomDistanceMin, _randomDistanceMax));
    }

    private static Queue<int> GenerateRandomQueue(int count, int min, int max)
    {
        System.Random random = new System.Random();
        HashSet<int> candidates = new HashSet<int>();

        for (int top = max - count; top < max; top++)
        {
            if (!candidates.Add(random.Next(min, top + 1)))
            {
                candidates.Add(top);
            }
        }
        List<int> result = new List<int>();
        foreach (int i in candidates)
        {
            result.Add(i);
        }

        for (int i = result.Count - 1; i > 0; i--)
        {
            int k = random.Next(i + 1);
            int tmp = result[k];
            result[k] = result[i];
            result[i] = tmp;
        }
        return new Queue<int>(result);
    }

    public Enemy GetNearestEnemy()
    {
        List<Enemy> activeTargetObj = new List<Enemy>(_activeEnemies);
        if (activeTargetObj.Count > 0)
        {
            Enemy nearestEnemy = null;
            float minDistance = 0;
            foreach (var enemy in activeTargetObj)
            {
                if (enemy.HP > 0)
                {
                    enemy.IsCurrentTarget = false;
                    float curDis = Vector3.Distance(Player.Position, enemy.Position);
                    if (enemy.Position.z <= Player.Position.z)
                    {
                        continue;
                    }
                    if (minDistance == 0)
                    {
                        minDistance = curDis;
                    }
                    
                    if (curDis <= minDistance)
                    {
                        nearestEnemy = enemy;
                        minDistance = curDis;
                    }
                }
            }
            if (nearestEnemy != null)
            {
                nearestEnemy.IsCurrentTarget = true;
            }
            //Debug.Log("fount nereast");
            return nearestEnemy;
        }
        return null;
    }

}

public struct SpawnData
{
    public int PrefabIndex;
    public Vector3 SpawnPosition;

    public SpawnData(int index, Vector3 position)
    {
        PrefabIndex = index;
        SpawnPosition = position;
    }
}
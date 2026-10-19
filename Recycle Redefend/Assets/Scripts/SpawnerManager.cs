using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance { get; private set; }

    [SerializeField] int spawnerCount = 3;
    [SerializeField] float spawnCooldown = 1.5f, subwaveCount = 5, spawnChance = 0.8f, upgradeChance = 0.1f, spawnRadius = 5;
    [SerializeField] GameObject prefabSpawner;
    [SerializeField] Transform commandCenter, spawnArea;
    [SerializeField] GameObject[] prefabEnemies;


    List<Transform> spawners;
    Dictionary<EnemyType, List<Enemy>> enemyPool;

    public bool AreEnemiesActive { get; private set; }
    public int ActiveEnemyCount { get; private set; }

    void Awake()
    {
        if (Instance) return;
        Instance = this;
        
        spawners = new List<Transform>();
        enemyPool = new Dictionary<EnemyType, List<Enemy>>()
        {
            { EnemyType.Weak, new () },
            { EnemyType.Normal, new () },
            { EnemyType.Strong, new () },
            { EnemyType.Dangerous, new () },
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BattleManager.Instance.AddOnWaveStartedListener((object obj, System.EventArgs args) => StartCoroutine(SpawnCoroutine()));

        for (int i = 0; i < spawnerCount; i++)
        {
            CreateSpawner();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateSpawner()
    {
        float x = 0;
        float y = 0;
        Vector2 bounds = spawnArea.localScale * 0.4f;
        if (Random.value > 0.5f)
        {
            x = Random.Range(-bounds.x, bounds.x);
            y = Mathf.Sign(Random.value - 0.5f) * bounds.y;
        }
        else
        {
            x = Mathf.Sign(Random.value - 0.5f) * bounds.x;
            y = Random.Range(-bounds.y, bounds.y);
        }
        GameObject spawner = Instantiate(prefabSpawner, new Vector2(x, y), Quaternion.identity);
        spawners.Add(spawner.transform);
    }

    Enemy RetrieveEnemy(EnemyType enemyType)
    {
        List<Enemy> currentPool = enemyPool[enemyType];
        Enemy output = currentPool.Find(enemy => !enemy.gameObject.activeInHierarchy);
        if (!output)
        {
            output = Instantiate(prefabEnemies[(int)enemyType]).GetComponent<Enemy>();
            currentPool.Add(output);
        }
        output.gameObject.SetActive(true);
        return output;
    }

    public void ReturnEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        ActiveEnemyCount--;

        if (ActiveEnemyCount > 0) return;
        AreEnemiesActive = false;
    }

    IEnumerator SpawnCoroutine()
    {
        AreEnemiesActive = true;
        for (int i = 0; i < subwaveCount; i++)
        {
            foreach (Transform spawner in spawners)
            {
                if (Random.value > spawnChance) continue;
                int strength = 0;
                for (; strength < prefabEnemies.Length; strength++)
                {
                    if (Random.value >= upgradeChance) break;
                }
                Enemy enemy = RetrieveEnemy((EnemyType)strength);
                Vector2 spawnPosition = (Vector2)spawner.position + Random.insideUnitCircle * spawnRadius;
                enemy.Initialize(spawnPosition, commandCenter.position);
                ActiveEnemyCount++;
            }
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    enum EnemyType { None = -1, Weak = 0, Normal = 1, Strong = 2, Dangerous = 3 }
}

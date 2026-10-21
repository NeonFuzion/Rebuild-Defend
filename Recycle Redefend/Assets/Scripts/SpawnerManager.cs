using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance { get; private set; }

    [SerializeField] int spawnerCount = 3, spawnerIncrementAmount = 1, subwaveCount = 5, subwaveIncrementAmount = 1;
    [SerializeField] float spawnCooldown = 1.5f, spawnChance = 0.8f, upgradeChance = 0.05f, upgradeIncrementAmount = 0.03f,
        spawnRadius = 5, shipmentSpawnMultiplier = 0.8f, edgePositionMultiplier = 0.4f;
    [SerializeField] GameObject prefabSpawner, prefabShipment;
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
        BattleManager.Instance.AddOnWaveEndedListener((object obj, System.EventArgs args) => CreateShipment());

        for (int i = 0; i < spawnerCount; i++)
        {
            CreateSpawner();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2 GetEdgePosition()
    {
        float x = 0;
        float y = 0;
        Vector2 bounds = spawnArea.localScale * edgePositionMultiplier;
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
        return new (x, y);
    }

    void CreateShipment()
    {
        //if (BattleManager.Instance.CurrentWave == 0) return;
        GameObject shipment = Instantiate(prefabShipment, GetEdgePosition() * shipmentSpawnMultiplier, Quaternion.identity);
    }

    void CreateSpawner()
    {
        GameObject spawner = Instantiate(prefabSpawner, GetEdgePosition(), Quaternion.identity);
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
        switch (Random.Range(0, 3))
        {
            case 0: spawnerCount += spawnerIncrementAmount; break;
            case 1: subwaveCount += subwaveIncrementAmount; break;
            case 2: upgradeChance += upgradeIncrementAmount; break;
        }

        AreEnemiesActive = true;
        for (int i = 0; i < subwaveCount; i++)
        {
            foreach (Transform spawner in spawners)
            {
                if (Random.value > spawnChance) continue;
                int strength = 0;
                for (; strength < prefabEnemies.Length - 1; strength++)
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

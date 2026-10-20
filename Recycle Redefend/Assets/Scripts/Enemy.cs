using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] AudioClip hitAudioClip, deathAudioClip;
    [SerializeField] LayerMask baseLayer;

    int hitAnimHash = Animator.StringToHash("EnemyHit"),
        deathAnimHash = Animator.StringToHash("EnemyDeath"),
        spawnAnimHash = Animator.StringToHash("EnemySpawn");

    EnemyState enemyState;
    NavMeshAgent navMeshAgent;
    Animator animator;
    Health healthScript;
    Vector2 targetPosition;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        healthScript = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState != EnemyState.Idle) return;
        navMeshAgent.destination = targetPosition;

        if (transform.position.magnitude > 2) return;
        BattleManager.Instance.DealDamage(damage);
        SpawnerManager.Instance.ReturnEnemy(this);
    }

    void OnAnimationDeath()
    {
        SpawnerManager.Instance.ReturnEnemy(this);
    }

    void OnAnimationHit()
    {
        enemyState = EnemyState.Idle;
    }

    public void Initialize(Vector2 position, Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        healthScript.Initialize();
        enemyState = EnemyState.Idle;
        transform.position = new (position.x, position.y, 0);
        animator.CrossFade(spawnAnimHash, 0, 0);
    }

    public void TakeDamage(int damage)
    {
        if (enemyState == EnemyState.Dead) return;
        bool isDead = healthScript.TakeDamage(damage);
        
        if (isDead)
        {
            enemyState = EnemyState.Dead;
            SoundEffectsManager.Instance.PlaySoundEffect(deathAudioClip, transform.position, 1);
            animator.CrossFade(deathAnimHash, 0, 0);
        }
        else
        {
            enemyState = EnemyState.Hit;
            SoundEffectsManager.Instance.PlaySoundEffect(hitAudioClip, transform.position, 1);
            animator.CrossFade(hitAnimHash, 0, 0);
        }
    }

    enum EnemyState { None, Idle, Hit, Dead }
}

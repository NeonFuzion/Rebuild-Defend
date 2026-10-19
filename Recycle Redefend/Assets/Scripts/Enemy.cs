using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] LayerMask baseLayer;

    int hitAnimHash = Animator.StringToHash("EnemyHit"),
        deathAnimHash = Animator.StringToHash("EnemyDeath"),
        spawnAnimHash = Animator.StringToHash("EnemySpawn");

    bool isMobile;

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
        if (isMobile) navMeshAgent.destination = targetPosition;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

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
        isMobile = true;
    }

    public void Initialize(Vector2 position, Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        healthScript.Initialize();
        isMobile = true;
        transform.position = new (position.x, position.y, 0);
        animator.CrossFade(spawnAnimHash, 0, 0);
    }

    public void TakeDamage(int damage)
    {
        healthScript.TakeDamage(damage);
        isMobile = false;
        animator.CrossFade(hitAnimHash, 0, 0);
    }

    public void OnDeath()
    {
        isMobile = false;
        animator.CrossFade(deathAnimHash, 0, 0);
    }
}

using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Turret : MonoBehaviour
{
    [SerializeField] int damage, durability;
    [SerializeField] float cooldown = 1, rotationSpeed = 3, detectionRange = 8, fireAngleThreshold = 0.5f;
    [SerializeField] AnimationCurve rotationSmoothCurve;
    [SerializeField] ProjectileSO projectileData;
    [SerializeField] Transform pivot, firePoint;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] LayerMask enemyLayer;

    int firingAnimHash = Animator.StringToHash("TurretFire"),
        placingAnimHash = Animator.StringToHash("TurretPlace"),
        idleAnimHash = Animator.StringToHash("TurretIdle"),
        breakAnimHash = Animator.StringToHash("TurretBreak");

    float currentAngle, targetAngle, aimTime;

    Vector2 targetPosition;
    TurretState turretState;
    Animator animator;
    UnityEvent<int> onTickDurability;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turretState = TurretState.Idle;
        animator = GetComponent<Animator>();

        BattleManager.Instance.AddOnWaveEndedListener((System.Object obj, EventArgs args) => UpdateDurability());
    }

    // Update is called once per frame
    void Update()
    {
        switch (turretState)
        {
            case TurretState.Idle:
                Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

                if (colliders.Length == 0) break;
                Transform target = colliders.OrderBy(collider => Vector2.Distance(collider.transform.position, transform.position)).FirstOrDefault().transform;
                NavMeshAgent agent = target.GetComponent<NavMeshAgent>();
                targetPosition = target.position;
                Vector2 positionDelta = targetPosition - (Vector2)transform.position;
                targetAngle = Mathf.Atan2(positionDelta.x, positionDelta.y) * 180 / Mathf.PI;
                aimTime = 0;
                turretState = TurretState.Aiming;
                break;
            case TurretState.Aiming:
                aimTime += rotationSpeed * Time.deltaTime;
                float progress = rotationSmoothCurve.Evaluate(aimTime);
                currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, progress);
                pivot.eulerAngles = new Vector3(0, 0, -currentAngle);

                if (progress < 1) break;
                pivot.eulerAngles = new Vector3(0, 0, -targetAngle);
                turretState = TurretState.Firing;
                animator.CrossFade(firingAnimHash, 0, 0);
                break;
        }
    }

    IEnumerator TargettingCoroutine()
    {
        turretState = TurretState.Recovering;
        yield return new WaitForSeconds(cooldown);
        turretState = TurretState.Idle;
        animator.CrossFade(idleAnimHash, 0, 0);
    }

    void UpdateDurability()
    {
        durability--;
        onTickDurability.Invoke(durability);

        if (durability > 0) return;
        turretState = TurretState.Breaking;
        animator.CrossFade(breakAnimHash, 0, 0);
    }

    void OnAnimationFire()
    {
        particleSystem.Play();
        Projectile projectile = ProjectileManager.Instance.RetrieveProjectile();
        projectile.Initialize(firePoint.position, targetPosition, projectileData, damage);
        StartCoroutine(TargettingCoroutine());
    }

    public void Initialize(DurabilityBar durabilityBar)
    {
        onTickDurability = new UnityEvent<int>();
        onTickDurability.AddListener(durabilityBar.UpdateDurability);
        durabilityBar.Initialize(durability);
    }

    enum TurretState { None, Idle, Aiming, Firing, Recovering, Breaking }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] Transform projectileVisual, projectileShadow;
    [SerializeField] LayerMask enemyLayer;

    float totalDistance, groundDirection;
    int damage, currentPenetrationCount;
    bool isDying;

    ProjectileSO data;
    ParticleSystem particleSystem;
    Vector2 targetPosition, startPosition;
    List<Collider> hitList;
    
    SpriteRenderer visualSpriteRenderer, shadowSpriteRenderer;

    void Awake()
    {
        visualSpriteRenderer = projectileVisual.GetComponent<SpriteRenderer>();
        shadowSpriteRenderer = projectileShadow.GetComponent<SpriteRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
        hitList = new List<Collider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying) return;
        totalDistance = Vector2.Distance(startPosition, targetPosition);
        transform.position += (Vector3)(targetPosition - startPosition).normalized * data.Speed * Time.deltaTime;

        float distanceCovered = Vector2.Distance(transform.position, startPosition);
        float distanceProgress = distanceCovered / totalDistance;
        float trajectoryCurveValue = data.TrajectoryCurve.Evaluate(distanceProgress);
        float projectileHeight = trajectoryCurveValue * data.MaxHeight * totalDistance / 8;
        projectileVisual.transform.localPosition = Vector2.up * projectileHeight;

        Vector2 differenceVector = targetPosition - (Vector2)transform.position;
        float radians = Mathf.Atan2(differenceVector.y, differenceVector.x);
        groundDirection = (radians > 0 ? radians : radians + 2 * Mathf.PI) * 180 / Mathf.PI % 360;

        float trajectoryAngle = (1 - trajectoryCurveValue) * (distanceProgress > 0.5f && data.MaxHeight != 0 ? -1 : 1) * data.MaxHeight * 20;
        projectileVisual.transform.eulerAngles = Vector3.forward * (groundDirection + trajectoryAngle);
        if (projectileShadow) projectileShadow.transform.eulerAngles = Vector3.forward * groundDirection;

        if (distanceProgress < 1 && data.MaxHeight > 0) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, data.DamageRadius, enemyLayer);

        if (data.MaxHeight > 0)
        {
            if (colliders.Length > 0) DealDamage(colliders);
            DestroySelf();
        }
        else
        {
            if (distanceProgress > 1)
            {
                DestroySelf();
                return;
            }

            if (colliders.Length == 0) return;
            DealDamage(colliders);
            
            if (data.PenetrationCount == 0) return;
            if (currentPenetrationCount > 0) return;
            DestroySelf();
        }
    }

    IEnumerator ExplosionCoroutine()
    {
        yield return new WaitWhile(() => particleSystem.isPlaying);
        ProjectileManager.Instance.ReturnProjectile(this);
    }

    void DealDamage(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (hitList.Contains(collider)) continue;
            Enemy enemy = collider.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            hitList.Add(collider);
            currentPenetrationCount--;
        }
    }

    void DestroySelf()
    {
        particleSystem.Play();
        isDying = true;
        visualSpriteRenderer.sprite = null;
        shadowSpriteRenderer.sprite = null;
        StartCoroutine(ExplosionCoroutine());
    }

    public void Initialize(Vector2 startPosition, Vector2 targetPosition, ProjectileSO data, int damage)
    {
        this.damage = damage;
        this.data = data;
        this.startPosition = startPosition;

        isDying = false;
        currentPenetrationCount = data.PenetrationCount;

        Vector3 correctedTargetPosition = (targetPosition - startPosition).normalized * data.MaxDistance + startPosition;
        this.targetPosition = data.MaxHeight == 0 ? correctedTargetPosition : targetPosition;
        
        transform.position = startPosition;
        visualSpriteRenderer.sprite = data.Sprite;
        shadowSpriteRenderer.sprite = data.Sprite;

        hitList.Clear();
    }
}
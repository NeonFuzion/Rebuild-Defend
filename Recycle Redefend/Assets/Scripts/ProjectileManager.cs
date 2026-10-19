using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance { get; private set; }

    [SerializeField] GameObject prefabProjectile;

    List<Projectile> projectiles;

    void Awake()
    {
        if (Instance) return;
        Instance = this;

        projectiles = new List<Projectile>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Projectile RetrieveProjectile()
    {
        Projectile projectile = projectiles.Find(element => !element.gameObject.activeInHierarchy);
        if (!projectile)
        {
            projectile = Instantiate(prefabProjectile).GetComponent<Projectile>();
            projectiles.Add(projectile);
        }
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void ReturnProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }
}

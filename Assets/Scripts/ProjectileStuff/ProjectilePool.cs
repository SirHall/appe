using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.LinqE;
using Excessives;
using Excessives.Unity;

//Is used to hold all projectiles currently in existence

//{TODO} Re-do
public class ProjectilePool : MonoBehaviour
{
    [SerializeField]
    GameObject defaultPrefab;

    [SerializeField]
    ProjectileData defaultData;

    public static ProjectilePool instance;

    [SerializeField]
    int poolSize = 10;

    Queue<AffectedProjectile> projectilePool = new Queue<AffectedProjectile>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //{TODO} Re-enable and fix
        // StatementsE.Repeat((ulong)poolSize, () => projectilePool.Enqueue(new AffectedProjectile(
        //    Vector3.zero,
        //    Vector3.zero,
        //    defaultData,
        //    defaultPrefab,
        //    0,//{TODO} Change this value
        //    false
        //)
        //));

        projectilePool.Count.Log();
    }

    void FixedUpdate()
    {
        projectilePool.ForEach(n => n.Tick(Time.fixedDeltaTime));
    }

    public void FireProjectile(
        Vector3 position,
        Vector3 direction,
        ProjectileData pd,
        float twist
    )
    {
        AffectedProjectile projectile = projectilePool.Dequeue();

        projectile.Initial(position, direction, pd, twist);

        projectilePool.Enqueue(projectile);
    }

    public void FireProjectile(
        Vector3 position,
        Vector3 direction,
        ProjectileData pd,
        GameObject newBulletPrefab,
        float twist
    )
    {
        AffectedProjectile projectile = projectilePool.Dequeue();

        projectile.Initial(position, direction, pd, twist);

        projectilePool.Enqueue(projectile);

    }

    public void DestroyPool()
    {
        //projectilePool.ForEach(n => GameObject.Destroy(n.gameObjectRep));

        projectilePool.Clear();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.LinqE;
using Excessives;
using Excessives.Unity;
using System.Linq;

//Is used to hold all projectiles currently in existence

//{TODO} Re-do
public class ProjectilePool : MonoBehaviour
{
	public static ProjectilePool instance;

	#region Private Vars

	[SerializeField]
	int _poolSize = 1000;

	AffectedProjectile[] projectilePool = new AffectedProjectile[0];

	//Projectiles should rarely reach this, meaning that not too much garbage will be created
	Queue<AffectedProjectile> projectileOverflow = new Queue<AffectedProjectile>();

	#endregion

	#region Properties

	public int PoolSize
	{
		get { return _poolSize; }
		set
		{
			if (_poolSize == value) //We did not resize the poolsize
				return;
			AffectedProjectile[] oldArray = projectilePool;
			projectilePool = new AffectedProjectile[_poolSize];

			if (value > _poolSize) //We have made the pool larger
				oldArray.CopyTo(projectilePool, 0);
			else //We have made the pool smaller
				oldArray.SubArray(0, value).ToArray().CopyTo(projectilePool, 0);

			_poolSize = value;
		}
	}

	#endregion


	void Awake()
	{
		instance = this;
		projectilePool = new AffectedProjectile[_poolSize];
	}

	void FixedUpdate()
	{
		for (int i = 0; i < projectilePool.Length; i++)
			if (projectilePool[i] != null && projectilePool[i].Active)
				projectilePool[i].Tick(Time.fixedDeltaTime);
	}

	private void Update()
	{
		//Attempt to clear the queue
		while (
			projectileOverflow.Count > 0 &&
			projectilePool.Any(n => n == null || !n.Active)
			)
			FireProjectile(projectileOverflow.Dequeue());
	}

	public void FireProjectile(AffectedProjectile projectile)
	{
		int index = -1;
		//{TODO} Could redo this with 'LinqE.FindIndex()'
		for (int i = 0; i < projectilePool.Length; i++)
			if (projectilePool[i] == null || !projectilePool[i].Active)
			{
				index = i;
				break;
			}

		if (index != -1)
		{
			projectilePool[index] = projectile;
			projectile.Active = true;
		}
		else
			projectileOverflow.Enqueue(projectile);



		//if (PoolSize == 0)
		//	Debug.LogError("Must have a poolsize of atleast '1' to fire a projectile");
		//AffectedProjectile projectile = projectilePool.First(n => !n.Active);

		//projectile.Initial(position, direction, pd, twist);

		//projectile.Active = true;
	}

	public void ForceOffPool()
	{
		projectilePool.For((n, i) => projectilePool[i] = null);
	}

}

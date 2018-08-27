using Excessives.Unity;
using Excessives.LinqE;
using System;
using UnityEngine;

//The purely simulated projectile

[System.Serializable]
public class AffectedProjectile : ICloneable
{
	public ProjectileData projectileData;

	public VirtualPhysicsTransform physicsTransform = new VirtualPhysicsTransform();

	public LayerMask rayCastLayerMask;

	[SerializeField]
	public AffectorBase[] affectors = new AffectorBase[]
	{
		new Affector_Gravity()//,
		//new Affector_Coriolis(),
		//new Affector_SpinTwist(),
		//new Affector_WindDragCubed()
	};

	public TerminalBallisticCalculator[] terminalCalculators = new TerminalBallisticCalculator[]
			{ new TerminalBallisticCalculator() };
	public TerminalBallisticDetector terminalDetector = new TerminalBallisticDetector();

	public bool Active { get; set; } = false;

	#region Initialization

	public AffectedProjectile()
	{
		//Initial(position, direction, projectileData);
		//SetupAffectors();
		affectors.ForEach(n => n.Projectile = this);
	}

	[System.Obsolete]
	public void Initial()
	{
		//physicsTransform = new VirtualPhysicsTransform();

		//this.physicsTransform.Position = position;

		//this.physicsTransform.Velocity = //Properly will result in a circle shot group, rather than square
		//								 //Quaternion.Euler(UnityEngine.Random.Range(0, projectileData.spreadAngle), 0, 0) //{TODO} Use this maths on the gunnery end
		//								 //*
		//								 //Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.forward)
		//								 //*
		//(direction * projectileData.initVelocity);


		//this.projectileData = projectileData;
		//this.physicsTransform.SpinVelocity = twist;
	}

	#region Setup

	//protected virtual void SetupAffectors()
	//{
	//	affectors = new AffectorBase[]
	//	{
	//		new Affector_Gravity(this),
	//		new Affector_Coriolis(this),
	//		new Affector_SpinTwist(this),
	//		new Affector_WindDragCubed(this)
	//	};
	//}

	#endregion

	#endregion

	public virtual void Tick(float deltaTime)
	{
		affectors.ForEach(n => n.Tick_PrePhysics(deltaTime));

		physicsTransform.Tick(deltaTime);

		//If we hit something, process it
		TerminalBallisticsData terminalData = terminalDetector.Tick(this); //Have we hit anything?
		if (terminalData != null)
			terminalCalculators.ForEach(n => n.ProcessTerminalHit(terminalData)); //Tell all the terminalCalculators, that he have hit something!

		foreach (AffectorBase affector in affectors)
			affector.Tick_PostPhysics(deltaTime);
		//affectors.ForEach(n => n.Tick_PostPhysics(deltaTime));

		//Debug information
		Debug.DrawLine(physicsTransform.PrevPosition, physicsTransform.Position, Color.blue, 120);
		DrawingFuncs.DrawStar(physicsTransform.Position, Color.green, 0.5f, 120);
	}

	public virtual object Clone()
	{
		AffectedProjectile newProj = (AffectedProjectile)MemberwiseClone();
		newProj.physicsTransform = physicsTransform.Clone() as VirtualPhysicsTransform;

		AffectorBase[] newAffectors = new AffectorBase[affectors.Length];
		for (int i = 0; i < newAffectors.Length; i++)
		{
			newAffectors[i] = (AffectorBase)affectors[i].Clone();
			newAffectors[i].Projectile = newProj;
		}
		newProj.affectors = newAffectors;

		return newProj;
	}
}
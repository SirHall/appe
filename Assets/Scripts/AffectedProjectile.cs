using Excessives.Unity;
using Excessives.LinqE;
using System;
using UnityEngine;
using System.Linq;

//The purely simulated projectile

[System.Serializable]
public class AffectedProjectile : ICloneable {
	public ProjectileData projectileData = new ProjectileData();

	public VirtualPhysicsTransform physicsTransform = new VirtualPhysicsTransform();

	//[SerializeField]
	public AffectorBase[] affectors = new AffectorBase[0];

	//{TODO} Setup cloning for below
	public TerminalBallisticCalculator[] terminalCalculators = new TerminalBallisticCalculator[]
			{ new Default_TerminalCalculator() };
	public TerminalBallisticDetector terminalDetector = new TerminalBallisticDetector();

	public float aliveVelocity = 0.01f; //When below this velocity, this projectile is dead

	public bool Active { get; set; } = false;

	#region Initialization

	public AffectedProjectile() {
		//Initial(position, direction, projectileData);
		SetupAffectors();
	}

	//[System.Obsolete]
	public void Initial() {
		SetupAffectors();
	}

	#region Setup

	protected virtual void SetupAffectors() {
		affectors = new AffectorBase[]
		{
			new Affector_Gravity(),
			new Affector_Coriolis(),
			new Affector_SpinTwist()
			//new Affector_WindDragCubed()
		};
		affectors.ForEach(n => n.Projectile = this);
	}

	#endregion

	#endregion

	public virtual void Tick(float deltaTime) {
		physicsTransform.Mass = projectileData.bulletMass;
		//affectors.ForEach(n => n.Tick_PrePhysics(deltaTime));

		foreach (AffectorBase affector in affectors)
			affector.Tick_PrePhysics(deltaTime);

		physicsTransform.Tick(deltaTime);

		//If we hit something, process it
		TerminalBallisticsData terminalData = terminalDetector.Tick(this, true); //Have we hit anything?
		if (terminalData != null)
			terminalCalculators.ForEach(n => n.ProcessTerminalHit(terminalData)); //Tell all the terminalCalculators, that he have hit something!

		if (IsDead()) //Do this speed check before an effector speeds us up again
			Active = false; //Only set this when we actually die

		//affectors.ForEach(n => n.Tick_PostPhysics(deltaTime));

		foreach (AffectorBase affector in affectors)
			affector.Tick_PostPhysics(deltaTime);

		//Debug information
		//Debug.DrawLine(physicsTransform.PrevPosition, physicsTransform.Position, Color.blue, 120);
		//DrawingFuncs.DrawStar(physicsTransform.Position, Color.green, 0.5f, 120);
		physicsTransform.RenderDebug(deltaTime, 0.0f, 1.0f);


	}

	public virtual bool IsDead() {
		if (physicsTransform.Velocity.magnitude < aliveVelocity)
			Debug.Log("Died to low speed");
		return physicsTransform.Velocity.magnitude < aliveVelocity;
	}

	public virtual object Clone() {
		AffectedProjectile newProj = (AffectedProjectile)MemberwiseClone();
		newProj.physicsTransform = physicsTransform.Clone() as VirtualPhysicsTransform;
		newProj.affectors = affectors.Select(n => n.Clone() as AffectorBase).ToArray();
		newProj.terminalDetector = this.terminalDetector.Clone() as TerminalBallisticDetector;
		newProj.terminalCalculators = terminalCalculators.Select(n => n.Clone() as TerminalBallisticCalculator).ToArray();

		return newProj;
	}
}
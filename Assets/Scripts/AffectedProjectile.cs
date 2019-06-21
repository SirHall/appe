using Excessives.Unity;
using Excessives.LinqE;
using System;
using UnityEngine;
using System.Linq;
using OdinSerializer;

//The purely simulated projectile

[System.Serializable]
public class AffectedProjectile : ICloneable {

	public ProjectileData projectileData = new ProjectileData();

	public VirtualPhysicsTransform physicsTransform = new VirtualPhysicsTransform();

	private AffectorBase[] affectors = new AffectorBase[0];

	private TerminalBallisticCalculator[] terminalCalculators = new TerminalBallisticCalculator[1];

	public TerminalBallisticDetector terminalDetector = new TerminalBallisticDetector();

	public float aliveVelocity = 0.01f; //When below this velocity, this projectile is dead

	public bool Active { get; set; } = false;

	#region Temporary
	//TODO: Switch this to use polymorphic serialization

	[Header("Affectors")]

	public Affector_Gravity affGravity = new Affector_Gravity();
	public Affector_Coriolis affCoriolis = new Affector_Coriolis();
	public Affector_SpinTwist affSpinTwist = new Affector_SpinTwist();
	public Affector_WindDrag affWindDrag = new Affector_WindDrag();

	public Default_TerminalCalculator termCalc = new Default_TerminalCalculator();

	#endregion

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
			affGravity, affCoriolis, affSpinTwist, affWindDrag
			//new Affector_Gravity(),
			//new Affector_Coriolis(),
			//new Affector_SpinTwist()
			//new Affector_WindDragCubed()
		};
		affectors.ForEach(n => n.Projectile = this);

		terminalCalculators[0] = termCalc;

		projectileData.GenerateDragCoefficientGraph();
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
		TerminalBallisticsData terminalData = terminalDetector.Tick(this);//, true); //Have we hit anything?
		if (terminalData != null)
			terminalCalculators.ForEach(n => n.ProcessTerminalHit(terminalData)); //We have hit something!

		if (IsDead()) //Do this speed check before an effector speeds us up again
			Active = false; //Only set this when we actually die

		//affectors.ForEach(n => n.Tick_PostPhysics(deltaTime));

		foreach (AffectorBase affector in affectors)
			affector.Tick_PostPhysics(deltaTime);

		//Debug information
		//Debug.DrawLine(physicsTransform.PrevPosition, physicsTransform.Position, Color.blue, 120);
		//DrawingFuncs.DrawStar(physicsTransform.Position, Color.green, 0.5f, 120);
		physicsTransform.RenderDebug(deltaTime, deltaTime, 2.0f);


	}

	public virtual bool IsDead() {
		return physicsTransform.Velocity.magnitude < aliveVelocity || physicsTransform.Position.y < -10;
	}

	public virtual object Clone() {
		AffectedProjectile newProj = (AffectedProjectile)MemberwiseClone();
		newProj.physicsTransform = physicsTransform.Clone() as VirtualPhysicsTransform;
		//newProj.affectors = affectors.Select(n => n.Clone() as AffectorBase).ToArray();


		newProj.affGravity = affGravity.Clone() as Affector_Gravity;
		newProj.affCoriolis = affCoriolis.Clone() as Affector_Coriolis;
		newProj.affSpinTwist = affSpinTwist.Clone() as Affector_SpinTwist;
		newProj.affWindDrag = affWindDrag.Clone() as Affector_WindDrag;
		newProj.termCalc = termCalc.Clone() as Default_TerminalCalculator;


		newProj.terminalDetector = this.terminalDetector.Clone() as TerminalBallisticDetector;
		//newProj.terminalCalculators = terminalCalculators.Select(n => n.Clone() as TerminalBallisticCalculator).ToArray();

		newProj.Initial();

		return newProj;
	}
}
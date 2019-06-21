using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringDirCalc_BruteForce : TrajectoryCalculationData {

	int numOfDirs = 4;

	List<Tuple<bool, AffectedProjectile>> projectiles = new List<Tuple<bool, AffectedProjectile>>();

	public override void Initialize() {
		for (int i = 0; i < numOfDirs; i++) {

		}
	}

	protected override void Tick(float deltaTime) {
		foreach (var pair in projectiles) {
			pair.Item2.Tick(deltaTime);
			//if()
		}
	}

	protected override void ReEvaluateFiringDirection() {

	}

	public override void ResetSimulation() {

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalBallistics {

    public float
        twistRate,
        vel,
        recoilImpulse;


    /// <summary>
    /// Gets the recoil velocity.
    /// </summary>
    /// <returns>The recoil velocity.</returns>
    /// <param name="projectileMass">Projectile mass in kg.</param>
    /// <param name="propellantMass">Propellant mass.</param>
    /// <param name="launcherMass">Launcher(gun) mass in kg.</param>
    /// <param name="muzzleVelocity">Muzzle velocity in m/s.</param>
    public static float GetRecoilVelocity(
        float projectileMass,
        float propellantMass,
        float launcherMass,
        float muzzleVelocity
    ) {
        return
            Mathf.Sqrt(
            ((projectileMass + propellantMass)
            * muzzleVelocity * muzzleVelocity)
            / launcherMass
        );
    }


    public static float CalculateMuzzleVelocity(
        ProjectileData projectile,
        float propellantMass,
        ExplosiveReaction propellantReaction,
        float barrelLength,
        float staticFrictionalCoefficient,
        float dynamicFrictionalCoefficient,
        float simulationTickDelta = 0.00001f,
        int maxSimulationSamples = 1000
    ) {
        float
            projectileVelocity = 0.0f,
            projectileDistance = 0.0f,
            pressure = 0.0f,
            elapsedTime = 0.0f,
            barrelVolume = Mathf.PI * projectile.crossSectionalArea;
        //V=M/D
        int numberOfGranules = (int)(
            (propellantMass * propellantReaction.density)//Propellant volume
            / propellantReaction.boundingBoxVolume); //Volume per granule
                                                     //How many granules of propellant do we have?


        for (int i = 0; i < maxSimulationSamples; i++) {
            //Build up pressure from the propellant
            pressure +=
                (propellantReaction.GetReleasedGasTimeSegment
                (elapsedTime, simulationTickDelta)
                * numberOfGranules)//Volume of released gas
                ;


            if (projectileVelocity == 0.0f) {//Use static friction

            }
            else {//Use dynamic friction

            }

            projectileDistance += projectileVelocity * simulationTickDelta;
            elapsedTime += simulationTickDelta;
        }

        //{TODO} Continue!


        return 0.0f;
    }
}
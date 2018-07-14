using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Stuff about the world that would affect the projectile

[CreateAssetMenu(fileName = "EnvironmentSetup", menuName = "AProjectiles/EnvironmentData")]
public class ProjectileEnvironmentData : ScriptableObject
{
    #region Air Density

    public bool useAdvancedAirDensity = false;
    public float airDensity = 1;

    //https://en.wikipedia.org/wiki/Troposphere
    //	public float troposphereDepthEquator = 20000;
    //	public float troposphereDepthMidLatitude = 17000;
    //	public float troposphereDepthPoles = 7000;

    public float GetAirDensity(float altitude)
    {
        return
            useAdvancedAirDensity ?
            ProjectileAffectors.AltitudeAirDensity(
                altitude,
                GetGravity(0).magnitude,
                ProjectileAffectors.AltitudeAirTemperature(altitude)
            )
            :
            airDensity;
    }

    #endregion

    public Vector3 windVelocity = Vector3.zero;

    #region Gravity

    public bool useAdvancedGravity = false;

    public Vector3 gravity = new Vector3(0, 0, 0);

    public Vector3 GetGravity(float altitude)
    {
        //Gravitational acceleration
        return useAdvancedGravity
            ?
        new Vector3(0,
            -(gravitationalConstant * planetaryMass) /
            (((planetaryDiameter / 2) + altitude) * ((planetaryDiameter / 2) + altitude))
                , 0)
                :
            gravity;
    }

    #endregion

    public float latitude = 45.0f;

    public Vector3 upVector = Vector2.up;

    //Earth is 6,371,000m across
    [Tooltip("In metres")]
    public float planetaryDiameter = 6371000f;

    [Tooltip("In seconds")]
    public float planetaryLengthOfDay = 86400.0f;

    [Tooltip("In kg")]
    public float planetaryMass = 5.972E24f;

    const float gravitationalConstant = 6.67408E-11f;
}

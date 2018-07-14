using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu (fileName = "Detonation", menuName = "AProjectiles/Reactions/Detonation")]
[System.Serializable]
public class ExplosiveReaction //: ScriptableObject
{
    public GasReactionMoles[] reactionResults;

    [Tooltip("In m/s")]
    public float burnRate;

    public GrainShape grainShape = GrainShape.Cylindrical;
    //If any
    [Tooltip("In mm")]
    float radius;
    //If Disc or Cylindrical
    [Tooltip("In mm")]
    float height;

    /// <summary>
    /// In kg/m^3
    /// </summary>
    public float density = 0.0f;

    /// <summary>
    /// A single granule's bounding box
    /// </summary>
    public float boundingBoxVolume
    {
        get
        {
            if ((grainShape == GrainShape.Disc) ||
                (grainShape == GrainShape.Cylindrical))
                return 2 * radius * height;
            return 4 * radius * radius;//We are a sphere
        }
    }

    public float GetBurnTimeOfGrain()
    {
        //Volume of a single grain in m^3
        float volume;

        switch (grainShape)
        {
            case GrainShape.Cylindrical:
                volume = Mathf.PI * radius * radius * height;
                break;
            case GrainShape.Disc://Volume of ellispoid/sphereoid
                volume = (4f / 3f) * Mathf.PI * radius * radius * height;
                break;
            case GrainShape.Spherical:
                volume = (4f / 3f) * Mathf.PI * radius * radius * radius;
                break;

        }

        //{TODO}Equations have been found, now implement them!
        return 0.0f;
    }

    /// <summary>
    /// Gets the amount of gas returned by a granule over a period of time,
    /// from a specific point in time.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public float GetReleasedGasTimeSegment(float time, float length)
    {

        //{TODO} Finish using already found equations
        return 0.0f;
    }

}

public enum GrainShape
{
    Disc,
    Cylindrical,
    Spherical
}

[System.Serializable]
public class GasReactionMoles
{
    public ReactionGas gas;
    public float molesCreated;
}

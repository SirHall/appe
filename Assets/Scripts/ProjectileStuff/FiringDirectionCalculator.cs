using Excessives;
using Excessives.LinqE;
using Excessives.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

// Is used to calculate what direction to fire a projectile at to hit a target
public class FiringDirectionCalculator : MonoBehaviour
{
    //{TODO} Multi-thread this!

    //public List<Thread> calculationThreads = new List<Thread>();

    //public List<TrajectoryCalculationData> trajectoryCalculations = new List<TrajectoryCalculationData>();



    //Temp
    public ProjectileData projData;

    public AffectedProjectile affectedProj;

    public TargetInformation targetInfo = new TargetInformation();

    public TrajectoryCalculationData trajCalcor = new TrajectoryCalculationData();


    #region Settings
    public float spinTwist = 1.0f;


    public Vector3
        targetPos = new Vector3(0, 0, 1000),
        targetVel = Vector3.zero,
        targetAccel = Vector3.zero;

    public float launchVelocity = 250.0f;

    #endregion

    private void Awake()
    {
        //Setup affectedProj
        affectedProj =
        new AffectedProjectile(
        Vector3.zero,
        Vector3.forward,
        projData,
        spinTwist
        );

        //Setup targetInfo
        targetInfo.position = targetPos;
        targetInfo.velocity = targetVel;
        targetInfo.acceleration = targetAccel;


        //Setup trajCalcor
        trajCalcor.launcherPosition = Vector3.zero;
        trajCalcor.launchVelocity = launchVelocity;
        trajCalcor.MaxDistanceThreshold = 0.01f;
        trajCalcor.originalPData = affectedProj;
        trajCalcor.originalTarget = targetInfo;
        trajCalcor.ResetSimulation();

    }

    private void Update()
    {
        Tick();
    }

    void Tick()
    {
        trajCalcor.Tick();

    }

}

public class TrajectoryCalculationThread
{
    Thread calculationThread;

    List<TrajectoryCalculationData> calcDataSet = new List<TrajectoryCalculationData>();

    //{TODO} Rewrite this to make it dynamic for each calcData element
    float deltaTime = 0.0f;

    public TrajectoryCalculationThread()
    { }

    public void CalculateAllTrajectories()
    {
        while (calcDataSet.Count > 0)
        {
            //Do all calculations together
            //calcDataSet.ForEach(n => n.Tick());
            //calcDataSet
            //    .Where(n => n.isWithinMaxDistance)
            //    .ForEach(n => n.FireWithinThreshold(n.currentFireDirection));

            //Do the calculations one at a time
            calcDataSet[0].Tick();
            if (calcDataSet[0].IsWithinMaxDistance)
            {
                calcDataSet[0].FireWithinThreshold(calcDataSet[0].currentFireDirection);
                calcDataSet.RemoveAt(0);
            }

        }
    }

}
[System.Serializable]
public class TrajectoryCalculationData
{
    //
    bool debug = true;

    public float launchVelocity = 250;

    public Vector3 currentFireDirection = Vector3.forward;

    public Vector3 launcherPosition { get; set; }

    public void FireWithinThreshold(Vector3 direction)
    {
        if (OnWithinThreshold != null)
            OnWithinThreshold.Invoke(direction);
    }

    public delegate void TrajCalcDelegate(Vector3 dir);

    public event TrajCalcDelegate OnWithinThreshold;

    public void Tick()
    {
        //pData.velocity.Log();
        //currentDeltaTime.Log();
        //Change the deltatime depending on how close to the target we are
        Tick(currentDeltaTime);
    }

    void Tick(float deltaTime)
    {
        //"isTicking".Log();
        pData.Tick(deltaTime);
        target.Tick(deltaTime);
        if (Vector3.Distance(pData.position, target.position) > prevDistance) //If we've gone passed the target
        {
            ReEvaluateFiringDirection();
            prevDistance = float.MaxValue;
        }
        else
        {
            prevDistance = Vector3.Distance(pData.position, target.position);
        }
        //Debug.Log(Vector3.Distance(pData.position, target.position));
    }

    public bool IsWithinMaxDistance
    {
        get { return Vector3.Distance(pData.position, target.position) < MaxDistanceThreshold; }
    }

    public float MaxDistanceThreshold { get; set; }

    public int CollisionMask { get; set; }

    #region Simulation and Re-testing new angles


    //{TODO} Add in option to change this to 'iterations per distance', would probably be a lot more stable

    public bool useTimePerIterationMethod = false;

    //If using the iteration per distance system
    public float
        outOfRangeTicksPerMetre = 1,
        maxTicksPerMetre = 100,
        minTicksPerMetre = 1,
        increaseTicksPerMetreDistanceThreshold = 100f;

    //If using time per iteration system
    public float
        maxDeltaTime = 0.1f,
        minDeltaTime = 0.0001f,
        deltaTimeDistanceMultiplier = 0.0001f; //Re-assign these values


    //Figure this out for the iteration per distance system
    float currentDeltaTime
    {
        get
        {
            if (useTimePerIterationMethod)
            {

                return
                    Mathf.Clamp(
                          Vector3.Distance(pData.position, target.position)
                          * deltaTimeDistanceMultiplier,
                          minDeltaTime, maxDeltaTime
                    );
            }

            float mult = Vector3.Distance(pData.position, target.position) /
               increaseTicksPerMetreDistanceThreshold;


            float safeVel = pData.velocity.magnitude;
            if (safeVel < 1) safeVel = 1;

            if (mult > 1)//We are out of range
                return 1.0f / (outOfRangeTicksPerMetre * safeVel);

            safeVel.Log();

            return
                1.0f /
                (
                        Mathf.Lerp(
                            maxTicksPerMetre, minTicksPerMetre,
                            mult
                        ) *
                        safeVel
                );
        }
    }

    float prevDistance = float.MaxValue; //Would be fairly hard to get a distance larger than this

    float errorAdjustmentSlope = 0.5f; //Will adjust by a multiplier of the error



    void ReEvaluateFiringDirection()
    {
        //"Re-evaluating firing direction".Log();

        //Find the projectile's position relative to the target

        Vector3 relativePos = target.position - pData.position; //There's something wrong here, what is it?
        Vector3 adjustment =
            (relativePos * errorAdjustmentSlope) /
            //The further away the target is from the launcher, the less we adjust
            Vector3.Distance(launcherPosition, target.position);

        currentFireDirection += adjustment;
        currentFireDirection = currentFireDirection.normalized;

        ("relativePos: " + relativePos.ToString()).Log();
        ("relativeDist: " + relativePos.magnitude.ToString()).Log();
        //"=====".Log();

        //Reset the simulation
        ResetSimulation();
        pData.position = launcherPosition;
        pData.prevPosition = launcherPosition;
        pData.velocity = launchVelocity * currentFireDirection;
        pData.prevVelocity = Vector3.zero;

    }

    #endregion

    public void ResetSimulation()
    {
        pData = (AffectedProjectile)originalPData.Clone();
        target = (TargetInformation)originalTarget.Clone();
    }

    #region ProjectileData

    AffectedProjectile pData { get; set; }

    public AffectedProjectile originalPData { get; set; }

    #endregion

    #region Target Data

    TargetInformation target { get; set; }

    public TargetInformation originalTarget { get; set; }

    #endregion
}

public class TargetInformation : ICloneable
{
    public Vector3 position { get; set; }
    public Vector3 velocity { get; set; }
    public Vector3 acceleration { get; set; }

    public object Clone()
    {
        return (TargetInformation)MemberwiseClone();
    }

    public void Tick(float deltaTime)
    {
        //"target is ticking".Log();

        velocity += acceleration * deltaTime;
        position += velocity * deltaTime;



        //acceleration.Log();
        //velocity.Log();
        //position.Log();
        //deltaTime.Log();

        DrawingFuncs.DrawStar(position, Color.red, 5);
    }
}

//{TODO} Move this elsewhere
public static class DrawingFuncs
{
    public static void DrawStar(Vector3 position, Color color, float starRadius = 1.0f, float time = 0.0f)
    {
        Debug.DrawLine(position - new Vector3(starRadius, 0, 0), position + new Vector3(starRadius, 0, 0), color, time);
        Debug.DrawLine(position - new Vector3(0, starRadius, 0), position + new Vector3(0, starRadius, 0), color, time);
        Debug.DrawLine(position - new Vector3(0, 0, starRadius), position + new Vector3(0, 0, starRadius), color, time);
    }
}
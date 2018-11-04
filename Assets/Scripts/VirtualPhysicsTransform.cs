using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class VirtualPhysicsTransform : ICloneable
{

    public VirtualPhysicsTransform()
    { }

    public VirtualPhysicsTransform(Vector3 initPos, Vector3 initVel, Quaternion initRot, Quaternion initRotVel)
    {
        Initialise(initPos, initVel, initRot, initRotVel);
    }

    public void Initialise(Vector3 initPos, Vector3 initVel, Quaternion initRot, Quaternion initRotVel)
    {
        pos = initPos;
        prevPos = initPos;

        vel = initVel;
        prevVel = initVel;

        rot = initRot;
        prevRot = initRot;

        rotVel = initRotVel;
        prevRotVel = initRotVel;
    }

    //{TODO} Complete
    public void Initialise(VirtualPhysicsTransform fromData)
    {

    }

    #region Private Vars

    [SerializeField]
    Vector3
        pos = Vector3.zero,
        vel = Vector3.zero,
        prevPos = Vector3.zero,
        prevVel = Vector3.zero;

    [SerializeField]
    Quaternion
        rot = Quaternion.identity,
        rotVel = Quaternion.identity,
        prevRot = Quaternion.identity,
        prevRotVel = Quaternion.identity;

    [SerializeField]
    float
        mass = 1.0f;

    #endregion

    #region Properties

    public Vector3 Position { get { return pos; } set { pos = value; } }
    public Vector3 Velocity { get { return vel; } set { vel = value; } }

    public Quaternion Rotation { get { return rot; } set { rot = value; } }
    public Quaternion RotationalVelocity { get { return rotVel; } set { rotVel = value; } }

    public float Mass { get { return mass; } set { mass = value; } }

    public Vector3 EulerRotation { get { return rot.eulerAngles; } set { rot.eulerAngles = value; } }
    public Vector3 EulerRotationalVelocity { get { return rotVel.eulerAngles; } set { rotVel.eulerAngles = value; } }

    public Vector3 Forward { get { return Rotation * Vector3.forward; } }
    public Vector3 Back { get { return Rotation * Vector3.back; } }
    public Vector3 Up { get { return Rotation * Vector3.up; } }
    public Vector3 Down { get { return Rotation * Vector3.down; } }
    public Vector3 Right { get { return Rotation * Vector3.right; } }
    public Vector3 Left { get { return Rotation * Vector3.left; } }

    public Vector3 PrevPosition => prevPos;
    public Vector3 PrevVelocity => prevVel;
    public Quaternion PrevRotation => prevRot;
    public Quaternion PrevRotationalVelocity => prevRotVel;

    public float SpinVelocity
    {
        get
        {
            float spinVel = 0.0f;
            Vector3 spinAxis = Vector3.zero;
            RotationalVelocity.ToAngleAxis(out spinVel, out spinAxis);
            return spinVel;
        }
        set
        {
            float spinVel = 0.0f;
            Vector3 spinAxis = Vector3.zero;
            RotationalVelocity.ToAngleAxis(out spinVel, out spinAxis);
            RotationalVelocity = Quaternion.AngleAxis(value, spinAxis);
        }
    }

    public Vector3 SpinAxis
    {
        get
        {
            float spinVel = 0.0f;
            Vector3 spinAxis = Vector3.zero;
            RotationalVelocity.ToAngleAxis(out spinVel, out spinAxis);
            return spinAxis;
        }
        set
        {
            float spinVel = 0.0f;
            Vector3 spinAxis = Vector3.zero;
            RotationalVelocity.ToAngleAxis(out spinVel, out spinAxis);
            RotationalVelocity = Quaternion.AngleAxis(spinVel, value);
        }
    }

    public float VelocityMagnitude
    {
        get { return vel.magnitude; }
        set { vel = vel.normalized * value; }
    }

    public Vector3 VelocityDirection
    {
        get { return Velocity.normalized; }
        set { vel = value * vel.magnitude; }
    }

    #endregion

    #region Public Interface

    public virtual void Tick(float deltaTime)
    {

        prevPos = pos;
        prevVel = vel;
        prevRot = rot;
        prevRotVel = rotVel;

        pos += vel * deltaTime;
        rot *= Quaternion.LerpUnclamped(Quaternion.identity, RotationalVelocity, deltaTime);

    }

    #region Transform

    public void LookAt(Vector3 pos) => LookAt(pos, Vector3.up);

    public void LookAt(Vector3 pos, Vector3 up) => Rotation = Quaternion.LookRotation(pos - Position, up);

    public void RotateAround(Vector3 axis, float degrees) => rot *= Quaternion.AngleAxis(degrees, axis);

    public void Translate(Vector3 dist) => Position += dist;

    #endregion

    #region Physics

    public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force, float deltaTime = float.NaN)
    {
        if (float.IsNaN(deltaTime))
            deltaTime = Time.deltaTime;

        //Debug.Log($"deltaTime: {deltaTime}, force: {force}");
        switch (mode)
        {
            case ForceMode.Acceleration:
                vel += force * deltaTime;
                return;
            case ForceMode.Force:
                vel += (force / mass) * deltaTime;
                return;
            case ForceMode.Impulse:
                vel += force / mass;
                return;
            case ForceMode.VelocityChange:
                vel += force;
                return;
        }
    }

    #endregion

    #endregion

    #region Debug

    public void RenderDebug(float deltaTime = float.NaN, float duration = 0.0f, float afterDrawDuration = 0.0f)
    {
        deltaTime = float.IsNaN(deltaTime) ? Time.deltaTime : deltaTime;

        Debug.DrawRay(Position, Velocity, new Color(0.5f, 0.5f, 0.5f, 1.0f), duration); //Velocity
        Debug.DrawRay(Position, (Velocity - PrevVelocity) / deltaTime, Color.yellow, duration); //Acceleration
        Debug.DrawLine(PrevPosition, Position, Color.white, afterDrawDuration); //Delta Position

        DrawingFuncs.DrawStar(PrevPosition, Color.magenta, 0.5f, afterDrawDuration); //Iteration positions

        //Directions
        Debug.DrawRay(Position, Right, Color.red, duration); //X-Direction
        Debug.DrawRay(Position, Up, Color.green, duration); //Y-Direction
        Debug.DrawRay(Position, Forward, Color.blue, duration); //Z-Direction
    }

    #endregion

    #region ICloneable

    public virtual object Clone()
    {
        return (VirtualPhysicsTransform)MemberwiseClone();
    }

    #endregion

}

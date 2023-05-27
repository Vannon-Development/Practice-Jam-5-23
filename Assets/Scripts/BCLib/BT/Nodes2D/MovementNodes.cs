using System.Collections.Generic;
using BCLib.BT.NodesInput;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace BCLib.BT.Nodes2D
{
    public interface IMotion : IInputContext
    {
        public float Velocity { get; }
        public Vector2 FrameVelocity { set; }
    }

    public interface IAcceleration : IMotion
    {
        public float Acceleration { get; }
    }

    public interface IRotation : ITreeContext
    {
        public float TargetRotation { get; }
        public float CurrentRotation { get; }
        public float RotationSpeed { get; }
        public float FrameRotation { set; }
    }

    public class Motion<T> : LeafNode<T> where T: class, IMotion
    {
        private readonly string _name;
        
        public Motion(T context, string inputName) : base(context)
        {
            _name = inputName;
        }
        public Motion(T context, IReadOnlyList<string> param) : this(context, param[0]) {}

        public override Status Tick()
        {
            Context.FrameVelocity = Context.Velocity * Context.GetAxis(_name);
            return Status.Success;
        }
    }

    public class Acceleration<T> : LeafNode<T> where T : class, IAcceleration
    {
        private readonly string _name;

        public Acceleration(T context, string inputName) : base(context)
        {
            _name = inputName;
        }
        public Acceleration(T context, IReadOnlyList<string> param) : this(context, param[0]) {}

        public override Status Tick()
        {
            var vel = Context.Velocity + Context.Acceleration / Time.fixedDeltaTime;
            Context.FrameVelocity = vel * Context.GetAxis(_name);
            return Status.Success;
        }
    }

    public class RotateAtTarget<T> : LeafNode<T> where T : class, IRotation
    {
        public RotateAtTarget(T context) : base(context) { }

        public override Status Tick()
        {
            return (Context.CurrentRotation - Context.TargetRotation).NormalizeAngle().NearZero()
                ? Status.Success
                : Status.Failure;
        }
    }

    public class RotateToTarget<T> : LeafNode<T> where T : class, IRotation
    {
        public RotateToTarget(T context) : base(context){}

        public override Status Tick()
        {
            var diff = (Context.TargetRotation - Context.CurrentRotation).NormalizeAngle();
            var frameMax = Context.RotationSpeed * Time.fixedDeltaTime;
            Context.FrameRotation = Mathf.Min(Mathf.Abs(diff), frameMax) * Mathf.Sign(diff) / Time.fixedDeltaTime;
            return Status.Success;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace BCLib.BT.NodesInput
{
    public interface IInputContext : ITreeContext
    {
        public Vector2 GetAxis(string name);
        public bool GetButton(string name);
    }

    public class StickActive<T> : LeafNode<T> where T : class, IInputContext
    {
        private readonly string _name;

        public StickActive(T context, string name) : base(context)
        {
            _name = name;
        }
        public StickActive(T context, IReadOnlyList<string> param) : this(context, param[0]){}

        public override Status Tick()
        {
            var input = Context.GetAxis(_name);
            return input.magnitude.NearZero() ? Status.Failure : Status.Success;
        }
    }

    public class StickXActive<T> : LeafNode<T> where T : class, IInputContext
    {
        private readonly string _name;

        public StickXActive(T context, string name) : base(context)
        {
            _name = name;
        }
        public StickXActive(T context, IReadOnlyList<string> param) : this(context, param[0]){}

        public override Status Tick()
        {
            var input = Context.GetAxis(_name);
            return input.x.NearZero() ? Status.Failure : Status.Success;
        }
    }

    public class StickYActive<T> : LeafNode<T> where T : class, IInputContext
    {
        private readonly string _name;

        public StickYActive(T context, string name) : base(context)
        {
            _name = name;
        }
        public StickYActive(T context, IReadOnlyList<string> param) : this(context, param[0]){}

        public override Status Tick()
        {
            var input = Context.GetAxis(_name);
            return input.y.NearZero() ? Status.Failure : Status.Success;
        }
    }

    public class ButtonPressed<T> : LeafNode<T> where T : class, IInputContext
    {
        private readonly string _name;
        
        public ButtonPressed(T context, string name) : base(context)
        {
            _name = name;
        }
        public ButtonPressed(T context, IReadOnlyList<string> param) : this(context, param[0]){}

        public override Status Tick()
        {
            return Context.GetButton(_name) ? Status.Success : Status.Failure;
        }
    }
}

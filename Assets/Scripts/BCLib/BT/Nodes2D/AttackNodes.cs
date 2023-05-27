using System.Collections.Generic;
using BCLib.BT.NodesInput;
using UnityEngine;

namespace BCLib.BT.Nodes2D
{
    public interface IAttack : ITreeContext
    {
        public float AttackTime(string attackName);
        public void TriggerAttack(string attackName);
    }

    public class Attack<T> : LeafNode<T> where T : class, IAttack
    {
        private string _name;
        
        public Attack(T context, string attackName) : base(context)
        {
            _name = attackName;
        }

        public Attack(T context, IReadOnlyList<string> param) : this(context, param[0]){}

        public override Status Tick()
        {
            Context.TriggerAttack(_name);
            return Status.Success;
        }
    }

    public class AttackTimer<T> : LeafNode<T> where T : class, IAttack
    {
        private readonly string _name;
        private float _lastShot;
        
        public AttackTimer(T context, string attackName) : base(context)
        {
            _name = attackName;
        }
        public AttackTimer(T context, IReadOnlyList<string> param) : this(context, param[0]){}

        public override Status Tick()
        {
            if(Time.fixedTime - _lastShot >= Context.AttackTime(_name))
            {
                _lastShot = Time.fixedTime;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}

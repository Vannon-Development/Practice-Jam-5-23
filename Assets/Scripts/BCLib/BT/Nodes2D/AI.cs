using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BCLib.BT.Nodes2D
{
    public interface IAITarget
    {
        public string Group { get; }
    }
    
    public interface ITargetingAI : ITreeContext
    {
        public string[] FilterGroups { get; }
        public List<GameObject> AllTargets { get; }
        public GameObject CurrentTarget { get; set; }
    }

    public class FindAllTargets<T> : LeafNode<T> where T : class, ITargetingAI
    {
        public FindAllTargets(T context) : base(context) { }

        public override Status Tick()
        {
            var objs = GameObject.FindGameObjectsWithTag("Target");
            Context.AllTargets.Clear();
            foreach (var obj in objs)
            {
                var target = obj.gameObject.GetComponent<IAITarget>();
                if (target != null && obj.gameObject && !Context.FilterGroups.Contains(target.Group))
                    Context.AllTargets.Add(obj.gameObject);
            }

            return Context.AllTargets.Count == 0 ? Status.Failure : Status.Success;
        }
    }

    public class HasTarget<T> : LeafNode<T> where T: class, ITargetingAI
    {
        public HasTarget(T context) : base(context) { }

        public override Status Tick()
        {
            return Context.CurrentTarget ? Status.Success : Status.Failure;
        }
    }

    public class SelectNearestTarget<T> : LeafNode<T> where T: class, ITargetingAI
    {
        public SelectNearestTarget(T context) : base(context) { }

        public override Status Tick()
        {
            float dist = float.MaxValue;
            foreach (var target in Context.AllTargets)
            {
                float d = (Context.ParentObject.transform.position - target.transform.position).magnitude;
                if (d < dist)
                {
                    dist = d;
                    Context.CurrentTarget = target;
                }
            }

            return Context.CurrentTarget == null ? Status.Failure : Status.Success;
        }
    }
}
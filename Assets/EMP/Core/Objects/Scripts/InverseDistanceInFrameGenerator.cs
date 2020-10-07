using EMP.Core;
using UnityEngine;

namespace EMP.Core
{
    [RequireComponent(typeof(MonopoleStrength))]
    public class InverseDistanceInFrameGenerator : FieldGenerator
    {
        public MonopoleStrength monopoleStrength;
        public Rigidbody rigidbody;
        public VelocityEstimator estimator;
    
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            rigidbody = GetComponent<Rigidbody>();
            if (!monopoleStrength)
            {
                monopoleStrength = GetComponent<MonopoleStrength>();
            }
        }

        public void OnValidate()
        {
            monopoleStrength = GetComponent<MonopoleStrength>();
            estimator = GetComponent<VelocityEstimator>();
        }

        public override Vector3 FieldValue(Vector3 fieldPoint)
        {
            return Vector3.zero;
        }
        
        public Vector3 FieldValue(Vector3 fieldPoint, Vector3 frameVelocity)
        {
            //Vector3 relativeVelocity = rigidbody.velocity - frameVelocity;
            Vector3 relativeVelocity = estimator.Velocity - frameVelocity;
            Vector3 field = monopoleStrength.Strength * relativeVelocity; // / Vector3.Distance(Position, fieldPoint);
            //field -= monopoleStrength.Strength / Mathf.Pow(Vector3.Distance(Position, fieldPoint), 2) *
            //         (fieldPoint - Position);
            return field;
        }
    }
}

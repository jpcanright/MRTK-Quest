using EMP.Core;
using UnityEngine;

namespace EMP.Core
{
    [RequireComponent(typeof(MonopoleStrength))]
    public class PointChargeGenerator : FieldGenerator
    {
        public MonopoleStrength monopoleStrength;
    
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (!monopoleStrength)
            {
                monopoleStrength = GetComponent<MonopoleStrength>();
            }
        }

        public void OnValidate()
        {
            monopoleStrength = GetComponent<MonopoleStrength>();
        }

        public override Vector3 FieldValue(Vector3 fieldPoint)
        {
            Vector3 field = monopoleStrength.Strength / Mathf.Pow(Vector3.Distance(Position, fieldPoint), 3) *
                    (fieldPoint - Position);
            return field;
        }
    }
}

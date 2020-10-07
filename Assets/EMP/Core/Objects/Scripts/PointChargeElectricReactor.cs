using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;

public class PointChargeElectricReactor : FieldReactor
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
    
    public override Vector3 ForceUponReactor()
    {
        Vector3 force = monopoleStrength.Strength *
                        EMController.Instance.GetFieldValue(transform.position, fieldReactingTo, associatedGenerator);
        return force;
    }
}

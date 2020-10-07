using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;

public class ExampleReactor : FieldReactor
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Vector3 ForceUponReactor()
    {
        throw new System.NotImplementedException();
    }

}

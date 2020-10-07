using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;

namespace EMP.Visualization
{
    [CreateAssetMenu(fileName = "Field Line Type Settings", menuName = "ScriptableObjects/FieldLineTypeSettings", order = 1)]
    public class FieldLineTypeSettings : ScriptableObject
    {
        public InteractionField fieldType;
        public int fieldLinesPerCharge;
        public CPUFieldLineManager.FieldLineDrawSpace drawSpace;
        // TODO CPU vs GPU field line switch
        public FieldLineCalculationSettings calculationSettings;
        public FieldLineAppearanceSettings appearanceSettings;
    }
}
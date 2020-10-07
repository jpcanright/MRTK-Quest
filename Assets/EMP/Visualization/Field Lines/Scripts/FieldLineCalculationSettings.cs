using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMP.Visualization
{
    [CreateAssetMenu(fileName = "Field Line Calculation Settings", menuName = "ScriptableObjects/FieldLineCalculationSettings", order = 1)]
    public class FieldLineCalculationSettings : ScriptableObject
    {
        public float segmentLength = 0.03f;
        public int maxSegments = 500;
        public CPUFieldLineModel.Accuracy accuracy = CPUFieldLineModel.Accuracy.SecondOrderMethod;
        
        // Termination conditions & tolerances; set to -1 to disable
        public float backTolerance = 0.75f; // Check if line is doubling back on itself
        public float stopTolerance = 0.95f; // Check if line has looped back on itself
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMP.Visualization
{
    [CreateAssetMenu(fileName = "Field Line Appearance Settings", menuName = "ScriptableObjects/FieldLineAppearanceSettings", order = 1)]
    public class FieldLineAppearanceSettings : ScriptableObject
    {
        public enum FieldLineThicknessMode
        {
            WorldSpace,
            ClipSpace
        }
        
        // TODO this does nothing yet
        public FieldLineThicknessMode thicknessMode = FieldLineThicknessMode.WorldSpace;
        public float fieldLineThickness = 0.03f;
        // TODO add option to pull from VisualizationManager
        public Color fieldLineColor;
        public Color debugFromNegativeLineColor;
        public Color debugCulledLineColor;
        public Material fieldLineMaterial;
    }
}

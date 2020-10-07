using System;
using System.Collections;
using System.Collections.Generic;
using EMP.Visualization;
using UnityEngine;

namespace EMP.Visualization
{
    public class CPUFieldLineView : MonoBehaviour
    {
        // Fields to be set by model creating this view
        public FieldLineAppearanceSettings appearanceSettings;
        //public int maxNumberOfSegments = 1000; // number of line segments
        public FieldLineDebugTag debugTag = FieldLineDebugTag.FromPositive;
        
        private LineRenderer line;
        private bool rendererSetUp = false;

        private float Thickness => appearanceSettings.fieldLineThickness;
        private Color LineColor => appearanceSettings.fieldLineColor;
        private FieldLineAppearanceSettings.FieldLineThicknessMode ThicknessMode => appearanceSettings.thicknessMode;
        private Material LineMaterial => appearanceSettings.fieldLineMaterial;

        public void Initialize(FieldLineAppearanceSettings settings)
        {
            appearanceSettings = settings;
        }
        
        // This should be called by the model associated with this view.
        public void DrawFieldLine(Vector3[] positions)
        {
            if (!rendererSetUp)
            {
                SetUpLineRenderer();
                rendererSetUp = true;
            }

            if (debugTag == FieldLineDebugTag.Culled)
            {
                line.startColor = appearanceSettings.debugCulledLineColor;
                line.endColor = appearanceSettings.debugCulledLineColor;
            }
            else if (debugTag == FieldLineDebugTag.FromNegative)
            {
                line.startColor = appearanceSettings.debugFromNegativeLineColor;
                line.endColor = appearanceSettings.debugFromNegativeLineColor;
            }

            line.positionCount = positions.Length;
            line.SetPositions(positions);
        }

        private void SetUpLineRenderer()
        {
            Debug.Log("FieldLine LineRenderer setting up.");
            if (!GetComponent<LineRenderer>())
            {
                gameObject.AddComponent<LineRenderer>();
            }

            line = GetComponent<LineRenderer>();
            
            // NOTE: I vaguely remember that there was a dumb reason why we specified a maximum number of segments
            // while initializing the LineRenderer. I do not know what that reason was or if it for sure existed.
            //line.positionCount = maxNumberOfSegments;
            line.material = LineMaterial;
            line.startColor = LineColor;
            line.endColor = LineColor;
            line.startWidth = Thickness;
            line.endWidth = Thickness;
        }

        public void OnDestroy()
        {
            Destroy(line);
        }
    }
}
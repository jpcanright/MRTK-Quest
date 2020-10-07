using System;
using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;

namespace EMP.Visualization
{
    public class EMPVisualizationScalingManager : Singleton<EMPVisualizationScalingManager>
    {
    
        // These should all be field-agnostic
        public float emVectorMinimumWidth = 0.01f;
        public float emVectorMaximumWidth = 0.05f;
        public float emVectorMinimumLength = 0.01f;
        public float emVectorMaximumLength = 0.5f;
        public VectorVisualizer.VectorScaling scalingMode;
        
        // TODO make this class handle fields as they become registered in EMController
        //public InteractionField field;

        // Field-specific
        public Dictionary<InteractionField, float> scaleFactors = new Dictionary<InteractionField, float>();
        
        // Field-specific
        //public float electricFieldScaleFactor = 1;
        //public float magneticFieldScaleFactor = 1;
        
        // Source-specific
        public float monopoleSampleRadius = 0.3f;
    
        
        public delegate void VisualizerScalingUpdate();
        public event VisualizerScalingUpdate OnVisualizerScalingUpdate;
        
        /// <summary>
        /// Recalculates all EM visualization parameters based on current EM objects in scene.
        /// </summary>
        public void Rescale()
        {
            foreach (InteractionField field in scaleFactors.Keys)
            {
                float fieldUpperLimit = GetFieldMagnitudeUpperLimit(field);
                scaleFactors[field] = emVectorMaximumLength / fieldUpperLimit;
            }
            
            OnVisualizerScalingUpdate?.Invoke();
        }
    
        private float GetFieldMagnitudeUpperLimit(InteractionField field)
        {
            // TODO make this class work with non-monopole sources
            var generators = EMController.Instance.GetFieldGenerators<PointChargeGenerator>(field);
            if (generators.Count == 0)
            {
                return 0;
            }
            float maxStrength = 0;
            int index = 0;
            
            for(int i=0; i<generators.Count; i++)
            {
                float charge = generators[i].monopoleStrength.Strength;
                if (Mathf.Abs(charge) > maxStrength)
                {
                    maxStrength = Mathf.Abs(charge);
                    index = i;
                }
            }
            Vector3 pos = generators[index].Position;
            Vector3 samplePos = pos + Vector3.up * monopoleSampleRadius;
            return EMController.Instance.GetFieldValue(samplePos, field).magnitude;
        }

        void RegisterInteractionField(InteractionField field)
        {
            if (!scaleFactors.ContainsKey(field))
            {
                scaleFactors.Add(field, 1);
            }
        }

        private void OnEnable()
        {
            EMController.Instance.OnInteractionFieldRegistered += RegisterInteractionField;
        }
        
        private void OnDisable()
        {
            EMController.Instance.OnInteractionFieldRegistered -= RegisterInteractionField;
        }
        
        void Update()
        {
            Rescale();
        }
    }
}


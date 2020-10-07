using EMP.Core;
using UnityEngine;

namespace EMP.Visualization
{
    public class EMPFieldSensor : VectorVisualizer
    {
        [SerializeField]
        public InteractionField field;

        // Start is called before the first frame update
        new void Awake()
        {
            base.Awake();

            arrowColor = field.visualColor;
            quantityFunc = GetFieldValue;

            SetArrowColor();
        
            GetScalingParameters();
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
        }

        private Vector3 GetFieldValue(Vector3 position)
        {
            return EMController.Instance.GetFieldValue(position, field);
        }

        private void GetScalingParameters()
        {
            minimumArrowWidth = EMPVisualizationScalingManager.Instance.emVectorMinimumWidth;
            maximumArrowWidth = EMPVisualizationScalingManager.Instance.emVectorMaximumWidth;
            scaling = EMPVisualizationScalingManager.Instance.scalingMode;
            scaleFactor = EMPVisualizationScalingManager.Instance.scaleFactors[field];

            maxMagnitude = EMPVisualizationScalingManager.Instance.emVectorMaximumLength;
        }
    
        private void OnEnable()
        {
            EMPVisualizationScalingManager.Instance.OnVisualizerScalingUpdate += GetScalingParameters;
        }

        private void OnDisable()
        {
            EMPVisualizationScalingManager.Instance.OnVisualizerScalingUpdate -= GetScalingParameters;
        }
    }
}

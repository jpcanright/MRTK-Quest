using System;
using System.Linq;
using UnityEngine;
using EMP.Core;

namespace EMP.Visualization
{
    public class CPUFieldLineModel : MonoBehaviour
    {
        private Func<Vector3,InteractionField,FieldGenerator,Vector3> field;
        private Func<Vector3,Vector3> calculate;
        
        // TODO specify with manager
        public InteractionField fieldType;
        public FieldLineCalculationSettings settings;
        public FieldLineAppearanceSettings appearanceSettings;
        
        [SerializeField] private CPUFieldLineView view;

        private Vector3[] _positions;
        
        public Vector3 FirstPoint
        {
            get { return _positions[0]; }
        }

        public Vector3 FirstDirection
        {
            get { return Vector3.Normalize(_positions[1] - _positions[0]); }
        }

        public Vector3 LastPoint
        {
            get { return _positions[_positions.Length-1]; }
        }

        public Vector3 LastDirection
        {
            get { return Vector3.Normalize(_positions[_positions.Length - 2] - _positions[_positions.Length-1]); }
        }
        
        private float SegmentLength => settings.segmentLength;   // length of each line segment
        private int MaxSegments => settings.maxSegments;
        private float BackTolerance => settings.backTolerance;
        private float StopTolerance => settings.stopTolerance;

        public enum Accuracy {
            EulersMethod,
            SecondOrderMethod,
            RungeKuttaMethod
        };

        public Accuracy CalculationMode => settings.accuracy;

        public FieldLineDebugTag debugTag;


        public IntegrationDirection integrationDirection = IntegrationDirection.FromNegative;

        public enum TerminationReason
        {
            BackTolerance,
            LoopCompleted,
            WorkspaceBoundary,
            MaximumLength,
            None
        }

        public TerminationReason terminationReason = TerminationReason.None;
        
        public void Initialize(FieldLineCalculationSettings calculationSettings, 
                               InteractionField fieldType, 
                               FieldLineAppearanceSettings appearanceSettings,
                               IntegrationDirection integrationDirection)
        {
            settings = calculationSettings;
            this.fieldType = fieldType;
            this.integrationDirection = integrationDirection;
            this.appearanceSettings = appearanceSettings;
            SetUpCalculation();
        }

        public void SetPositionAndIntegrationDirection(Vector3 position, IntegrationDirection direction)
        {
            integrationDirection = direction;
            transform.position = position;
        }
        
        private void SetUpCalculation()
        {
            field = EMController.Instance.GetFieldValue;

            switch (CalculationMode)
            {
                case Accuracy.EulersMethod:
                    calculate = EulersMethod;
                    break;
                case Accuracy.SecondOrderMethod:
                    calculate = SecondOrderMethod;
                    break;
                case Accuracy.RungeKuttaMethod:
                    calculate = RungeKuttaMethod;
                    break;
            }
        }

        public void ExecuteCalculation()
        {
            // TODO CPUFieldLineModel public do-calculation command
            Vector3[] positions = new Vector3[MaxSegments];
            int positionIndex = 0;
            positions[positionIndex] = transform.position;
            positionIndex++;
        
//            if (integrationDirection = IntegrationDirection.DownField)
//            {
//                upFieldTermination = TerminationReason.MaximumLength;
//            }
//            else
//            {
//                downFieldTermination = TerminationReason.MaximumLength;
//            }

            int i = 1;
            for (; i < MaxSegments; i++)
            {
                Vector3 pos = positions[i-1];
                // TODO what is this supposed to catch?
                //try
                //{
                Vector3 step = calculate(pos);
                //}
                //catch { }

                if (i > 2)
                {
                    // Shorten step if the field line is curving significantly
                    Vector3 lastStepDirection = (positions[i - 1] - positions[i - 2]).normalized;

                    float stepModifier = 1;
                    
                    // Don't do anything if this would get caught by the BackTolerance check
                    if (!(Vector3.Dot(lastStepDirection, step) < BackTolerance))
                    {
                        //stepModifier = Mathf.Sin(Vector3.Dot(lastStepDirection, step) * Mathf.PI / 2);
                        stepModifier = DetermineStepLengthModifier(pos, step);
                    }
                    
                    step *= stepModifier;
//                    step *= (Vector3.Dot(step.normalized, lastStepDirection)+0.25f)/1.25f;
                }
//                else
//                {
//                    step *= 0.1f;
//                }
            
            
                if (integrationDirection == IntegrationDirection.FromPositive)
                {
                    pos -= SegmentLength * step;
                }
                else
                {
                    pos += SegmentLength * step;
                }

                terminationReason = ShouldTerminateFieldLine(positions, pos, i);
            
                if (terminationReason != TerminationReason.None)
                {
                    break;
                }

                positions[i] = pos;
            }
            Vector3[] truncatedPositions = new Vector3[i];
            truncatedPositions = positions.Take(i).ToArray();
            _positions = truncatedPositions;
            
            // Report where this field line has landed to the CPU field line manager
            CPUFieldLineManager.Instance.ReportFieldLineTermination(this, terminationReason);
        }

        private float DetermineStepLengthModifier(Vector3 lastPosition, Vector3 step)
        {
            float stepLengthModifier = 1;
            
            float fieldValueThreshold = 1f / (0.1f * 0.1f) - 1f / (0.2f * 0.2f);
            float angleChangeThreshold = 20f;

            Vector3 fieldAtLastPosition = GetFieldValue(lastPosition);
            Vector3 fieldAtNewPosition = GetFieldValue(lastPosition + step);

            if (angleChangeThreshold <= Vector3.Angle(fieldAtLastPosition, fieldAtNewPosition))
            {
                stepLengthModifier /= 2f;
                //fieldAtNewPosition = GetFieldValue(lastPosition + step * stepLengthModifier);
            }

            if (fieldValueThreshold <= (fieldAtLastPosition - fieldAtNewPosition).magnitude)
            {
                stepLengthModifier /= 2f;
                //fieldAtNewPosition = GetFieldValue(lastPosition + step * stepLengthModifier);
            }

            return stepLengthModifier;
        }
        
        private TerminationReason ShouldTerminateFieldLine(Vector3[] positions, Vector3 nextPosition, int i)
        {
        
            int numPositions = positions.Length;
        
            // Make sure there is more than one existing field line point
            if (i > 1)
            {
                Vector3 lastStep = (positions[i - 1] - positions[i - 2])/SegmentLength;
                Vector3 step = (nextPosition - positions[i - 1])/SegmentLength;
        
                // If the field line changes too abruptly (e.g. if it passes a point charge and so points backwards),
                // this terminates it.
                if (Vector3.Dot(lastStep, step) < BackTolerance)
                {
                    return TerminationReason.BackTolerance;
                }
            }
       
        
            // Check to see if line has looped back on itself
            if (StopTolerance > 0.0f)
            {
                for (int j = 0; j < numPositions; j++)
                {
                    if (Vector3.Distance(nextPosition, positions[j]) < StopTolerance * SegmentLength)
                    {
                        return TerminationReason.LoopCompleted;
                    }
                }
            }
        
            // If the field line is supposed to terminate at workspace boundaries, make that happen
//            if (terminateAtWorkspace && workspace)
//            {
//                // Nested if statement avoids null reference exception in case the workspace isn't set.
//                if (workspace.IsInsideWorkspace(nextPosition) != workspace.IsInsideWorkspace(positions[numPositions - 1]))
//                {
//                    return TerminationReason.WorkspaceBoundary;
//                }
//            
//            }
            return TerminationReason.None;
        }
        
        /// <summary>
        /// This creates a field line view object if one does not already exist, applies the current debug tag,
        /// and feeds the last-calculated list of positions to the view for immediate rendering. This does not
        /// cause any calculation of field lines; that must be called before this method on each frame for the field
        /// lines' positions to update.
        /// </summary>
        public void Draw()
        {
            ConfigureFieldLineView();
            view.debugTag = debugTag;
            view.DrawFieldLine(_positions);
        }
        
        /// <summary>
        /// If a <see cref="CPUFieldLineView"/> object does not already exist for this model, create & configure one.
        /// </summary>
        private void ConfigureFieldLineView()
        {
            if (!view)
            {
                if (!GetComponent<CPUFieldLineView>())
                {
                    view = gameObject.AddComponent<CPUFieldLineView>();
                    view.Initialize(appearanceSettings);
                }
                else
                {
                    view = GetComponent<CPUFieldLineView>();
                }
            }
        }

        Vector3 GetFieldValue(Vector3 position)
        {
            return field(position, fieldType, null);
        }
        
        Vector3 EulersMethod(Vector3 pos) {
            Vector3 k1 = GetFieldValue(pos);
//	    if (k1 == Vector3.zero) { throw new System.Exception(); }
            if (k1 == Vector3.zero)
            {
                return Vector3.zero;
            }
            return k1.normalized;
        }
	
        Vector3 SecondOrderMethod(Vector3 pos) {
            Vector3 k1 = GetFieldValue(pos);
            if (k1 == Vector3.zero) { throw new System.Exception(); }
            Vector3 k2 = GetFieldValue(pos + 0.5f * SegmentLength * k1.normalized);
            if (k2 == Vector3.zero) { throw new System.Exception(); }
            return k2.normalized;
        }
	
        Vector3 RungeKuttaMethod(Vector3 pos) {
            Vector3 k1 = field(pos, fieldType, null);
            if (k1 == Vector3.zero) { throw new System.Exception(); }
            Vector3 k2 = GetFieldValue(pos + 0.5f * SegmentLength * k1.normalized);
            if (k2 == Vector3.zero) { throw new System.Exception(); }
            Vector3 k3 = GetFieldValue(pos + 0.5f * SegmentLength * k2.normalized);
            if (k3 == Vector3.zero) { throw new System.Exception(); }
            Vector3 k4 = GetFieldValue(pos + SegmentLength * k3.normalized);
            if (k4 == Vector3.zero) { throw new System.Exception(); }
            return (k1/6.0f + k2/3.0f + k3/3.0f + k4/6.0f).normalized;
        }

        public void OnDestroy()
        {
            if (view)
            {
                Destroy(view);
            }
        }
    }

    public enum IntegrationDirection
    {
        FromPositive,
        FromNegative
    }

    public enum FieldLineDebugTag
    {
        FromPositive,
        FromNegative,
        Culled
    }
    
    
}

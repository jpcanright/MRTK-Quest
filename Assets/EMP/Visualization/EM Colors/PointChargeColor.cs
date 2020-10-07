using EMP.Core;
using UnityEngine;
using Visualization;

namespace EMP.Visualization
{
    /// <summary>
    /// Colors a point charge based on the sign and nature of its charge.
    /// </summary>
    [System.Serializable]
    [ExecuteInEditMode]
    public class PointChargeColor : MonoBehaviour
    {
        public VisColorSettings colorSettings;
        public MonopoleStrength monopoleStrength;
        public FieldGenerator generator;
        private MeshRenderer _meshRenderer;
    
        // Start is called before the first frame update
        void OnEnable()
        {
            if (!_meshRenderer)
            {
                _meshRenderer = GetComponent<MeshRenderer>();            
            }
        
            if (!monopoleStrength)
            {
                monopoleStrength = GetComponent<MonopoleStrength>();
            }

            if (!generator)
            {
                generator = GetComponent<FieldGenerator>();
            }
        
            UpdateMaterial();
        }

        void Update()
        {
            UpdateMaterial();
        }
    
        void UpdateMaterial()
        {
            _meshRenderer.material = GetMaterial();
        }
    
        /// <summary>
        /// Determines, retrieves, and returns the appropriate material for the current state of the monopole.
        /// </summary>
        /// <returns>Material to be applied to the monopole.</returns>
        public Material GetMaterial()
        {
            VisColorSettings.ColorMeaning meaning = VisColorSettings.ColorMeaning.Error;
            if (monopoleStrength.Strength == 0)
            {
                meaning = VisColorSettings.ColorMeaning.Neutral;
            }
            else if (monopoleStrength.Strength > 0)
            {
                meaning = VisColorSettings.ColorMeaning.Positive;
            }
            else if (monopoleStrength.Strength < 0)
            {
                meaning = VisColorSettings.ColorMeaning.Negative;
            }

            return colorSettings.GetMaterial(meaning, generator.generatedField);
        }
    
        public void OnValidate()
        {
            monopoleStrength = GetComponent<MonopoleStrength>();
            generator = GetComponent<FieldGenerator>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
    }
}

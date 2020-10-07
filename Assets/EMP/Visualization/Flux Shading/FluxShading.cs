using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EMP.Core;
using UnityEngine;

namespace Visualization
{
    public class FluxShading : MonoBehaviour
    {
        public VisColorSettings colorSettings;

        public List<FluxMaterial> materials;

        void OnEnable()
        {
            SetColors();
        }

        public void SetColors()
        {
            foreach (FluxMaterial fluxMaterial in materials)
            {
                fluxMaterial.material.SetColor("_PositiveColor",
                    colorSettings.GetColor(VisColorSettings.ColorMeaning.Positive, fluxMaterial.fieldType));
                fluxMaterial.material.SetColor("_NegativeColor",
                    colorSettings.GetColor(VisColorSettings.ColorMeaning.Negative, fluxMaterial.fieldType));
            }
        }
    }

    [Serializable]
    public sealed class FluxMaterial
    {
        public Material material;
        public InteractionField fieldType;
    }
}
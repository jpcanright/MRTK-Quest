using System;
using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;


namespace Visualization
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Visualization Colors", menuName = "ScriptableObjects/VisColorSettings", order = 1)]
    public class VisColorSettings : ScriptableObject
    {
        /// <summary>
        /// Significance of each color.
        /// </summary>
        public enum ColorMeaning
        {
            Positive,
            Negative,
            Neutral,
            Error
        }

        [SerializeField] 
        private List<FieldColors> fieldColors;
        [SerializeField]
        private Color neutral;
        [SerializeField]
        private Color error;

        //private Dictionary<ColorMeaning, Color> colorDict = new Dictionary<ColorMeaning, Color>();
        private Dictionary<ColorMeaning, Material> materialDict = new Dictionary<ColorMeaning, Material>();
        
        
        [SerializeField]
        private Material baseMaterial;

        private FieldColors GetFieldColors(InteractionField fieldType)
        {
            foreach (FieldColors colorSet in fieldColors)
            {
                if (colorSet.FieldType == fieldType)
                {
                    return colorSet;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Retrieve color associated with ColorMeaning <paramref name="meaning"/>.
        /// </summary>
        /// <param name="meaning"></param>
        /// <returns></returns>
        public Color GetColor(ColorMeaning meaning, InteractionField fieldType)
        {
            switch (meaning)
            {
                case ColorMeaning.Positive: return GetFieldColors(fieldType).positiveColor;
                case ColorMeaning.Negative: return GetFieldColors(fieldType).negativeColor;
                case ColorMeaning.Neutral: return neutral;
                case ColorMeaning.Error: return error;
                default: return error;
            }
        }
        
        public Material GetMaterial(ColorMeaning meaning, InteractionField fieldType)
        {
            if (meaning == ColorMeaning.Positive || meaning == ColorMeaning.Negative)
            {
                FieldColors colorSet = GetFieldColors(fieldType);
                if (meaning == ColorMeaning.Positive)
                {
                    if (!colorSet.positiveMaterial)
                    {
                        colorSet.positiveMaterial = Instantiate(baseMaterial);
                        colorSet.positiveMaterial.color = colorSet.positiveColor;
                    }

                    return colorSet.positiveMaterial;
                }
                if (meaning == ColorMeaning.Negative)
                {
                    if (!colorSet.negativeMaterial)
                    {
                        colorSet.negativeMaterial = Instantiate(baseMaterial);
                        colorSet.negativeMaterial.color = colorSet.negativeColor;
                    }

                    return colorSet.negativeMaterial;
                }
            }
            
            if (!materialDict.ContainsKey(meaning) || !materialDict[meaning])
            {
                materialDict[meaning] = Instantiate(baseMaterial);
                materialDict[meaning].color = GetColor(meaning, fieldType);
            }
            return materialDict[meaning];
        }
    }

    
    [Serializable]
    public sealed class FieldColors
    {
        public Color positiveColor;
        public Color negativeColor;
        public InteractionField FieldType;
        [HideInInspector]
        public Material positiveMaterial;
        [HideInInspector]
        public Material negativeMaterial;
    }
}
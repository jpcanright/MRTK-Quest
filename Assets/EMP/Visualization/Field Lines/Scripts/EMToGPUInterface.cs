using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using EMP.Core;
using UnityEngine;

namespace Visualization
{
    /// <summary>
    /// Sends information about EM objects in the scene to the GPU for visualization purposes.
    /// Currently only applies to electric charges.
    /// </summary>
    public class EMToGPUInterface : MonoBehaviour
    {
        private static readonly int ChargeArrayLength = Shader.PropertyToID("_ChargeArrayLength");
        private static readonly int ChargesProperty = Shader.PropertyToID("_Charges");

        public readonly int maxChargeArrayLength = 32;

        public InteractionField fieldType;
        
        private Vector4[] charges;

        public Vector4[] Charges => charges;

        // Update is called once per frame
        void Update()
        {
            charges = ChargePositionsAndStrengths();
            Shader.SetGlobalInt(ChargeArrayLength, charges.Length);
            Shader.SetGlobalVectorArray(ChargesProperty, charges);
        }

        Vector4[] ChargePositionsAndStrengths()
        {
            List<PointChargeGenerator> generators =
                EMController.Instance.GetFieldGenerators<PointChargeGenerator>(fieldType);

            // Cap array length at maximum number of charges supported by GPU
            int minLength = Math.Min(generators.Count, maxChargeArrayLength);
            Vector4[] output = new Vector4[minLength];

            for (int i = 0; i < minLength; i++)
            {
                try
                {
                    Vector3 chargePosition = generators[i].transform.position;
                    float chargeStrength = generators[i].monopoleStrength.Strength;
                    output[i] = new Vector4(chargePosition.x, chargePosition.y, chargePosition.z, chargeStrength);
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }

            return output;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMP.Core
{
    /// <summary>
    /// Stores the moment associated with a dipole, e.g. an electric or magnetic dipole.
    /// </summary>
    public class DipoleStrength : EMStrength
    {
        [Tooltip("Dipole moment vector.")]
        [SerializeField]
        private Vector3 _moment = Vector3.forward;
        
        /// <summary>
        /// Dipole moment vector.
        /// </summary>
        public Vector3 Moment
        {
            get => _moment;
            set => _moment = value;
        }
    }
}
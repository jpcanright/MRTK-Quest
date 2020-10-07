using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMP.Core
{
    public class MonopoleStrength : EMStrength
    {
        [SerializeField]
        private float _strength = 1;
        
        public float Strength
        {
            get => _strength;
            set => _strength = value;
        }

        //If a value is not provided in the inspector then set it to 1.  I could see this happening if object does not include EMStrength and it is generated at runtime.
        void Start()
        {
            if (_strength == 0)
            {
                _strength = 1;
            }
        }
    }
}
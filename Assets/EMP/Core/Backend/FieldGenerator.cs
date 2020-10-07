using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace EMP.Core
{

    //[RequireComponent(typeof(EMStrength))]

    public abstract class FieldGenerator : MonoBehaviour
    {
        
        [SerializeField]
        public InteractionField generatedField;
        
        private Vector3 _cachedPosition;
        private bool _positionCachedThisFrame = false;
        
        public Vector3 Position
        {
            get
            {
                return transform.position;
                //                if (!_positionCachedThisFrame)
//                {
//                    _cachedPosition = transform.position;
//                    _positionCachedThisFrame = true;
//                }
//                return _cachedPosition;
            }
        }

        protected virtual void Start(){}

        protected void LateUpdate()
        {
            _positionCachedThisFrame = false;
        }

        public virtual void Register()
        {
            EMController.Instance.RegisterFieldGenerator(this);

        }

        public virtual void UnRegister()
        {
            // If the application is quitting, EMController.Instance returns null, which can cause an error.
            if (EMController.Instance)
            {
                EMController.Instance.UnRegisterFieldGenerator(this);
            }
        }

        public abstract Vector3 FieldValue(Vector3 fieldPoint);

        protected virtual void OnEnable()
        {
            Register();
        }

        protected virtual void OnDisable()
        {
            UnRegister();
        }
    }
}
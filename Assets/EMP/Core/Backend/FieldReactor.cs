using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMP.Core
{
    [RequireComponent(typeof(EMStrength))]

    public abstract class FieldReactor : MonoBehaviour
    {
        
        [SerializeField]
        public InteractionField fieldReactingTo;
        
        public Rigidbody RigidBody { get; private set; }

        public FieldGenerator associatedGenerator;
        [SerializeField]
        protected float multiplier = 1;
        

        // Start is called before the first frame update
        protected virtual void Start()
        {
            if (!RigidBody)
            {
                RigidBody = gameObject.GetComponent<Rigidbody>();                
            }

            if (!associatedGenerator)
            {
                FieldGenerator generator = GetComponent<FieldGenerator>();
                if (generator)
                {
                    if (generator.generatedField == fieldReactingTo)
                    {
                        associatedGenerator = GetComponent<FieldGenerator>();
                    }
                }
            }
        }
        
        /// <summary>
        /// Determines the direction and magnitude of the force upon this reactor due to the field <see cref="fieldType"/>.
        /// This should be unique to each FieldReactor.
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 ForceUponReactor();
        
        /// <summary>
        /// Determines the direction and magnitude of torque upon this reactor due to the field <see cref="fieldType"/>.
        /// If not overridden, this function does nothing.
        /// </summary>
        public virtual void ApplyTorques(){}
        
        /// <summary>
        /// Call to calculate and apply force upon this reactor due to the field <see cref="fieldType"/>.
        /// </summary>
        public virtual void ApplyForces()
        {
            RigidBody.AddForce(ForceUponReactor());
        }
        
        /// <summary>
        /// Registers this <see cref="FieldReactor"/> with the <see cref="EMController"/>. Must be called on enable.
        /// </summary>
        public virtual void Register()
        {
            EMController.Instance.RegisterFieldReactor(this);
        }

        public virtual void UnRegister()
        {
            // If the application is quitting, EMController.Instance returns null, which can cause an error.
            if (EMController.Instance)
            {
                EMController.Instance.UnRegisterFieldReactor(this);
            }
        }

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
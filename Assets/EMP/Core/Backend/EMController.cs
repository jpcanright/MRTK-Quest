using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EMP.Core
{
    public class EMController : Singleton<EMController>
    {
        // Keep a list of references to all FieldGenerators in the scene.
        protected List<FieldGenerator> fieldGenerators = new List<FieldGenerator>();

        // Keep a list of references to all FieldReactors in the scene.
        protected List<FieldReactor> fieldReactors = new List<FieldReactor>();
        
        // Keep a list of all InteractionFields that are or have been present in the scene.
        private List<InteractionField> fieldsInScene = new List<InteractionField>();
        
        public delegate void FieldGeneratorRegistrationHandler (FieldGenerator generator);

        public delegate void FieldReactorRegistrationHandler(FieldReactor reactor);

        public delegate void InteractionFieldRegistrationHandler(InteractionField field);
 
        // Field generator registration events
        public event FieldGeneratorRegistrationHandler OnFieldGeneratorRegistered;
        public event FieldGeneratorRegistrationHandler OnFieldGeneratorUnregistered;
        
        // Field reactor registration events
        public event FieldReactorRegistrationHandler OnFieldReactorRegistered;
        public event FieldReactorRegistrationHandler OnFieldReactorUnregistered;
        
        // Interaction field registration event
        public event InteractionFieldRegistrationHandler OnInteractionFieldRegistered;
        
        // Each physics update tick, each FieldReactor calculates and applies relevant forces and torques upon itself.
        protected virtual void FixedUpdate()
        {
            foreach (FieldReactor FR in fieldReactors)
            {
                FR.ApplyForces();
                FR.ApplyTorques();
            }
        }
        
        // Automatically called by every FieldGenerator on scene start or on its creation.
        public virtual void RegisterFieldGenerator(FieldGenerator NewFieldGenerator)
        {
            // Make sure new field generator isn't already registered
            if (fieldGenerators.Contains(NewFieldGenerator))
            {
                return;
            }
            // Add new field generator
            fieldGenerators.Add(NewFieldGenerator);
            
            // Register field type
            RegisterFieldType(NewFieldGenerator.generatedField);
            OnFieldGeneratorRegistered?.Invoke(NewFieldGenerator);
        }
        
        // Automatically called by every FieldGenerator on its destruction.
        public virtual void UnRegisterFieldGenerator(FieldGenerator RemoveFieldGenerator)
        {
            fieldGenerators.Remove(RemoveFieldGenerator);
            OnFieldGeneratorUnregistered?.Invoke(RemoveFieldGenerator);
        }

        // Automatically called by every FieldReactor on scene start or on its creation.
        public virtual void RegisterFieldReactor(FieldReactor NewFieldReactor)
        {
            // Make sure new field reactor isn't already registered
            if (fieldReactors.Contains(NewFieldReactor))
            {
                return;
            }
            // Add new field reactor
            fieldReactors.Add(NewFieldReactor);
            
            // Register field type
            RegisterFieldType(NewFieldReactor.fieldReactingTo);
            OnFieldReactorRegistered?.Invoke(NewFieldReactor);
        }
        
        // Automatically called by every FieldReactor on its destruction.
        public virtual void UnRegisterFieldReactor(FieldReactor RemoveFieldReactor)
        {
            fieldReactors.Remove(RemoveFieldReactor);
            OnFieldReactorUnregistered?.Invoke(RemoveFieldReactor);
        }

        protected virtual void RegisterFieldType(InteractionField field)
        {
            if (!fieldsInScene.Contains(field))
            {
                fieldsInScene.Add(field);
                OnInteractionFieldRegistered?.Invoke(field);
            }
        }
    
        
        /// <summary>
        /// Calculate the value of the <paramref name="field"/> field at world-space <paramref name="position"/>.
        /// If <paramref name="generatorToExclude"/> is specified, it is left out of the calculation, e.g. for
        /// calculating the electric field at a point charge due to other charges.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="field"></param>
        /// <param name="generatorToExclude"></param>
        /// <returns>Direction and magnitude of the field.</returns>
        public virtual Vector3 GetFieldValue(Vector3 position, InteractionField field, FieldGenerator generatorToExclude = null)
        {
            Vector3 fieldValue = new Vector3(0, 0, 0);

            foreach (FieldGenerator fieldGenerator in fieldGenerators)
            {
                if (generatorToExclude)
                {
                    if (fieldGenerator == generatorToExclude)
                    {
                        continue;
                    }
                }
                if (fieldGenerator.generatedField == field)
                {
                    fieldValue = fieldValue + fieldGenerator.FieldValue(position);
                }
            }

            return fieldValue;
        }

        public virtual Vector3 GetFrameDependentFieldValue(Vector3 position, Vector3 frameVelocityInWorld,
            InteractionField field, InverseDistanceInFrameGenerator generatorToExclude = null)
        {
            Vector3 fieldValue = new Vector3(0, 0, 0);

            foreach (FieldGenerator fieldGenerator in fieldGenerators)
            {
                if (generatorToExclude)
                {
                    if (fieldGenerator == generatorToExclude)
                    {
                        continue;
                    }
                }
                if (fieldGenerator.generatedField == field)
                {
                    fieldValue = fieldValue + ((InverseDistanceInFrameGenerator)fieldGenerator).FieldValue(position,frameVelocityInWorld);
                }
            }

            return fieldValue;
        }
        
        public virtual List<T> GetFieldGenerators<T>(InteractionField field) where T: FieldGenerator
        {
            List<T> outList = new List<T>();
            foreach (FieldGenerator generator in fieldGenerators)
            {
                if (generator.GetType() == typeof(T) && field == generator.generatedField)
                {
                    outList.Add((T)generator);
                }
            }

            return outList;
        }
    }
}
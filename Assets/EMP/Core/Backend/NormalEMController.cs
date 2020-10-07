using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using EMP.Core;
using Normal.Realtime;
using UnityEngine;

public partial class NormalEMController : EMController
{
    public bool isSimulator;
    public int myClientId;
    public int simulatorId = -1;
    public bool isInitialized;
    public Realtime realtime;
    public RealtimeAvatarManager avatarManager;
    
    protected List<RealtimeTransform> particleTransforms = new List<RealtimeTransform>();
    protected List<NormalParticleState> particleStates = new List<NormalParticleState>();
    
    public override void RegisterFieldGenerator(FieldGenerator NewFieldGenerator)
    {
        RealtimeTransform realTransform = NewFieldGenerator.GetComponent<RealtimeTransform>();
        if (realTransform && !particleTransforms.Contains(realTransform))
        {
            particleTransforms.Add(realTransform);
        }
        NormalParticleState particleState = NewFieldGenerator.GetComponent<NormalParticleState>();
        if (realTransform && !particleStates.Contains(particleState))
        {
            particleStates.Add(particleState);
        }
        
        base.RegisterFieldGenerator(NewFieldGenerator);
    }

    void Update()
    {
        if (!isInitialized)
        {
            AttemptInitialize();            
        }

        if (isSimulator)
        {
            foreach (NormalParticleState particleState in particleStates)
            {
                if (particleState.state == NormalParticleState.OwnershipState.NeedsSimulatorPickup)
                {
                    //particle.realtimeView.RequestOwnership();
                    particleState.realtimeTransform.RequestOwnership();
                    particleState.SimulatorTakeOwnership();
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        // Only begin applying physics updates if Normal has initialized and we're the simulator
        if (isSimulator && isInitialized)
        {
            // Only apply physics updates if a particle is not being grabbed
            foreach (FieldReactor FR in fieldReactors)
            {
                if (FR.TryGetComponent(out NormalParticleState state))
                {
                    if (state.state != NormalParticleState.OwnershipState.OwnedBySimulator)
                    {
                        continue;
                    }
                }
                FR.ApplyForces();
                FR.ApplyTorques();
            }
        }
    }

    void AttemptInitialize()
    {
        if (realtime.connected)
        {
            if (avatarManager.avatars.Count == 1)
            {
                isSimulator = true;
                simulatorId = myClientId;
                isInitialized = true;
            }
            else if (avatarManager.avatars.Count == 2)
            {
                if (avatarManager.avatars.First().Value == avatarManager.localAvatar)
                {
                    simulatorId = avatarManager.avatars.Last().Key;
                }
                else
                {
                    simulatorId = avatarManager.avatars.First().Key;                    
                }
                isInitialized = true;
            }
            else
            {
                Debug.LogError($"Too many/few RealtimeAvatars: {avatarManager.avatars.Count}");
            }
            myClientId = realtime.clientID;
        }
    }
}

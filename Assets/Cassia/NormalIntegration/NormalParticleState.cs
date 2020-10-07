using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.Assertions.Must;

public partial class NormalParticleState : RealtimeComponent
{
    [SerializeField] private ParticleStateModel _model;
    [SerializeField] private OwnershipState m_ownershipState;
    [SerializeField] public RealtimeTransform realtimeTransform;
    
    private ParticleStateModel model
    {
        set
        {
            if (_model != null)
            {
                // Unregister from events
                _model.ownershipStateDidChange -= StateDidChange;
            }
            
            // Store model
            _model = value;

            if (_model != null)
            {
                m_ownershipState = value.ownershipState;
                _model.ownershipStateDidChange += StateDidChange;
            }
        }
    }
    
    public OwnershipState state
    {
        get => _model.ownershipState;
    }

    public bool isAnchored
    {
        get => _model.isAnchored;
        set
        {
            if (realtimeView.isOwnedLocally)
            {
                _model.isAnchored = value;
            }
        }
    }
    
    public enum OwnershipState
    {
        OwnedBySimulator,
        OwnedByInteractingPartner,
        NeedsSimulatorPickup
    }

    private void OnValidate()
    {
        realtimeTransform = GetComponent<RealtimeTransform>();
    }

    private void Awake()
    {
        //Release();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10f, 10f, 100f, 30f), "Grab"))
        {
            Grab();
        }
        if (GUI.Button(new Rect(10f, 50f, 100f, 30f), "Release"))
        {
            Release();
        }
        if (GUI.Button(new Rect(10f, 90f, 150f, 30f), "SimulatorTakeOwnership"))
        {
            SimulatorTakeOwnership();
        }
    }

    private void StateDidChange(ParticleStateModel model, OwnershipState value)
    {
        m_ownershipState = value;
    }
    
    public void Grab()
    {
        
        realtimeView.RequestOwnership();
        realtimeTransform.RequestOwnership();
        _model.ownershipState = OwnershipState.OwnedByInteractingPartner;
    }
    
    public void Release()
    {
        _model.ownershipState = OwnershipState.NeedsSimulatorPickup;
        // Only release ownership of the realtimeView; we want Simulator's update loop to take ownership of
        // the transform on its own such that it isn't at any point owned by the server (and thus not simulated).
        realtimeView.ClearOwnership();
    }

    public void SimulatorTakeOwnership()
    {
        _model.ownershipState = OwnershipState.OwnedBySimulator;
    }
}

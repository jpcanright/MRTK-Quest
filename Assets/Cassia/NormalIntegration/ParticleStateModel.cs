using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class ParticleStateModel
{
    [RealtimeProperty(1, true, true)] private NormalParticleState.OwnershipState _ownershipState;
    [RealtimeProperty(2, true, true)] private bool _isAnchored;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class ParticleStateModel : IModel {
    // Properties
    public NormalParticleState.OwnershipState ownershipState {
        get { return _cache.LookForValueInCache(_ownershipState, entry => entry.ownershipStateSet, entry => entry.ownershipState); }
        set { if (value == ownershipState) return; _cache.UpdateLocalCache(entry => { entry.ownershipStateSet = true; entry.ownershipState = value; return entry; }); FireOwnershipStateDidChange(value); }
    }
    public bool isAnchored {
        get { return _cache.LookForValueInCache(_isAnchored, entry => entry.isAnchoredSet, entry => entry.isAnchored); }
        set { if (value == isAnchored) return; _cache.UpdateLocalCache(entry => { entry.isAnchoredSet = true; entry.isAnchored = value; return entry; }); FireIsAnchoredDidChange(value); }
    }
    
    // Events
    public delegate void OwnershipStateDidChange(ParticleStateModel model, NormalParticleState.OwnershipState value);
    public event         OwnershipStateDidChange ownershipStateDidChange;
    public delegate void IsAnchoredDidChange(ParticleStateModel model, bool value);
    public event         IsAnchoredDidChange isAnchoredDidChange;
    
    // Delta updates
    private struct LocalCacheEntry {
        public bool                               ownershipStateSet;
        public NormalParticleState.OwnershipState ownershipState;
        public bool                               isAnchoredSet;
        public bool                               isAnchored;
    }
    
    private LocalChangeCache<LocalCacheEntry> _cache;
    
    public ParticleStateModel() {
        _cache = new LocalChangeCache<LocalCacheEntry>();
    }
    
    // Events
    public void FireOwnershipStateDidChange(NormalParticleState.OwnershipState value) {
        try {
            if (ownershipStateDidChange != null)
                ownershipStateDidChange(this, value);
        } catch (System.Exception exception) {
            Debug.LogException(exception);
        }
    }
    public void FireIsAnchoredDidChange(bool value) {
        try {
            if (isAnchoredDidChange != null)
                isAnchoredDidChange(this, value);
        } catch (System.Exception exception) {
            Debug.LogException(exception);
        }
    }
    
    // Serialization
    enum PropertyID {
        OwnershipState = 1,
        IsAnchored = 2,
    }
    
    public int WriteLength(StreamContext context) {
        int length = 0;
        
        if (context.fullModel) {
            // Mark unreliable properties as clean and flatten the in-flight cache.
            // TODO: Move this out of WriteLength() once we have a prepareToWrite method.
            _ownershipState = ownershipState;
            _isAnchored = isAnchored;
            _cache.Clear();
            
            // Write all properties
            length += WriteStream.WriteVarint32Length((uint)PropertyID.OwnershipState, (uint)_ownershipState);
            length += WriteStream.WriteVarint32Length((uint)PropertyID.IsAnchored, _isAnchored ? 1u : 0u);
        } else {
            // Reliable properties
            if (context.reliableChannel) {
                LocalCacheEntry entry = _cache.localCache;
                if (entry.ownershipStateSet)
                    length += WriteStream.WriteVarint32Length((uint)PropertyID.OwnershipState, (uint)entry.ownershipState);
                if (entry.isAnchoredSet)
                    length += WriteStream.WriteVarint32Length((uint)PropertyID.IsAnchored, entry.isAnchored ? 1u : 0u);
            }
        }
        
        return length;
    }
    
    public void Write(WriteStream stream, StreamContext context) {
        if (context.fullModel) {
            // Write all properties
            stream.WriteVarint32((uint)PropertyID.OwnershipState, (uint)_ownershipState);
            stream.WriteVarint32((uint)PropertyID.IsAnchored, _isAnchored ? 1u : 0u);
        } else {
            // Reliable properties
            if (context.reliableChannel) {
                LocalCacheEntry entry = _cache.localCache;
                if (entry.ownershipStateSet || entry.isAnchoredSet)
                    _cache.PushLocalCacheToInflight(context.updateID);
                
                if (entry.ownershipStateSet)
                    stream.WriteVarint32((uint)PropertyID.OwnershipState, (uint)entry.ownershipState);
                if (entry.isAnchoredSet)
                    stream.WriteVarint32((uint)PropertyID.IsAnchored, entry.isAnchored ? 1u : 0u);
            }
        }
    }
    
    public void Read(ReadStream stream, StreamContext context) {
        bool ownershipStateExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.ownershipStateSet);
        bool isAnchoredExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.isAnchoredSet);
        
        // Remove from in-flight
        if (context.deltaUpdatesOnly && context.reliableChannel)
            _cache.RemoveUpdateFromInflight(context.updateID);
        
        // Loop through each property and deserialize
        uint propertyID;
        while (stream.ReadNextPropertyID(out propertyID)) {
            switch (propertyID) {
                case (uint)PropertyID.OwnershipState: {
                    NormalParticleState.OwnershipState previousValue = _ownershipState;
                    
                    _ownershipState = (NormalParticleState.OwnershipState)stream.ReadVarint32();
                    
                    if (!ownershipStateExistsInChangeCache && _ownershipState != previousValue)
                        FireOwnershipStateDidChange(_ownershipState);
                    break;
                }
                case (uint)PropertyID.IsAnchored: {
                    bool previousValue = _isAnchored;
                    
                    _isAnchored = (stream.ReadVarint32() != 0);
                    
                    if (!isAnchoredExistsInChangeCache && _isAnchored != previousValue)
                        FireIsAnchoredDidChange(_isAnchored);
                    break;
                }
                default:
                    stream.SkipProperty();
                    break;
            }
        }
    }
}
/* ----- End Normal Autogenerated Code ----- */

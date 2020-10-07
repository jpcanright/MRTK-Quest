using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EMP.Core;
using EMP.Visualization;
using UnityEngine;

public class CPUFieldLineManager : Singleton<CPUFieldLineManager>
{

    public FieldLineTypeSettings settings;
    
    public int LinesPerCharge => settings.fieldLinesPerCharge;

    public enum FieldLineDrawSpace
    {
        Plane,
        Volume
    }

    public FieldLineDrawSpace DrawSpace => settings.drawSpace;
    public InteractionField FieldType => settings.fieldType;

    public Vector3 planeDrawSpaceNormal;
    public Plane fieldLinePlane;

    public List<FieldLineTerminalGeometry> terminalGeometries;

    // TODO Move pointChargeRadius field to visualization manager when this script is ready
    //[Tooltip("World-space radius of spheres associated w/ point charges.")]
    //public float pointChargeRadius = 0.1f;
    
    // Start is called before the first frame update
    void Awake()
    {
        terminalGeometries = new List<FieldLineTerminalGeometry>();
    }

    // Update is called once per frame
    void Update()
    {
        _landedLines = new Dictionary<FieldLineTerminalGeometry, List<CPUFieldLineModel>>();
        DrawLines();
    }

    private void DrawLines()
    {
        List<PointChargeGenerator> charges = GetCharges();
        
        // If we have no charges in the scene, do nothing.
        if (charges.Count == 0)
        {
            return;
        }

        List<Vector4> fieldLineSeeds;
        
        if (DrawSpace == FieldLineDrawSpace.Plane) // Drawing on plane
        {
            // If we have more than 3 charges, we cannot draw field lines on a plane
            // TODO allow this but only if all charges are coplanar or very nearly so
            if (charges.Count > 3)
            {
                Debug.LogWarning("Cannot draw field lines on plane; too many charges in scene.");
                return;
            }
            // Determine position and orientation of plane on which to draw field lines
            Plane plane = DetermineDrawingPlane(charges);
            // Get list of field line seeds
            fieldLineSeeds = SeedFieldLinesOnPlane(plane, charges);
            
        }
        else // Drawing in volume
        {
            // TODO implement field line drawing in 3D space
            return;
        }
        
        // Determine number of field line seeds of each type
        int negativeSources = 0;
        int positiveSources = 0;
        foreach(Vector4 seed in fieldLineSeeds)
        {
            if (seed[3] > 0)
            {
                positiveSources++;
            }
            else
            {
                negativeSources++;
            }
        }

        IntegrationDirection firstDirection;
        IntegrationDirection secondDirection;
        if (positiveSources >= negativeSources)
        {
            firstDirection = IntegrationDirection.FromPositive;
            secondDirection = IntegrationDirection.FromNegative;
        }
        else
        {
            firstDirection = IntegrationDirection.FromNegative;
            secondDirection = IntegrationDirection.FromPositive;
        }

        // Draw either positive-sourced (integrating down) or negative-sourced (integrating up) field lines first,
        // whichever there are more of. If equal, draw positive-sourced first.
        // We call the field lines drawn first the field lines of the first kind.

        foreach (FieldLineTerminalGeometry geometry in terminalGeometries)
        {
            if (geometry.Sign > 0 && firstDirection == IntegrationDirection.FromPositive)
            {
                geometry.CalculateFieldLines();
                geometry.DebugTagAll(FieldLineDebugTag.FromPositive);
                // TEMP
                //geometry.DrawCalculatedFieldLines();
            }
            else if (geometry.Sign < 0 && firstDirection == IntegrationDirection.FromNegative)
            {
                geometry.CalculateFieldLines();
                geometry.DebugTagAll(FieldLineDebugTag.FromNegative);
                // TEMP
                //geometry.DrawCalculatedFieldLines();
            }
            FieldLineDebugTag debugTag;
            
            if (geometry.Sign > 0)
            {
                debugTag = FieldLineDebugTag.FromPositive;
            }
            else
            {
                debugTag = FieldLineDebugTag.FromNegative;
            }
            
            //geometry.CalculateFieldLines();
            // TODO add editor toggle, or even conditional compilation block to debug tagging
            // Apply source-dependent field line colors
            //geometry.DebugTagAll(debugTag);
        }
        
        // Now that all field lines of the first kind are calculated,
        // Report landed field lines to geometries of the second kind
        // Tag culled field lines
        // Calculate field lines of the second kind
        foreach (FieldLineTerminalGeometry geometry in terminalGeometries)
        {
            FieldLineDebugTag debugTag;

            if (geometry.Sign > 0)
            {
                debugTag = FieldLineDebugTag.FromPositive;
            }
            else
            {
                debugTag = FieldLineDebugTag.FromNegative;
            }

            if (geometry.Sign > 0 && secondDirection == IntegrationDirection.FromPositive ||
                geometry.Sign < 0 && secondDirection == IntegrationDirection.FromNegative)
            {
                geometry.DebugTagAll(debugTag);

                if (_landedLines.ContainsKey(geometry))
                {
                    Vector4[] updatedSeeds = geometry.RotateToMatchExistingFieldLines(_landedLines[geometry]);
                    Vector4[] seedsToCull = geometry.RejectSeedsForExistingFieldLines(_landedLines[geometry]);
                    //geometry.CullBySeed(seedsToCull);
                    geometry.DebugTagBySeed(seedsToCull, FieldLineDebugTag.Culled);
                }

                geometry.CalculateFieldLines();

                // TEMP
                //geometry.DrawCalculatedFieldLines();
            }
            else
            {
                continue;
            }
        }

        // Draw everything!
        foreach (FieldLineTerminalGeometry geometry in terminalGeometries)
        {
            geometry.DrawCalculatedFieldLines();
        }

        // Report the endpoints of the field lines of the first kind to each FL terminal geometry of the second
        // kind, and have the FL terminal geometries of the second kind rotate (for now) to match, and disable
        // terminals where a FL of the first kind has landed.
        
        // If any FL terminals of the second kind remain, draw them.
    }

    private Plane DetermineDrawingPlane(List<PointChargeGenerator> charges)
    {
        Plane drawingPlane = new Plane();
        
        // TODO implement one-charge case
        // In the one-charge case, we pick the plane to face the player; we can change this later if we really want.
        if (charges.Count == 1)
        {
            drawingPlane = new Plane(-Camera.main.transform.position.normalized, 
                             charges[0].transform.position);
        }
        // In the two-charge case, we pick the plane to be parallel with the y-axis (it goes up and down).
        // By taking the cross product of the y unit vector and a vector pointing between the charges,
        // we get the normal of the plane.
        // If the inter-charge vector is parallel to the y unit vector, cross it with the x unit vector I guess.
        else if (charges.Count == 2)
        {
            Vector3 interChargeVector = charges[0].transform.position - charges[1].transform.position;

            Vector3 normal;
            if (interChargeVector.normalized == Vector3.up || interChargeVector.normalized == Vector3.down)
            {
                normal = Vector3.Normalize(Vector3.Cross(Vector3.left, interChargeVector));
            }
            else
            {
                normal = Vector3.Normalize(Vector3.Cross(Vector3.up, interChargeVector));
            }
            
            drawingPlane = new Plane(normal, charges[0].transform.position);
            
        }
        // If there are three charges, the plane is fully specified by their positions.
        // Does not take into account player position.
        else if (charges.Count == 3)
        {
            drawingPlane.Set3Points(charges[0].transform.position,
                charges[1].transform.position,
                charges[2].transform.position);
        }

        return drawingPlane;
    }
    
    private List<PointChargeGenerator> GetCharges()
    {
        // NOTE: Hard-coded for monopoles
        return EMController.Instance.GetFieldGenerators<PointChargeGenerator>(FieldType);
    }
    
    // Will need to be cleared each frame
    private Dictionary<FieldLineTerminalGeometry, List<CPUFieldLineModel>> _landedLines = 
        new Dictionary<FieldLineTerminalGeometry, List<CPUFieldLineModel>>();

    public void ReportFieldLineTermination(CPUFieldLineModel model, CPUFieldLineModel.TerminationReason termReason)
    {
        // Determine whether the field line could have landed on another geometry
        // It is not clear whether this is a sufficient check.
        if (termReason != CPUFieldLineModel.TerminationReason.BackTolerance)
        {
            return;
        }
        
        // TODO skip cases where e.g. the field line is positive-sourced and the geometry is associated with a positive charge.
        FieldLineTerminalGeometry geometryTerminatedUpon = null;
        foreach (FieldLineTerminalGeometry geometry in terminalGeometries)
        {
            if (geometry.IsPointWithinGeometry(model.LastPoint))
            {
                geometryTerminatedUpon = geometry;
                break;
            }
        }
        // If we didn't find a geometry the field line landed on, that's weird; throw a warning and return
        if (!geometryTerminatedUpon)
        {
            Debug.LogWarning("This field line thinks it terminated on a geometry, but its endpoint is not contained within any terminal geometry. This shouldn't happen.", model);
        }
        else
        {
            // If a geometry this field line landed on was found, add the field line to the dict.
            
            // Check to see if this geometry is already in the dict.
            // If it isn't, add it as a key with an empty list of CPUFieldLineModels as its value.
            if (!_landedLines.ContainsKey(geometryTerminatedUpon))
            {
                _landedLines.Add(geometryTerminatedUpon, new List<CPUFieldLineModel>());
            }
            // Add the landed field line model to the geometry's list of field line models.
            _landedLines[geometryTerminatedUpon].Add(model);
        }
    }

    private List<Vector4> SeedFieldLinesOnPlane(Plane plane, List<PointChargeGenerator> charges)
    {
       
        List<PointChargeGenerator> needGeometries = charges;

        // Update our list of geometries, adding them as needed
        // TODO delete old geometries without associated charge
        foreach (var charge in needGeometries)
        {
            bool existingGeometry = false;
            foreach (var geometry in terminalGeometries)
            {
                if ((PointChargeGenerator) geometry.associatedGenerator == charge)
                {
                    //needGeometries.Remove(charge);
                    existingGeometry = true;
                }
            }

            if (!existingGeometry)
            {
                var newGameObj = new GameObject("TerminalGeometry");
                // Let's make terminal geometries children of their corresponding charges.
                newGameObj.transform.SetParent(charge.transform);
                SphereFieldLineTerminal terminal = newGameObj.AddComponent<SphereFieldLineTerminal>();
                terminalGeometries.Add(terminal);
                terminal.associatedGenerator = charge;
                terminal.transform.position = charge.transform.position;
            }
        }
        
        List<Vector4> fieldLineSeeds = new List<Vector4>();
        
        // Grabs field line seeds from each charge geometry.
        foreach (var geometry in terminalGeometries)
        {
            fieldLineSeeds.AddRange(geometry.FieldLineSeedsInPlane(
                (int)((PointChargeGenerator)geometry.associatedGenerator).monopoleStrength.Strength, 
                plane).ToList());
        }

        return fieldLineSeeds;
    }
}

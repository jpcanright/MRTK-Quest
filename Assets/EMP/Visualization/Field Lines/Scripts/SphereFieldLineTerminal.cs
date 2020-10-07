using System;
using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using EMP.Visualization;
using UnityEngine;

public class SphereFieldLineTerminal : FieldLineTerminalGeometry
{

    public float _sphereRadius = 0.1f;
    //public EMStrength associatedEMSource;

    public override bool IsPointWithinGeometry(Vector3 point)
    {
        // Return true if the given point is less than the sphere's radius away from its center.
        return Vector3.Magnitude(point - transform.position) < _sphereRadius;
    }

    public override Vector4[] FieldLineSeedsInPlane(int charge, Plane plane)
    {
        // Clear landed field lines set last time field lines were drawn
        _landedFieldLines = new List<CPUFieldLineModel>();
        
        // Multiply charge-to-lines scale factor by the charge of this point source; take absolute value as charge
        // may be negative.
        int linesToDraw = Math.Abs(charge * CPUFieldLineManager.Instance.LinesPerCharge);
        int sign = Math.Sign(charge);
        //Sign = sign;
        
        Vector4[] fieldLinePositions = new Vector4[linesToDraw];
        
        // On a plane, we draw field lines from a source starting from equally-spaced points on a circle which
        // lies in that plane.
        float angleBetweenCharges = 360f / linesToDraw;

        Vector3 normal = plane.normal;
        Vector3 position = transform.position;
        
        // Pick the projection of the world-space y unit vector onto the plane as our 0-degree starting angle,
        // and proceed clockwise. If the plane's normal vector is parallel to the y unit vector, use the z unit vector.

        Vector3 toProject;
        
        if (normal == Vector3.up || normal == Vector3.down)
        {
            toProject = Vector3.forward;
        }
        else
        {
            toProject = Vector3.up;
        }
        
        // Direction of offset from charge position of first field line seed.
        Vector3 seedOffsetDirection = Vector3.Normalize(Vector3.ProjectOnPlane(toProject, normal));
        
        // Rotation between adjacent field line seeds
        Quaternion intermediateRotation = Quaternion.AngleAxis(angleBetweenCharges, normal);
        
        // Record first field line seed position
        fieldLinePositions[0] = DirectionToSeed(seedOffsetDirection, position, sign);
        
        for (int i = 1; i < linesToDraw; i++)
        {
            seedOffsetDirection = intermediateRotation * seedOffsetDirection;
            fieldLinePositions[i] = DirectionToSeed(seedOffsetDirection, position, sign);
        }

        storedSeeds = fieldLinePositions;
        return fieldLinePositions;
    }

    public override Vector4[] RotateToMatchExistingFieldLines(List<CPUFieldLineModel> existingFieldLines)
    {
        // For now, let's just rotate to match the first of any landed field lines.
        Vector3 incomingFieldLineDirection = existingFieldLines[0].LastDirection;
        Vector4 firstSeed = storedSeeds[0];
        Vector3 seededFieldLineDirection =
            (new Vector3(firstSeed.x, firstSeed.y, firstSeed.z) - transform.position).normalized;
        
            
        // Angle between landed and seeded field lines--hopefully.
        float angleBetweenLines = Vector3.Angle(seededFieldLineDirection, incomingFieldLineDirection);
        Quaternion rotation = Quaternion.FromToRotation(seededFieldLineDirection, incomingFieldLineDirection);
        
        for( int i=0; i< storedSeeds.Length; i++)
        {
            Vector4 seed = storedSeeds[i];
            Vector3 oldOffset = new Vector3(seed.x, seed.y, seed.z) - transform.position;
            Vector3 newOffset = rotation * oldOffset;
            Vector3 newPosition = newOffset + transform.position;
            storedSeeds[i] = new Vector4(newPosition.x, newPosition.y, newPosition.z, seed.w);
        }

        return storedSeeds;
    }

    public override Vector4[] RejectSeedsForExistingFieldLines(List<CPUFieldLineModel> existingFieldLines)
    {
        CPUFieldLineModel firstLandedLine = existingFieldLines[0];
        // Get the direction of approach of the first field line to land, and rotate our geometry to match
        
        Vector3 firstLandedLineIncomingDirection = firstLandedLine.LastDirection;
        Vector3 firstLandedLineLastPoint = firstLandedLine.LastPoint;

        List<Vector4> seedsToRemove = new List<Vector4>();

        foreach (CPUFieldLineModel landedLine in existingFieldLines)
        {
            foreach (Vector4 seed in storedSeeds)
            {
                if (LandedFieldLineIsNearSeed(landedLine.LastDirection, seed))
                {
                    seedsToRemove.Add(seed);
                }
            }
        }

        return seedsToRemove.ToArray();
    }

    

    private bool LandedFieldLineIsNearSeed(Vector3 incomingFieldLineDirection, Vector4 fieldLineSeed)
    {
        // The angle used to determine if a landed field line is "close enough" to an existing seed to be matched with it.
        float proximityAngle = (360f / storedSeeds.Length) / 2f;
        
        // The incoming field line direction is given toward the center of the charge, so we invert it
        //Vector3 incomingFieldLineOutwardDirection = -incomingFieldLineDirection;
        // Or maybe not? Runtime testing said I had this reversed
        Vector3 incomingFieldLineOutwardDirection = incomingFieldLineDirection;
        // Yup, that was the case. No idea why.
        
        
        // The initial direction of the field line which would be drawn from the given field line seed
        Vector3 seededFieldLineDirection =
            (new Vector3(fieldLineSeed.x, fieldLineSeed.y, fieldLineSeed.z) - transform.position).normalized;
        
            
        // Angle between landed and seeded field lines--hopefully.
        float angleBetweenLines = Vector3.Angle(seededFieldLineDirection, incomingFieldLineOutwardDirection);
        // If the angle between the lines is less than or equal to the proximity angle, return true.
        if (angleBetweenLines <= proximityAngle)
        {
            Debug.DrawLine(transform.position, transform.position + incomingFieldLineDirection * _sphereRadius, Color.magenta);
            Debug.DrawLine(transform.position, transform.position + seededFieldLineDirection * _sphereRadius, Color.cyan);
            Debug.DrawLine(transform.position + incomingFieldLineDirection * _sphereRadius, transform.position + seededFieldLineDirection * _sphereRadius, Color.black);
        }
        
        return angleBetweenLines <= proximityAngle;
    }

    private Vector4 DirectionToSeed(Vector3 seedDirection, Vector3 position, int sign)
    {
        Vector3 seedPosition = _sphereRadius * seedDirection + position;
        return new Vector4(seedPosition.x, seedPosition.y, seedPosition.z, sign);
    }

    public override Vector4[] FieldLineSeedsInVolume(int charge)
    {
        // TODO Field line seeding algorithm for sphere in volume, lol
        throw new NotImplementedException();
    }

}

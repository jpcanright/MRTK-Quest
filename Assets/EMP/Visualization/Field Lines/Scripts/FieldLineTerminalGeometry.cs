using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EMP.Core;
using EMP.Visualization;
using UnityEngine;

public abstract class FieldLineTerminalGeometry : MonoBehaviour
{
    public FieldGenerator associatedGenerator;

    protected List<CPUFieldLineModel> fieldLineModels = new List<CPUFieldLineModel>();

    protected Vector4[] storedSeeds;
    
    protected List<CPUFieldLineModel> _landedFieldLines = new List<CPUFieldLineModel>();
    
    /// <summary>
    /// Returns Math.Sign(strength of charge). WARNING: Works only for point charges.
    /// </summary>
    public int Sign
    {
        get
        {
            if (associatedGenerator.GetType() != typeof(PointChargeGenerator))
            {
                Debug.LogWarning("This getter only valid for PointChargeGenerator field generators.");
            }
            return Math.Sign(((PointChargeGenerator) associatedGenerator).monopoleStrength.Strength);
        }
    }

//    /// <summary>
//    /// Planar mode. Calculate each point of each field line associated with this geometry and stage for drawing.
//    /// Removed because the do-calculation command for each geometry should be independent of draw space setting.
//    /// </summary>
//    public virtual void CalculatePlanarFieldLines(int charge, Plane plane)
//    {
//        Vector4[] seeds = FieldLineSeedsInPlane(charge, plane);
//        AssociateModelsWithSeeds(seeds);
//        for (int i = 0; i < fieldLineModels.Count; i++)
//        {
//            CPUFieldLineModel model = fieldLineModels[i];
//            Vector4 seed = seeds[i];
//            Vector3 position = new Vector3(seed.x, seed.y, seed.z);
//            CPUFieldLineModel.IntegrationDirection integrationDirection = seed.w > 0
//                ? CPUFieldLineModel.IntegrationDirection.DownField
//                : CPUFieldLineModel.IntegrationDirection.UpField;
//            model.SetPositionAndIntegrationDirection(position, integrationDirection);
//            model.ExecuteCalculation();
//        }
//    }
//    /// <summary>
//    /// Volumetric mode. Calculate each point of each field line associated with this geometry and stage for drawing.
//    /// </summary>
//    public abstract void CalculateVolumetricFieldLines(int charge);

    public virtual void CalculateFieldLines()
    {
        AssociateModelsWithSeeds(storedSeeds);
        for (int i = 0; i < fieldLineModels.Count; i++)
        {
            CPUFieldLineModel model = fieldLineModels[i];
            Vector4 seed = storedSeeds[i];
            Vector3 position = new Vector3(seed.x, seed.y, seed.z);
            IntegrationDirection integrationDirection = seed.w > 0
                ? IntegrationDirection.FromNegative
                : IntegrationDirection.FromPositive;
            model.SetPositionAndIntegrationDirection(position, integrationDirection);
            model.ExecuteCalculation();
        }
    }

    public abstract Vector4[] RotateToMatchExistingFieldLines(List<CPUFieldLineModel> seeds);
    
    /// <summary>
    /// Cull field lines of the second kind which are made redundant by existing field lines of the first kind,
    /// and transform (rotate) this geometry to match positions of existing field lines.
    /// </summary>
    public abstract Vector4[] RejectSeedsForExistingFieldLines(List<CPUFieldLineModel> existingFieldLines);
    
    /// <summary>
    /// Draw staged field lines to scene. Should be called after calling either calculation method.
    /// </summary>
    public virtual void DrawCalculatedFieldLines()
    {
        for (int i = 0; i < fieldLineModels.Count; i++)
        {
            fieldLineModels[i].Draw();
        }
    }
    

    private void AssociateModelsWithSeeds(Vector4[] seeds)
    {
        if (fieldLineModels.Count == 0)
        {
            fieldLineModels = SeedsToNewModels(seeds);
        }
        else if(fieldLineModels.Count > seeds.Length)
        {
            for (int i = fieldLineModels.Count - 1; i >= seeds.Length; i--)
            {
                Destroy(fieldLineModels[i]);
                fieldLineModels[i] = null;
            }
        }
        else
        {
            while (fieldLineModels.Count < seeds.Length)
            {
                fieldLineModels.Add(SeedsToNewModels(seeds[fieldLineModels.Count]));
            }
        }
    }

    // We're reporting field line terminations directly to CPUFieldLineManager for the moment, so don't need this method.
//    public void ReportLandedFieldLine(CPUFieldLineModel model)
//    {
//        _landedFieldLines.Add(model);
//    }
    
    private List<CPUFieldLineModel> SeedsToNewModels(Vector4[] seeds)
    {
        List<CPUFieldLineModel> outList = new List<CPUFieldLineModel>();
        foreach (Vector4 seed in seeds)
        {
            var gameObj = new GameObject("FieldLineModel");
            gameObj.transform.SetParent(transform);
            CPUFieldLineModel newModel = gameObj.AddComponent<CPUFieldLineModel>();
            newModel.Initialize(CPUFieldLineManager.Instance.settings.calculationSettings,
                CPUFieldLineManager.Instance.FieldType,
                CPUFieldLineManager.Instance.settings.appearanceSettings,
                seed.w > 0 ? IntegrationDirection.FromNegative : IntegrationDirection.FromPositive);
            // Didn't seem to be working; changed Append to Add
            outList.Add(newModel);
        }

        return outList;
    }

    public void DebugTagAll(FieldLineDebugTag tag)
    {
        foreach (CPUFieldLineModel model in fieldLineModels)
        {
            model.debugTag = tag;
        }
    }
    
    public void DebugTagBySeed(Vector4[] seeds, FieldLineDebugTag tag)
    {
        List<CPUFieldLineModel> modelsToTag = ModelsAssociatedWithSeeds(seeds);
        foreach (CPUFieldLineModel model in modelsToTag)
        {
            model.debugTag = tag;
        }
    }
    
    public void DebugTagByModel(List<CPUFieldLineModel> models, FieldLineDebugTag tag)
    {
        foreach (CPUFieldLineModel model in models)
        {
            model.debugTag = tag;
        }
    }

    public void CullBySeed(Vector4[] seeds)
    {
        List<CPUFieldLineModel> modelsToCull = ModelsAssociatedWithSeeds(seeds);
        foreach (CPUFieldLineModel model in modelsToCull)
        {
            fieldLineModels.Remove(model);
            Destroy(model);
        }
    }
    
    /// <summary>
    /// Returns the CPUFieldLineModels associated with the given list of seeds, if they exist.
    /// Originally written so that culled field lines can instead be recolored.
    /// </summary>
    /// <param name="seeds"></param>
    /// <returns></returns>
    protected List<CPUFieldLineModel> ModelsAssociatedWithSeeds(Vector4[] seeds)
    {
        List<CPUFieldLineModel> outList = new List<CPUFieldLineModel>();

        foreach (Vector4 seed in seeds)
        {
            foreach(CPUFieldLineModel model in fieldLineModels)
            {
                Vector3 seedPosition = new Vector3(seed.x,seed.y,seed.z);
                Vector3 modelPosition = model.transform.position;
                if(Vector3.Magnitude(seedPosition - modelPosition) < 0.00001)
                {
                    outList.Add(model);
                }
            }
        }

        return outList;
    }
    
    private CPUFieldLineModel SeedsToNewModels(Vector4 seed)
    {
        
        var gameObj = new GameObject("FieldLineModel");
        gameObj.transform.SetParent(transform);
        CPUFieldLineModel newModel = gameObj.AddComponent<CPUFieldLineModel>();
        newModel.Initialize(CPUFieldLineManager.Instance.settings.calculationSettings,
            CPUFieldLineManager.Instance.FieldType,
            CPUFieldLineManager.Instance.settings.appearanceSettings,
            seed.w > 0 ? IntegrationDirection.FromNegative : IntegrationDirection.FromPositive);
        
        return newModel;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="point">3D position in world space.</param>
    /// <returns>True if the given point is within the extent of the terminal geometry, false otherwise.</returns>
    public abstract bool IsPointWithinGeometry(Vector3 point);
    
    /// <summary>
    /// Returns an array of field line terminals in the object's space. The first three components of each vector
    /// represent position, and the last represents whether the terminal is a source (+1) or a sink (-1).
    /// </summary>
    /// <param name="drawSpace"></param>
    /// <param name="charge"></param>
    /// <returns></returns>
    //public abstract Vector4[] FieldLinePositions(CPUFieldLineManager.FieldLineDrawSpace drawSpace, int charge);
    public abstract Vector4[] FieldLineSeedsInPlane(int charge, Plane plane);
    public abstract Vector4[] FieldLineSeedsInVolume(int charge);

}

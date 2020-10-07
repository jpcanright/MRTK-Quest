using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;

public class PaintManager : Singleton<PaintManager>
{
    private Dictionary<string, Material> permanentPaint = new Dictionary<string, Material>();
    [SerializeField] private Material defaultPaint;

    private List<Paintable> paintables = new List<Paintable>();
    
    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<string, List<Paintable>> identityDict = new Dictionary<string, List<Paintable>>();
        Dictionary<Material, List<Paintable>> materialDict = new Dictionary<Material, List<Paintable>>();
        
        // Assemble dictionaries
        foreach (Paintable paintable in paintables)
        {
            // Ignore any perm-painted particles
            if (paintable.permanentPaint)
            {
                continue;
            }
            
            string identity = paintable.particleIdentity;
            Material material = paintable.renderer.material;
            
            if (!identityDict.ContainsKey(identity))
            {
                identityDict[identity] = new List<Paintable>();
            }
            identityDict[identity].Add(paintable);
            
            if (!materialDict.ContainsKey(material))
            {
                materialDict[material] = new List<Paintable>();
            }
            materialDict[material].Add(paintable);
        }
        
        // Throw out default material
        materialDict.Remove(defaultPaint);

        // Identify any material lists of length 5 or greater
        Dictionary<Material, List<Paintable>> qualifyingMaterialDict = new Dictionary<Material, List<Paintable>>();
        foreach (var tempMaterial in materialDict.Keys)
        {
            if (materialDict[tempMaterial].Count >= 5 && !permanentPaint.ContainsValue(tempMaterial))
            {
                qualifyingMaterialDict[tempMaterial] = materialDict[tempMaterial];
            }
        }
        
        materialDict = qualifyingMaterialDict;
        if (materialDict.Count == 0)
        {
            return;
        }
        
        foreach (Material tempMaterial in materialDict.Keys)
        {
            bool qualifies = true;
            string cachedIdentity = materialDict[tempMaterial][0].particleIdentity;
            foreach (Paintable paintable in materialDict[tempMaterial])
            {
                if (cachedIdentity != paintable.particleIdentity)
                {
                    qualifies = false;
                }
            }

            if (qualifies)
            {
                foreach (Paintable paintable in materialDict[tempMaterial])
                {
                    paintable.PaintPermanent(tempMaterial);
                    permanentPaint[cachedIdentity] = tempMaterial;
                }
            }
        }
    }
    
    
    
    public void RegisterPaintable(Paintable paintable)
    {
        paintables.Add(paintable);
        if (permanentPaint.ContainsKey(paintable.particleIdentity))
        {
            paintable.PaintPermanent(permanentPaint[paintable.particleIdentity]);
        }
    }

    public void UnregisterPaintable(Paintable paintable)
    {
        paintables.Remove(paintable);
    }
}

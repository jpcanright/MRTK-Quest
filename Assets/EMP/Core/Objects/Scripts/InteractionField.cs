using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "FieldNameHere", menuName = "ScriptableObjects/InteractionField", order = 1)]
public class InteractionField : ScriptableObject
{
    
    /// <summary>
    /// Name of this interaction field, e.g. "Electric".
    /// </summary>
    public string name;

    /// <summary>
    /// Color to be used to represent this field with gizmos and vectors. Two-tone scalar colors are set with the
    /// VisColorSettings ScriptableObject class.
    /// </summary>
    public Color visualColor;

}

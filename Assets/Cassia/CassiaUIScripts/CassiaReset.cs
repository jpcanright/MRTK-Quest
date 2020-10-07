using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CassiaReset : MonoBehaviour
{
    public void ExecuteReset()
    {
        foreach (IResettable resettable in FindObjectsOfType<MonoBehaviour>().OfType<IResettable>())
        {
            resettable.OnSceneReset();
        }
    }
}

public interface IResettable
{
    void OnSceneReset();
}

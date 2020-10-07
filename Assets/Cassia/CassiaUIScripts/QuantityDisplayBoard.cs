using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantityDisplayBoard : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private string units = "";

    public delegate float QuantityDelegate();

    [SerializeField] private QuantityDelegate m_displayedQuantity;

    public void SetQuantityFunction(QuantityDelegate method)
    {
        m_displayedQuantity = method;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_displayedQuantity != null)
        {
            //text.text = m_displayedQuantity().ToString("F2") + units;
            
            // TODO this is an awful hack and you should fix it
            if (units == "m")
            {
                text.text = m_displayedQuantity().ToString("F2") + units;
            }
            else
            {
                // Info on string format specifier:
                // https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
                text.text = m_displayedQuantity().ToString("+0.##;-0.##;0") + units;
            }
            
        }
        else
        {
            text.text = "No Quantity";
        }
        
        Camera camera = Camera.main;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}

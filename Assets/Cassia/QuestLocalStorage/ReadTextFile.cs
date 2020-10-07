using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReadTextFile : MonoBehaviour
{
    public Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        StreamReader reader;        
#if UNITY_EDITOR
        reader = new StreamReader("C:/Users/jpcan/Documents/GitHub/Physics-Not-Included/Assets/Cassia/QuestLocalStorage/serial.txt");
        #elif UNITY_ANDROID
        reader = new StreamReader("/sdcard/TestFolder/serial.txt");
        #endif
        text.text = reader.ReadLine();
        reader.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

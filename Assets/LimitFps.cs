using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitFps : MonoBehaviour
{

    // You can change this to any color you like
    public Color textColor = Color.green;
    private float deltaTime = 0.0f;


    void Awake()
    {
        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();

        // Make the font 4x larger than before
        Rect rect = new Rect(10, h - (h * 8 / 100) - 10, w, h * 8 / 100);
        style.alignment = TextAnchor.LowerLeft;
        style.fontSize = h * 8 / 100;
        style.normal.textColor = textColor;

        // Calculate FPS
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // Display both ms and fps
        string text = string.Format("{0:0.0} ms ({1:0.} FPS)", msec, fps);
        GUI.Label(rect, text, style);
    }
}



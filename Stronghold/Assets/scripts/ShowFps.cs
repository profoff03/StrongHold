using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFps : MonoBehaviour
{
    //public static float fps;
    public Text FPSText;
    int FPSCounter = 0;
    float MTimeCounter = 0.0f;
    float LastFrameRate = 0.0f;
    public float RefreshTime = 0.5f;
    const string format = "{0} FPS";
    // Update is called once per frame
    void Update()
    {
        if (MTimeCounter < RefreshTime)
        {
            MTimeCounter += Time.deltaTime;
            FPSCounter++;
        }
        else
        {
            LastFrameRate = (float)FPSCounter / MTimeCounter;
            MTimeCounter = 0.0f;
            FPSCounter = 0;
        }

        FPSText.text = string.Format(format, (int)LastFrameRate);
    }

    //private void OnGUI()
    //{
    //    fps = 1f / Time.deltaTime;
    //    GUILayout.Label("FPS" + (int)fps);
    //}
}

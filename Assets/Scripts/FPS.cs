using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public int fps = 60;
    public bool update;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = fps;
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            Application.targetFrameRate = fps;
            update = false;
        }
        //Application.targetFrameRate = fps;
    }
}
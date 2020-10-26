using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using DlibFaceLandmarkDetectorExample;

public class EnvironmentRotation : MonoBehaviour
{
    public OriginalARHeadWebCamTextureExample arTexture;
    public Mat tvec;

    float xRotation = 90;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tvec = arTexture.tvec;
        if (tvec != null)
        {
            float tvec_x = (float)tvec.get(0, 0)[0];
            float tvec_y = (float)tvec.get(1, 0)[0] - 20;
            if (tvec_x < 0)
            {
                xRotation = Mathf.Atan(-tvec_y / tvec_x) * 180 / Mathf.PI + 180;
            }
            else
            {
                xRotation = Mathf.Atan(-tvec_y / tvec_x) * 180 / Mathf.PI;
            }
            //float xRotation = Mathf.Atan(-tvec_y/ tvec_x) * 180 / Mathf.PI * Mathf.Abs(tvec_x) / tvec_x;
            transform.localRotation = Quaternion.Euler(new Vector3(xRotation, -90, -90));
        }
    }
}

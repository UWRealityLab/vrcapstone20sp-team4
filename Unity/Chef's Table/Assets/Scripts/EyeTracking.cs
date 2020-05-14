using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class EyeTracking : MonoBehaviour
{
    #region Public Variables
    public int blinkMode = 0;
    public GameObject Interface;
    public Camera mainCamera;
    #endregion

    #region Private Variables
   // private Vector3 _heading;
    #endregion

    private float blinkUpdateTimer = 0;


    #region Unity Methods
    void Start()
    {
        MLEyes.Start();
        
        // Get the meshRenderer component
    }
    private void OnDisable()
    {
        MLEyes.Stop();
    }
    void Update()
    {
        if (MLEyes.IsStarted)
        {
            RaycastHit rayHit;
            Vector3 heading = MLEyes.FixationPoint - mainCamera.transform.position;


            if (Interface.activeSelf)
            {
                if (detectBlinking(blinkMode))
                {
                    Debug.Log("Mode: " + blinkMode + " Blink detected");
                    // handle the blocking issue now
                    Vector3 headtoInterface = Interface.transform.position - mainCamera.transform.position;
                    heading.Normalize();
                    headtoInterface.Normalize();
                    Vector3 c_product = Vector3.Cross(heading, headtoInterface);
                    float degree = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(heading, headtoInterface));
                    // Debug.Log(degree);
                    if (c_product.y > 0)
                    {
                        if (degree < 50)
                        {
                            Interface.transform.RotateAround(mainCamera.transform.position, Vector3.up, 60);
                        }
                        
                    } else
                    {
                        if (degree < 50) { 
                            Interface.transform.RotateAround(mainCamera.transform.position, Vector3.up, -60);
                        }
                        
                    }


                    
                }
            }


        }
    }
    #endregion


    bool detectBlinking(int mode)
    {
        MLEyes.MLEye right = MLEyes.RightEye;
        MLEyes.MLEye left = MLEyes.LeftEye;
        

        if (mode == 1)
        {
                // mode 2 close for 2 sec
            if (right.IsBlinking && left.IsBlinking)
            {
                blinkUpdateTimer += Time.deltaTime;
            } else
            {
                blinkUpdateTimer = 0;
            }
        }
        else
        {
            // mode 3, close right eye for 2 seconds
            if (right.IsBlinking && !left.IsBlinking)
            {
                blinkUpdateTimer += Time.deltaTime;
            }
            else
            {
                blinkUpdateTimer = 0;
            }

        }
        if (blinkUpdateTimer > 2)
        {
            blinkUpdateTimer = 0;
           
            return true;
        }


        return false;
    }
}


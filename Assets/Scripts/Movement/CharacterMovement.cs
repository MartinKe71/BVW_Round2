using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using DlibFaceLandmarkDetectorExample;
using TMPro;
using OpenCVForUnity.UnityUtils;
using DG.Tweening;

public class CharacterMovement : MonoBehaviour
{
    public TextMeshProUGUI isMoveText;
    public TextMeshProUGUI isAttackText;

    [Header("Player Status")]
    public bool isWalk = false;
    public bool isAttack = false;

    [Header("Movement Params")]
    public float _walkSpeed;
    public float _runSpeed;   
    public float _walkThreshold;
    public float _runThreshold;
    public float _camSensitivity;
    public float _angleStep;
    
    public float _faceRotation;
    public float rvec_y;

    [Header("Camera Rotation")]
    public Camera cam;

    [Header("Get face rotation and position")]
    public ARHeadWebCamTextureExample arTexture;
    public Vector3 tVec = new Vector3(0, 0, 0);
    public Vector3 rVec = new Vector3(0, 0, 0);
    float _rvecYOffset = -2f;
    Mat tvec;
    Mat rvec;
    public bool turningBack = false;
    int turningCount = 0;
    int _historySize;
    float[] _yRotationHistory = new float[20];
    float _yRotateOffset;
    bool _isMouthOpen;

    // Start is called before the first frame update
    void Start()
    {
        _yRotateOffset = transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {

        SetWalkStatus();
        SetRotation();
        SetAttack();
    }

    private void SetWalkStatus()
    {
        tvec = arTexture.tvec;
        if (tvec != null)
        {
            tVec.x = (float)tvec.get(0, 0)[0];
            tVec.y = (float)tvec.get(1, 0)[0];
            tVec.z = Mathf.Abs((float)tvec.get(2, 0)[0]);
            // Set run and walk threshold once get tvec
            if (_runThreshold == 0)
            {
                _runThreshold = tVec.z * 0.9f;
                _walkThreshold = tVec.z * 1.1f;
            }
            // Set movement
            if (tVec.z < _runThreshold)
            {
                transform.position += transform.forward * _runSpeed * Time.deltaTime;
                isMoveText.text = "The player is running";
                isWalk = true;
            }
            else if (tVec.z < _walkThreshold)
            {
                transform.position += transform.forward * _walkSpeed * Time.deltaTime;
                isMoveText.text = "The player is walking";
                isWalk = true;
            }
            else
            {
                isMoveText.text = "the player walks back";
                transform.position -= transform.forward * _walkSpeed * Time.deltaTime;
                isWalk = false;
            }
        }
    }

    private void SetRotation()
    {
        // check whether rotation 180 degree back
        rvec = arTexture.rvec;
        
        if (rvec != null)
        {
            // set public rVec vector
            rVec.x = (float)rvec.get(0, 0)[0];
            rVec.y = (float)rvec.get(1, 0)[0];
            rVec.z = (float)rvec.get(2, 0)[0];

            if (_rvecYOffset == -2f)
            {
                _rvecYOffset = rVec.z;
            }

            rvec_y = -((float)rvec.get(2, 0)[0] - _rvecYOffset) * Mathf.Rad2Deg;

            if (Mathf.Abs(rvec_y) > 34f && !turningBack)
            {
                if (rvec_y > 0)
                {
                    _yRotateOffset = (_yRotateOffset + 90f);
                }
                else
                {
                    _yRotateOffset = (_yRotateOffset - 90f);
                }
                turningBack = true;
                StartCoroutine(ResetTurningBack());
            }

            //rvec_y = Mathf.Floor(rvec_y /_angleStep + 0.5f) * _angleStep * _camSensitivity;
            rvec_y *= _camSensitivity;

            Quaternion toRotation = Quaternion.Euler(new Vector3(0, rvec_y + _yRotateOffset, 0));
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 2f);
        }
    }

    private void SetAttack()
    {
        _isMouthOpen = arTexture.isMouthOpen;
        if (_isMouthOpen)
        {
            isAttackText.text = "Attack!";
            isAttack = true;
        }
        else
        {
            isAttackText.text = "Not Attack";
            isAttack = false;
        }
    }

    IEnumerator ResetTurningBack()
    {
        yield return new WaitForSecondsRealtime(2f);
        turningBack = false;
    }
}

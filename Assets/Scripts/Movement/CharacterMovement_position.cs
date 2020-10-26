using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using DlibFaceLandmarkDetectorExample;
using TMPro;
using OpenCVForUnity.UnityUtils;
using DG.Tweening;

public enum ControlScheme
{
    FaceRotation,
    FacePosition,
    Hybrid
}

[System.Serializable]
public class CharacterSpeed
{
    public GuestState guestState;
    public float walkSpeed;
    public float runSpeed;
    public bool canVerticalMove;
}
public class CharacterMovement_position : MonoBehaviour
{
    public TextMeshProUGUI isMoveText;
    public TextMeshProUGUI isAttackText;
    public Image upFang;
    public Image downFang;

    [Header("Player Status")]
    public bool isWalk = false;
    public bool isAttack = false;

    [Header("Movement Params")]
    public ControlScheme _controlScheme;
    public bool _invertX = false;
    public bool _calibrateHeadPos = false;
    public Vector3 _movingDirection = new Vector3(0, 0, 0);

    public List<CharacterSpeed> characterSpeedSets;
    public CharacterSpeed _characterSpeed;  
    public float _walkThreshold;
    public float _runThreshold;
    public float _xHeadRadius = 0f;
    public float _radiuToHeadRatio = 1f;
    public float _angleRange;

    float _rotationMultiplier;
    Rigidbody rb;

    [Header("Camera Rotation")]
    public Camera cam;
    public float _camSensitivity;


    [Header("Get face rotation and position")]
    public Vector3 tVec = new Vector3(0, 0, 0);
    public Vector3 rVec = new Vector3(0, 0, 0);
    public float tVec_x;
    public float rVec_x;
    public float rVec_y;
    public float _faceRotation;
    public float _verticalRotation;
    public float _xHeadOffset = 0f;
    float _rvecYOffset = -2f;

    public ARHeadWebCamTextureExample arTexture;
    Mat tvec;
    Mat rvec;
    public bool turningBack = false;
    float _yRotateOffset;
    float _yRotateOffsetRotation;
    float _yRotateOffsetPosition;
    bool _isMouthOpen;

    TriggerDrinkBlood triggerDrinkBlood;
    TerrainDetector terrainDetector;

    delegate void SetRotationOption();
    SetRotationOption _setRotation;

    // Start is called before the first frame update
    void Start()
    {
        // get components
        rb = GetComponent<Rigidbody>();
        triggerDrinkBlood = GetComponent<TriggerDrinkBlood>();
        cam = GetComponentInChildren<Camera>();

        // set variables
        terrainDetector = new TerrainDetector();
        _yRotateOffset = transform.rotation.eulerAngles.y;
        _yRotateOffsetPosition = _yRotateOffset;
        _yRotateOffsetRotation = _yRotateOffset;

        switch (_controlScheme)
        {
            case ControlScheme.FacePosition:
                _setRotation = SetRotationByFacePosition;
                break;
            case ControlScheme.FaceRotation:
                _setRotation = SetRotationByFaceRotation;
                break;
            case ControlScheme.Hybrid:
                _setRotation = SetRotationHybrid;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // set up head detection radius once the face was found
        // we can do it in update just in case the guest get closer to the camera and their faces get larger
        // later business though
        if (_xHeadRadius == 0)
        {
            if (arTexture.ears != null )
            {
                _xHeadRadius = _radiuToHeadRatio * Mathf.Abs((float)(arTexture.ears[0].x - arTexture.ears[1].x));
            }
        }

        if (!triggerDrinkBlood.isAttacking)
        {
            _setRotation();
        }

        SetAttack();
    }

    private void FixedUpdate() 
    {
        if (!triggerDrinkBlood.isAttacking)
        {
            SetWalkStatus();
        }
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
                _walkThreshold = tVec.z * 1.2f;
                if (_calibrateHeadPos) _xHeadOffset = tVec.x;
            }

            // Set movement direction and speed
            _movingDirection = transform.forward;
            _characterSpeed = characterSpeedSets.Find(x => x.guestState == GetComponent<Transformation>().currentGuestState);
            if (!_characterSpeed.canVerticalMove)
            {
                _movingDirection.y = 0f;
            }

            // Set movement
            if (tVec.z < _runThreshold)
            {
                // Run
                rb.MovePosition( transform.position + _movingDirection * _characterSpeed.runSpeed * Time.deltaTime);
                // transform.position += _movingDirection * _characterSpeed.runSpeed * Time.deltaTime;
                isMoveText.text = "The player is running";
                SoundManager.instance.isMoving = true;
                SoundManager.instance.isRunning = true;

            }
            else if (tVec.z < _walkThreshold)
            {
                // Walk
                rb.MovePosition( transform.position + _movingDirection * _characterSpeed.walkSpeed * Time.deltaTime);
                // transform.position += _movingDirection * _characterSpeed.walkSpeed * Time.deltaTime;
                isMoveText.text = "The player is walking";
                SoundManager.instance.isMoving = true;
                SoundManager.instance.isRunning = false;
            }
            else
            {
                // Walk back (can be stop, will discuss later)
                //transform.position -= _movingDirection * _characterSpeed.walkSpeed * Time.deltaTime;

                isMoveText.text = "the player stops";
                SoundManager.instance.isMoving = false;
                SoundManager.instance.isRunning = false;
            }
        }
        else
        {
            SoundManager.instance.isMoving = false;
        }
        if (GetComponent<Transformation>().currentGuestState == GuestState.Vampier)
        {
            PlayMovementSound();
        }
        else
        {
            SoundManager.instance.StopFootstep();
        }
    }

    private void SetRotationByFaceRotation()
    {
        rvec = arTexture.rvec;
        _rotationMultiplier = _invertX ? -1 : 1;

        if (rvec != null)
        {
            // set public rVec vector
            rVec.x = (float)rvec.get(0, 0)[0];
            rVec.y = (float)rvec.get(1, 0)[0];
            rVec.z = (float)rvec.get(2, 0)[0];

            rVec_y = -((float)rvec.get(2, 0)[0]) * Mathf.Rad2Deg;

            if (Mathf.Abs(rVec_y) > 34f)
            {
                if (rVec_y > 0)
                {
                    _yRotateOffset += 40f * Time.deltaTime;
                }
                else
                {
                    _yRotateOffset -= 40f * Time.deltaTime;
                }
            }
            else if (Mathf.Abs(rVec_y) > 5f)
            {
                if (rVec_y > 0)
                {
                    _yRotateOffset += 20f * Time.deltaTime;
                }
                else
                {
                    _yRotateOffset -= 20f * Time.deltaTime;
                }
            }

            //rvec_y = Mathf.Floor(rvec_y /_angleStep + 0.5f) * _angleStep * _camSensitivity;
            rVec_y *= _camSensitivity;

            Quaternion toRotation = Quaternion.Euler(new Vector3(0, rVec_y + _yRotateOffset, 0) * _rotationMultiplier) ;
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 2f);

            rVec_x = (float)rvec.get(0, 0)[0];
            //_verticalRotation = Mathf.Clamp((rVec_x + Mathf.PI) * Mathf.Rad2Deg, -30f, 30f) ;
            _verticalRotation = (rVec_x + Mathf.PI) * Mathf.Rad2Deg;

            Quaternion camRotation = Quaternion.Euler(new Vector3(_verticalRotation, 0, 0));
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, camRotation, Time.deltaTime * 2f);
        }
    }

    private void SetRotationByFacePosition()
    {
        rvec = arTexture.rvec;
        _rotationMultiplier = _invertX ? -1 : 1;

        if (tvec != null)
        {
            // Set horizontal rotation
            tVec_x = tVec.x - _xHeadOffset;

            // Regular rotation within the head range
            if (Mathf.Abs(tVec_x) < _xHeadRadius)
            {
                _faceRotation = tVec_x / _xHeadRadius * _angleRange;
                // Dynamically adjust toward the moving direction
                if (tVec_x > 0)
                {
                    _yRotateOffset += 20f * Time.deltaTime * Mathf.Abs(tVec_x / _xHeadRadius);
                }
                else
                {
                    _yRotateOffset -= 20f * Time.deltaTime * Mathf.Abs(tVec_x / _xHeadRadius);
                }                
            }
            else
            {
                if (tVec_x > 0)
                {
                    _yRotateOffset += 40f * Time.deltaTime * Mathf.Abs(tVec_x/_xHeadRadius);
                }
                else
                {
                    _yRotateOffset -= 40f * Time.deltaTime * Mathf.Abs(tVec_x / _xHeadRadius);
                }
            }

            // Set vertical rotation
            if (rvec != null)
            {
                rVec_x = (float)rvec.get(0, 0)[0];
                //_verticalRotation = Mathf.Clamp((rVec_x + Mathf.PI) * Mathf.Rad2Deg, -30f, 30f) ;
                _verticalRotation = (rVec_x + Mathf.PI) * Mathf.Rad2Deg;
            }

            Quaternion toRotation = Quaternion.Euler(new Vector3(0, _faceRotation + _yRotateOffset, 0) * _rotationMultiplier);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 2f);

            Quaternion camRotation = Quaternion.Euler(new Vector3(_verticalRotation, 0, 0) * _rotationMultiplier);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, camRotation, Time.deltaTime * 2f);
        }
    }

    private void SetRotationHybrid()
    {
        rvec = arTexture.rvec;
        _rotationMultiplier = _invertX ? -1 : 1;

        if (tvec != null && rvec != null)
        {
            // Set horizontal rotation
            tVec_x = tVec.x - _xHeadOffset;

            // Regular rotation within the head range
            if (Mathf.Abs(tVec_x) < _xHeadRadius)
            {
                _faceRotation = tVec_x / _xHeadRadius * _angleRange;
                // Dynamically adjust toward the moving direction
                if (tVec_x > 0)
                {
                    _yRotateOffsetPosition += 20f * Time.deltaTime * Mathf.Abs(tVec_x / _xHeadRadius);
                }
                else
                {
                    _yRotateOffsetPosition -= 20f * Time.deltaTime * Mathf.Abs(tVec_x / _xHeadRadius);
                }
            }
            else
            {
                if (tVec_x > 0)
                {
                    _yRotateOffsetPosition += 40f * Time.deltaTime * Mathf.Abs(tVec_x / _xHeadRadius);
                }
                else
                {
                    _yRotateOffsetPosition -= 40f * Time.deltaTime * Mathf.Abs(tVec_x / _xHeadRadius);
                }

            }

            // set public rVec vector
            rVec.x = (float)rvec.get(0, 0)[0];
            rVec.y = (float)rvec.get(1, 0)[0];
            rVec.z = (float)rvec.get(2, 0)[0];

            rVec_y = -((float)rvec.get(2, 0)[0]) * Mathf.Rad2Deg;

            if (Mathf.Abs(rVec_y) > 34f)
            {
                if (rVec_y > 0)
                {
                    _yRotateOffsetRotation += 40f * Time.deltaTime;
                }
                else
                {
                    _yRotateOffsetRotation -= 40f * Time.deltaTime;
                }
            }
            else if (Mathf.Abs(rVec_y) > 5f)
            {
                if (rVec_y > 0)
                {
                    _yRotateOffsetRotation += 20f * Time.deltaTime;
                }
                else
                {
                    _yRotateOffsetRotation -= 20f * Time.deltaTime;
                }
            }

            //rvec_y = Mathf.Floor(rvec_y /_angleStep + 0.5f) * _angleStep * _camSensitivity;
            rVec_y *= _camSensitivity;

            // Set vertical rotation
            
            rVec_x = (float)rvec.get(0, 0)[0];
            //_verticalRotation = Mathf.Clamp((rVec_x + Mathf.PI) * Mathf.Rad2Deg, -30f, 30f) ;
            _verticalRotation = (rVec_x + Mathf.PI) * Mathf.Rad2Deg;

            float rotationY = (Mathf.Abs(rVec_y + _yRotateOffsetRotation) > Mathf.Abs(_faceRotation + _yRotateOffsetPosition))
                ? rVec_y + _yRotateOffsetRotation
                : _faceRotation + _yRotateOffsetPosition;


            Quaternion toRotation = Quaternion.Euler(new Vector3(0, rotationY, 0) * _rotationMultiplier);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 2f);

            Quaternion camRotation = Quaternion.Euler(new Vector3(_verticalRotation, 0, 0) * _rotationMultiplier);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, camRotation, Time.deltaTime * 2f);
        }
    }

    private void SetAttack()
    {
        if (!GetComponent<TriggerDrinkBlood>().isAttacking)
        {
            _isMouthOpen = arTexture.isMouthOpen;
            if (_isMouthOpen)
            {
                //Debug.Log("Mouse/Nose Ratio is: "+ arTexture.mouseDistance * 5f/arTexture.noseDistance);

                if (!upFang.enabled)
                {
                    upFang.enabled = true;
                    downFang.enabled = true;
                }
                float mouthNoseRatio = Mathf.Clamp(arTexture.mouseDistance * 5f / arTexture.noseDistance, 1, 2);
                Vector2 fangPos = new Vector2(0, 100 * mouthNoseRatio * mouthNoseRatio);
                upFang.rectTransform.anchoredPosition = Vector2.Lerp(upFang.rectTransform.anchoredPosition, fangPos, Time.deltaTime);
                downFang.rectTransform.anchoredPosition = Vector2.Lerp(downFang.rectTransform.anchoredPosition, -fangPos, Time.deltaTime);


                isAttackText.text = "Attack!";
                isAttack = true;
            }
            else
            {
                upFang.rectTransform.anchoredPosition = new Vector2(0, 100);
                upFang.enabled = false;

                downFang.rectTransform.anchoredPosition = new Vector2(0, -100);
                downFang.enabled = false;

                isAttackText.text = "Not Attack";
                isAttack = false;
            }
        }
        else
        {
            upFang.enabled = false;
            downFang.enabled = false;
            isAttack = false;
        }
    }

    private void PlayMovementSound()
    {
        // int textureIndex = terrainDetector.GetTerrainTextureIndex(transform.position);
        // switch (textureIndex)
        // {
        //     case 0:
        //         SoundManager.instance.FootstepOnGrass();
        //         return;
        //     case 1:
        //         SoundManager.instance.FootstepOnRoad();
        //         return;
        //     case 2:
        //         SoundManager.instance.FootstepOnRoad();
        //         return;
        //     default:
        //         Debug.Log("In Default!!");
        //         return;
        // }
    }

    IEnumerator ResetTurningBack()
    {
        yield return new WaitForSecondsRealtime(2f);
        turningBack = false;
    }
}

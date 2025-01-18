using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _minXAngle = -85; //min angle around x axis

    [SerializeField]
    private float _maxXAngle = 85; // max angle around x axis

    [SerializeField]
    private Slider _changeDistance;

    public static Vector3Int LocalForward = Vector3Int.up;

    public static Vector3Int LocalRight = Vector3Int.right;

    private float _targetHorizontalRotation = 0;

    private float _mouseRotateSpeed = 5f;

    //change in settings
    private float _touchRotateSpeed = 0.1f;
    //change in settings

    private float _slerpSmoothValue = 0.3f;

    private float _scrollSmoothTime = 0.12f;
    private float _editorFOVSensitivity = 5f;
    private float _touchFOVSensitivity = 5f;

    //Can we rotate camera, which means we are not blocking the view
    [SerializeField]
    private bool _isCanRotate = true;

    private Vector2 _swipeDirection; //swipe delta vector2

    private Vector2 _touch1OldPos;
    private Vector2 _touch2OldPos;

    private Vector2 _touch1CurrentPos;
    private Vector2 _touch2CurrentPos;

    private Quaternion _currentRot; // store the quaternion after the slerp operation
    private Quaternion _targetRot;

    private Touch _touch;

    //Mouse rotation related
    private float _rotX; // around x
    private float _rotY; // around y
                        //Mouse Scroll
    private float _cameraFieldOfView;
    private float _cameraFOVDamp; //Damped value
    private float _fovChangeVelocity = 0;

    private float _minCameraFieldOfView = 6;
    private float _maxCameraFieldOfView = 30;

    private Vector3 _baseDirection;

    private Vector3 _direction;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        float distanceBetweenCameraAndTarget = Vector3.Distance(_camera.transform.position, _target.position);

        _direction = new Vector3(0, 0, distanceBetweenCameraAndTarget);//assign value to the distance between the maincamera and the target

        _baseDirection = _direction;

        _camera.transform.position = _target.position + _direction; //Initialize camera position

        //_cameraFOVDamp = _camera.fieldOfView;
        //_cameraFieldOfView = _camera.fieldOfView;
    }

    public void ResetDirection()
    {
        _direction = _baseDirection;
    }

    public void SetDistanceBetweenCameraAndTarget(float distance)//for slider
    {
        _direction.z = distance;
    }

    public void IncreaseDistanceBetweenCameraAndTarget(float value)//for script
    {
        _direction.z += value;

        _changeDistance.minValue += value;

        _changeDistance.maxValue += value;
    }

    public void SetTouchRotateSpeed(float speed)
    {
        _touchRotateSpeed = speed;
    }

    private void Update()
    {
        if (!_isCanRotate)
        {
            return;
        }
        //We are in editor
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            EditorCameraInput();
        }
        else //We are in mobile mode
        {
            TouchCameraInput();
        }

        _targetHorizontalRotation = transform.eulerAngles.y;

        _targetHorizontalRotation = _targetHorizontalRotation % 360;

        Quaternion rotation = Quaternion.Euler(0, 0, -_targetHorizontalRotation);

        LocalForward = RotateRoundToInt(rotation, Vector3Int.up);
        LocalRight = RotateRoundToInt(rotation, Vector3Int.right);
    }

    private Vector3Int RotateRoundToInt(Quaternion rotation, Vector3Int vector)
    {
        Vector3 newVector = rotation * vector;
        return new Vector3Int(Mathf.RoundToInt(newVector.x), Mathf.RoundToInt(newVector.y), Mathf.RoundToInt(newVector.z));
    }

    private void LateUpdate()
    {
        if (!_isCanRotate)
        {
            return;
        }

        RotateCamera();
        //SetCameraFOV();
    }

    private void EditorCameraInput()
    {
        //Camera Rotation
        if (Input.GetMouseButton(0))
        {
            _rotX += Input.GetAxis("Mouse Y") * _mouseRotateSpeed; // around X
            _rotY += Input.GetAxis("Mouse X") * _mouseRotateSpeed;

            if (_rotX < _minXAngle)
            {
                _rotX = _minXAngle;
            }
            else if (_rotX > _maxXAngle)
            {
                _rotX = _maxXAngle;
            }
        }
        //Camera Field Of View
        if (Input.mouseScrollDelta.magnitude > 0)
        {
            _cameraFieldOfView += Input.mouseScrollDelta.y * _editorFOVSensitivity * -1;//-1 make FOV change natual
        }
    }

    private void TouchCameraInput()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                _touch = Input.GetTouch(0);

                if (_touch.phase == TouchPhase.Began)
                {
                    //Debug.Log("Touch Began");
                }
                else if (_touch.phase == TouchPhase.Moved)  // the problem lies in we are still rotating object even if we move our finger toward another direction
                {
                    _swipeDirection.x += _touch.deltaPosition.x * _touchRotateSpeed; //-1 make rotate direction natural
                    _swipeDirection.y += -_touch.deltaPosition.y * _touchRotateSpeed;
                }
                else if (_touch.phase == TouchPhase.Ended)
                {
                    //Debug.Log("Touch Ended");
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);
                if (touch1.phase == TouchPhase.Began && touch2.phase == TouchPhase.Began)
                {
                    _touch1OldPos = touch1.position;
                    _touch2OldPos = touch2.position;
                }
                if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
                {
                    _touch1CurrentPos = touch1.position;
                    _touch2CurrentPos = touch2.position;

                    float deltaDistance = Vector2.Distance(_touch1CurrentPos, _touch2CurrentPos) - Vector2.Distance(_touch1OldPos, _touch2OldPos);

                    _cameraFieldOfView += deltaDistance * -1 * _touchFOVSensitivity; // Make rotate direction natual

                    _touch1OldPos = _touch1CurrentPos;
                    _touch2OldPos = _touch2CurrentPos;
                }
            }
        }

        if (_swipeDirection.y < _minXAngle)
        {
            _swipeDirection.y = _minXAngle;
        }
        else if (_swipeDirection.y > _maxXAngle)
        {
            _swipeDirection.y = _maxXAngle;
        }
    }

    public void SetOrthographic(Vector3 pos)
    {
        _isCanRotate = false;
        _camera.orthographic = true;

        Vector3 newsd = new Vector3(0f, pos.y, Mathf.Abs(_camera.transform.position.z));

        _camera.transform.position = newsd;


        _camera.transform.rotation = Quaternion.Euler(0, -180, 0);

        _targetHorizontalRotation = transform.eulerAngles.y;

        _targetHorizontalRotation = _targetHorizontalRotation % 360;

        Quaternion rotation = Quaternion.Euler(0, 0, -_targetHorizontalRotation);

        LocalForward = RotateRoundToInt(rotation, Vector3Int.up);
        LocalRight = RotateRoundToInt(rotation, Vector3Int.right);
    }


    public void UnSetOrthographic()
    {
        _isCanRotate = true;
        _camera.orthographic = false;
    }

    private void RotateCamera()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Vector3 tempV = new Vector3(_rotX, _rotY, 0);
            _targetRot = Quaternion.Euler(tempV); //We are setting the rotation around X, Y, Z axis respectively
        }
        else
        {
            _targetRot = Quaternion.Euler(-_swipeDirection.y, _swipeDirection.x, 0);
        }
        //Rotate Camera
        _currentRot = Quaternion.Slerp(_currentRot, _targetRot, Time.smoothDeltaTime * _slerpSmoothValue * 50);  //let cameraRot value gradually reach newQ which corresponds to our touch
                                                                                                             //Multiplying a quaternion by a Vector3 is essentially to apply the rotation to the Vector3
                                                                                                             //This case it's like rotate a stick the length of the distance between the camera and the target and then look at the target to rotate the camera.
        _camera.transform.position = _target.position + _currentRot * _direction;
        _camera.transform.LookAt(_target.position);
    }

    //private void SetCameraFOV()
    //{
    //    //Set Camera Field Of View
    //    //Clamp Camera FOV value
    //    if (cameraFieldOfView <= minCameraFieldOfView)
    //    {
    //        cameraFieldOfView = minCameraFieldOfView;
    //    }
    //    else if (cameraFieldOfView >= maxCameraFieldOfView)
    //    {
    //        cameraFieldOfView = maxCameraFieldOfView;
    //    }

    //    cameraFOVDamp = Mathf.SmoothDamp(cameraFOVDamp, cameraFieldOfView, ref fovChangeVelocity, scrollSmoothTime);
    //    _camera.fieldOfView = cameraFOVDamp;
    //}
}
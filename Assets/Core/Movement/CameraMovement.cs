using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Transform _target;

    [Space(10)]
    [SerializeField]
    private float _minXAngle = -85;

    [SerializeField]
    private float _maxXAngle = 85;

    [Space(10)]
    [SerializeField]
    private Slider _changeDistance;

    private Vector3 _baseDirection;

    private Vector3 _direction;

    private Touch _touch;

    private Vector2 _swipeDirection;

    private Vector2 _touch1OldPos;
    private Vector2 _touch2OldPos;

    private Vector2 _touch1CurrentPos;
    private Vector2 _touch2CurrentPos;

    private Quaternion _currentRotation; // store the quaternion after the slerp operation
    private Quaternion _targetRotation;

    private float _rotateX; // around x
    private float _rotateY; // around y
    //Mouse Scroll
    private float _cameraFOV;

    private float _cameraFOVDamp;

    private float _fovChangeVelocity = 0;

    private float _minCameraFOV = 6;
    private float _maxCameraFOV = 30;

    private float _targetHorizontalRotation = 0;

    private float _mouseRotateSpeed = 5f;

    private float _touchRotateSpeed = 0.1f;

    private float _slerpSmoothValue = 0.3f;

    private float _scrollSmoothTime = 0.12f;

    private float _editorFOVSensitivity = 5f;
    private float _touchFOVSensitivity = 5f;

    private bool _isCanRotate = true;

    public static Vector3Int LocalForward = Vector3Int.up;

    public static Vector3Int LocalRight = Vector3Int.right;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        float distanceBetweenCameraAndTarget = Vector3.Distance(_camera.transform.position, _target.position);

        _direction = new Vector3(0, 0, distanceBetweenCameraAndTarget);

        _baseDirection = _direction;

        _camera.transform.position = _target.position + _direction;

        _changeDistance.minValue = distanceBetweenCameraAndTarget;

        _changeDistance.onValueChanged.AddListener(SetDistanceBetweenCameraAndTarget);
    }

    public void SetTouchRotateSpeed(float speed) => _touchRotateSpeed = speed;

    public void ResetDirection() => _direction = _baseDirection;

    public void SetDistanceBetweenCameraAndTarget(float distance) => _direction.z = distance;

    public void IncreaseDistanceBetweenCameraAndTarget(float value)
    {
        _direction.z += value;

        _changeDistance.minValue += value;

        _changeDistance.maxValue += value;
    }

    public void SetOrthographic(bool value, float positionY)
    {
        _isCanRotate = !value;

        _camera.orthographic = value;

        Vector3 positionCamera = new Vector3(0f, positionY, Mathf.Abs(_camera.transform.position.z));

        _camera.transform.SetPositionAndRotation(positionCamera, Quaternion.Euler(0, -180, 0));

        InitLocalDirections();
    }

    private void Update()
    {
        if (_isCanRotate == false)
            return;

        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            EditorCameraInput();
        else
            TouchCameraInput();

        InitLocalDirections();
    }

    private void InitLocalDirections()
    {
        _targetHorizontalRotation = transform.eulerAngles.y;

        _targetHorizontalRotation = _targetHorizontalRotation % 360;

        Quaternion rotation = Quaternion.Euler(0, 0, -_targetHorizontalRotation);

        LocalForward = MathfCalculations.RotateRoundToInt(rotation, Vector3Int.up);
        LocalRight = MathfCalculations.RotateRoundToInt(rotation, Vector3Int.right);
    }

    private void LateUpdate()
    {
        if (_isCanRotate == false)
            return;

        RotateCamera();
    }

    private void EditorCameraInput()
    {
        if (Input.GetMouseButton(0))
        {
            _rotateX += Input.GetAxis("Mouse Y") * _mouseRotateSpeed; // around X
            _rotateY += Input.GetAxis("Mouse X") * _mouseRotateSpeed;

            if (_rotateX < _minXAngle)
            {
                _rotateX = _minXAngle;
            }
            else if (_rotateX > _maxXAngle)
            {
                _rotateX = _maxXAngle;
            }
        }
        //Camera Field Of View
        if (Input.mouseScrollDelta.magnitude > 0)
        {
            _cameraFOV += Input.mouseScrollDelta.y * _editorFOVSensitivity * -1;//-1 make FOV change natual
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

                    _cameraFOV += deltaDistance * -1 * _touchFOVSensitivity; // Make rotate direction natual

                    _touch1OldPos = _touch1CurrentPos;
                    _touch2OldPos = _touch2CurrentPos;
                }
            }
        }

        if (_swipeDirection.y < _minXAngle)
            _swipeDirection.y = _minXAngle;
        else if (_swipeDirection.y > _maxXAngle)
            _swipeDirection.y = _maxXAngle;
    }

    private void RotateCamera()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Vector3 tempV = new Vector3(_rotateX, _rotateY, 0);
            _targetRotation = Quaternion.Euler(tempV); //We are setting the rotation around X, Y, Z axis respectively
        }
        else
        {
            _targetRotation = Quaternion.Euler(-_swipeDirection.y, _swipeDirection.x, 0);
        }
        //Rotate Camera
        _currentRotation = Quaternion.Slerp(_currentRotation, _targetRotation, Time.smoothDeltaTime * _slerpSmoothValue * 50);

        _camera.transform.position = _target.position + _currentRotation * _direction;
        _camera.transform.LookAt(_target.position);
    }
}
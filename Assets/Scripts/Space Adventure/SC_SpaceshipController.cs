using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.LegacyInputHelpers;
using TMPro;

[RequireComponent(typeof(Rigidbody))]

public class SC_SpaceshipController : MonoBehaviour
{
    public float normalSpeed = 25f;
    public float accelerationSpeed = 45f;
    public float warpSpeed = 1000f;

    [SerializeField]
    private GetInputValues _getInputValues;

    private float HorizontalAxis, VerticalAxis, HorizontalDirection, VerticalDirection;

    [SerializeField]
    private float _maxSpeed;

    [SerializeField]
    private TextMesh _displaySpeed;

    [SerializeField]
    private TextMeshProUGUI _displayInfo;

    [SerializeField]
    private Transform rearCameraPosition;

    [SerializeField]
    private Transform _joystick, _throttleControl, _turretAnchor;

    [SerializeField]
    private Camera rearCamera;

    //[SerializeField]
    //private StarShipSetup _starShipSetup;

    //[SerializeField]
    //private SA_PlayerInput _playerInput;

    [SerializeField]
    private ParticleSystem[] _mainThrusters, _backThrusters;

    [SerializeField]
    private AudioClip _engineIdle, _engineSlow, _engineOn, _engineWarp;

    //[SerializeField]
    //private Animator _animator;

    private AudioSource _audioSource;

    public Transform spaceshipRoot;
    public float rotationSpeed = 2.0f;
    public float cameraSmooth = 4f;
    private float throttleAmount;
    private float verticalAxis;
    private bool _isBoosting, _isWarping;
    private bool _wasBoosting, _wasWarping;
    private bool _freelook;
    private bool _isCameraAligned = true;

    private Quaternion nullQuaternion = Quaternion.identity;

    //private Quaternion nullQuaternion = Quaternion.identity;

    public RectTransform crosshairTexture;

    [SerializeField]
    private float _timeToMaxSpeed = 3f;
    //private float _resetTimeToMaxSpeed;

    float speed;
    float rotationZTmp;
    Rigidbody r;
    Quaternion lookRotation;
    Quaternion cameraLookRotation;
    float rotationZ = 0;
    float mouseXSmooth = 0;
    float mouseYSmooth = 0;
    Vector3 defaultShipRotation;

    //public StarShipSetup StarShipSetup { get => _starShipSetup; set => _starShipSetup = value; }
    public float TimeToMaxSpeed { get => _timeToMaxSpeed; set => _timeToMaxSpeed = value; }
    public bool IsBoosting { get => _isBoosting; set => _isBoosting = value; }
    public bool IsWarping { get => _isWarping; set => _isWarping = value; }
    public bool Freelook { get => _freelook; set => _freelook = value; }
    public bool IsCameraAligned { get => _isCameraAligned; set => _isCameraAligned = value; }
    //public Animator Animator { get => _animator; set => _animator = value; }
    public Transform TurretAnchor { get => _turretAnchor; set => _turretAnchor = value; }

    [SerializeField]
    private Transform[] _cameraContainers;

    private int gasQuantity;

    //private Controller _controller;

    private void Awake()
    {
        //_resetTimeToMaxSpeed = _timeToMaxSpeed;
        Debug.Log($"XR Device: {XRSettings.isDeviceActive}");

        //_controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //HorizontalAxis = _getInputValues.joystickValues.x;
        VerticalAxis = _getInputValues.throttleValue;
        HorizontalDirection = _getInputValues.joystickValues.x;
        VerticalDirection = _getInputValues.joystickValues.y;


        r = GetComponent<Rigidbody>();
        r.useGravity = false;
        lookRotation = transform.rotation;
        defaultShipRotation = spaceshipRoot.localEulerAngles;
        rotationZ = defaultShipRotation.z;

        _audioSource = GetComponent<AudioSource>();

        _audioSource.Play();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void TurnTowardsTarget(Vector3 targetPosition, float speed)
    {
        //Debug.Log(speed);
        Vector3 lookDirection = targetPosition - transform.position;
        lookDirection.Normalize();

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), speed);

        //transform.rotation = Controller.SmoothDamp(transform.rotation, Quaternion.LookRotation(lookDirection), ref nullQuaternion, speed);
    }


    private void Update()
    {
        //if (!_controller.IsPaused && !StarShipSetup.IsDead)
        //{
        //    IsBoosting = _playerInput.BoostAxis != 0;
        //    IsWarping = _playerInput.WarpAxis != 0;

        //    TurretAnchor.localPosition = new Vector3(0f, 0f, speed * 0.1f);
        //    r.constraints = RigidbodyConstraints.None;
        //}

        //else
        //{
        //    r.constraints = RigidbodyConstraints.FreezeAll;
        //}

        //Cursor.lockState = _controller.IsPaused || StarShipSetup.IsDead ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = _controller.IsPaused || StarShipSetup.IsDead;


    }

    void FixedUpdate()
    {
        //HorizontalAxis = _getInputValues.joystickValues.x;
        VerticalAxis = _getInputValues.throttleValue;
        HorizontalDirection = _getInputValues.joystickValues.x;
        VerticalDirection = _getInputValues.joystickValues.y;

        /*foreach (Transform cameraContainer in _cameraContainers)
        {
            cameraContainer.localRotation = Quaternion.Euler(cameraContainer.rotation.x, _playerInput.HorizontalLook * 90f, cameraContainer.rotation.z);
        }*/

        //if (!_controller.IsPaused && !StarShipSetup.IsDead)
        //{

            //Animator.SetFloat("Veering", HorizontalDirection);

            ApplyThrust();

            ChangeAudioClip();

            Thrusters();

            ////Rotation
            //if (!Freelook)
            //{
            //    rotationZTmp = HorizontalAxis * -1f;
            //    MoveJoystick();
            //    MoveThrottleControl();
            //}

            mouseXSmooth = Mathf.Lerp(mouseXSmooth, HorizontalDirection * rotationSpeed, Time.deltaTime * cameraSmooth);
            mouseYSmooth = Mathf.Lerp(mouseYSmooth, VerticalDirection * rotationSpeed, Time.deltaTime * cameraSmooth);
            Quaternion localRotation = Quaternion.Euler(-mouseYSmooth, mouseXSmooth, rotationZTmp * rotationSpeed);
            lookRotation = lookRotation * localRotation;

            RotateShip();



            //Update crosshair texture
            if (crosshairTexture)
            {
                crosshairTexture.position = rearCamera.WorldToScreenPoint(transform.position + transform.forward * 100);
            }
        //}


    }

    private void MoveJoystick()
    {

        _joystick.localRotation = Quaternion.Euler(Mathf.Clamp(-1f, mouseYSmooth, 1f) * -45f, _joystick.localRotation.y, Mathf.Clamp(-1f, mouseXSmooth, 1f) * -45f);
    }

    private void MoveThrottleControl()
    {
        throttleAmount = Mathf.Lerp(throttleAmount, IsBoosting ? VerticalAxis : VerticalAxis * 0.5f, Time.deltaTime * cameraSmooth);

        _throttleControl.localRotation = Quaternion.Euler(throttleAmount * 60f, _throttleControl.localRotation.y, _throttleControl.localRotation.z);
    }

    private void AlignCamera()
    {
        //if (!IsCameraAligned)
        //{
        //    Quaternion cameraRotation = StarShipSetup.ActiveCamera.transform.rotation;
        //    //Debug.Log($"Camera: {cameraRotation}\n" +
        //    //    $"Starship: {transform.rotation}");
        //    StarShipSetup.ActiveCamera.transform.rotation = Quaternion.Lerp(cameraRotation, transform.rotation, Time.deltaTime * cameraSmooth);

        //    if (cameraRotation == transform.rotation)
        //    {
        //        IsCameraAligned = true;
        //        lookRotation = transform.rotation;
        //    }
        //}
    }

    private void RotateShip()
    {
        transform.rotation = lookRotation;
        rotationZ -= mouseXSmooth;
        rotationZ = Mathf.Clamp(rotationZ, -45, 45);
        spaceshipRoot.transform.localEulerAngles = new Vector3(defaultShipRotation.x, defaultShipRotation.y, rotationZ);
        rotationZ = Mathf.Lerp(rotationZ, defaultShipRotation.z, Time.deltaTime * cameraSmooth);
    }

    private void ApplyThrust()
    {



        if (VerticalAxis > 0)
        {
            float maxSpeed = GoToSpeed(
                speed,
                IsWarping ?
                    warpSpeed :
                    IsBoosting ?
                        accelerationSpeed :
                        normalSpeed,
                3f);

            _timeToMaxSpeed -= Time.deltaTime;


            speed += VerticalAxis *
                Time.deltaTime *
                Mathf.Max(
                    speed,
                    _wasWarping ?
                        warpSpeed :
                        _wasBoosting ?
                            accelerationSpeed :
                            normalSpeed);

            if (speed <= 50f)
            {
                speed = 50f;
            }
            if (speed >= maxSpeed)
            {
                speed = maxSpeed;
            }

            //if (IsWarping)
            //{
            //    StarShipSetup.Hydrogen -= Time.deltaTime;
            //}

        }

        else if (VerticalAxis < 0)
        {
            speed += VerticalAxis * Time.deltaTime * Mathf.Min(
                    speed, normalSpeed * -1f);

            if (speed >= -125f)
            {
                speed = -125f;
            }
        }

        else
        {
            speed = GoToSpeed(speed, 0f, 3f);
        }


        speed = Mathf.Round(speed * 100f) / 100f;

        if (_displaySpeed != null)
        {
            _displaySpeed.text = Mathf.RoundToInt(speed).ToString();
        }

        //_displaySpeed.text = $"Speed: {speed}\nRotationZ: {rotationZTmp}";

        //Set moveDirection to the vertical axis (up and down keys) * speed
        Vector3 moveDirection = new Vector3(0, 0, speed);

        //Debug.Log($"{moveDirection} - {transform.TransformDirection(moveDirection)}");

        //Transform the vector3 to local space
        moveDirection = transform.TransformDirection(moveDirection);
        //Set the velocity, so you can move
        r.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);

        _wasBoosting = IsBoosting && speed > normalSpeed;
        _wasWarping = IsWarping && speed > accelerationSpeed;
    }

    private float GoToSpeed(float thisSpeed, float targetSpeed, float thrust)
    {
        if (Mathf.Abs(thisSpeed) - targetSpeed <= 0.1)
        {
            thisSpeed = targetSpeed;
        }
        else
        {
            thisSpeed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * thrust);
        }

        return thisSpeed;
    }

    private void ChangeAudioClip()
    {
        AudioClip currentAudioClip = _audioSource.clip;
        AudioClip newAudioClip;


        if (VerticalAxis == 0f)
        {
            newAudioClip = _engineIdle;
        }
        else
        {
            if (VerticalAxis > 0)
            {
                newAudioClip = IsWarping ? _engineWarp : IsBoosting ? _engineOn : _engineSlow;
            }

            else if (VerticalAxis < 0)
            {
                newAudioClip = _engineOn;
            }

            else
            {
                newAudioClip = _engineSlow;
            }
        }


        if (newAudioClip != currentAudioClip)
        {
            _audioSource.clip = newAudioClip;
            _audioSource.Play();
        }
    }

    private void Thrusters()
    {
        foreach (ParticleSystem _backThruster in _backThrusters)
        {
            var main = _backThruster.main;

            Vector3 _backThrusterScale;

            if (VerticalAxis < 0f)
            {
                _backThrusterScale = new Vector3(0.5f, 1f, 1f);
            }
            else
            {
                _backThrusterScale = new Vector3(0.5f, 1f, 0f);
            }

            _backThruster.transform.localScale = Vector3.Lerp(_backThruster.transform.localScale, _backThrusterScale, Time.deltaTime * 20f);
        }

        foreach (ParticleSystem _thruster in _mainThrusters)
        {
            var main = _thruster.main;

            Vector3 _thrusterSCale;
            Color _thrusterColor;
            Color _defaultColor = new Color(255, 162, 0, 255);
            if (VerticalAxis == 0f)
            {
                _thrusterColor = new Color(255, 162, 0, 255);
                //var main =_thruster.main.startColor = new Color(255, 162, 0, 255);
                _thrusterSCale = new Vector3(1f, 1f, 0f);
            }
            else
            {
                if (VerticalAxis > 0)
                {
                    _thrusterColor = (IsBoosting || IsWarping) ? new Color(0, 138, 255, 255) : new Color(255, 162, 0, 255);
                    //_thrusterSCale = new Vector3(IsBoosting ? 1.5f : 1f, IsBoosting ? 1.5f : 1f, IsBoosting ? 1.5f : 1f);
                    _thrusterSCale = new Vector3(
                        1f,
                        1f,
                        IsWarping ?
                            2.0f :
                            IsBoosting ?
                                1.5f :
                                1f);
                }

                else
                {
                    _thrusterColor = new Color(255, 162, 0, 255);
                    _thrusterSCale = new Vector3(1f, 1f, 0.1f);
                }
            }

            main.startColor = Color.Lerp(_defaultColor, _thrusterColor, Time.deltaTime * 20f);
            _thruster.transform.localScale = Vector3.Lerp(_thruster.transform.localScale, _thrusterSCale, Time.deltaTime * 20f);
        }

    }
}
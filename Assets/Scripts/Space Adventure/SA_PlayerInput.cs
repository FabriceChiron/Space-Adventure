using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.LegacyInputHelpers;

public class SA_PlayerInput : MonoBehaviour
{
    public float HorizontalAxis;
    public float HorizontalLook;
    public float VerticalAxis;
    public float HorizontalDirection;
    public float VerticalDirection;
    public float FireAxis;
    public float BoostAxis;
    public float WarpAxis;
    public bool SwitchCameraButton, TogglePause;

    [SerializeField]
    private StandaloneInputModule _inputModule;

    private void Awake()
    {
        GetInputs();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (XRSettings.isDeviceActive)
        {
            _inputModule.horizontalAxis = "XRI_Right_Primary2DAxis_Horizontal";
            _inputModule.verticalAxis = "XRI_Right_Primary2DAxis_Vertical";
            _inputModule.submitButton = "XRI_Right_PrimaryButton";
            _inputModule.cancelButton = "XRI_Right_SecondaryButton";
        }
        else
        {
            _inputModule.horizontalAxis = "Horizontal";
            _inputModule.verticalAxis = "Vertical";
            _inputModule.submitButton = "Submit";
            _inputModule.cancelButton = "Cancel";
        }
    }

    private void FixedUpdate()
    {
        GetInputs();
    }

    public void GetInputs()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            HorizontalAxis = Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal");
            VerticalAxis = Input.GetAxis("XRI_Left_Primary2DAxis_Vertical") * -1f;

            HorizontalDirection = Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal");
            VerticalDirection = Input.GetAxis("XRI_Right_Primary2DAxis_Vertical") * -1f;
            FireAxis = Input.GetAxis("XRI_Right_Trigger");
            BoostAxis = Input.GetAxis("XRI_Left_Grip");
            WarpAxis = Input.GetAxis("XRI_Left_Trigger");

            SwitchCameraButton = Input.GetButtonDown("XRI_Left_PrimaryButton");

            TogglePause = Input.GetButtonDown("XRI_Left_MenuButton");
        }
        else
        {
            HorizontalAxis = Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal") != 0 ? Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal") : Input.GetAxis("Horizontal");

            HorizontalLook = Input.GetAxis("HorizontalLook");

            VerticalAxis = Input.GetAxis("XRI_Left_Primary2DAxis_Vertical") != 0 ? Input.GetAxis("XRI_Left_Primary2DAxis_Vertical") * -1f : Input.GetAxis("Vertical");

            HorizontalDirection = Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal") != 0 ? Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal") : Input.GetAxis("Mouse X");
            VerticalDirection = Input.GetAxis("XRI_Right_Primary2DAxis_Vertical") != 0 ? Input.GetAxis("XRI_Right_Primary2DAxis_Vertical") * -1f : Input.GetAxis("Mouse Y");
            FireAxis = Input.GetAxis("XRI_Right_Trigger") != 0 ? Input.GetAxis("XRI_Right_Trigger") : Input.GetAxis("Fire1");
            BoostAxis = Input.GetAxis("XRI_Left_Grip") != 0 ? Input.GetAxis("XRI_Left_Grip") : Input.GetAxis("Boost");
            WarpAxis = Input.GetAxis("XRI_Left_Trigger") != 0 ? Input.GetAxis("XRI_Left_Trigger") : Input.GetAxis("Warp");

            SwitchCameraButton = Input.GetButtonDown("XRI_Left_PrimaryButton") ?
                        Input.GetButtonDown("XRI_Left_PrimaryButton") :
                    Input.GetKeyDown(KeyCode.C);

            TogglePause = Input.GetButtonDown("XRI_Left_MenuButton") ? Input.GetButtonDown("XRI_Left_MenuButton") : Input.GetKeyDown(KeyCode.P);
        }


    }
}


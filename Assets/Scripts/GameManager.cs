using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using SpaceGraphicsToolkit;
using TMPro;
using UnityEngine.InputSystem;

public enum PlayMode
{
    FREELOOK,
    STARSHIP
}
public class GameManager : MonoBehaviour
{

    [SerializeField]
    private bool _isPaused;

    [SerializeField]
    private PlayMode _playMode;

    [SerializeField]
    private Utils Utils;

    [SerializeField]
    private Canvas _UIContainer;

    [SerializeField]
    private Scales _freeLookScales, _starshipScales, _currentScales;

    [SerializeField]
    private List<StellarSystemData> stellarSystemsDataList;

    [SerializeField]
    private StellarSystemData _currentSystemData;

    [SerializeField]
    private TMP_Dropdown _stellarSystemDropdown, _stellarBodyDropdown;

    [SerializeField]
    private SgtFloatingWarpPin _warpComp;

    private StellarSystemData _oldSystemData;

    private Transform _targetTransfom;

    [SerializeField]
    private GameObject _cameraContainer, _flatCamera, _VRCamera;

    [SerializeField]
    private SgtFloatingObject centerPoint;

    [SerializeField]
    private Transform stellarSystemContainer;

    [SerializeField]
    private GameObject JovianPrefab, RockyPrefab, OrbitVisualPrefab;

    private float scaleFactor;

    float stellarSystemCenterSize;

    [SerializeField]
    private List<float> starSizes, starOrbits;

    public bool IsPaused { get => _isPaused; set => _isPaused = value; }
    public Transform TargetTransfom { get => _targetTransfom; set => _targetTransfom = value; }

    // Start is called before the first frame update
    void Start()
    {
        ChooseScales();

        ToggleCameraMode();

        for(int i = 0; i < stellarSystemsDataList.Count; i++)
        {
            _stellarSystemDropdown.AddOptions(new List<string> { stellarSystemsDataList[i].name });
            if(stellarSystemsDataList[i] == _currentSystemData)
            {
                _stellarSystemDropdown.SetValueWithoutNotify(i);
            }
        }
        
        //foreach (StellarSystemData stellarSystemData in stellarSystemsDataList)
        //{

        //    _stellarSystemDropdown.AddOptions(new List<string> { stellarSystemData.name });

        //}

        GenerateStellarSystem(_currentSystemData);

    }

    //public bool IsPaused
    //{
    //    get { return _isPaused; }
    //    set
    //    {
    //        if (_isPaused != value)
    //        {
    //            _isPaused = value;

    //            // Run some function or event here
    //            Debug.Log("Boolean variable chaged from:" + _isPaused + " to: " + value);
    //            foreach (SgtRotate RotateComp in FindObjectsOfType<SgtRotate>())
    //            {
    //                RotateComp.enabled = !_isPaused;
    //            }

    //            foreach (SgtFloatingOrbit OrbitComp in FindObjectsOfType<SgtFloatingOrbit>())
    //            {
    //                OrbitComp.enabled = !_isPaused;
    //            }
    //        }
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        foreach (SgtRotate RotateComp in FindObjectsOfType<SgtRotate>())
        {
            RotateComp.enabled = !IsPaused;
        }

        foreach (SgtFloatingOrbit OrbitComp in FindObjectsOfType<SgtFloatingOrbit>())
        {
            OrbitComp.enabled = !IsPaused;
        }

        DetectMouseClick();
    }

    private void FixedUpdate()
    {

        FollowTarget();

        //if (_currentSystemData != _oldSystemData)
        //{
        //    ChangeStellarSystem();
        //}
    }

    public void ChangeStellarSystem()
    {
        Debug.Log($"Changing Stellar System: {stellarSystemsDataList[_stellarSystemDropdown.value]}");

        _currentSystemData = stellarSystemsDataList[_stellarSystemDropdown.value];

        foreach (Transform child in stellarSystemContainer)
        {
            Destroy(child.gameObject);
        }

        GenerateStellarSystem(_currentSystemData);
    }

    public void TargetStellarBody()
    {
        string targetName = $"{_stellarBodyDropdown.transform.GetComponentInChildren<TextMeshProUGUI>().text.Replace("<b>","").Replace("</b>", "")}";
        Debug.Log(targetName);
        SgtFloatingTarget targetObj = GameObject.Find(targetName).GetComponent<SgtFloatingTarget>();

        _warpComp.CurrentTarget = targetObj;
        _warpComp.ClickWarp();

        Debug.Log("yo");

        TargetTransfom = targetObj.transform;
    }

    private void FollowTarget()
    {
        if(TargetTransfom != null)
        {
            //_cameraContainer.transform.LookAt(TargetTransfom);

            float smoothSpeed = 0.125f;

            /*Vector3 lookDirection = TargetTransfom.position - transform.position;
            lookDirection.Normalize();*/

            /*Quaternion targetRotation = Quaternion.LookRotation(TargetTransfom.position - _cameraContainer.transform.position);
            _cameraContainer.transform.rotation = Quaternion.RotateTowards(_cameraContainer.transform.rotation, targetRotation, smoothSpeed);*/

            
        }
    }

    private void ChooseScales()
    {
        switch (_playMode)
        {
            case PlayMode.FREELOOK:

                _currentScales = _freeLookScales;

                break;

            case PlayMode.STARSHIP:

                _currentScales = _starshipScales;

                break;
        }
    }

    private void GenerateStellarSystem(StellarSystemData stellarSystemData)
    {
        _stellarBodyDropdown.ClearOptions();

        _oldSystemData = stellarSystemData;

        Debug.Log(stellarSystemData.ChildrenItem.Length);

        scaleFactor = stellarSystemData.ScaleFactor;

        starOrbits.Add(0);
        starSizes.Add(0);

        Debug.Log($"At function start: {stellarSystemCenterSize}");



        foreach (StarData starData in stellarSystemData.StarsItem)
        {
            if(starData.ChildrenItem.Length == 0 || stellarSystemData.StarsItem.Length == 1)
            {
                stellarSystemCenterSize = starData.Orbit * _currentScales.Orbit * scaleFactor * 2;
                Debug.Log($"In starData loop: {stellarSystemCenterSize}");

                starOrbits.Add(starData.Orbit * _currentScales.Orbit * scaleFactor * 2);
            }
            CreateStar(starData, stellarSystemData.StarsItem.Length);
        }

        float maxStarSize = starSizes.Max();

        Debug.Log($"starSizes: {starSizes}\nmaxStarSize: {maxStarSize}");

        stellarSystemCenterSize = (starSizes.Max() + starOrbits.Max()) * 2;

        centerPoint.transform.GetChild(0).localScale = new Vector3(stellarSystemCenterSize, stellarSystemCenterSize, stellarSystemCenterSize);

        foreach(PlanetData planetData in stellarSystemData.ChildrenItem)
        {
            CreateStellarObject(planetData, centerPoint, "Planet");
        }

        Debug.Log($"stellarSystemCenterSize: {stellarSystemCenterSize}");

        GameObject.FindGameObjectWithTag("CamerasContainer").transform.localPosition = new Vector3(0, stellarSystemCenterSize, stellarSystemCenterSize * -2);

        Camera.main.transform.localPosition = Vector3.zero;
        //Camera.main.GetComponent<SgtFloatingCamera>().Position = Vector3.zero;

        Camera.main.transform.LookAt(centerPoint.transform.position);
        //_cameraContainer.transform.LookAt(centerPoint.transform.position);
    }

    private void CreateStellarObject(PlanetData planetData, SgtFloatingObject thisCenter, string Type)
    {

        GameObject prefab = (planetData.Prefab == null) ? (planetData.Gaseous ? JovianPrefab : RockyPrefab) : planetData.Prefab;

        GameObject newStellarObject = Instantiate(prefab, stellarSystemContainer);

        newStellarObject.name = planetData.Name;

        _stellarBodyDropdown.AddOptions(new List<string>() { newStellarObject.name });

        SgtFloatingTarget targetComp = newStellarObject.AddComponent<SgtFloatingTarget>();
        targetComp.WarpName = planetData.Name;
        

        SgtFloatingOrbit OrbitComp = newStellarObject.transform.GetComponent<SgtFloatingOrbit>();

        OrbitComp.ParentPoint = thisCenter;

        Transform stellarBody = newStellarObject.transform.GetChild(0);

        SphereCollider stellarBodyCollider = stellarBody.gameObject.AddComponent<SphereCollider>();
        stellarBodyCollider.radius = 1f;

        
        Rigidbody rb = (stellarBody.gameObject.GetComponent<Rigidbody>() != null) ? stellarBody.gameObject.GetComponent<Rigidbody>() : stellarBody.gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        SgtRotate RotateComp = stellarBody.gameObject.AddComponent<SgtRotate>();

        RotateComp.RelativeTo = Space.Self;

        SgtShadowSphere ShadowComp = stellarBody.gameObject.GetComponent<SgtShadowSphere>();

        ShadowComp.SharpnessB = planetData.Size * _currentScales.Planet;
        ShadowComp.SharpnessG = planetData.Size * _currentScales.Planet;
        ShadowComp.SharpnessR = planetData.Size * _currentScales.Planet;

        stellarBody.rotation = Quaternion.Euler(planetData.BodyTilt + planetData.OrbitTilt, 0, 0);

        if(planetData.Prefab == null)
        {
            if (planetData.Gaseous)
            {
                if (planetData.Texture.Length > 0)
                {
                    stellarBody.GetComponent<SgtJovian>().MainTex = (Cubemap)planetData.Texture[0];

                    if (planetData.Texture.Length > 1)
                    {
                        stellarBody.GetComponent<SgtJovian>().FlowTex = (Cubemap)planetData.Texture[1];
                    }
                }
            }
            else
            {
                stellarBody.GetComponent<SgtPlanet>().Material = planetData.Material;
            }
        }

        stellarBody.localScale *= planetData.Size * _currentScales.Planet;

        targetComp.WarpDistance = planetData.Size * _currentScales.Planet * 5;


        float orbitRadius = (thisCenter.transform.GetChild(0).localScale.x * 2) + (planetData.Orbit * _currentScales.Orbit * scaleFactor) + (planetData.Size * _currentScales.Planet);

        OrbitComp.Radius = orbitRadius;

        GameObject OrbitVisualGO = Instantiate(OrbitVisualPrefab, stellarBody);

        SgtFloatingOrbitVisual orbitVisual = OrbitVisualGO.GetComponent<SgtFloatingOrbitVisual>();

        orbitVisual.Orbit = OrbitComp;

        orbitVisual.Points = Mathf.Max((int)orbitRadius * 50, 30);

        OrbitComp.Angle = Utils.GetOrbitOrientationStart(planetData.Coords);
        OrbitComp.Tilt = new Vector3(planetData.OrbitTilt, 0f, 0f);

        double revolutionPeriod = 360 / (planetData.YearLength * _currentScales.Year * scaleFactor);

        OrbitComp.DegreesPerSecond = revolutionPeriod;

        if(planetData.RingsPrefab != null)
        {
            Debug.Log($"{planetData.Name} has rings: {planetData.RingsPrefab.name}");
            GameObject stellarObjectRings = Instantiate(planetData.RingsPrefab, stellarBody);
        }

        if (planetData.TidallyLocked)
        {
            RotateComp.AngularVelocity = new Vector3(0, (float)OrbitComp.DegreesPerSecond, 0);
        }
        else
        {
            RotateComp.AngularVelocity = new Vector3(0, 360 / ((float.IsNaN(planetData.DayLength) ? 1f : planetData.DayLength) * _currentScales.Day), 0);
        }

        if(Type == "Moon")
        {
            //stellarBody.gameObject.AddComponent<SgtShadowSphere>();
            orbitVisual.Thickness = 0.25;
            OrbitComp.DegreesPerSecond = (revolutionPeriod < 360) ? revolutionPeriod : revolutionPeriod / 360;

            if (planetData.TidallyLocked)
            {
                RotateComp.AngularVelocity = new Vector3(0, (float)OrbitComp.DegreesPerSecond, 0);
            }
        }

        if(Type == "Planet")
        {
            orbitVisual.Thickness = 0.5;

            foreach (SgtLight sgtLight in FindObjectsOfType<SgtLight>())
            {
                sgtLight.GetComponent<Light>().range = Mathf.Max((float)OrbitComp.Radius * 1.2f, 1000f);
            }

            Camera.main.farClipPlane = Mathf.Max((float)OrbitComp.Radius * 1.2f, 1000f);
        }

        foreach (PlanetData moonData in planetData.ChildrenItem)
        {
            
            CreateStellarObject(moonData, newStellarObject.transform.GetComponent<SgtFloatingObject>(), "Moon");
            
        }

        
    }

    private void CreateStar(StarData starData, int starsCount)
    {
        GameObject newStar = Instantiate(starData.Prefab, stellarSystemContainer);

        newStar.name = starData.Name;

        _stellarBodyDropdown.AddOptions(new List<string>() { $"<b>{newStar.name}</b>" });

        SgtFloatingTarget targetComp = newStar.AddComponent<SgtFloatingTarget>();
        targetComp.WarpName = starData.Name;

        SgtFloatingOrbit OrbitComp = newStar.transform.GetComponent<SgtFloatingOrbit>();

        Transform starBody = newStar.transform.GetChild(0);

        starBody.localScale *= Utils.dimRet(starData.Size * _currentScales.Planet, 3.5f, true);

        targetComp.WarpDistance = starBody.localScale.x * 5;

        //stellarSystemCenterSize += starData.Size * _currentScales.Planet * 2;

        Debug.Log($"In CreateStar: {stellarSystemCenterSize}");

        OrbitComp.ParentPoint = centerPoint;

        if(starsCount > 1)
        {
            OrbitComp.Radius = (starData.Orbit * _currentScales.Orbit * scaleFactor) + (starData.Size * _currentScales.Planet);
        }

        if(starData.ChildrenItem.Length == 0 || starsCount == 1)
        {
            starSizes.Add(starBody.localScale.x);
            starOrbits.Add((float)OrbitComp.Radius);
        }

        GameObject OrbitVisualGO = Instantiate(OrbitVisualPrefab, newStar.transform);

        SgtFloatingOrbitVisual orbitVisual = OrbitVisualGO.GetComponent<SgtFloatingOrbitVisual>();

        orbitVisual.Orbit = OrbitComp;

        orbitVisual.Points = (int)OrbitComp.Radius * 30;

        OrbitComp.Angle = Utils.GetOrbitOrientationStart(starData.Coords);

        OrbitComp.DegreesPerSecond = (starData.YearLength != 0f) ? 360 / (starData.YearLength * _currentScales.Year) : 0;

        foreach (PlanetData planetData in starData.ChildrenItem)
        {
            CreateStellarObject(planetData, newStar.transform.GetComponent<SgtFloatingObject>(), "Planet");
        }
    }

    private void ToggleCameraMode()
    {
        if (XRSettings.isDeviceActive)
        {
            _VRCamera.SetActive(true);
            _flatCamera.SetActive(false);

            _UIContainer.renderMode = RenderMode.WorldSpace;
        }
        else
        {
            _VRCamera.SetActive(false);
            _flatCamera.SetActive(true);

            _UIContainer.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    private void DetectMouseClick()
    {
        Vector2 _pointerPosition;

        _pointerPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Visualize Ray on Scene (no impact on Game view)
        //Debug.DrawRay(ray.origin, ray.direction * 20f);

        RaycastHit hit;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out hit))
            {
                SgtFloatingTarget stellarObjectTarget = null;

                if (hit.transform.GetComponent<SgtFloatingTarget>() != null)
                {
                    stellarObjectTarget = hit.transform.GetComponent<SgtFloatingTarget>();
                }

                if (hit.transform.parent.GetComponent<SgtFloatingTarget>() != null)
                {
                    stellarObjectTarget = hit.transform.parent.GetComponent<SgtFloatingTarget>();
                }

                Debug.Log(stellarObjectTarget.name);

                for (int i = 0; i < _stellarBodyDropdown.options.Count; i++)
                {
                    string StrippedCameraTarget = _stellarBodyDropdown.options[i].text.Replace("    ", "").Replace("<b>", "").Replace("</b>", "");

                    if (stellarObjectTarget != null && stellarObjectTarget.name == StrippedCameraTarget)
                    {
                        _stellarBodyDropdown.value = i;
                    }
                }
            }
            else
            {

            }
        }

        
    }
}

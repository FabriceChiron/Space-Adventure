using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    PLANET,
    MOON
}

public class StellarObject : MonoBehaviour
{
    [SerializeField]
    private float Orbit;

    [SerializeField]
    private double _revolutionPeriod;

    [SerializeField]
    private float _rotationPeriod;

    [SerializeField]
    private Transform _stellarBody;

    [SerializeField]
    private ObjectType _objectType;

    [SerializeField]
    private GameManager _gameManager;

    public double RevolutionPeriod { get => _revolutionPeriod; set => _revolutionPeriod = value; }
    public float RotationPeriod { get => _rotationPeriod; set => _rotationPeriod = value; }
    public ObjectType ObjectType { get => _objectType; set => _objectType = value; }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        Debug.Log(_gameManager);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void RotateObject(Transform objTransform, float RotationTime, bool inverted)
    {

        //RotationTime = Mathf.Max(RotationTime, 0.01f);
        float degreesPerSecond = 360f / RotationTime * (inverted ? 1f : -1f);
        objTransform.Rotate(new Vector3(0, degreesPerSecond * Time.deltaTime, 0));
    }
}

/*                                                                                     *\
           _____ _____   _____   _      _              _____  _     _           
          / ____|  __ \ / ____| | |    (_)            |  __ \(_)   | |          
         | |  __| |  | | (___   | |     _ _ __   ___  | |__) |_  __| | ___ _ __ 
         | | |_ | |  | |\___ \  | |    | | '_ \ / _ \ |  _  /| |/ _` |/ _ \ '__|
         | |__| | |__| |____) | | |____| | | | |  __/ | | \ \| | (_| |  __/ |   
          \_____|_____/|_____/  |______|_|_| |_|\___| |_|  \_\_|\__,_|\___|_|   
      ˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽
        ©2016 Sionera Entertainment - Daniele Banovaz (daniele.banovaz@gmail.com)

        Developed as a tutorial for Game Developement Saturday #1, 2016-03-19, PN

\*                                                                                     */


using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Car
/// </summary>
public class Car : MonoBehaviour
{
    /// <summary>
    /// Main chassis of the Car
    /// </summary>
    public Rigidbody2D CarBody;

    /// <summary>
    /// Maximum zoom (at Car's minimum speed)
    /// </summary>
    public float MaxZoom = 3f;

    /// <summary>
    /// Minumum zoom (at Car's maximum speed)
    /// </summary>
    public float MinZoom = 12f;

    /// <summary>
    /// Sound of the engine
    /// 
    /// This field has been decorated with [SerializeField] attribute:
    /// this way, Unity Editor is still able to expose this property in
    /// the inspector, as well as serialize it, while keeping it hidden
    /// from other Scripts
    /// </summary>
    [SerializeField]
    private AudioSource _engineSound;

    /// <summary>
    /// Particle System used to render engine smoke
    /// </summary>
    [SerializeField]
    private ParticleSystem _smoke;

    /// <summary>
    /// Main camera
    /// </summary>
    private Camera _mainCamera;

    /// <summary>
    /// Link to Car's 2D Rigidbodies
    /// </summary>
    private Rigidbody2D[] _bodies; 

    /// <summary>
    /// Initial positions of Car's parts: useful to reset its initial state
    /// </summary>
    private List<Vector2> _startingPoints = new List<Vector2>();

    /// <summary>
    /// Locks/unlocks Car physics
    /// </summary>
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            if (_isRunning == value)
                return;

            _isRunning = value;

            if (_isRunning)
            {
                // Unlock physics
                for (int i = 0; i < _bodies.Length; i++)
                    _bodies[i].isKinematic = false;

                // Start engine sound
                _engineSound.Play();

                // Start smoke ParticleFX
                _smoke.gameObject.SetActive(true);
                _smoke.Play();
            }
            else
            {
                // Reset position and lock physics
                for (int i = 0; i < _bodies.Length; i++)
                {
                    _bodies[i].position = _startingPoints[i];
                    _bodies[i].rotation = 0;
                    _bodies[i].isKinematic = true;
                }

                // Center camera on Car
                FocusCameraOnCar();

                // Stop engine sound
                _engineSound.Stop();

                // Stop smoke ParticleFX
                _smoke.Stop();
                _smoke.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Backing field for IsRunning property
    /// </summary>
    private bool _isRunning;

    /// <summary>
    /// Called by Unity when this Component is created
    /// </summary>
    private void Awake()
    {
        // Link main Camera
        _mainCamera = Camera.main;

        // Link Rigidbodies and save their initial positions
        _bodies = GetComponentsInChildren<Rigidbody2D>(true);
        foreach (Rigidbody2D child in _bodies)
            _startingPoints.Add(child.position);
    }

    /// <summary>
    /// Called by Unity engine once per frame
    /// </summary>
    private void Update()
    {
        if (!_isRunning)
            return;

        // Center camera on Car
        FocusCameraOnCar();

        // Adapt engine sound pitch to current Car velocity
        _engineSound.pitch = Mathf.Lerp(0.5f, 1.5f, CarBody.velocity.magnitude / 35);
    }

    /// <summary>
    /// Focus Main camera on running Car
    /// </summary>
    private void FocusCameraOnCar()
    {
        // Set camera position to follow running car
        Vector3 newCameraPosition = CarBody.position;
        newCameraPosition.z = _mainCamera.transform.position.z;
        _mainCamera.transform.position = newCameraPosition;

        // Set zoom according to speed
        _mainCamera.orthographicSize = Mathf.Lerp(MaxZoom, MinZoom, CarBody.velocity.magnitude / 6);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float damping;
    private Vector3 velocity = Vector3.zero;
    Camera mainCamera;
    [SerializeField] float scrollSpeed = 0.005f;
    [SerializeField] float clampZoomMin = 6f;
    [SerializeField] float clampZoomMax = 20f;

    InputActions playerInput;

    private void Awake()
    {
        playerInput = new InputActions();

    }

    private void OnEnable()
    {
        playerInput.Enable();

    }

    private void OnDisable()
    {
        playerInput.Disable();

    }
    private void Start() 
    {
        mainCamera = Camera.main;

    }
    private void Update() {
        CameraZoom();
    }
    void FixedUpdate()
    {
        FollowPlayer();
        
    }

    private void FollowPlayer()
    {
        Vector3 move_position = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, move_position, ref velocity, damping);
    }

    private void CameraZoom()
    {
        float scroll = playerInput.Player.CameraZoom.ReadValue<float>();
        mainCamera.orthographicSize -= scroll * scrollSpeed * Time.deltaTime;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, clampZoomMin, clampZoomMax);
        
    }
}

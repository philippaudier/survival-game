using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera C_MainCamera;
    [Header("Zoom Settings")]
    [SerializeField] float C_ScrollSpeed = 0.005f;
    [SerializeField] float C_MinClamp = 6f;
    [SerializeField] float C_MaxClamp = 20f;
    [SerializeField] Transform C_Target;
    [SerializeField] Vector3 C_Offset;
    [SerializeField] float C_Damping;
    private Vector3 C_Velocity = Vector3.zero;
    [SerializeField] float C_ZoomStartAt = 6f;
    [SerializeField] float C_ZoomSmoothCoeff = 1f;
    [SerializeField] float C_CurrentDistance;

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
        C_MainCamera = Camera.main;
        C_CurrentDistance = Mathf.Clamp(C_ZoomStartAt > C_MinClamp ? C_MinClamp : C_ZoomStartAt, C_MinClamp, C_MaxClamp);

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
        Vector3 move_position = C_Target.position + C_Offset;
        transform.position = Vector3.SmoothDamp(transform.position, move_position, ref C_Velocity, C_Damping);
    }

    private void CameraZoom()
    {
        C_CurrentDistance -= playerInput.Player.CameraZoom.ReadValue<float>() * C_ScrollSpeed;
        C_CurrentDistance = Mathf.Clamp(C_CurrentDistance, C_MinClamp, C_MaxClamp);
        C_MainCamera.orthographicSize = Mathf.Lerp(C_MainCamera.orthographicSize, C_CurrentDistance, C_ZoomSmoothCoeff * Time.deltaTime);
        
    }
}

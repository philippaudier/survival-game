using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DefaultZombie : MonoBehaviour
{
    [Header("Deafault Zombie Settings")]
    public float Z_WalkSpeed = 1.8f;
    public float Z_RunSpeed = 2.5f;
    public float Z_TurnSpeed = 400f;

    
	private Rigidbody Z_RB;
	private Camera C_MainCamera;
    private FieldOfView Z_FOVScript;
    private Vector3 Z_TargetLocation;
    private bool hasReachedPlayer = false;

    private void Start()
    {
        C_MainCamera = Camera.main;
        Z_RB = GetComponent<Rigidbody>();
        Z_FOVScript = GetComponent<FieldOfView>();
    }

    private void Update()
    {
        ChasePlayer();
    }

    private void Move()
    {
        if (!hasReachedPlayer)
        {
            Z_TargetLocation = Z_FOVScript.Z_DirectionToTarget;
        // enemyRigidbody.MovePosition(transform.position + targetPlayer * walkSpeed * Time.deltaTime);
        Z_RB.transform.position = Vector3.MoveTowards(transform.position, transform.position + Z_TargetLocation, Z_WalkSpeed * Time.deltaTime);
        }
        
        else
        {
            Z_RB.transform.position = Z_RB.transform.position;
        }
        
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Player")
        {
            hasReachedPlayer = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.transform.tag == "Player")
        {
            hasReachedPlayer = false;
        }
    }
    private void MoveLate()
    {   
        Vector3 lastPosition = Z_FOVScript.Z_TargetLastLocation;
        Z_RB.transform.position = Vector3.MoveTowards(transform.position, lastPosition, Z_WalkSpeed * Time.deltaTime);

    }
    private void Look()
    {
        Vector3 position = Z_FOVScript.P_PlayerReference.transform.position;
        Vector3 direction = position - transform.position;

            // Ignore the height difference.
            direction.y = 0;

            // Quaternion for a smooth rotation towards the pointer
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Make the transform look in the direction.
            // transform.forward = direction;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Z_TurnSpeed * Time.deltaTime);
    }

    private void ChasePlayer()
    {
        if (Z_FOVScript.zombieCanSeePlayer)
        {
            Look();
            Move();
        }
        else
        {
            MoveLate();
            Z_FOVScript.soundPlayed = false;
        }
    }

}

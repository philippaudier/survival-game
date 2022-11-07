using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    GameVariables GV;
    [Header("Zombie Field Of View Settings")]
    public float Z_FOV_Radius;
    [Range(0,360)]
    public float Z_FOV_Angle;
    public LayerMask Z_TargetMask;
    public LayerMask Z_ViewBlockerMask;
    public AudioClip[] Z_AlertSound;
    public bool soundPlayed = false;
    public bool alertSoundPlayed = false;
    [HideInInspector]
    public GameObject P_PlayerReference;
    private AudioSource audioSource1;
    [HideInInspector]
    public Vector3 Z_DirectionToTarget;
    [HideInInspector]
    public Vector3 Z_TargetLastLocation;
    [HideInInspector]
    public float distanceToTarget;
    public bool zombieCanSeePlayer = false;
   
    

    private void Start()
    {
        GV = GetComponent<GameVariables>();
        P_PlayerReference = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
        Z_TargetLastLocation = transform.position;
        audioSource1 = GetComponent<AudioSource>();

    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void Update() {
        
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, Z_FOV_Radius, Z_TargetMask);

        if (rangeChecks.Length != 0)
        {
            Transform Z_Target = rangeChecks[0].transform;
            Z_DirectionToTarget = (Z_Target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, Z_DirectionToTarget) < Z_FOV_Angle / 2)
            {
                distanceToTarget = Vector3.Distance(transform.position, Z_Target.position);

                if (!Physics.Raycast(transform.position, Z_DirectionToTarget, distanceToTarget, Z_ViewBlockerMask))
                {
                    zombieCanSeePlayer = true;
                    Z_TargetLastLocation = Z_Target.position;

                    if (!soundPlayed) {
                        audioSource1.PlayOneShot(Z_AlertSound[UnityEngine.Random.Range(0, Z_AlertSound.Length)]);
                        soundPlayed = true;
                    }
                }
                    
                else
                    zombieCanSeePlayer = false;
            }
            else
                zombieCanSeePlayer = false;
                
        }
        else if (zombieCanSeePlayer)
            zombieCanSeePlayer = false;
            
    }
    
}
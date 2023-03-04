using System;
using Core;
using Enums;
using UnityEngine;

/// <summary>
/// Controller contains functions for change crane system params
/// </summary>
public class CraneController : MonoBehaviour
{
    [Header("Rotation around Y axis")]
    [SerializeField] private Transform rotatingTurretTransform;                         
    [SerializeField] private CraneSystemControlParams rotationTurretParams;

    [Header("Moving along Y axis")]
    [SerializeField] private ConfigurableJoint[] ropeSegmentsJoints;                    //Set of joints used for simulating rope behaviour
    [SerializeField] private CraneSystemControlParams ropeLenghtParams;
    
    [Header("Moving along Jib local X axis")]
    [SerializeField] private Rigidbody ropeAttachToJibAnchor;                           //Kinematic rigidbody parented to jib and moving along local X axis with player input
    [SerializeField] private CraneSystemControlParams ropeLocalXMovementParams;


    private void FixedUpdate()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        ControlTurretRotation();
        ControlRopeLenght();
        ControlRopeLocalXPosition();
    }

    private void ControlTurretRotation()
    {
        eMoveDirection rotationDirection = GetMoveDirectionFromControls(KeyCode.E, KeyCode.Q);
        if(rotationDirection == eMoveDirection.none) return;
        
        Vector3 newTurretRotation = rotatingTurretTransform.rotation.eulerAngles;
        newTurretRotation.y += rotationTurretParams.GetCraneSystemParamChange(rotationDirection, newTurretRotation.y);
        rotatingTurretTransform.eulerAngles = newTurretRotation;
    }
    
    private void ControlRopeLenght()
    {
        eMoveDirection ropeChangeDirection = GetMoveDirectionFromControls(KeyCode.W, KeyCode.S);
        if(ropeChangeDirection == eMoveDirection.none) return;

        Vector3 newConnectedAnchorForRopeJoints = ropeSegmentsJoints[0].connectedAnchor;
        newConnectedAnchorForRopeJoints.y += ropeLenghtParams.GetCraneSystemParamChange(ropeChangeDirection, newConnectedAnchorForRopeJoints.y);
        Array.ForEach(ropeSegmentsJoints, j => j.connectedAnchor = newConnectedAnchorForRopeJoints);
    }

    private void ControlRopeLocalXPosition()
    {
        eMoveDirection ropeXPositionMoveDirection = GetMoveDirectionFromControls(KeyCode.D, KeyCode.A);
        if (ropeXPositionMoveDirection == eMoveDirection.none) return;

        Vector3 newRopeAnchorLocalPos = ropeAttachToJibAnchor.transform.localPosition;
        newRopeAnchorLocalPos.x += ropeLocalXMovementParams.GetCraneSystemParamChange(ropeXPositionMoveDirection, newRopeAnchorLocalPos.x);
        ropeAttachToJibAnchor.transform.localPosition = newRopeAnchorLocalPos;
    }
    
    private eMoveDirection GetMoveDirectionFromControls(KeyCode positiveDirectionKey, KeyCode negativeDirectionKey)
    {
        bool positiveDirection = Input.GetKey(positiveDirectionKey);
        bool negativeDirection = Input.GetKey(negativeDirectionKey);

        if (positiveDirection == negativeDirection) return eMoveDirection.none;
        return positiveDirection ? eMoveDirection.positive : eMoveDirection.negative;
    }
}

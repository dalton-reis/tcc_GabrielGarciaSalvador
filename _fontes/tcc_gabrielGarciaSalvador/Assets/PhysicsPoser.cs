using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsPoser : MonoBehaviour
{
    public float physicsRange = 0.1f;
    public LayerMask physicsMask = 0;

    [Range(0, 1)] public float slowDownVelocity = 0.75f;
    [Range(0, 1)] public float slowDownAngularVelocity = 0.75f;

    [Range(0, 100)] public float maxPositionChange = 75.0f;
    [Range(0, 100)] public float maxRotationChange = 75.0f;

    private Rigidbody rigidBody = null;
    private XRController controller = null;
    private XRBaseInteractor interactor = null;

    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        controller = GetComponent<XRController>();
        interactor = GetComponent<XRBaseInteractor>();
    }

    void Start()
    {
        UpdateTracking(controller.inputDevice);
        MoveUsingTransform();
        RotateUsingTransform();
    }

    private void RotateUsingTransform()
    {
        rigidBody.angularVelocity = Vector3.zero;
        transform.localRotation = targetRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, physicsRange);
    }

    private void OnValidate()
    {
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.useGravity = false;
        }
    }

    private void MoveUsingTransform()
    {
        rigidBody.velocity = Vector3.zero;
        transform.localPosition = targetPosition;
    }

    private void UpdateTracking(InputDevice inputDevice)
    {
        inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out targetPosition);
        inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out targetRotation);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTracking(controller.inputDevice);
    }

    private void FixedUpdate()
    {
        if(IsHoldingObject() || !WithingPhysicsRange())
        {
            MoveUsingTransform();
            RotateUsingTransform();
        }

        else
        {
            MoveUsingPhysics();
            RotateUsingPhysics();
        }
    }

    private void RotateUsingPhysics()
    {
        rigidBody.angularVelocity *= slowDownAngularVelocity;

        Vector3 angularVelocity = FindNewAngularVelocity();

        if (IsValidVelocity(angularVelocity.x))
        {
            float maxChange = maxRotationChange * Time.deltaTime;
            rigidBody.angularVelocity = Vector3.MoveTowards(rigidBody.angularVelocity, angularVelocity, maxChange);
        }
    }

    private bool IsValidVelocity(float x)
    {
        return !float.IsNaN(x) && !float.IsInfinity(x);
    }

    private Vector3 FindNewAngularVelocity()
    {
        Quaternion delta = targetRotation * Quaternion.Inverse(rigidBody.rotation);
        delta.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        if (angleInDegrees > 180) {
            angleInDegrees -= 360;
        }

        return (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.deltaTime;
    }

    private void MoveUsingPhysics()
    {
        rigidBody.velocity *= slowDownVelocity;
        Vector3 velocity = FindNewVelocity();
        if (IsValidVelocity(velocity.x))
        {
            float maxChange = maxPositionChange * Time.deltaTime;
            rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, velocity, maxChange);
        }
    }

    private Vector3 FindNewVelocity()
    {
        Vector3 difference = targetPosition - rigidBody.position;
        return difference / Time.deltaTime;
    }

    private bool WithingPhysicsRange()
    {
        return Physics.CheckSphere(transform.position, physicsRange, physicsMask, QueryTriggerInteraction.Ignore);
    }

    private bool IsHoldingObject()
    {
        return interactor.selectTarget;
    }
}

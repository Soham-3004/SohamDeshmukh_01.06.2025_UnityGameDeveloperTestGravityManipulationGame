using UnityEngine;
using Cinemachine;

public class GravityCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public Transform player;
    public float rotationSpeed = 5f;

    private Vector3 currentUp;

    void Start()
    {
        currentUp = Vector3.up; // Initial world up
    }

    void LateUpdate()
    {
        Vector3 newUp = CustomGravityForCamera.GetCurrentGravityUp();

        if (Vector3.Angle(currentUp, newUp) > 0.01f)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(currentUp, newUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            currentUp = newUp;
        }

        virtualCam.LookAt = player;
        virtualCam.Follow = player;
    }
}

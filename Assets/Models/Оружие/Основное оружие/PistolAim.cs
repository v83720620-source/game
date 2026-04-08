using UnityEngine;

public class PistolAim : MonoBehaviour
{
    public Transform aimPivot;
    public Camera playerCamera;

    [Header("Позиция без прицела")]
    public Vector3 hipPosition;
    public Vector3 hipRotation;

    [Header("Позиция прицеливания")]
    public Vector3 aimPosition;
    public Vector3 aimRotation;

    [Header("Скорость")]
    public float aimSpeed = 12f;

    [Header("FOV")]
    public float normalFOV = 60f;
    public float aimFOV = 50f;
    public float fovSpeed = 10f;

    void Update()
    {
        bool aiming = Input.GetMouseButton(1);

        Vector3 targetPos = aiming ? aimPosition : hipPosition;
        Vector3 targetRot = aiming ? aimRotation : hipRotation;

        aimPivot.localPosition = Vector3.Lerp(
            aimPivot.localPosition,
            targetPos,
            Time.deltaTime * aimSpeed
        );

        aimPivot.localRotation = Quaternion.Lerp(
            aimPivot.localRotation,
            Quaternion.Euler(targetRot),
            Time.deltaTime * aimSpeed
        );

        if (playerCamera != null)
        {
            float targetFOV = aiming ? aimFOV : normalFOV;
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * fovSpeed
            );
        }
    }
}
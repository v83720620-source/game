using UnityEngine;

public class WeaponAimCode : MonoBehaviour
{
    public Transform aimPivot;

    [Header("Обычная позиция")]
    public Vector3 hipPosition;
    public Vector3 hipRotation;

    [Header("Позиция прицеливания")]
    public Vector3 aimPosition;
    public Vector3 aimRotation;

    [Header("Настройки")]
    public float speed = 10f;

    void Update()
    {
        bool aiming = Input.GetMouseButton(1);

        Vector3 targetPos = aiming ? aimPosition : hipPosition;
        Vector3 targetRot = aiming ? aimRotation : hipRotation;

        aimPivot.localPosition = Vector3.Lerp(
            aimPivot.localPosition,
            targetPos,
            Time.deltaTime * speed
        );

        aimPivot.localRotation = Quaternion.Lerp(
            aimPivot.localRotation,
            Quaternion.Euler(targetRot),
            Time.deltaTime * speed
        );
    }
}
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayAmount = 2f;
    public float smooth = 6f;

    float mouseX;
    float mouseY;

    Quaternion startRot;

    void Start()
    {
        startRot = transform.localRotation;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * swayAmount;
        mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        Quaternion target = Quaternion.Euler(
            -mouseY,
            mouseX,
            0
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            startRot * target,
            Time.deltaTime * smooth
        );
    }
}
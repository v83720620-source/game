using UnityEngine;

public class AttachWeapon : MonoBehaviour
{
    public Transform weaponRoot;
    public Transform weaponPoint;

    void Start()
    {
        weaponRoot.SetParent(weaponPoint, false);
        weaponRoot.localPosition = Vector3.zero;
        weaponRoot.localRotation = Quaternion.identity;
    }
}
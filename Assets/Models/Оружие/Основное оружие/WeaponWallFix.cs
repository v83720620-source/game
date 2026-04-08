using UnityEngine;

public class WeaponWallFix : MonoBehaviour
{
    public Transform cameraTransform;
    public float normalZ = 0.5f;
    public float minZ = 0.05f;
    public float checkDistance = 1.2f;
    public float smoothSpeed = 15f;
    public LayerMask wallMask = ~0;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        float targetZ = normalZ;

        if (Physics.SphereCast(cameraTransform.position, 0.2f, cameraTransform.forward, out RaycastHit hit, checkDistance, wallMask))
        {
            float pushedZ = normalZ - (checkDistance - hit.distance);
            targetZ = Mathf.Clamp(pushedZ, minZ, normalZ);
        }

        Vector3 targetPos = new Vector3(startLocalPos.x, startLocalPos.y, targetZ);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothSpeed);
    }
}
using UnityEngine;

public class BulletHoleSpawner : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject bulletHolePrefab;
    public float shootDistance = 100f;
    public KeyCode shootKey = KeyCode.Mouse0;

    void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (playerCamera == null || bulletHolePrefab == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootDistance))
        {
            GameObject hole = Instantiate(
                bulletHolePrefab,
                hit.point + hit.normal * 0.01f,
                Quaternion.LookRotation(hit.normal)
            );

            hole.transform.SetParent(hit.transform);

            Destroy(hole, 10f);
        }
    }
}
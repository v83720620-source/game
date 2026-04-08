using UnityEngine;

public class BulletHoleTest : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject bulletHolePrefab;
    public float range = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                GameObject hole = Instantiate(
                    bulletHolePrefab,
                    hit.point + hit.normal * 0.001f,
                    Quaternion.LookRotation(-hit.normal)
                );

                hole.transform.SetParent(hit.collider.transform, true);

                Destroy(hole, 5f);
            }
        }
    }
}
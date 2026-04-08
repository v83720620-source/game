using UnityEngine;

public class HitEffectSpawner : MonoBehaviour
{
    public Camera cam;
    public GameObject hitEffect;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
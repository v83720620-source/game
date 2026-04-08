using UnityEngine;

public class SniperScope : MonoBehaviour
{
    public GameObject scopeOverlay;
    public Camera cam;

    public float normalFOV = 60f;
    public float scopeFOV = 20f;
    public float speed = 10f;

    private bool isScope = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isScope = true;
            scopeOverlay.SetActive(true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            isScope = false;
            scopeOverlay.SetActive(false);
        }

        float target = isScope ? scopeFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, target, Time.deltaTime * speed);
    }
}
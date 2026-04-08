using UnityEngine;

public class MuzzleFlashCode : MonoBehaviour
{
    public Transform muzzlePoint;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateFlash();
        }
    }

    void CreateFlash()
    {
        GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Quad);

        flash.transform.position = muzzlePoint.position;
        flash.transform.rotation = Camera.main.transform.rotation;
        flash.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        Destroy(flash.GetComponent<Collider>());

        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = Color.yellow;

        flash.GetComponent<MeshRenderer>().material = mat;

        Destroy(flash, 0.05f);
    }
}
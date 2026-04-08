using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    private Vector3 _target;
    public float speed = 300f;

    public void Init(Vector3 targetPoint)
    {
        _target = targetPoint;
        Destroy(gameObject, 0.2f);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _target) < 0.05f)
            Destroy(gameObject);
    }
}
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.03f;

    public float runBobSpeed = 16f;
    public float runBobAmount = 0.07f;

    float defaultY;
    float timer;

    void Start()
    {
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        if (!isMoving)
        {
            timer = 0f;

            Vector3 pos = transform.localPosition;
            pos.y = Mathf.Lerp(pos.y, defaultY, Time.deltaTime * 5f);
            transform.localPosition = pos;

            return;
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float speed = isRunning ? runBobSpeed : walkBobSpeed;
        float amount = isRunning ? runBobAmount : walkBobAmount;

        timer += Time.deltaTime * speed;

        float newY = defaultY + Mathf.Sin(timer) * amount;

        Vector3 newPos = transform.localPosition;
        newPos.y = newY;
        transform.localPosition = newPos;
    }
}
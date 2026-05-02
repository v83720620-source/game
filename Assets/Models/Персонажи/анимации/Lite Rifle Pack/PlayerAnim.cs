using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float speed = Input.GetAxis("Vertical");
        anim.SetFloat("Speed", Mathf.Abs(speed));
    }
}
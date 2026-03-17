using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    Animator anim;

    Vector3 startPos;
    public Vector3 reloadOffset = new Vector3(0f, -0.12f, -0.04f);

    public float reloadDuration = 0.15f;
    public float reloadWait = 0.1f;

    bool isReloading = false;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (anim == null) return;

        if (Input.GetKey(KeyCode.LeftShift))
            anim.SetBool("Aiming", false);
        else
            anim.SetBool("Aiming", true);

        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            anim.SetTrigger("Fire");
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadMove());
        }
    }

    IEnumerator ReloadMove()
    {
        isReloading = true;

        anim.ResetTrigger("Reload");
        anim.SetTrigger("Reload");

        Vector3 reloadPos = startPos + reloadOffset;

        float t = 0f;

        // вниз
        while (t < reloadDuration)
        {
            t += Time.deltaTime;
            float k = t / reloadDuration;
            transform.localPosition = Vector3.Lerp(startPos, reloadPos, k);
            yield return null;
        }

        transform.localPosition = reloadPos;

        yield return new WaitForSeconds(reloadWait);

        t = 0f;

        // вверх
        while (t < reloadDuration)
        {
            t += Time.deltaTime;
            float k = t / reloadDuration;
            transform.localPosition = Vector3.Lerp(reloadPos, startPos, k);
            yield return null;
        }

        transform.localPosition = startPos;

        isReloading = false;
    }
}
using UnityEngine;

public class PlayerPoseSwitcher : MonoBehaviour
{
    public GameObject[] poses;

    int currentIndex = -1;

    public void SwitchPose(int index)
    {
        if (poses == null) return;
        if (index < 0 || index >= poses.Length) return;

        if (index == currentIndex) return;

        currentIndex = index;

        for (int i = 0; i < poses.Length; i++)
        {
            if (poses[i] != null)
                poses[i].SetActive(i == index);
        }
    }
}
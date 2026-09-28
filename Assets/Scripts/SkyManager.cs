using UnityEngine;

public class SkyManager : MonoBehaviour
{
    [SerializeField] float skySpeed;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skySpeed);
    }
}
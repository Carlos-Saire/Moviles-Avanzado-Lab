using UnityEngine;

public class LookCamCanvas : MonoBehaviour
{
    private Transform cam;
    private void Awake()
    {
        cam = Camera.main.transform;
    }
    private void Update()
    {
        transform.LookAt(cam.position);
    }
}

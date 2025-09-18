using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineCamera _cam; 
    private CinemachineCamera _camcombat;

    [Header("Cam")]
    [SerializeField] private GameObject camCombat;
    [SerializeField] private GameObject cam;

    [Header("Look")]
    [SerializeField] private GameObject lookCamCombat;
    private void OnEnable()
    {
        PlayerController.OnplayerPosition += SetTarget;
    }
    private void OnDisable()
    {
        PlayerController.OnplayerPosition -= SetTarget;
    }
    private void Awake()
    {
        _cam = cam.GetComponent<CinemachineCamera>();
        _camcombat = camCombat.GetComponent<CinemachineCamera>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            camCombat.SetActive(true);
            lookCamCombat.SetActive(true);
            camCombat.SetActive(false);

        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            camCombat.SetActive(false);
            lookCamCombat.SetActive(false);
            camCombat.SetActive(true);
        }
    }
    private void SetTarget(Transform player)
    {
        _cam.Target.TrackingTarget= player;
    }
}

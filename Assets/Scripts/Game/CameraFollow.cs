using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineCamera _cam;
    private CinemachineCamera _camCombat;

    [Header("Cam")]
    [SerializeField] private GameObject combatCam;
    [SerializeField] private GameObject thidPersonCam;

    [Header("Ui")]
    [SerializeField] private GameObject lookCombat;

    [Header("Player")]
    private Transform player; 
    [SerializeField] private float rotateSpeed = 10f;

    private bool inCombatMode = false;

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
        _cam = thidPersonCam.GetComponent<CinemachineCamera>();
        _camCombat = combatCam.GetComponent<CinemachineCamera>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lookCombat.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            EnterCombatMode();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            ExitCombatMode();
        }
        if (inCombatMode)
        {
            RotatePlayerToCamera();
        }
    }

    private void EnterCombatMode()
    {
        combatCam.SetActive(true);
        lookCombat.SetActive(true);
        thidPersonCam.SetActive(false);
        inCombatMode = true;
    }

    private void ExitCombatMode()
    {
        thidPersonCam.SetActive(true);
        lookCombat.SetActive(false);
        combatCam.SetActive(false);
        inCombatMode = false;
    }

    private void RotatePlayerToCamera()
    {
        Vector3 lookDirection = _camCombat.transform.forward;
        lookDirection.y = 0; 

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private void SetTarget(Transform targetThirdPerson, Transform targetCombat,Transform playerTranforms)
    {
        _cam.Target.TrackingTarget = targetThirdPerson;
        _camCombat.Target.TrackingTarget = targetCombat;
        player = playerTranforms;
    }
}

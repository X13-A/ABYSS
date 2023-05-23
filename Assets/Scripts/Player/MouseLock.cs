using UnityEngine;

public class MouseLock : MonoBehaviour
{
    private static MouseLock m_Instance;
    public static MouseLock Instance => m_Instance;

    [SerializeField] private Cinemachine.CinemachineFreeLook freeLookCamera;

    private CharacterController characterController;
    private bool isCameraRotationEnabled = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool IsActive()
    {
        if (GameManager.Instance.State == GAMESTATE.PLAY)
        {
            return PlayerManager.Instance.ActiveAimingMode == AimingMode.CAMERA;
        }
        else
        {
            return false;
        }
    }

    private void DisableFreeLookRotation()
    {
        freeLookCamera.m_XAxis.m_InputAxisName = "";
        freeLookCamera.m_YAxis.m_InputAxisName = "";
        freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
        freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
        isCameraRotationEnabled = false;
    }

    private void EnableFreeLookRotation()
    {
        freeLookCamera.m_XAxis.m_InputAxisName = "Mouse X";
        freeLookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
        isCameraRotationEnabled = true;
    }

    private void RotatePlayerAccordingly()
    {
        float cameraRotationY = freeLookCamera.m_XAxis.Value;
        characterController.transform.rotation = Quaternion.Euler(0f, cameraRotationY, 0f);
    }

    private void FixedUpdate()
    {
        if (IsActive())
        {
            if (!isCameraRotationEnabled) // to avoid setting the values each Fixed update
            {
                EnableFreeLookRotation();
            }
            RotatePlayerAccordingly();
        }
        else
        {
            if (isCameraRotationEnabled)
            {
                DisableFreeLookRotation();
            }
        }
    }
}

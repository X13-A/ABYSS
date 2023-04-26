using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimUtil : MonoBehaviour
{
    private static AimUtil m_Instance;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject target;
    [SerializeField] private RenderTexture rt;

    public static AimUtil Instance { get { return m_Instance; } }
    private void Awake()
    {
        if (!m_Instance) m_Instance = this;
        else Destroy(gameObject);
    }

    public RaycastHit Aim()
    {
        // Convert screen position to camera position
        Vector3 mousePos = Input.mousePosition;
        float xRatio = (float) rt.width / Screen.width;
        float yRatio = (float) rt.height / Screen.height;
        mousePos.x *= xRatio;
        mousePos.y *= yRatio;

        // Cast ray from camera to find where to aim
        Ray ray = cam.ScreenPointToRay(mousePos);
       ;
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Aim")));
        return hit;
    }
}

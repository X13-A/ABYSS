using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuild : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject target;
    [SerializeField] private RenderTexture rt;

    [SerializeField] private GameObject blocInHand;

    private void Aim()
    {
        // Convert screen position to camera position
        Vector3 mousePos = Input.mousePosition;
        float xRatio = (float) rt.width / Screen.width;
        float yRatio = (float) rt.height / Screen.height;
        mousePos.x *= xRatio;
        mousePos.y *= yRatio;

        // Cast ray from camera to find where to aim
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            blocInHand.transform.position = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
        }
    }
    private void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY)
        {
            return;
        }

        if (Input.GetButtonDown("Pickaxe"))
        {
            EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = PlayerMode.PICKAXE });
            Debug.Log(PlayerManager.Instance.ActivePlayerMode);
        }

        if (Input.GetButtonDown("Build"))
        {
            EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = PlayerMode.BUILD});
        }

        if (PlayerManager.Instance.ActivePlayerMode != PlayerMode.BUILD)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Aim();
            Instantiate(blocInHand);
        }
    }
}

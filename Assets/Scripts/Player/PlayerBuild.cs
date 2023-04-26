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

    private void Build()
    {
        RaycastHit hit = AimUtil.Instance.Aim();
        if (hit.collider)
        {
            GameObject currentCube = Instantiate(blocInHand);
            currentCube.transform.position = hit.collider.transform.position + hit.normal;
            currentCube.AddComponent<Rigidbody>();
            currentCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            currentCube.GetComponent<Rigidbody>().isKinematic = true;
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
        }

        if (Input.GetButtonDown("Build"))
        {
            EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = PlayerMode.BUILD });
        }

        if (PlayerManager.Instance.ActivePlayerMode != PlayerMode.BUILD)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && PlayerManager.Instance.ActiveAimingMode == AimingMode.CURSOR)
        {
            Build();
        }
    }
}

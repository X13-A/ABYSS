using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuild : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RenderTexture rt;

    private GameObject blockInHand;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<UpdateObjectInhand>(setBlockInHand);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateObjectInhand>(setBlockInHand);
    }

    private void Build()
    {
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
        if (hit.collider)
        {
            GameObject currentCube = Instantiate(blockInHand);
            currentCube.SetActive(true);
            currentCube.transform.position = hit.collider.transform.position + hit.normal;
            currentCube.transform.localScale = Vector3.one;
            currentCube.transform.rotation = Quaternion.identity;
            currentCube.GetComponent<BoxCollider>().enabled = true;
            currentCube.GetComponent<BoxCollider>().isTrigger = false;
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

        if (PlayerManager.Instance.ActivePlayerMode != PlayerMode.BUILD)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && PlayerManager.Instance.ActiveAimingMode == AimingMode.CURSOR)
        {
            Build();
        }
    }

    private void setBlockInHand(UpdateObjectInhand e)
    {
        blockInHand = e.objectInSlot;
    }
}

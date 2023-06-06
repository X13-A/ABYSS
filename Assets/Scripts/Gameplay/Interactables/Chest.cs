using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Chest : MonoBehaviour
{
    [SerializeField] private List<ItemId> content;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float torqueForce = 5f;
    [SerializeField] private float throwAngle = 1;
    [SerializeField] private Transform throwStart;
    [SerializeField] private GameObject lid;
    [SerializeField] private float lidPopForce = 1;
    [SerializeField] private float lidRotateForce = 1;
    [SerializeField] private Vector3 lidRotation = new Vector3(-1, 0, 0);
    [SerializeField] private Vector3 lidPop = new Vector3(0, 1, 0);

    private bool opened = false;
    public bool Opened => opened;
    public void Init()
    {
        content = new List<ItemId>();
    }

    public void Init(List<ItemId> content)
    {
        this.content = content;
    }

    public void Open()
    {
        if (opened) return;
        opened = true;
        OpenLid();
        throwItems();
    }

    private void OpenLid()
    {
        Rigidbody rigidbody = lid.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = lid.AddComponent<Rigidbody>();
        }

        rigidbody.AddForce(lidPop.normalized * lidPopForce, ForceMode.Impulse);
        rigidbody.AddTorque(lidRotation.normalized * lidRotateForce, ForceMode.Impulse);

        StartCoroutine(CoroutineUtil.DelayAction(3f, () =>
        {
            StartCoroutine(CoroutineUtil.ScaleTo(lid.transform, 0.25f, new Vector3(0,0,0), () =>
            {
                Destroy(lid);
            }));
        }));
    }

    private void throwItems()
    {
        foreach (ItemId item in content)
        {
            throwItem(item);
        }
        content.Clear();
    }

    private void throwItem(ItemId item)
    {
        GameObject droppedPrefab = ItemBank.GetDroppedPrefab(item);
        GameObject droppedGameObject = Instantiate(droppedPrefab, throwStart.position, Quaternion.identity);
        GameObject particles = ItemBank.GetDroppedParticles(item);

        GameObject particlesInstance = Instantiate(particles, droppedGameObject.transform.position, Quaternion.identity);
        particlesInstance.transform.SetParent(droppedGameObject.transform, false);
        particlesInstance.transform.localPosition = Vector3.zero;

        Rigidbody rigidbody = droppedGameObject.GetComponent<Rigidbody>();
        if (rigidbody != null )
        {
            rigidbody.useGravity = true;
            rigidbody.AddForce(RandomizeDirection() * throwForce, ForceMode.Impulse);
            rigidbody.AddTorque(RandomizeTorque() * torqueForce, ForceMode.Impulse);
        }

        if (droppedGameObject.GetComponent<DroppedItem>() == null)
        {
            droppedGameObject.AddComponent<DroppedItem>().Init(item);
        }

        droppedGameObject.transform.localScale = Vector3.zero;
        StartCoroutine(CoroutineUtil.ScaleTo(droppedGameObject.transform, 0.5f, new Vector3(1, 1, 1)));
    }

    private Vector3 RandomizeTorque()
    {
        float x = Random.Range(-1, 1);
        float y = Random.Range(-1, 1);
        float z = Random.Range(-1, 1);
        return new Vector3(x, y, z).normalized;
    }

    private Vector3 RandomizeDirection()
    {
        float xzAngle = Random.Range(0f, 360f); // Random horizontal angle
        float yRadians = throwAngle * Mathf.Deg2Rad;
        float xzRadians = xzAngle * Mathf.Deg2Rad;
        float x = Mathf.Sin(yRadians) * Mathf.Cos(xzRadians);
        float y = Mathf.Cos(yRadians);
        float z = Mathf.Sin(yRadians) * Mathf.Sin(xzRadians);
        return new Vector3(x, y, z).normalized;
    }


}

using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostAttackIndicator : MonoBehaviour
{
    [SerializeField] private GameObject lightening;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject pickaxe;
    [SerializeField] private GameObject wand;

    private bool display = false;
    private bool isCoroutineRunning = false;

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
        EventManager.Instance.AddListener<StartAttackBoostPlayerEvent>(FlashBoostIndicator);
        EventManager.Instance.AddListener<SwitchSlotEvent>(DisplaySelectedName);
        EventManager.Instance.AddListener<EndAttackBoostPlayerEvent>(NotDisplaySelectedName);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<StartAttackBoostPlayerEvent>(FlashBoostIndicator);
        EventManager.Instance.RemoveListener<SwitchSlotEvent>(DisplaySelectedName);
        EventManager.Instance.RemoveListener<EndAttackBoostPlayerEvent>(NotDisplaySelectedName);
    }

    private void FlashBoostIndicator(StartAttackBoostPlayerEvent e)
    {
        if (!isCoroutineRunning)
        {
            StartCoroutine(FlashBoostCoroutine());
        }
    }

    private void DisplaySelectedName(SwitchSlotEvent e)
    {
        if (!display) return;

        ItemId? item = InventoryManager.Instance.ActiveItem;
        switch (item)
        {
            case ItemId.Sword:
                pickaxe.SetActive(false);
                wand.SetActive(false);
                sword.SetActive(true);
                break;

            case ItemId.Pickaxe:
                sword.SetActive(false);
                wand.SetActive(false);
                pickaxe.SetActive(true);
                break;

            case ItemId.Wand:
                sword.SetActive(false);
                pickaxe.SetActive(false);
                wand.SetActive(true);
                break;

            default:
                sword.SetActive(false);
                pickaxe.SetActive(false);
                wand.SetActive(false);
                break;
        }
    }

    private void NotDisplaySelectedName(EndAttackBoostPlayerEvent e)
    {
        sword.SetActive(false);
        pickaxe.SetActive(false);
        wand.SetActive(false);
    }

    private IEnumerator FlashBoostCoroutine()
    {
        if (isCoroutineRunning)
        {
            yield break;
        }
        
        display = true;
        lightening.SetActive(true);

        float initalSpeed = PlayerManager.Instance.PlayerAttackSpeedMultiplier;

        EventManager.Instance.Raise(new AttackSpeedMultiplierEvent { speed = 3f });

        float elapsedTime = -1f;
        float flashDuration = 4f;

        isCoroutineRunning = true;
        lightening.SetActive(true);

        yield return new WaitForSeconds(flashDuration);

        while (elapsedTime < flashDuration)
        {
            lightening.SetActive(!lightening.activeSelf);

            yield return new WaitForSeconds(0.5f);

            elapsedTime++;
        }

        lightening.SetActive(false);
        display = false;
        isCoroutineRunning = false;

        EventManager.Instance.Raise(new AttackSpeedMultiplierEvent { speed = initalSpeed });

        EventManager.Instance.Raise(new EndAttackBoostPlayerEvent { });
    }
}

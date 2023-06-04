using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;


#region Player events
public class PlayerSwitchModeEvent : SDD.Events.Event
{
    public PlayerMode mode;
}

public class PlayerBuildEvent : SDD.Events.Event
{
    public GameObject block;
}

public class PlayerAttackEvent : SDD.Events.Event
{
    public float damage;
    public float cooldown; // Time before player can attack again after using this attack
    public float animationDuration; // Duration of the animation, affects damage start time
    public float damageStartPercentage; // Percentage of "animationDuration" (0.0 -> 1.0), after that the damage will be inflicted
    public float hitDuration; // Duration during which damage is dealt
    public float projectileSpeed;
    public AttackType type;
}

public class PlayerSpawnedEvent : SDD.Events.Event
{
}

public class AimingModeUpdateEvent : SDD.Events.Event
{
    public AimingMode mode;
}

public class PortalDiscoveredEvent : SDD.Events.Event
{
}

public class DamagePlayerEvent : SDD.Events.Event
{
    public float damage;
}

public class HealthPlayerEvent : SDD.Events.Event
{
    public float health;
}

#endregion

#region Enemy

public class EnemyAttackEvent : SDD.Events.Event
{
    public float damage;
    // peut être rajouté un paramètre de son, @Alexou
}

#endregion

#region Scene events
public class SceneAboutToChangeEvent : SDD.Events.Event
{
    public int levelGenerated;
    public string targetScene;
}

public class SceneReadyToChangeEvent : SDD.Events.Event
{
    public int levelGenerated;
    public string targetScene;
}

#endregion

#region Settings Events
public class ResolutionScaleSliderChangeEvent : SDD.Events.Event
{
    public float value;
}

#endregion

#region Render Events

public class WindowResizeEvent : SDD.Events.Event
{
    public int oldWidth;
    public int oldHeight;

    public int newWidth;
    public int newHeight;

    public float resolutionScale;
}

public class ResolutionScaleUpdateEvent : SDD.Events.Event
{
}

public class RenderTextureUpdateEvent : SDD.Events.Event
{
    public RenderTexture updatedRt;
}

#endregion

#region GameManager Events
public class GameMainMenuEvent : SDD.Events.Event
{
}
public class GameSettingsMenuEvent : SDD.Events.Event
{
}
public class GameSaveSettingsEvent : SDD.Events.Event
{
}
public class GameCancelSettingsEvent : SDD.Events.Event
{
}
public class GamePlayEvent : SDD.Events.Event
{
}
public class GamePauseMenuEvent : SDD.Events.Event
{
}
public class GameResumeEvent : SDD.Events.Event
{
}

public class GameOverMenuEvent : SDD.Events.Event
{
}

public class GameOverEvent : SDD.Events.Event
{
}
#endregion

#region MenuManager Events
public class EscapeButtonClickedEvent : SDD.Events.Event
{
}
public class PlayButtonClickedEvent : SDD.Events.Event
{
}
public class ResumeButtonClickedEvent : SDD.Events.Event
{
}
public class MainMenuButtonClickedEvent : SDD.Events.Event
{
}
public class SettingsButtonClickedEvent : SDD.Events.Event
{
}
public class SaveSettingsButtonClickedEvent : SDD.Events.Event
{
}
public class CancelSettingsButtonClickedEvent : SDD.Events.Event
{
}
public class QuitButtonClickedEvent : SDD.Events.Event
{
}

public class CreditsButtonClickedEvent : SDD.Events.Event
{
}
#endregion

#region UI events
public class LoadingProgressUpdateEvent : SDD.Events.Event
{
    public float progress;
    public string message;
}

public class CursorUpdateEvent : SDD.Events.Event
{
    public CursorType type;
    public Sprite sprite;
}

public class ToggleMapEvent : SDD.Events.Event
{
    public bool value;
}

public class MessageEvent : SDD.Events.Event
{
    public string text;
    public float delay;
}

#endregion

#region Animation

public class AnimateItemEvent : SDD.Events.Event
{
    public ItemId itemId;
    public Dictionary<string, float> animations; // First string is the animation name, Second one is the delay before it triggers
}

public class AnimateAttackEvent : SDD.Events.Event
{
    public string name;
    public float animationDuration;
}

#endregion

#region New inventory events

public class ItemRemovedEvent : SDD.Events.Event
{
    public ItemId itemId;
}

public class ItemUsedEvent : SDD.Events.Event
{
    public ItemId itemId;
}
public class ItemPickedUpEvent : SDD.Events.Event
{
    public ItemId itemId;
}

public class ItemDroppedEvent : SDD.Events.Event
{
    public ItemId itemId;
}

public class UpdateCollidingItemsEvent : SDD.Events.Event
{
    public HashSet<DroppedItem> items;
}

public class SwitchSlotEvent : SDD.Events.Event
{
    public int slot;
}
public class PickupKeyPressedEvent : SDD.Events.Event
{
}

public class DropKeyPressedEvent : SDD.Events.Event
{
}

public class UseKeyPressedEvent : SDD.Events.Event
{
}

public class UseItemEvent : SDD.Events.Event
{
}

public class PlayerHeldItemUpdateEvent : SDD.Events.Event
{
    public ItemId? itemId;
}

public class HeldGameObjectEvent : SDD.Events.Event
{
    public GameObject heldGameObject;
}


#endregion

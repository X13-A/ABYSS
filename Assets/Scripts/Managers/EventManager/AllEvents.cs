using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;


#region Player events
public class PlayerSwitchModeEvent : SDD.Events.Event
{
    public PlayerMode mode;
}

public class PlayerAttackEvent : SDD.Events.Event
{
    public float damage;
    public float duration;
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

#endregion

#region enemy

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

#endregion

#region Inventory

public class ItemAddedEvent : SDD.Events.Event
{
    public IInventoryItem item;
    public int count;
}

public class ItemRemovedEvent : SDD.Events.Event
{
    public IInventoryItem item;
    public int count;
}

public class ItemUsedEvent : SDD.Events.Event
{
    public IInventoryItem item;
}

public class ItemCollideWithPlayerEvent : SDD.Events.Event
{
    public IInventoryItem item;
}

public class ItemEndCollideWithPlayerEvent : SDD.Events.Event
{
    public IInventoryItem item;
}

public class SwitchSlot : SDD.Events.Event
{
    public int slot;
}

public class UpdateObjectInhand : SDD.Events.Event
{
    public GameObject objectInSlot;
}

#endregion

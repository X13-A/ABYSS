using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

#region Scene events
public class SceneAboutToChangeEvent : SDD.Events.Event
{
    public string targetScene;
}
public class SceneReadyToChangeEvent : SDD.Events.Event
{
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
#endregion
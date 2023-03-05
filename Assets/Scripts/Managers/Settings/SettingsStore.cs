using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsStore
{

    public float ResolutionScale { get; set; }

    public SettingsStore()
    {

    }

    // Clone constructor
    public SettingsStore(SettingsStore settings)
    {
        ResolutionScale = settings.ResolutionScale;
    }
}

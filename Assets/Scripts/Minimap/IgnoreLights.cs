using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreLights: MonoBehaviour
{
    [SerializeField] private Light[] lights;

    private void OnPreCull()
    {
        foreach (var light in lights)
        {
            light.enabled = false;
        }
    }

    private void OnPreRender()
    {
        foreach (var light in lights)
        {
            light.enabled = false;
        }
    }
    private void OnPostRender()
    {
        foreach (var light in lights)
        {
            light.enabled = true;
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

// MADE BY CHAT GPT TO LIST THE PATH TO ALL THE ANIMATION EVENTS
public class ListAllAnimationEvents : MonoBehaviour
{
    void Start()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        foreach (var root in rootObjects)
        {
            var animators = root.GetComponentsInChildren<Animator>();
            foreach (var animator in animators)
            {
                RuntimeAnimatorController ac = animator.runtimeAnimatorController;
                foreach (var clip in ac.animationClips)
                {
                    foreach (var animEvent in clip.events)
                    {
                        Debug.Log("GameObject: " + root.name + ", Clip: " + clip.name + ", Event: " + animEvent.functionName);
                    }
                }
            }
        }
    }
}

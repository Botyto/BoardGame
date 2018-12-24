using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenProgress : MonoBehaviour
{
    public Slider progress;

    void Update()
    {
        if (!SceneController.isCurrentlyLoading) { return; }
        if (progress == null) { return; }
        progress.value = progress.minValue + SceneController.loadingProgress * (progress.maxValue - progress.minValue);
    }
}

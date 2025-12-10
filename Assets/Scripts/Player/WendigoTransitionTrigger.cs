using UnityEngine;
using UnityEngine.SceneManagement;

public class WendigoTransitionTrigger : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string wendigoTransitionSceneName = "WendigoTransition";
    [SerializeField] private float delayBeforeTransition = 2f;

    [Header("References")]
    [SerializeField] private GameProgressionManager progressionManager;

    private void OnEnable()
    {
        if (progressionManager != null)
        {
            progressionManager.OnWendigoTransformation.AddListener(OnWendigoTransformation);
        }
    }

    private void OnDisable()
    {
        if (progressionManager != null)
        {
            progressionManager.OnWendigoTransformation.RemoveListener(OnWendigoTransformation);
        }
    }

    private void OnWendigoTransformation()
    {
        Invoke(nameof(LoadTransitionScene), delayBeforeTransition);
    }

    private void LoadTransitionScene()
    {
        SceneManager.LoadScene(wendigoTransitionSceneName);
    }
}
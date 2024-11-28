using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnButtonPress : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName;

    // Reference to the button
    public Button yourButton;

    void Start()
    {
        // Make sure the button is assigned in the inspector
        if (yourButton != null)
        {
            // Add a listener to the button to call the LoadScene method when clicked
            yourButton.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogError("Button reference is missing!");
        }
    }

    void LoadScene()
    {
        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}

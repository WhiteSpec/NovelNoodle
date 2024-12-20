using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public string loaderScene;

    public TextMeshProUGUI musicValue;
    public AudioMixer musicMixer;
    public TextMeshProUGUI soundsValue;
    public AudioMixer soundsMixer;
    public Button loadButton;

    public DataHolder dataHolder; // Reference to the DataHolder

    private Animator animator;
    private int _window = 0;

    public void Start()
    {
        animator = GetComponent<Animator>();
        string dataHolderID = dataHolder.name; // Get the DataHolder ID
        loadButton.interactable = SaveManager.IsGameSaved(dataHolderID);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _window == 1)
        {
            animator.SetTrigger("HideOptions");
            _window = 0;
        }
    }

    public void NewGame()
    {
        string dataHolderID = dataHolder.name; // Get the DataHolder ID
        SaveManager.ClearSavedGame(dataHolderID);
        Load();
    }

    public void DeleteSavedGame()
    {
        string dataHolderID = dataHolder.name; // Get the DataHolder ID
        SaveManager.ClearSavedGame(dataHolderID);
    }

    public void Load()
    {
        SceneManager.LoadScene(loaderScene, LoadSceneMode.Additive);
    }

    public void ShowOptions()
    {
        animator.SetTrigger("ShowOptions");
        _window = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnMusicChanged(float value)
    {
        musicValue.SetText(value + "%");
        musicMixer.SetFloat("volume", -50 + value / 2);
    }

    public void OnSoundsChanged(float value)
    {
        soundsValue.SetText(value + "%");
        soundsMixer.SetFloat("volume", -50 + value / 2);
    }
}

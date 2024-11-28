using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameScene currentScene;
    public BottomBarController bottomBar;
    public SpriteSwitcher backgroundController;
    public ChooseController chooseController;
    public AudioController audioController;
    public VideoController videoController;
    public Button backToTitleButton;

    public DataHolder data;
    public string menuScene;

    private State state = State.IDLE;
    private bool isAutoAdvanceEnabled = false;
    private bool isAutoAdvanceRoutineRunning = false;

    private List<StoryScene> history = new List<StoryScene>();

    private enum State
    {
        IDLE, ANIMATE, CHOOSE, VIDEO
    }

    void Start()
    {
        Debug.Log("Start function executed at " + Time.time);
        string dataHolderID = data.name;

        if (SaveManager.IsGameSaved(dataHolderID))
        {
            SaveData saveData = SaveManager.LoadGame(dataHolderID);
            saveData.prevScenes.ForEach(sceneIndex =>
            {
                if (sceneIndex >= 0 && sceneIndex < this.data.scenes.Count)
                {
                    history.Add(this.data.scenes[sceneIndex] as StoryScene);
                }
                else
                {
                    Debug.LogWarning("Invalid scene index: " + sceneIndex);
                }
            });

            if (history.Count > 0)
            {
                currentScene = history[history.Count - 1];
                history.RemoveAt(history.Count - 1);
                bottomBar.SetSentenceIndex(saveData.sentence - 1);
            }
            else
            {
                Debug.LogError("History is empty after loading saved game.");
                // Handle the case where history is empty, maybe load a default scene or show an error message
            }
        }

        if (currentScene is StoryScene)
        {
            StoryScene storyScene = currentScene as StoryScene;
            history.Add(storyScene);
            bottomBar.PlayScene(storyScene, bottomBar.GetSentenceIndex());
            backgroundController.SetImage(storyScene.background);
            PlayAudio(storyScene.sentences[bottomBar.GetSentenceIndex()]);
        }

        if (backToTitleButton != null)
        {
            backToTitleButton.onClick.AddListener(OnBackToTitleButtonClicked);
        }
    }

    void Update()
    {
        if (state == State.IDLE)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                HandleNextSentence();
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (bottomBar.IsFirstSentence())
                {
                    if (history.Count > 1)
                    {
                        bottomBar.StopTyping();
                        bottomBar.HideSprites();
                        history.RemoveAt(history.Count - 1);
                        StoryScene scene = history[history.Count - 1];
                        history.RemoveAt(history.Count - 1);
                        PlayScene(scene, scene.sentences.Count - 2, false);
                    }
                }
                else
                {
                    bottomBar.GoBack();
                }
            }
            /*
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveCurrentGameState();
                SceneManager.LoadScene(menuScene);
            }
            */
            if (isAutoAdvanceEnabled && !isAutoAdvanceRoutineRunning)
            {
                StartCoroutine(AutoAdvanceRoutine());
            }
        }
    }

    private void HandleNextSentence()
    {
        Debug.Log("HandleNextSentence function executed at " + Time.time);
        if (bottomBar.IsCompleted())
        {
            bottomBar.StopTyping();
            if (bottomBar.IsLastSentence())
            {
                PlayScene((currentScene as StoryScene).nextScene);
            }
            else
            {
                bottomBar.PlayNextSentence();
                PlayAudio((currentScene as StoryScene).sentences[bottomBar.GetSentenceIndex()]);
            }
        }
        else
        {
            bottomBar.SpeedUp();
        }
    }

    public void PlayScene(GameScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        Debug.Log("PlayScene function executed at " + Time.time);
        StartCoroutine(SwitchScene(scene, sentenceIndex, isAnimated));
    }

    private IEnumerator SwitchScene(GameScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        Debug.Log("SwitchScene function executed at " + Time.time);
        state = State.ANIMATE;
        currentScene = scene;
        if (isAnimated)
        {
            bottomBar.Hide();
            yield return new WaitForSeconds(1f);
        }
        if (scene is StoryScene storyScene)
        {
            history.Add(storyScene);
            PlayAudio(storyScene.sentences[sentenceIndex + 1]);
            if (isAnimated)
            {
                backgroundController.SwitchImage(storyScene.background);
                yield return new WaitForSeconds(1f);
                bottomBar.ClearText();
                bottomBar.Show();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                backgroundController.SetImage(storyScene.background);
                bottomBar.ClearText();
            }
            bottomBar.PlayScene(storyScene, sentenceIndex, isAnimated);
            state = State.IDLE;
        }
        else if (scene is ChooseScene chooseScene)
        {
            state = State.CHOOSE;
            chooseController.SetupChoose(chooseScene);
        }
        else if (scene is VideoScene videoScene)
        {
            videoController.PlayVideo(videoScene);
        }
        isAutoAdvanceEnabled = false;
    }

    private void PlayAudio(StoryScene.Sentence sentence)
    {
        Debug.Log("PlayAudio function executed at " + Time.time);
        audioController.PlayAudio(sentence.music, sentence.sound);
    }

    private IEnumerator AutoAdvanceRoutine()
    {
        Debug.Log("AutoAdvanceRoutine started at " + Time.time);
        isAutoAdvanceRoutineRunning = true;
        while (isAutoAdvanceEnabled && state == State.IDLE)
        {
            yield return new WaitForSeconds(2f); // Adjust the delay as needed
            if (bottomBar.IsCompleted())
            {
                HandleNextSentence();
            }
        }
        isAutoAdvanceRoutineRunning = false;
        Debug.Log("AutoAdvanceRoutine ended at " + Time.time);
    }

    public void ToggleAutoAdvance()
    {
        Debug.Log("ToggleAutoAdvance function executed at " + Time.time);
        isAutoAdvanceEnabled = !isAutoAdvanceEnabled;
        Debug.Log("AutoAdvance " + (isAutoAdvanceEnabled ? "enabled" : "disabled") + " at " + Time.time);
    }

    private void OnBackToTitleButtonClicked()
    {
        SaveCurrentGameState();
        SceneManager.LoadScene(menuScene);
    }

    private void SaveCurrentGameState()
    {
        string dataHolderID = data.name;
        List<int> historyIndices = new List<int>();
        history.ForEach(scene =>
        {
            historyIndices.Add(this.data.scenes.IndexOf(scene));
        });
        SaveData saveData = new SaveData
        {
            sentence = bottomBar.GetSentenceIndex(),
            prevScenes = historyIndices
        };
        SaveManager.ClearSavedGame(dataHolderID);
        SaveManager.SaveGame(saveData, dataHolderID);
    }
}

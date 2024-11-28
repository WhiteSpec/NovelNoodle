using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NewVideoScene", menuName = "Data/New Video Scene")]
public class VideoScene : GameScene
{
    public VideoClip videoClip;
    public StoryScene nextScene;
}

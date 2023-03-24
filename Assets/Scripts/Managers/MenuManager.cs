using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private Button[] buttons;

    private void Awake() 
    {
        Instance = this;

        foreach(Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
            });
        }
    }

    public void LoadMenuScenes(int levelIndex)
    {
        StartCoroutine(CloseTheScene(levelIndex));
        // BackgroundMusic.instance.gameObject.GetComponent<AudioSource>().UnPause();
    }

    //Fader Closing
    private IEnumerator CloseTheScene(int levelIndex)
    {
        FaderController.instance.FadeOpen(1f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);
    }

    public void QuitTheGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    private void PlayButtonClickSound()
    {
        AudioManager.instance.Play("ButtonClickSound");
    }
}

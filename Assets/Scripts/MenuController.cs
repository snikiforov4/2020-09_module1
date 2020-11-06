using UnityEngine;

public class MenuController : MonoBehaviour
{
    enum Screen
    {
        None,
        Main,
        Settings,
        NewGame,
    }

    public CanvasGroup mainScreen;
    public CanvasGroup settingsScreen;
    public CanvasGroup newGameScreen;

    void SetCurrentScreen(Screen screen)
    {
        Utility.SetCanvasGroupEnabled(mainScreen, screen == Screen.Main);
        Utility.SetCanvasGroupEnabled(settingsScreen, screen == Screen.Settings);
        Utility.SetCanvasGroupEnabled(newGameScreen, screen == Screen.NewGame);
    }

    void Start()
    {
        SetCurrentScreen(Screen.Main);
    }

    public void StartNewGame(string levelName)
    {
        SetCurrentScreen(Screen.None);
        LoadingScreen.instance.LoadScene(levelName);
    }

    public void OpenSettings()
    {
        SetCurrentScreen(Screen.Settings);
    }
    
    public void OpenNewGameMenu()
    {
        SetCurrentScreen(Screen.NewGame);
    }

    public void ReturnToMainMenu()
    {
        SetCurrentScreen(Screen.Main);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

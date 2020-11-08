using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private const string WinColor = "#97FF88";
    private const string LostColor = "#C3000A";

    public CanvasGroup gameControlsCanvasGroup;
    public CanvasGroup endGameCanvasGroup;
    public CanvasGroup allInGameUICanvasGroup;
    public CanvasGroup menuCanvasGroup;
    public Button attackButton;
    public TextMeshProUGUI gameResultText;
    public Character[] playerCharacter;
    public Character[] enemyCharacter;
    Character currentTarget;
    bool waitingForInput;

    Character FirstAliveCharacter(Character[] characters)
    {
        // LINQ: return enemyCharacter.FirstOrDefault(x => !x.IsDead());
        foreach (var character in characters)
        {
            if (!character.IsDead())
                return character;
        }

        return null;
    }

    void PlayerWon()
    {
        SetGameResultTextColor(WinColor);
        gameResultText.text = "You Won";
        Utility.SetCanvasGroupEnabled(endGameCanvasGroup, true);
    }

    void PlayerLost()
    {
        SetGameResultTextColor(LostColor);
        gameResultText.text = "You Lost";
        Utility.SetCanvasGroupEnabled(endGameCanvasGroup, true);
    }

    private void SetGameResultTextColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newCol))
        {
            gameResultText.color = newCol;
        }
    }

    bool CheckEndGame()
    {
        if (FirstAliveCharacter(playerCharacter) == null)
        {
            PlayerLost();
            return true;
        }

        if (FirstAliveCharacter(enemyCharacter) == null)
        {
            PlayerWon();
            return true;
        }

        return false;
    }

    void PlayerAttack()
    {
        waitingForInput = false;
    }

    public void NextTarget()
    {
        int index = Array.IndexOf(enemyCharacter, currentTarget);
        for (int i = 1; i < enemyCharacter.Length; i++)
        {
            int next = (index + i) % enemyCharacter.Length;
            if (!enemyCharacter[next].IsDead())
            {
                currentTarget.targetIndicator.gameObject.SetActive(false);
                currentTarget = enemyCharacter[next];
                currentTarget.targetIndicator.gameObject.SetActive(true);
                return;
            }
        }
    }

    IEnumerator GameLoop()
    {
        yield return null;
        while (!CheckEndGame())
        {
            foreach (var player in playerCharacter)
            {
                if (!player.IsDead())
                {
                    currentTarget = FirstAliveCharacter(enemyCharacter);
                    if (currentTarget == null)
                        break;

                    currentTarget.targetIndicator.gameObject.SetActive(true);
                    Utility.SetCanvasGroupEnabled(gameControlsCanvasGroup, true);

                    waitingForInput = true;
                    while (waitingForInput)
                        yield return null;

                    Utility.SetCanvasGroupEnabled(gameControlsCanvasGroup, false);
                    currentTarget.targetIndicator.gameObject.SetActive(false);

                    player.target = currentTarget.transform;
                    player.AttackEnemy();

                    while (!player.IsIdle())
                        yield return null;

                    break;
                }
            }

            foreach (var enemy in enemyCharacter)
            {
                if (!enemy.IsDead())
                {
                    Character target = FirstAliveCharacter(playerCharacter);
                    if (target == null)
                        break;

                    enemy.target = target.transform;
                    enemy.AttackEnemy();

                    while (!enemy.IsIdle())
                        yield return null;

                    break;
                }
            }
        }
    }

    void Start()
    {
        attackButton.onClick.AddListener(PlayerAttack);
        Utility.SetCanvasGroupEnabled(gameControlsCanvasGroup, false);
        Utility.SetCanvasGroupEnabled(endGameCanvasGroup, false);
        Utility.SetCanvasGroupEnabled(menuCanvasGroup, false);
        StartCoroutine(GameLoop());
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenMenu()
    {
        ShowMenu(true);
    }
    
    public void CloseMenu()
    {
        ShowMenu(false);
    }

    public void RestartGame()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    
    private void ShowMenu(bool show)
    {
        Utility.SetCanvasGroupEnabled(allInGameUICanvasGroup, !show);
        Utility.SetCanvasGroupEnabled(menuCanvasGroup, show);
    }
}
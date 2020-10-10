using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public CanvasGroup buttonPanel;
    public Button button;
    public Character[] playerCharacter;
    public Character[] enemyCharacter;
    Character currentTarget;
    bool waitingForInput;

    Character FirstAliveCharacter(Character[] characters)
    {
        // LINQ: return enemyCharacter.FirstOrDefault(x => !x.IsDead());
        foreach (var character in characters) {
            if (!character.IsDead())
                return character;
        }
        return null;
    }

    void PlayerWon()
    {
        Debug.Log("Player won.");
    }

    void PlayerLost()
    {
        Debug.Log("Player lost.");
    }

    bool CheckEndGame()
    {
        if (FirstAliveCharacter(playerCharacter) == null) {
            PlayerLost();
            return true;
        }

        if (FirstAliveCharacter(enemyCharacter) == null) {
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
        for (int i = 1; i < enemyCharacter.Length; i++) {
            int next = (index + i) % enemyCharacter.Length;
            if (!enemyCharacter[next].IsDead()) {
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
        while (!CheckEndGame()) {
            foreach (var player in playerCharacter) {
                if (!player.IsDead()) {
                    currentTarget = FirstAliveCharacter(enemyCharacter);
                    if (currentTarget == null)
                        break;

                    currentTarget.targetIndicator.gameObject.SetActive(true);
                    Utility.SetCanvasGroupEnabled(buttonPanel, true);

                    waitingForInput = true;
                    while (waitingForInput)
                        yield return null;

                    Utility.SetCanvasGroupEnabled(buttonPanel, false);
                    currentTarget.targetIndicator.gameObject.SetActive(false);

                    player.target = currentTarget.transform;
                    player.AttackEnemy();

                    while (!player.IsIdle())
                        yield return null;

                    break;
                }
            }

            foreach (var enemy in enemyCharacter) {
                if (!enemy.IsDead()) {
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

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(PlayerAttack);
        Utility.SetCanvasGroupEnabled(buttonPanel, false);
        StartCoroutine(GameLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

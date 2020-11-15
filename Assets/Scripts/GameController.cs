using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Character[] playerCharacters;
    public Character[] enemyCharacters;
    private Character _currentTarget;
    private bool _waitingPlayerInput;
    private AudioPlay _audioPlay;

    void Start()
    {
        _audioPlay = GetComponentInChildren<AudioPlay>();
        StartCoroutine(GameLoop());
    }

    [ContextMenu("Player Move")]
    public void PlayerMove()
    {
        if (_waitingPlayerInput)
            _waitingPlayerInput = false;
    }

    [ContextMenu("Switch character")]
    public void SwitchCharacter()
    {
        for (int i = 0; i < enemyCharacters.Length; i++) {
            // Найти текущего персонажа (i = индекс текущего)
            if (enemyCharacters[i] == _currentTarget) {
                int start = i;
                ++i;
                // Идем в сторону конца массива и ищем живого персонажа
                for (; i < enemyCharacters.Length; i++) {
                    if (enemyCharacters[i].IsDead())
                        continue;

                    // Нашли живого, меняем currentTarget
                    _currentTarget.GetComponentInChildren<TargetIndicator>(true).gameObject.SetActive(false);
                    _currentTarget = enemyCharacters[i];
                    _currentTarget.GetComponentInChildren<TargetIndicator>(true).gameObject.SetActive(true);

                    return;
                }
                // Идем от начала массива до текущего и смотрим, если там кто живой
                for (i = 0; i < start; i++) {
                    if (enemyCharacters[i].IsDead())
                        continue;

                    // Нашли живого, меняем currentTarget
                    _currentTarget.GetComponentInChildren<TargetIndicator>(true).gameObject.SetActive(false);
                    _currentTarget = enemyCharacters[i];
                    _currentTarget.GetComponentInChildren<TargetIndicator>(true).gameObject.SetActive(true);

                    return;
                }
                // Живых больше не осталось, не меняем currentTarget
                return;
            }
        }
    }

    void PlayerWon()
    {
        Debug.Log("Player won");
        _audioPlay.Play(SoundNames.Win);
    }

    void PlayerLost()
    {
        Debug.Log("Player lost");
        _audioPlay.Play(SoundNames.Lose);
    }

    Character FirstAliveCharacter(Character[] characters)
    {
        foreach (var character in characters) {
            if (!character.IsDead())
                return character;
        }
        return null;
    }

    bool CheckEndGame()
    {
        if (FirstAliveCharacter(playerCharacters) == null) {
            PlayerLost();
            return true;
        }

        if (FirstAliveCharacter(enemyCharacters) == null) {
            PlayerWon();
            return true;
        }

        return false;
    }

    IEnumerator GameLoop()
    {
        while (!CheckEndGame()) {
            foreach (var player in playerCharacters) {
                if (player.IsDead())
                    continue;

                Character target = FirstAliveCharacter(enemyCharacters);
                if (target == null)
                    break;

                _currentTarget = target;
                _currentTarget.GetComponentInChildren<TargetIndicator>(true).gameObject.SetActive(true);

                _waitingPlayerInput = true;
                while (_waitingPlayerInput)
                    yield return null;

                _currentTarget.GetComponentInChildren<TargetIndicator>().gameObject.SetActive(false);

                player.target = _currentTarget;
                player.AttackEnemy();
                while (!player.IsIdle())
                    yield return null;
            }

            foreach (var enemy in enemyCharacters) {
                if (enemy.IsDead())
                    continue;

                Character target = FirstAliveCharacter(playerCharacters);
                if (target == null)
                    break;

                enemy.target = target;
                enemy.AttackEnemy();
                while (!enemy.IsIdle())
                    yield return null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerDice = new GameObject[6];
    [SerializeField] private GameObject[] playerPlaceholderDice = new GameObject[6];
    [SerializeField] private GameObject[] playerActiveDice = new GameObject[6];

    [SerializeField] private GameObject[] eivorDice = new GameObject[6];
    [SerializeField] private GameObject[] eivorPlaceholderDice = new GameObject[6];
    [SerializeField] private GameObject[] eivorActiveDice = new GameObject[6];

    [SerializeField] private Sprite axe;
    [SerializeField] private Sprite arrow;
    [SerializeField] private Sprite arrow_plus;
    [SerializeField] private Sprite shield;
    [SerializeField] private Sprite shield_plus;
    [SerializeField] private Sprite helmet;
    [SerializeField] private Sprite helmet_plus;
    [SerializeField] private Sprite steal;
    [SerializeField] private Sprite steal_plus;
    private Sprite[,] diceFaces = new Sprite[6, 6];

    [SerializeField] private StartMatch startMatch;

    [SerializeField] private float rollTime;
    [SerializeField] private float diceChangeTime;

    private bool isPlayerStart;
    private GameObject[] remainingPlayerDice;
    private GameObject[] remainingEivorDice;
    private int playerRolls;
    private int eivorRolls;

    void Start()
    {
        setDiceActive(playerDice.Concat(playerPlaceholderDice).Concat(playerActiveDice)
            .Concat(eivorDice).Concat(eivorPlaceholderDice).Concat(eivorActiveDice).ToArray(), false);
        setDiceFaces();
    }

    private void setDiceActive(GameObject[] allDice, bool active)
    {
        foreach (GameObject dice in allDice)
        {
            dice.SetActive(active);
        }
    }

    public void startRound()
    {
        isPlayerStart = startMatch.getIsPlayerStart();
        remainingPlayerDice = (GameObject[]) playerDice.Clone();
        remainingEivorDice = (GameObject[]) eivorDice.Clone();
        playerRolls = 0;
        eivorRolls = 0;
        roll();
    }

    public void roll()
    {
        if (playerRolls == 3 && eivorRolls == 3)
        {
            Debug.Log("roll phase done");
            //move on to the next part of the round
        }
        else
        {
            bool isPlayerRoll = playerRolls != eivorRolls
                ? playerRolls < eivorRolls
                : isPlayerStart;

            if (isPlayerRoll && remainingPlayerDice.Length == 0)
            {
                playerRolls++;
                roll();
            }
            else if (!isPlayerRoll && remainingEivorDice.Length == 0)
            {
                eivorRolls++;
                roll();
            }
            else if (isPlayerRoll)
            {
                performRoll(remainingPlayerDice, remainingEivorDice);
                playerRolls++;
            }
            else
            {
                performRoll(remainingEivorDice, remainingPlayerDice);
                eivorRolls++;
            }
        }
    }

    private void performRoll(GameObject[] remainingDice, GameObject[] enemyDice)
    {
        setDiceActive(enemyDice, false);
        setDiceActive(remainingDice, true);
        int[] results = getDiceResults(remainingDice.Length);
        /*
        int[] currentDiceImages = getDiceResults(remainingDice.Length);
        Debug.Log(results);

        float lastDiceChange = 0f;
        float startRollTime = Time.time;
        Debug.Log(startRollTime);

        while (Time.time < startRollTime + rollTime)
        {
            Debug.Log(Time.time);
            if (Time.time >= lastDiceChange + diceChangeTime)
            {
                changeDiceImages(remainingDice, currentDiceImages);
                getNextDiceImages(currentDiceImages);
                lastDiceChange = Time.time;
            }
        }
        */

        changeDiceImages(remainingDice, results);
    }

    private void getNextDiceImages(int[] diceImages)
    {
        for (int i = 0; i < diceImages.Length; i++)
        {
            diceImages[i] = (diceImages[i] + 1) % diceImages.Length;
        }
    }

    private void changeDiceImages(GameObject[] remainingDice, int[] diceImages)
    {
        for (int i = 0; i < remainingDice.Length; i++)
        {
            GameObject dice = remainingDice[i];
            int diceNumber = int.Parse(dice.name.Substring(4));
            dice.GetComponentsInChildren<Image>()[0].sprite = diceFaces[diceNumber - 1, diceImages[i]];
        }
    }

    private int[] getDiceResults(int size)
    {
        int[] results = new int[size];

        for (int i = 0; i < size; i++)
        {
            results[i] = Random.Range(0, 6);
        }

        return results;
    }

    public void diceClick(Outline outline)
    {
        outline.enabled = !outline.enabled;
    }

    private void setDiceFaces()
    {
        diceFaces[0, 0] = axe;
        diceFaces[0, 1] = shield;
        diceFaces[0, 2] = arrow_plus;
        diceFaces[0, 3] = axe;
        diceFaces[0, 4] = helmet;
        diceFaces[0, 5] = steal_plus;

        diceFaces[1, 0] = axe;
        diceFaces[1, 1] = shield_plus;
        diceFaces[1, 2] = arrow;
        diceFaces[1, 3] = axe;
        diceFaces[1, 4] = steal_plus;
        diceFaces[1, 5] = helmet;

        diceFaces[2, 0] = axe;
        diceFaces[2, 1] = arrow_plus;
        diceFaces[2, 2] = steal;
        diceFaces[2, 3] = axe;
        diceFaces[2, 4] = helmet_plus;
        diceFaces[2, 5] = shield;

        diceFaces[3, 0] = axe;
        diceFaces[3, 1] = shield;
        diceFaces[3, 2] = steal_plus;
        diceFaces[3, 3] = arrow;
        diceFaces[3, 4] = helmet_plus;
        diceFaces[3, 5] = axe;

        diceFaces[4, 0] = axe;
        diceFaces[4, 1] = shield_plus;
        diceFaces[4, 2] = steal;
        diceFaces[4, 3] = axe;
        diceFaces[4, 4] = helmet;
        diceFaces[4, 5] = arrow_plus;

        diceFaces[5, 0] = axe;
        diceFaces[5, 1] = shield_plus;
        diceFaces[5, 2] = steal;
        diceFaces[5, 3] = axe;
        diceFaces[5, 4] = arrow;
        diceFaces[5, 5] = helmet_plus;
    }
}

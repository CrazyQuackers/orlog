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
    [SerializeField] private GameObject confirmUI;

    [SerializeField] private float diceRollTime;
    [SerializeField] private int diceRollFrames;
    [SerializeField] private float eivorThinkTime;

    private bool isPlayerStart;
    private List<GameObject> remainingPlayerDice = new List<GameObject>();
    private List<GameObject> remainingEivorDice = new List<GameObject>();
    private int playerRolls;
    private int eivorRolls;
    private List<string> chosenDice = new List<string>();

    void Start()
    {
        setDiceActive(playerDice.Concat(playerPlaceholderDice).Concat(playerActiveDice)
            .Concat(eivorDice).Concat(eivorPlaceholderDice).Concat(eivorActiveDice).ToList(), false);
        setDiceFaces();
        confirmUI.SetActive(false);
    }

    private void setDiceActive(List<GameObject> allDice, bool active)
    {
        foreach (GameObject dice in allDice)
        {
            dice.SetActive(active);
        }
    }

    public void startRound()
    {
        isPlayerStart = startMatch.getIsPlayerStart();
        remainingPlayerDice = playerDice.ToList();
        remainingEivorDice = eivorDice.ToList();
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

            if (isPlayerRoll && remainingPlayerDice.Count == 0)
            {
                playerRolls++;
                roll();
            }
            else if (!isPlayerRoll && remainingEivorDice.Count == 0)
            {
                eivorRolls++;
                roll();
            }
            else if (isPlayerRoll)
            {
                StartCoroutine(playerTurn());
            }
            else
            {
                StartCoroutine(eivorTurn());
            }
        }
    }

    IEnumerator playerTurn()
    {
        yield return StartCoroutine(performRoll(remainingPlayerDice));
        playerRolls++;

        if (playerRolls == 3)
        {
            confirmChoice(true, true);
        }
        else
        {
            confirmUI.SetActive(true);
        }
    }

    IEnumerator eivorTurn()
    {
        yield return StartCoroutine(performRoll(remainingEivorDice));
        eivorRolls++;
        yield return new WaitForSeconds(eivorThinkTime);
        eivorAIChooseDice();
        yield return new WaitForSeconds(eivorThinkTime);
        confirmChoice(false, false);
    }

    IEnumerator performRoll(List<GameObject> remainingDice)
    {
        setDiceActive(remainingDice, true);
        int[] currentDiceImages = getDiceResults(remainingDice.Count);

        for (int i = 0; i < diceRollFrames; i++)
        {
            changeDiceImages(remainingDice, currentDiceImages);
            getNextDiceImages(currentDiceImages);
            yield return new WaitForSeconds(diceRollTime);
        }

        changeDiceImages(remainingDice, getDiceResults(remainingDice.Count));
    }

    private void getNextDiceImages(int[] diceImages)
    {
        for (int i = 0; i < diceImages.Length; i++)
        {
            diceImages[i] = (diceImages[i] + 1) % diceImages.Length;
        }
    }

    private void changeDiceImages(List<GameObject> remainingDice, int[] diceImages)
    {
        for (int i = 0; i < remainingDice.Count; i++)
        {
            GameObject dice = remainingDice[i];
            int diceNumber = int.Parse(dice.name.Substring(4));
            dice.GetComponentInChildren<Image>().sprite = diceFaces[diceNumber - 1, diceImages[i]];
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

    public void diceClick(GameObject dice)
    {
        string name = dice.name;

        if (chosenDice.Contains(name))
        {
            chosenDice.Remove(name);
            dice.GetComponentInChildren<Outline>().enabled = false;
        }
        else
        {
            chosenDice.Add(name);
            dice.GetComponentInChildren<Outline>().enabled = true;
        }
    }

    private void eivorAIChooseDice()
    {
        //make AI choose best dice later
        int diceCount = remainingEivorDice.Count;
        int firstDiceNumber = Random.Range(0, diceCount);
        int secondDiceNumber = Random.Range(0, diceCount);

        while (firstDiceNumber == secondDiceNumber)
        {
            secondDiceNumber = Random.Range(0, diceCount);
        }

        diceClick(remainingEivorDice[firstDiceNumber]);
        diceClick(remainingEivorDice[secondDiceNumber]);
    }

    public void confirmClick()
    {
        confirmChoice(true, playerRolls == 3);
    }

    private void confirmChoice(bool isPlayerTurn, bool finalTurn)
    {
        confirmUI.SetActive(false);
        List<GameObject> remainingDice = isPlayerTurn ? remainingPlayerDice : remainingEivorDice;
        setDiceActive(remainingDice, false);

        foreach (GameObject dice in remainingDice)
        {
            dice.GetComponentInChildren<Outline>().enabled = false;

            if (chosenDice.Contains(dice.name) || finalTurn)
            {
                showPlaceHolderDice(isPlayerTurn, dice.GetComponentInChildren<Image>().sprite);
            }
        }

        setUpNextDiceRoll(isPlayerTurn, remainingDice);
    }

    private void showPlaceHolderDice(bool isPlayerTurn, Sprite diceImage)
    {
        GameObject[] placeholderDice = isPlayerTurn ? playerPlaceholderDice : eivorPlaceholderDice;

        for (int i = 0; i < placeholderDice.Length; i++)
        {
            GameObject dice = placeholderDice[i];

            if (!dice.activeSelf)
            {
                dice.GetComponentInChildren<Image>().sprite = diceImage;
                dice.SetActive(true);
                break;
            }
        }
    }

    private void setUpNextDiceRoll(bool isPlayerTurn, List<GameObject> remainingDice)
    {
        if (isPlayerTurn)
        {
            remainingPlayerDice = remainingDice.FindAll(dice => !chosenDice.Contains(dice.name));
        }
        else
        {
            remainingEivorDice = remainingDice.FindAll(dice => !chosenDice.Contains(dice.name));
        }

        chosenDice.Clear();
        roll();
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GodFavorPhase : MonoBehaviour
{

    [SerializeField] private StartMatch startMatch;
    [SerializeField] private DiceManager diceManager;
    [SerializeField] private GameObject dice;

    private GameObject[] playerPlaceholderDice;
    private GameObject[] eivorPlaceholderDice;

    private List<Sprite> playerDice;
    private List<Sprite> eivorDice;
    private float[] playerZPositions;
    private float[] eivorZPositions;

    private Sprite[] startingPlayerDiceOrder;
    private Sprite[] otherPlayerDiceOrder;

    private bool isPlayerStart;
    private float eivorThinkTime;

    private float playerX = 0.806f;
    private float eivorX = 1.344f;
    private float diceY = 0.379f;
    private float diceZ = 1.038f;
    private float diceIncrement = -0.25f;

    public IEnumerator godFavorPhase()
    {
        getData();
        yield return new WaitForSeconds(eivorThinkTime);
        diceManager.setDiceActive(playerPlaceholderDice.Concat(eivorPlaceholderDice).ToList(), false);
        sortDice();
        calculateZPositions();
        showDice();
    }

    private void showDice()
    {
        createDice(playerDice, playerX, playerZPositions, "Player");
        createDice(eivorDice, eivorX, eivorZPositions, "Eivor");
    }

    private void createDice(List<Sprite> diceToCreate, float diceX, float[] diceZPositions, string player)
    {
        for (int i = 0; i < diceToCreate.Count; i++)
        {
            GameObject newDice = Instantiate(dice, new Vector3(diceX, diceY, diceZPositions[i]), Quaternion.identity);
            newDice.GetComponentInChildren<Image>().sprite = diceToCreate[i];
            newDice.name = player + "ActiveDice" + (i + 1);
        }
    }

    private void calculateZPositions()
    {
        playerZPositions = new float[playerDice.Count];
        eivorZPositions = new float[eivorDice.Count];
        float currentZ = diceZ;
        int currentPlayerIndex = 0;
        int currentEivorIndex = 0;

        while (currentPlayerIndex < playerDice.Count || currentEivorIndex < eivorDice.Count)
        {
            if (currentPlayerIndex >= playerDice.Count)
            {
                currentEivorIndex = setEivorZPosition(currentEivorIndex, currentZ);
            }
            else if (currentEivorIndex >= eivorDice.Count)
            {
                currentPlayerIndex = setPlayerZPosition(currentPlayerIndex, currentZ);
            }
            else
            {
                Sprite playerDiceImage = playerDice[currentPlayerIndex];
                Sprite eivorDiceImage = eivorDice[currentEivorIndex];
                int playerOrderIndex = isPlayerStart ? getStartingPlayerOrderIndex(playerDiceImage) : getOtherPlayerOrderIndex(playerDiceImage);
                int eivorOrderIndex = isPlayerStart ? getOtherPlayerOrderIndex(eivorDiceImage) : getStartingPlayerOrderIndex(eivorDiceImage);

                if (playerOrderIndex == eivorOrderIndex)
                {
                    currentPlayerIndex = setPlayerZPosition(currentPlayerIndex, currentZ);
                    currentEivorIndex = setEivorZPosition(currentEivorIndex, currentZ);
                }
                else if (playerOrderIndex < eivorOrderIndex)
                {
                    currentPlayerIndex = setPlayerZPosition(currentPlayerIndex, currentZ);
                }
                else
                {
                    currentEivorIndex = setEivorZPosition(currentEivorIndex, currentZ);
                }
            }

            currentZ += diceIncrement;
        }
    }

    private void sortDice()
    {
        if (isPlayerStart)
        {
            playerDice.Sort(startingOrder);
            eivorDice.Sort(otherOrder);
        } 
        else
        {
            playerDice.Sort(otherOrder);
            eivorDice.Sort(startingOrder);
        }
    }

    private void getData()
    {
        isPlayerStart = startMatch.getIsPlayerStart();
        playerPlaceholderDice = diceManager.getPlayerPlaceholderDice();
        eivorPlaceholderDice = diceManager.getEivorPlaceholderDice();
        startingPlayerDiceOrder = diceManager.getStartingPlayerDiceOrder();
        otherPlayerDiceOrder = diceManager.getOtherPlayerDiceOrder();
        eivorThinkTime = diceManager.getEivorThinkTime();
        playerDice = getDiceImages(playerPlaceholderDice);
        eivorDice = getDiceImages(eivorPlaceholderDice);
    }

    private int startingOrder(Sprite current, Sprite next)
    {
        int currentIndex = findIndex(startingPlayerDiceOrder, current);
        int nextIndex = findIndex(startingPlayerDiceOrder, next);

        return currentIndex.CompareTo(nextIndex);
    }

    private int otherOrder(Sprite current, Sprite next)
    {
        int currentIndex = findIndex(otherPlayerDiceOrder, current);
        int nextIndex = findIndex(otherPlayerDiceOrder, next);

        return currentIndex.CompareTo(nextIndex);
    }

    private int findIndex(Sprite[] array, Sprite value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(value))
            {
                return i;
            }
        }

        return -1;
    }

    private List<Sprite> getDiceImages(GameObject[] placeholderDice)
    {
        List<Sprite> diceImages = new List<Sprite>();

        foreach (GameObject placeholder in placeholderDice)
        {
            diceImages.Add(placeholder.GetComponentInChildren<Image>().sprite);
        }

        return diceImages;
    }

    private int getStartingPlayerOrderIndex(Sprite diceImage)
    {
        return (findIndex(startingPlayerDiceOrder, diceImage) + 1) / 2;
    }

    private int getOtherPlayerOrderIndex(Sprite diceImage)
    {
        int index = findIndex(otherPlayerDiceOrder, diceImage);
        return index <= 4 ? index / 2 : (index + 1) / 2;
    }

    private int setPlayerZPosition(int index, float z)
    {
        playerZPositions[index] = z;
        return index + 1;
    }

    private int setEivorZPosition(int index, float z)
    {
        eivorZPositions[index] = z;
        return index + 1;
    }
}

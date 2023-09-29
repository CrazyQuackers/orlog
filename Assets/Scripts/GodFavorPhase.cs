using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GodFavorPhase : MonoBehaviour
{

    [SerializeField] private StartMatch startMatch;
    [SerializeField] private DiceManager diceManager;

    private GameObject[] playerPlaceholderDice;
    private GameObject[] eivorPlaceholderDice;

    private Sprite[] startingPlayerDiceOrder;
    private Sprite[] otherPlayerDiceOrder;

    private bool isPlayerStart;
    private float eivorThinkTime;

    public IEnumerator godFavorPhase()
    {
        getData();
        yield return new WaitForSeconds(eivorThinkTime);
        diceManager.setDiceActive(playerPlaceholderDice.Concat(eivorPlaceholderDice).ToList(), false);
        showDice();
    }

    private void showDice()
    {
        //
    }

    private void getData()
    {
        isPlayerStart = startMatch.getIsPlayerStart();
        playerPlaceholderDice = diceManager.getPlayerPlaceholderDice();
        eivorPlaceholderDice = diceManager.getEivorPlaceholderDice();
        startingPlayerDiceOrder = diceManager.getStartingPlayerDiceOrder();
        otherPlayerDiceOrder = diceManager.getOtherPlayerDiceOrder();
        eivorThinkTime = diceManager.getEivorThinkTime();
    }
}

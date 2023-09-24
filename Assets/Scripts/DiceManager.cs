using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerDice = new GameObject[6];
    [SerializeField] private GameObject[] playerPlaceholderDice = new GameObject[6];
    [SerializeField] private GameObject[] playerActiveDice = new GameObject[6];

    [SerializeField] private GameObject[] eivorDice = new GameObject[6];
    [SerializeField] private GameObject[] eivorPlaceholderDice = new GameObject[6];
    [SerializeField] private GameObject[] eivorActiveDice = new GameObject[6];

    [SerializeField] private StartMatch startMatch;

    void Start()
    {
        GameObject[] allDice = playerDice.Concat(playerPlaceholderDice).Concat(playerActiveDice)
            .Concat(eivorDice).Concat(eivorPlaceholderDice).Concat(eivorActiveDice).ToArray();

        foreach (GameObject dice in allDice)
        {
            dice.SetActive(false);
        }
    }

    public void roll()
    {
        bool isPlayerStart = startMatch.getIsPlayerStart();
    }
}

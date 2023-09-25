using TMPro;
using UnityEngine;

public class StartMatch : MonoBehaviour
{
    [SerializeField] private GameObject flipCoinUI;
    [SerializeField] private GameObject whoStartsUI;
    [SerializeField] private TextMeshProUGUI result;
    [SerializeField] private TextMeshProUGUI whoStartsText;
    [SerializeField] private TextMeshProUGUI actionText;

    private bool isPlayerStart;

    void Start()
    {
        flipCoinUI.SetActive(true);
        whoStartsUI.SetActive(false);
    }

    public void whoStarts(bool chosenHeads) 
    {
        int predictedCoinFlip = chosenHeads ? 0 : 1;
        int actualCoinFlip = Random.Range(0, 2);
        isPlayerStart = actualCoinFlip == predictedCoinFlip;

        result.text = actualCoinFlip == 0 ? "Heads!" : "Tails!";
        whoStartsText.text = isPlayerStart ? "You Start!" : "Eivor Starts!";
        actionText.text = isPlayerStart ? "Roll" : "Play";
        flipCoinUI.SetActive(false);
        whoStartsUI.SetActive(true);
    }

    public bool getIsPlayerStart()
    {
        return isPlayerStart;
    }
}

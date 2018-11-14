using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : Singleton<DiceController>
{
    [Header("Dices")]
    public Dice[] dices;
    public int diceSum = 0;

    [Header("Visuals")]
    public Text uiText = null;
    
    public IEnumerator RollDice(int n = -1)
    {
        if (uiText != null)
        {
            uiText.text = string.Format("Current Player: {0}", GameController.instance.currentPlayerIndex);
        }

        FollowCamera.Push(Board.instance.ground.transform);
        yield return new WaitForCamera();
        
        //TODO - Maybe this should be a flag (or something..) to allow other types of input?
        //(can we avoid making another WaitFor* instruction, but also avoid starting another heavy UnityCoroutine as in GameController?)
        yield return new WaitForKeyDown(KeyCode.Space);

        if (uiText != null)
        {
            uiText.text = "";
        }

        diceSum = 0;
        n = (n <= 0) ? dices.Length : Mathf.Min(dices.Length, n);
        for (int i = 0; i < n; ++i)
        {
            StartCoroutine(dices[i].ShakeDice());
        }

        while (!dices[0].rollComplete && !dices[1].rollComplete)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        FollowCamera.Pop();
        yield return new WaitForCamera();
    }

    public int RollFakeDice(int n = 1)
    {
        //TODO - make more elegant solution later?
        var cheat = FindObjectOfType<DiceCheat>();
        if (cheat != null)
        {
            return cheat.RollFakeDice(n);
        }

        int total = 0;
        for (int i = 0; i < n; ++i)
        {
            total += Random.Range(1, 6);
        }

        return total;
    }
}

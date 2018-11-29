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

#if UNITY_EDITOR
        //Dice force cheat
        var fakeDiceSum = RollFakeDice(n);
        if (fakeDiceSum > 0)
        {
            diceSum = fakeDiceSum;
            FollowCamera.Pop();
            yield return new WaitForCamera();
            yield break;
        }
#endif

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

#if UNITY_EDITOR
    public int RollFakeDice(int n = 1)
    {
        var cheat = FindObjectOfType<DiceCheat>();
        if (cheat == null)
        {
            return 0;
        }
        else
        {
            return cheat.RollFakeDice();
        }
    }
#endif
}

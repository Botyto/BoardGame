using System;
using System.Collections;
using System.Collections.Generic;
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

        if (GameSettings.Get<bool>("Shake"))
        {
            ///Debug.Log("Wait for shake (space key)!"); yield return new WaitForKeyDown(KeyCode.Space);
            // TODO: Think of way to optimise it ?

            AccelerometerReaction accelerometer = GameObject.Find("GameController").GetComponent<AccelerometerReaction>();
            yield return accelerometer.WaitForShake();
        }
        else
        {
            var msgBox = Dialog.Spawn<MessageBox>();
            msgBox.titleControl.text = "Throw dice";
            Debug.Log("Dice throwing skipped !");
            yield return new WaitForObjectDestroyed(msgBox.gameObject);
        }



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
        // Spawn dices
        for (int i = 0; i < n; i++)
        {
            GameObject dice = Resources.Load("Board/Die") as GameObject;
            var obj = GameObject.Instantiate(dice);
            dices[i] = obj.GetComponent<Dice>();
        }
        yield return null;

        // Shake dices and getting their values
        diceSum = 0;
        n = (n <= 0) ? dices.Length : Mathf.Min(dices.Length, n);
        for (int i = 0; i < n; ++i)
        {
            StartCoroutine(dices[i].ShakeDice());
        }

        // Wait all dices finish moving/rolling
        for (int i = 0; i < n; ++i)
        {
            while (!dices[i].rollComplete || !dices[i].GetComponent<Rigidbody>().IsSleeping())
            {
                yield return null;
            }
        }

        // Reroll (Dice is not grounded)
        for (int i = 0; i < n; ++i)
        {
            if (dices[i].GetCurrentValue() == 0)
            {
                yield return RollDice();
            }
        }

        // Show dices to Camera
        var sortedDice = SortDiceCameraAlongAxis(dices, Camera.main);
        for (int i = 0; i < n; ++i)
        {
            StartCoroutine(sortedDice[i].ShowToCamera(i));

        }
        yield return new WaitForSeconds(5f);

        // Destroy dices
        for (int i = 0; i < n; ++i)
        {
            Destroy(dices[i].gameObject);
        }

        FollowCamera.Pop();
        yield return new WaitForCamera();
    }

    private Dice[] SortDiceCameraAlongAxis(Dice[] list, Camera cam)
    {
        if (list.Length == 0) { return list; }

        var sorted = new KeyValuePair<Dice, float>[list.Length];
        for (int i = 0; i < list.Length; ++i)
        {
            var dice = list[i];
            var dist = -cam.WorldToScreenPoint(dice.transform.position).x;
            sorted[i] = new KeyValuePair<Dice, float>(dice, dist);
        }

        Array.Sort(sorted, (a, b) => Mathf.RoundToInt(a.Value - b.Value));
        var result = new Dice[list.Length];
        for (int i = 0; i < list.Length; ++i)
        {
            result[i] = sorted[i].Key;
        }

        return result;
    }

#if UNITY_EDITOR
    public int RollFakeDice(int n = 1)
    {
        var cheat = FindObjectOfType<DiceCheat>();

        if (GameSettings.Get<bool>("SkipThrow"))
        {
            int diceSum = 0;
            for (int i = 0; i < n; ++i)
            {
                diceSum += UnityEngine.Random.Range(1, 6);
            }
            Debug.LogFormat("Dice throw skipped - sum: {0}", diceSum);
            return diceSum;
        }
        if (cheat != null)
        {
            return cheat.RollFakeDice();
        }
        return 0;
    }
#endif
}

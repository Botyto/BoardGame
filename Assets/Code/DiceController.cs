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
        for (int i = 0; i < n; i++)
        {
            GameObject dice = Resources.Load("Board/Die") as GameObject;
            var obj = GameObject.Instantiate(dice);
            dices[i] = obj.GetComponent<Dice>();
        }
        yield return null;
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
        yield return new WaitForSeconds(1f);

        var sortedDice = SortDiceAlongAxis(dices, Camera.main.transform.right);
        for (int i = 0; i < n; ++i)
        {
            StartCoroutine(sortedDice[i].ShowToCamera(i == 0));
        }


        //yield return dices[1].ShowToCamera();

        yield return new WaitForSeconds(5f);

        for (int i = 0; i < n; ++i)
        {
            Destroy(dices[i].gameObject);
        }

        FollowCamera.Pop();
        yield return new WaitForCamera();
    }

    private Dice[] SortDiceAlongAxis(Dice[] list, Vector3 axis)
    {
        if (list.Length == 0) { return list; }

        var e1 = axis.normalized;
        var e2 = new Vector3(2 * e1.y * e1.z, -e1.x * e1.z, -e1.x * e1.y).normalized;
        var e3 = Vector3.Cross(e1, e2);
        Debug.Assert(Mathf.Approximately(Vector3.Dot(e1, e2), 0.0f));
        Debug.Assert(Mathf.Approximately(Vector3.Dot(e1, e3), 0.0f));
        Debug.Assert(Mathf.Approximately(Vector3.Dot(e2, e3), 0.0f));
        var T = new Matrix4x4(e1, e2, e3, new Vector4(0, 0, 0, 1)).inverse;
        
        var sorted = new KeyValuePair<Dice, float>[list.Length];
        for (int i = 0; i < list.Length; ++i)
        {
            var dice = list[i];
            var dist = T.MultiplyVector(dice.transform.position).x;
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

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiceCheat : MonoBehaviour
{
    public Slider[] sliders;
    public Text totalText;

    public void OnEnable()
    {
        if (sliders != null && sliders.Length > 0)
        {
            foreach (var slider in sliders)
            {
                slider.onValueChanged.AddListener(delegate (float n)
                {
                    var total = 0;
                    var allSet = true;
                    for (int i = 0; i < 2; ++i)
                    {
                        var _slider = sliders[i % sliders.Length];
                        if ((int)_slider.value == 0)
                        {
                            allSet = false;
                        }
                        else
                        {
                            total += (int)_slider.value;
                        }
                    }

                    slider.transform.parent.GetComponentInChildren<Text>().text = ((int)n).ToString();
                    totalText.text = allSet ? total.ToString() : (total.ToString() + "+?");
                });
            }
        }
    }

    public IEnumerator RollFakeDice(int n = 1)
    {
        if (sliders == null || sliders.Length == 0)
        {
            for (int i = 0; i < n; ++i)
            {
                DiceController.instance.diceSum += Random.Range(1, 6);
            }
        }
        else
        {
            for (int i = 0; i < n; ++i)
            {
                var slider = sliders[i % sliders.Length];
                if ((int)slider.value == 0)
                {
                    DiceController.instance.diceSum += Random.Range(1, 6);
                }
                else
                {
                    DiceController.instance.diceSum += (int)slider.value;
                }
            }
        }
        yield return null;

    }
}

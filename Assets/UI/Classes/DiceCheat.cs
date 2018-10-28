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

    public int RollDice(int n = 1)
    {
        var total = 0;
        if (sliders == null || sliders.Length == 0)
        {
            for (int i = 0; i < n; ++i)
            {
                total += Random.Range(1, 6);
            }
        }
        else
        {
            for (int i = 0; i < n; ++i)
            {
                var slider = sliders[i % sliders.Length];
                if ((int)slider.value == 0)
                {
                    total += Random.Range(1, 6);
                }
                else
                {
                    total += (int)slider.value;
                }
            }
        }

        return total;
    }
}

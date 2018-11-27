using System;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : Dialog
{
    [Serializable]
    public struct ButtonEntry
    {
        public string text;
        public Button.ButtonClickedEvent onClick;
    }

    [Header("Controls")]
    public Text titleControl;
    public Text textControl;
    public Button closeButton;
    public bool hasCloseButton = true;

    [Header("Buttons")]
    public ButtonEntry[] additionalButtons;

    public override void OnOpen()
    {
        foreach (var btn in additionalButtons)
        {
            var newButtonObj = Instantiate(closeButton, closeButton.transform.parent);
            var newButton = newButtonObj.GetComponent<Button>();
            newButton.onClick = btn.onClick;
            SetButtonText(newButton, btn.text);
        }

        if (hasCloseButton)
        {
            closeButton.transform.SetAsLastSibling();
            closeButton.onClick.AddListener(Close);
            SetButtonText(closeButton, "Close");
        }
        else
        {
            Destroy(closeButton.gameObject);
        }
    }

    private void SetButtonText(Button button, string text)
    {
        var textTransform = button.transform.Find("Text");
        if (textTransform == null) { return; }

        var textComp = textTransform.GetComponent<Text>();
        if (textComp == null) { return; }

        textComp.text = text;
    }
}

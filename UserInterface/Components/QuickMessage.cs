namespace ChatReset.UserInterface.Components;

public class QuickMessage : SUIText
{
    public QuickMessage(string sender, string message, Color messageColor)
    {
        WordWrap = true;

        TextScale = 0.8f;
        TextColor = messageColor;
        Text = $"[c/ff9999:<{DateTime.Now}>] [c/ffff00:<{sender}>] {message}";

        SetWidth(0f, 1f);
    }
}
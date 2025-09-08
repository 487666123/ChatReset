namespace ChatReset.UserInterface.Components;

public class WindowMessage : UIElementGroup
{
    public UITextView SenderText { get; private set; }
    public UITextView MessageText { get; private set; }

    /// <summary>
    /// 发送者和消息内容
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="message">消息</param>
    /// <param name="messageColor">消息颜色</param>
    /// <param name="myMessage">true: 消息在右边</param>
    public WindowMessage(string sender, string message, Color messageColor, bool myMessage = false)
    {
        FitWidth = true;
        SetWidth(percent: 1f);
        LayoutType = LayoutType.Flexbox;
        FlexDirection = FlexDirection.Row;
        MainAlignment = myMessage ? MainAlignment.End : MainAlignment.Start;

        var card = new UIElementGroup
        {
            LayoutType = LayoutType.Flexbox,
            FlexDirection = FlexDirection.Column,
            Gap = new Vector2(4f),
            Border = 2f,
            BorderColor = Color.Black * 0.2f,
            BackgroundColor = Color.Black * 0.1f,
            BorderRadius = new Vector4(8f),
        }.Join(this);
        card.SetWidth(0f, 0.65f);
        card.SetPadding(8f);

        var header = new UIElementGroup
        {
            LayoutType = LayoutType.Flexbox,
            FlexDirection = FlexDirection.Row,
            MainAlignment = MainAlignment.SpaceBetween,
        }.Join(card);
        header.SetWidth(0f, 1f);

        if (myMessage)
        {
            new UITextView
            {
                TextScale = 0.7f,
                TextColor = Color.Pink,
                Text = $"{DateTime.Now}",
                TextAlign = Vector2.Zero,
            }.Join(header);
        }

        SenderText = new UITextView
        {
            TextScale = 0.7f,
            TextColor = Color.Yellow,
            Text = $"{sender}",
            TextAlign = myMessage ? Vector2.One : Vector2.Zero,
        }.Join(header);

        if (!myMessage)
        {
            new UITextView
            {
                TextScale = 0.7f,
                TextColor = Color.Pink,
                Text = $"{DateTime.Now}",
                TextAlign = Vector2.Zero,
            }.Join(header);
        }

        MessageText = new UITextView
        {
            WordWrap = true,
            TextScale = 0.8f,
            Text = message,
            TextColor = messageColor,
        }.Join(card);
        MessageText.SetWidth(0f, 1f);
    }
}
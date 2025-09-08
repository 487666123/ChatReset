using ChatReset.UserInterface.Components;

namespace ChatReset.UserInterface;

[RegisterUI("Vanilla: Radial Hotbars", "SilkyUI: ChatWindowUI")]
public class ChatWindowUI : BasicBody
{
    public static ChatWindowUI Instance { get; private set; }
    public ChatWindowUI() => Instance = this;

    private SUIDraggableView MainPanel { get; set; }
    private SUIScrollView MessageContainer { get; set; }
    public SUIEditText EditText { get; private set; }

    protected override void OnInitialize()
    {
        Enabled = false;

        MainPanel = new SUIDraggableView
        {
            OccupyMouseInterface = true,
            LayoutType = LayoutType.Flexbox,
            FlexDirection = FlexDirection.Column,
            Border = 2f,
            Gap = new Vector2(-0.5f),
            FinallyDrawBorder = true,
            BorderRadius = new Vector4(12f),
            DragIncrement = new Vector2(5f),
            Width = new Dimension(600f),
            Left = new Anchor(0f, 0f, 0.5f),
            Top = new Anchor(0f, 0f, 0.5f),
        }.Join(this);

        MessageContainer = new SUIScrollView
        {
            BorderRadius = new Vector4(10f, 10f, 0f, 0f),
            CrossAlignment = CrossAlignment.End,
            BackgroundColor = Color.Black * 0.1f,
        }.Join(MainPanel);
        MessageContainer.SetPadding(8f);
        MessageContainer.SetSize(0, 340f, 1f);

        // 填充一大段空白, 使得滚动条在最底部
        var blankSpace = new SUIBlankSpace();
        blankSpace.Join(MessageContainer.Container);

        var greetings = new UITextView
        {
            TextScale = 0.6f,
            Text = Language.GetText("Mods.ChatReset.UI.KittenMeowing").Value,
            TextAlign = new Vector2(0.5f),
        };
        greetings.UseDeathText();
        greetings.SetSize(0, 0, 1f, 1f);
        greetings.Join(blankSpace);

        // 分界线
        SUIDividingLine.Horizontal(SUIColor.Border * 0.75f).Join(MainPanel);

        InputContainerInitialize();
        MessageContainer.ScrollBar.ScrollByEnd();
    }

    private void InputContainerInitialize()
    {
        var backgroundColor = new Color(63, 65, 151);
        var borderColor = new Color(18, 18, 38);

        var activeBar = new UIElementGroup
        {
            Gap = new Vector2(8f),
            LayoutType = LayoutType.Flexbox,
            FlexDirection = FlexDirection.Column,
            MainAlignment = MainAlignment.SpaceBetween,
            CrossAlignment = CrossAlignment.End,
            BorderRadius = new Vector4(0f, 0f, 10f, 10f),
            BackgroundColor = Color.Black * 0.1f,
        }.Join(MainPanel);
        activeBar.SetPadding(8f);
        activeBar.SetWidth(0f, 1f);

        var inputScrollView = new SUIScrollView
        {
            BackgroundColor = Color.Red * 0.25f,
        }.Join(activeBar);
        inputScrollView.DrawAction += delegate
        {
            inputScrollView.BorderColor =
                EditText.IsFocus && EditText.OccupyPlayerInput
                    ? new Color(240, 240, 20)
                    : new Color(18, 18, 38) * 0.75f;
        };
        inputScrollView.SetPadding(0f);
        inputScrollView.SetSize(0f, 100f, 1f, 0f);

        // 编辑框
        EditText = new SUIEditText
        {
            TextScale = 0.85f,
            TextAlign = new Vector2(0, 0f),
            WordWrap = true,
            MinHeight = new Dimension(0f, 1f),
            Padding = new Margin(2f, 0f),
        }.Join(inputScrollView);
        EditText.ContentChanged += delegate
        {
            if (EditText.CursorIndex == EditText.Text.Length)
                inputScrollView.ScrollBar.ScrollByEnd();
        };
        EditText.OnEnterKeyDown += () =>
        {
            SendChatMessageByLocalPlayer(EditText.Text);
            EditText.Text = "";
        };
        EditText.SetWidth(0f, 1f);

        var buttonContainer = new UIElementGroup
        {
            Gap = new Vector2(8f),
            LayoutType = LayoutType.Flexbox,
            FlexDirection = FlexDirection.Row,
            MainAlignment = MainAlignment.End,
        }.Join(activeBar);
        buttonContainer.SetWidth(0f, 1f);

        var sendImageButton = new UITextView
        {
            Padding = new Margin(10f, 0f),
            BorderRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BackgroundColor = backgroundColor * 0.75f,
            TextScale = 0.8f,
            Text = Language.GetText("Mods.ChatReset.UI.SendImage").Value,
            TextAlign = new Vector2(0.5f),
            TextColor = Color.Yellow
        }.Join(buttonContainer);
        sendImageButton.SetHeight(30f);

        var clearButton = new UITextView
        {
            Padding = new Margin(10f, 0f),
            BorderRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BackgroundColor = backgroundColor * 0.75f,
            TextScale = 0.8f,
            Text = Language.GetText("Mods.ChatReset.UI.Clear").Value,
            TextAlign = new Vector2(0.5f),
            TextColor = Color.Red
        }.Join(buttonContainer);
        clearButton.SetHeight(30f);
        clearButton.LeftMouseDown += (_, _) => ClearMessage();

        var send = new UITextView
        {
            Padding = new Margin(10f, 0f),
            BorderRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BackgroundColor = backgroundColor * 0.75f,
            TextScale = 0.8f,
            Text = Language.GetText("Mods.ChatReset.UI.Send").Value,
            TextAlign = new Vector2(0.5f),
        }.Join(buttonContainer);
        send.SetHeight(30f);
        send.LeftMouseDown += (_, _) =>
        {
            SendChatMessageByLocalPlayer(EditText.Text);
            EditText.Text = "";
        };

        send.RectangleRender.ShadowColor = borderColor * 0.1f;
        send.RectangleRender.ShadowSize = 5f;
        send.RectangleRender.ShadowBlurSize = 5f;
    }

    private readonly AnimationTimer _startTimer = new(2);

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        if (EditText.OccupyPlayerInput && EditText.IsFocus)
        {
            if (_startTimer.IsReverse)
                _startTimer.StartUpdate();
        }
        else
        {
            if (_startTimer.IsForward)
                _startTimer.StartReverseUpdate();
        }

        _startTimer.Update(gameTime);

        Opacity = _startTimer.Lerp(0f, 1f);
        var center = MainPanel.InnerBounds.Center;
        var scale = _startTimer.Lerp(0.8f, 1f);

        MainPanel.DisableMouseInteraction = _startTimer.IsReverse;
        UseRenderTarget = _startTimer.IsReverseCompleted;
    }

    private static void SendChatMessageByLocalPlayer(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (!HandleCommand(text))
        {
            var message = ChatManager.Commands.CreateOutgoingMessage(text);
            switch (Main.netMode)
            {
                case NetmodeID.MultiplayerClient:
                    ChatHelper.SendChatMessageFromClient(message);
                    break;
                case NetmodeID.SinglePlayer:
                    ChatManager.Commands.ProcessIncomingMessage(message, Main.myPlayer);
                    break;
            }
        }

        SoundEngine.PlaySound(SoundID.MenuClose);
    }

    private static bool HandleCommand(string text) =>
        text[0] == '/' && CommandLoader.HandleCommand(text, new ChatCommandCaller());

    /// <summary>
    /// 清除消息 (全部删除)
    /// </summary>
    public void ClearMessage()
    {
        if (MessageContainer is null) return;

        var children = MessageContainer.Container.Children.ToList();
        foreach (var child in children.Where(child => child is not SUIBlankSpace))
        {
            child?.Remove();
        }

        if (QuickBox.Instance is { } quickBox)
        {
            quickBox.ClearMessage();
        }
    }

    /// <summary>
    /// 清理消息 (默认最多拥有 101 条消息, 第一条是 SUIBlankSpace 要保留)
    /// </summary>
    private void CleanUpMessage(int max = 101)
    {
        if (MessageContainer?.Container == null) return;

        var children = MessageContainer.Container.Children.ToList();
        var count = children.Count - max;
        if (count <= 0) return;

        for (var i = 0; i < count; i++)
        {
            var child = children[i];
            if (child is not SUIBlankSpace)
                child.Remove();
        }

        MessageContainer.ScrollBar.CurrentScrollPosition = MessageContainer.ScrollBar.CurrentScrollPosition;
    }

    /// <summary>
    /// 追加一条消息
    /// </summary>
    public void AppendMessage(string sender, string message, Color messageColor, bool myMessage)
    {
        return;
        CleanUpMessage();
        new WindowMessage(sender, message, messageColor, myMessage).Join(MessageContainer);
        MessageContainer.ScrollBar.ScrollByEnd();

        if (QuickBox.Instance is { } quickBox)
        {
            quickBox.AppendMessage(sender, message, messageColor);
        }
    }
}
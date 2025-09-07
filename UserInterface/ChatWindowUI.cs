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

    public override void OnInitialize()
    {
        Enabled = true;

        MainPanel = new SUIDraggableView
        {
            OccupyMouseInterface = true,
            Display = Display.Flexbox,
            LayoutDirection = LayoutDirection.Column,
            Border = 2f,
            Gap = new Vector2(-0.5f),
            FinallyDrawBorder = true,
            CornerRadius = new Vector4(12f),
            Draggable = false,
            DragIncrement = new Vector2(5f)
        }.Join(this);
        MainPanel.HAlign = 0.5f;
        MainPanel.VAlign = 0.5f;
        MainPanel.SetWidth(600f);
        MainPanel.SetPadding(-1f);

        MessageContainer = new SUIScrollView
        {
            CornerRadius = new Vector4(10f, 10f, 0f, 0f),
            CrossAlignment = CrossAlignment.End,
            BgColor = Color.Black * 0.1f,
        }.Join(MainPanel);
        MessageContainer.SetPadding(8f);
        MessageContainer.SetSize(0, 340f, 1f);

        // 填充一大段空白, 使得滚动条在最底部
        var blankSpace = new SUIBlankSpace();
        blankSpace.Join(MessageContainer.Container);

        var greetings = new SUIText
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

        var activeBar = new View
        {
            Gap = new Vector2(8f),
            Display = Display.Flexbox,
            LayoutDirection = LayoutDirection.Column,
            MainAlignment = MainAlignment.SpaceBetween,
            CrossAlignment = CrossAlignment.End,
            CornerRadius = new Vector4(0f, 0f, 10f, 10f),
            BgColor = Color.Black * 0.1f,
        }.Join(MainPanel);
        activeBar.SetPadding(8f);
        activeBar.SetWidth(0f, 1f);

        var inputScrollView = new SUIScrollView
        {
            BgColor = Color.Red * 0.25f,
        }.Join(activeBar);
        inputScrollView.OnDraw += _ =>
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
            OverflowHidden = true,
            WordWrap = true,
            MinHeight = { Percent = 1f },
            PaddingLeft = 2f,
            PaddingRight = 2f,
        }.Join(inputScrollView);
        EditText.OnTextChanged += () =>
        {
            inputScrollView.Recalculate();
            if (EditText.CursorIndex == EditText.Text.Length)
                inputScrollView.ScrollBar.ScrollByEnd();
        };
        EditText.OnEnterKeyDown += () =>
        {
            SendChatMessageByLocalPlayer(EditText.Text);
            EditText.Text = "";
        };
        EditText.SetWidth(0f, 1f);

        var buttonContainer = new View
        {
            Gap = new Vector2(8f),
            Display = Display.Flexbox,
            LayoutDirection = LayoutDirection.Row,
            MainAlignment = MainAlignment.End,
        }.Join(activeBar);
        buttonContainer.SetWidth(0f, 1f);

        var sendImageButton = new SUIText
        {
            PaddingLeft = 10f,
            PaddingRight = 10f,
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
            TextScale = 0.8f,
            Text = Language.GetText("Mods.ChatReset.UI.SendImage").Value,
            TextAlign = new Vector2(0.5f),
            DragIgnore = false,
            TextColor = Color.Yellow
        }.Join(buttonContainer);
        sendImageButton.SetHeight(30f);

        var clearButton = new SUIText
        {
            PaddingLeft = 10f,
            PaddingRight = 10f,
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
            TextScale = 0.8f,
            Text = Language.GetText("Mods.ChatReset.UI.Clear").Value,
            TextAlign = new Vector2(0.5f),
            DragIgnore = false,
            TextColor = Color.Red
        }.Join(buttonContainer);
        clearButton.SetHeight(30f);
        clearButton.OnLeftMouseDown += (_, _) => ClearMessage();

        var send = new SUIText
        {
            PaddingLeft = 10f,
            PaddingRight = 10f,
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
            TextScale = 0.8f,
            Text = Language.GetText("Mods.ChatReset.UI.Send").Value,
            TextAlign = new Vector2(0.5f),
            DragIgnore = false,
        }.Join(buttonContainer);
        send.SetHeight(30f);
        send.OnLeftMouseDown += (_, _) =>
        {
            SendChatMessageByLocalPlayer(EditText.Text);
            EditText.Text = "";
        };

        send.RoundedRectangle.ShadowColor = borderColor * 0.1f;
        send.RoundedRectangle.ShadowExpand = 5f;
        send.RoundedRectangle.ShadowWidth = 5f;
    }

    private readonly AnimationTimer _startTimer = new(2);

    protected override void UpdateAnimationTimer(GameTime gameTime)
    {
        //if (EditText.OccupyPlayerInput && EditText.IsFocus)
        //{
        //    if (_startTimer.IsReverse)
        //        _startTimer.StartForwardUpdate();
        //}
        //else
        //{
        //    if (_startTimer.IsForward)
        //        _startTimer.StartReverseUpdate();
        //}
        if (_startTimer.IsReverse)
            _startTimer.StartForwardUpdate();

        _startTimer.Update((float)Main.gameTimeCache.ElapsedGameTime.TotalSeconds * 60f);

        base.UpdateAnimationTimer(gameTime);

        Opacity = _startTimer.Lerp(0f, 1f);
        var center = MainPanel.GetDimensions().Center();
        var scale = _startTimer.Lerp(0.8f, 1f);
        MainPanel.TransformMatrix =
            Matrix.CreateTranslation(-center.X, -center.Y, 0f) *
            Matrix.CreateScale(scale, scale, 1f) *
            Matrix.CreateTranslation(center.X, center.Y, 0f);

        MainPanel.Invalidate = _startTimer.IsReverse;
        MainPanel.Invalidate = _startTimer.Status is AnimationTimerStaus.ReverseUpdateCompleted;
        UseRenderTarget = _startTimer.Status is not AnimationTimerStaus.ReverseUpdateCompleted;
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

        MessageContainer.Recalculate();

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

        MessageContainer.Recalculate();
        MessageContainer.ScrollBar.CurrentScrollPosition = MessageContainer.ScrollBar.CurrentScrollPosition;
    }

    /// <summary>
    /// 追加一条消息
    /// </summary>
    public void AppendMessage(string sender, string message, Color messageColor, bool myMessage)
    {
        CleanUpMessage();
        new WindowMessage(sender, message, messageColor, myMessage).Join(MessageContainer);
        MessageContainer.Recalculate();
        MessageContainer.ScrollBar.ScrollByEnd();

        if (QuickBox.Instance is { } quickBox)
        {
            quickBox.AppendMessage(sender, message, messageColor);
        }
    }
}
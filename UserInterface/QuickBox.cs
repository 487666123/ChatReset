using ChatReset.UserInterface.Components;

namespace ChatReset.UserInterface;

/// <summary>
/// 未开启聊天窗时显示
/// </summary>
[RegisterUI("Vanilla: Radial Hotbars", "SilkyUI: QuickBox")]
public class QuickBox : BasicBody
{
    public static QuickBox Instance { get; private set; }

    public QuickBox() => Instance = this;

    private float _closeTimer;

    public SUIDraggableView MainPanel;
    public SUIScrollView MessageBox;

    public override void OnInitialize()
    {
        MainPanel = new SUIDraggableView
        {
            OccupyMouseInterface = false,
            Draggable = false,
            VAlign = 1f,
            CornerRadius = new Vector4(12f),
            BorderColor = Color.Transparent,
            BgColor = Color.Black * 0.15f,
        }.Join(this);
        MainPanel.RoundedRectangle.ShadowColor = Color.Transparent;
        MainPanel.SetSize(0, 200, 0.35f, 0f);
        MainPanel.SetPositionPixels(80f, -5f);

        MessageBox = new SUIScrollView
        {
            Gap = new Vector2(4f)
        }.Join(MainPanel);
        MessageBox.SetSize(0, 0, 1f, 1f);
        MessageBox.Container.SetPadding(2f);

        // 填充一大段空白, 使得滚动条在最底部
        var blankSpace = new SUIBlankSpace();
        blankSpace.Join(MessageBox.Container);
    }

    private AnimationTimer _startTimer = new(6);

    protected override void UpdateAnimationTimer(GameTime gameTime)
    {
        _closeTimer = Math.Min(3600, _closeTimer + (float)gameTime.ElapsedGameTime.TotalSeconds * 60f);

        if (MainPanel.IsMouseHovering)
        {
            _closeTimer = Math.Min(60 * 4, _closeTimer);
        }

        if (_closeTimer >= 60 * 5)
            _startTimer.StartForwardUpdate();
        else _startTimer.StartReverseUpdate();

        _startTimer.Update();
        base.UpdateAnimationTimer(gameTime);

        MainPanel.TransformMatrix = Matrix.CreateTranslation(0f, _startTimer.Lerp(0f, 250f), 0f);
    }

    /// <summary>
    /// 清除消息 (全部删除)
    /// </summary>
    public void ClearMessage()
    {
        if (MessageBox is null) return;

        var children = MessageBox.Container.Children.ToList();
        foreach (var child in children.Where(child => child is not SUIBlankSpace))
        {
            child?.Remove();
        }

        MessageBox.Recalculate();
    }

    /// <summary>
    /// 清理消息 (默认最多拥有 101 条消息, 第一条是 SUIBlankSpace 要保留)
    /// </summary>
    private void CleanUpMessage(int max = 101)
    {
        if (MessageBox?.Container == null) return;

        var children = MessageBox.Container.Children.ToList();
        var count = children.Count - max;
        if (count <= 0) return;

        for (var i = 0; i < count; i++)
        {
            var child = children[i];
            if (child is not SUIBlankSpace)
                child.Remove();
        }

        MessageBox.Recalculate();
        MessageBox.ScrollBar.CurrentScrollPosition = MessageBox.ScrollBar.CurrentScrollPosition;
    }

    public void AppendMessage(string sender, string message, Color messageColor)
    {
        CleanUpMessage();
        _closeTimer = 0f;
        new QuickMessage(sender, message, messageColor).Join(MessageBox.Container);
        MessageBox.Recalculate();
        MessageBox.ScrollBar.ScrollByEnd();
    }
}
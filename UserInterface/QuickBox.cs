using System.Windows.Forms;
using System.Xml;
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

    public SUIScrollView MessageBox;

    protected override void OnInitialize()
    {
        Left = new Anchor(80f);
        Top = new Anchor(-5f, 0f, 1f);
        Padding = new Margin(6f);
        BorderRadius = new Vector4(6f);
        BorderColor = Color.Transparent;
        BackgroundColor = Color.Black * 0.15f;
        RectangleRender.ShadowColor = Color.Transparent;

        SetSize(0, 200, 0.35f, 0f);

        MessageBox = new SUIScrollView
        {
            Gap = new Vector2(4f),
            Container =
            {
                FlexDirection = FlexDirection.Column,
                CrossAlignment = CrossAlignment.Stretch,
            }
        }.Join(this);
        MessageBox.SetSize(0, 0, 1f, 1f);

        // 填充一大段空白, 使得滚动条在最底部
        var blankSpace = new SUIBlankSpace
        {
            BackgroundColor = Color.Black * 0.25f
        };
        blankSpace.Join(MessageBox.Container);
        blankSpace.OnUpdateStatus += delegate
        {
            blankSpace.SetSize(MessageBox.Mask.InnerBounds.Width, MessageBox.Mask.InnerBounds.Height);
        };
    }

    private readonly AnimationTimer _startTimer = new(6);

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        AppendMessage("Player", "Message", Color.White);

        _closeTimer = Math.Min(3600, _closeTimer + (float)gameTime.ElapsedGameTime.TotalSeconds * 60f);

        if (IsMouseHovering)
        {
            _closeTimer = Math.Min(60 * 4, _closeTimer);
        }

        if (_closeTimer >= 60 * 5)
            _startTimer.StartUpdate();
        else _startTimer.StartReverseUpdate();

        _startTimer.Update(gameTime);
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

        MessageBox.ScrollBar.CurrentScrollPosition = MessageBox.ScrollBar.CurrentScrollPosition;
    }

    public void AppendMessage(string sender, string message, Color messageColor)
    {
        CleanUpMessage();
        _closeTimer = 0f;
        var quick = new QuickMessage(sender, message, messageColor).Join(MessageBox.Container);
        quick.IgnoreTextColor = false;
        MessageBox.ScrollBar.ScrollByEnd();
    }
}
using System.Text.RegularExpressions;
using ChatReset.UserInterface;
using Terraria.GameContent.UI.Chat;

namespace ChatReset.Hooks;

public partial class RemadeChatMonitorHook : ILoadable
{
    public void Load(Mod mod)
    {
        On_RemadeChatMonitor.AddNewMessage += AddNewMessageHook;
    }

    public void Unload()
    {
        On_RemadeChatMonitor.AddNewMessage -= AddNewMessageHook;
    }

    private static void AddNewMessageHook(On_RemadeChatMonitor.orig_AddNewMessage orig,
        RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
    {
        // orig?.Invoke(self, text, color, widthLimitInPixels);

        if (ChatWindowUI.Instance is not { } chatUI || string.IsNullOrEmpty(text)) return;
        var sender = MatchUsername().Match(text);
        if (sender.Success)
        {
            text = text[(sender.Value.Length + 1)..];
            var playerName = sender.Groups[1].Value;
            if (Main.LocalPlayer is { } localPlayer && localPlayer.name.Equals(playerName))
            {
                chatUI.AppendMessage(sender.Groups[1].Value, text, color, true);
            }
            else
                chatUI.AppendMessage(sender.Groups[1].Value, text, color, false);
        }
        else
        {
            chatUI.AppendMessage("系统消息", text, color, false);
        }
    }

    // public static bool HasChatSource() => ModLoader.HasMod("ChatSource");

    [GeneratedRegex(@"^\[n:(.*?)\]")]
    private static partial Regex MatchUsername();
}
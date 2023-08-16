namespace RisuAIApp;

using PrometheusAPI;

using RisuAIApp.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Bing
{
    public static async Task<string> SendAsync(string chat)
    {
        try
        {
            Conversation conv = await Conversation.CreateAsync();
            ChatResult result = await conv.ChatAsync(chat);
            await result.GetMessagesAsync();
            return result.SucessfulText;
        }
        catch
        {
            return null;
        }
    }

    public static string MessagesToText(IEnumerable<Message> messages) {
        StringBuilder sb = new();
        sb.AppendLine("Follow the instructions and fill out the answers from the assistants.");
        foreach(var msg in messages) {
            switch(msg.Role) {
                case Message.ROLE_USER:
                    sb.AppendLine("user:");
                    break;
                case Message.ROLE_ASSISTANT:
                    sb.AppendLine("assistant:");
                    break;
                case Message.ROLE_SYSTEM:
                    sb.AppendLine("instruction:");
                    break;
            }
            sb.AppendLine(msg.Content);
            sb.AppendLine();
        }
        return sb.ToString();
    } 
}

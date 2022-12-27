using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkDialogParser.Database.Models;
using VkDialogParser.Database;

namespace VkDialogParser.VkUtils
{
    internal static class VkProcessor
    {
        internal static async IAsyncEnumerable<ChatModel> ParseConversations(this VkHttpProvider vk, int count)
        {
            dynamic? response = await vk.GetAsync("messages.getConversations", new() { ["count"] = count + "" });

            foreach (dynamic item in response.response.items)
            {
                var chat = new ChatModel
                {
                    VkId = item.conversation.peer.id,
                    Name = item.conversation.chat_settings.title,
                };
                yield return chat;
            }
        }

        internal static async IAsyncEnumerable<MessageModel> ParseMessages(this VkHttpProvider vk, HttpClient http, ChatModel chat, int count = 200_000)
        {
            int offset = 0;
            while (true)
            {
                var args = new Dictionary<string, string>();
                args.Add("count", "200");
                args.Add("offset", offset + "");
                args.Add("peer_id", chat.VkId + "");

                dynamic? response = await vk.GetAsync("messages.getHistory", args);

                foreach (dynamic item in response.response.items)
                {
                    var msg = new MessageModel
                    {
                        VkId = item.id,
                        SenderVkId = item.from_id,
                        Chat = chat,
                        Text = item.text,
                        VkDate = Extensions.UnixTimeStampToDateTime((long)item.date)
                    };

                    foreach(dynamic content in item.attachments)
                    {
                        try
                        {
                            switch ((string)content.type)
                            {
                                case "audio_message":
                                {
                                    string link = content.audio_message.link_ogg;
                                    var ogg_response = await http.GetAsync(link);

                                    var data = await ogg_response.Content.ReadAsStreamAsync();

                                    var attach = new FileContentModel
                                    {
                                        FileName = link.Split('/').Last(),
                                        Message = msg,
                                        Content = data.ReadFully()
                                    };

                                    msg.Attachments.Add(attach);
    
                                }; break;
                            }
                        }
                        catch (Exception ex) { ex.Log(); }
                    }

                    yield return msg;

                }

                offset += 200;

                if (offset >= count) break;
            }

        }

    }
}

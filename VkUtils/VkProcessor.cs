using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkDialogParser.Database.Models;
using VkDialogParser.Database;
using System.Diagnostics;

namespace VkDialogParser.VkUtils
{
    internal static class VkProcessor
    {
        internal static async IAsyncEnumerable<ChatModel> ParseConversations(this VkHttpProvider vk, int count)
        {
            int offset = 0;
            while (true)
            {
                dynamic? response = null;

                try { response = await vk.GetAsync("messages.getConversations", new() { ["count"] = 200 + "", ["offset"] = offset + "" }); }
                catch (Exception ex) { ex.Log(); goto Step; }

                dynamic? items = null;
                try{ items = response.response.items; }
                catch (Exception ex) { ex.Log(); goto Step; }

                foreach (dynamic item in response.response.items)
                {
                    ChatModel? chat = null;

                    try
                    {
                        chat = new ChatModel
                        {
                            VkId = item.conversation.peer.id,
                            Name = item.conversation.chat_settings.title,
                        };
                    }
                    catch (Exception ex) { ex.Log(); }

                    if (chat is not null) yield return chat;
                }

                Step:
                offset += 200;

                if (offset >= count) break;
            }
        }

        internal static async Task<UserModel> GetUserInfo(this VkHttpProvider vk, EfModel db, HttpClient http, long VkId)
        {
            var user = db.Users.FirstOrDefault(u => u.VkId == VkId);
            if (user is not null) return user;

            var args = new Dictionary<string, string>();
            args.Add("user_ids", VkId + "");
            args.Add("fields", "status,photo_200,status,city,bdate");

            dynamic response = (await vk.GetAsync("users.get", args)).response[0];

            user = new UserModel
            {
                VkId = VkId,
                Name = response.first_name,
                Surname = response.last_name,
            };

            try { user.Status = response.status; } catch { }
            try { user.DateOfBirth = response.bdate; } catch { }
            try { user.City = response.city.title; } catch { }
            try
            {
                string link = response.photo_200;
                var contentResponse = await http.GetAsync(link);
                var data = await contentResponse.Content.ReadAsStreamAsync();
                user.Photo = data.ReadFully();
            }
            catch { }

            return user;
        }
        internal static async IAsyncEnumerable<MessageModel> ParseMessages(this VkHttpProvider vk, EfModel db, HttpClient http, ChatModel chat, Complete stopIndicator, int count = 200_000, int offset = 0)
        {
            async Task<MessageModel> setMessage(dynamic item) => new MessageModel
            {
                VkId = (long)item.id,
                Sender = await vk.GetUserInfo(db, http, (long)item.from_id),
                Chat = db.Chats.First(c => c.Id == chat.Id),
                Text = item.text,
                VkDate = (long)item.date
            };


            while (true)
            {

                var args = new Dictionary<string, string>();
                args.Add("count", "200");
                args.Add("offset", offset + "");
                args.Add("peer_id", chat.VkId + "");

                dynamic? response = null;

                try { response = await vk.GetAsync("messages.getHistory", args); }
                catch (Exception ex) { ex.Log(); goto Step; }

                bool contains = false;

                foreach (dynamic item in response.response.items)
                {
                    MessageModel? msg = null;
                    contains = true;

                    try { msg = await setMessage(item); }
                    catch (Exception ex) { ex.Log(); }

                    if (msg is null) { goto Step; }

                    try { msg.Replay = await setMessage(item.reply_message); }
                    catch { }

                    try
                    {
                        foreach (dynamic content in item.attachments)
                        {
                            try
                            {
                                string? link = null;
                                switch ((string)content.type)
                                {
                                    case "audio_message": link = content.audio_message.link_ogg; break;

                                    case "photo":
                                        {
                                            int MaxWidth = 0;
                                            dynamic? MaxSize = null;

                                            foreach (dynamic size in content.photo.sizes)
                                            {
                                                int width = (int)size.width;
                                                if (MaxWidth < width)
                                                {
                                                    MaxWidth = width;
                                                    MaxSize = size;
                                                }
                                            }
                                            if (MaxSize is not null)
                                            {
                                                link = (string)MaxSize.url;
                                            }

                                        }; break;

                                }

                                if (link is not null)
                                {
                                    var contentResponse = await http.GetAsync(link);

                                    var data = await contentResponse.Content.ReadAsStreamAsync();

                                    var attach = new FileContentModel
                                    {
                                        FileName = link.Split('/').Last().Split('?').First(),
                                        Message = msg,
                                        Content = link.Contains(".jpg", StringComparison.OrdinalIgnoreCase) 
                                                ? data.SaveImage() : data.ReadFully()
                                    };

                                    msg.Attachments.Add(attach);
                                }
                            }
                            catch (Exception ex) { ex.Log(); }
                        }
                    }
                    catch (Exception ex) { ex.Log(); }

                    if (msg is not null) yield return msg;

                }

                if (!contains)
                {
                    stopIndicator.IsComplete = true;
                    break;
                }
                
                Step:
                offset += 200;

                if (offset >= count) break;
            }

        }

    }
}

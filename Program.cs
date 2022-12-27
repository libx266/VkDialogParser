﻿using VkDialogParser.Database;
using VkDialogParser.Database.Models;
using VkDialogParser.VkUtils;

namespace VkDialogParser
{
    internal class Program
    {
        static void Main(string[] args) => Parse(args.First()).Wait();

        static async Task Parse(string token)
        {
            var vk = new VkHttpProvider(token);
            var http = new HttpClient();

            List<ChatModel>? chats = null;
            using (var db = new EfModel())
            {
                await vk.ParseConversations(800)
                        .ForEachAsync(chat => chat.Save(db, insert: true));
                chats = db.Chats.ToList();
            }

            foreach (var chat in chats)
            {
                using (var db = new EfModel())
                {
                    await vk.ParseMessages(http, chat, 1_000_000)
                            .ForEachAsync(msg => msg.Save(db, insert: true));
                }

                GC.Collect(); 
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}
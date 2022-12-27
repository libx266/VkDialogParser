using VkDialogParser.Database;
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
                var db = new EfModel();
                int count = 0;

                await foreach(var msg in vk.ParseMessages(http, chat, 1_000_000))
                {
                    if (msg.Replay is not null)
                        msg.Replay.Save(db, true);
                    
                    msg.Save(db, insert: true);
                    count++;

                    if (count % 1000 == 0)
                    {
                        db.Dispose();
                        http.Dispose();
                        vk.Dispose();

                        db = new();
                        http = new();
                        vk = new(token);

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }
               
            }
        }
    }
}
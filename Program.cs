using VkDialogParser.Database;
using VkDialogParser.Database.Models;
using VkDialogParser.VkUtils;

namespace VkDialogParser
{
    internal class Program
    {
        static void Main(string[] args) => Parse(args.First()).Wait(-1);

        static async Task Parse(string token)
        {

            List<ChatModel>? chats = null;
            using (var db = new EfModel())
            {
                using (var vk = new VkHttpProvider(token))
                {
                    await vk.ParseConversations(800)
                            .ForEachAsync(chat => chat.Save(db, insert: true));
                    chats = db.Chats.ToList();
                }
            }

            foreach (var chat in chats)
            {
                for (int i = 0; i < 1_000_000; i += 1000)
                {
                    using (var vk = new VkHttpProvider(token))
                    {
                        using (var http = new HttpClient())
                        {
                            using (var db = new EfModel())
                            {
                                await vk.ParseMessages(db, http, chat, 1_000, i)
                                        .ForEachAsync(msg => msg.Save(db, insert: true));
                            }
                        }
                    }
                }
               
            }
        }
    }
}
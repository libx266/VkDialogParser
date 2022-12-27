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

            using (var db = new EfModel())
            {
                await foreach(var chat in vk.ParseConversations(600))
                {
                    chat.Save(db, insert: true);

                    await foreach(var msg in vk.ParseMessages(http, chat))
                    {
                        msg.Save(db, insert: true);
                    }
                }
            }
        }
    }
}
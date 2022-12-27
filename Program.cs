using VkDialogParser.Database;
using VkDialogParser.Database.Models;
using VkDialogParser.VkUtils;

namespace VkDialogParser
{
    internal class Program
    {
        static void Main(string[] args) => Parse("vk1.a.Elo31yh-ZKCDyl9hQ_zhMk0B3TFuT1wsiIWfw5OaAqyVmsx99CT3B_Y-Mgcwqc8TwS0dn-0jrNSfRHYLTX68llYLZ9b_hM3xM79KXTLM2Lozo_RHlZrT5-OmXSAP2jBynHyJUQ_dps4p1ftT08QpUy6XMQOemogSmHrQZ0fEY95AFaG_cxg9r6V8zYzVMa0Xy1aYvk7Z0zPOmxJ1WcyEbg").Wait();

        static async Task Parse(string token)
        {
            var vk = new VkHttpProvider(token);
            var http = new HttpClient();

            using (var db = new EfModel())
            {
                await vk.ParseConversations(800).ForEachAsync(chat => chat.Save(db, insert: true));
                db.Chats.ToList().ForEach(async chat => await vk.ParseMessages(http, chat, 1_000_000).ForEachAsync(msg => msg.Save(db, insert: true)));
            }
        }
    }
}
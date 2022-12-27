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
            var client = new VkHttpProvider(token);

            dynamic? response = await client.GetAsync("messages.getDialogs", new() { ["count"] = 20.ToString() });

            using (var db = new EfModel())
            {
                foreach (dynamic item in response["response"]["items"])
                {
                    var chat = new ChatModel
                    {
                        VkId = item["message"].chat_id,
                        Name = item["message"].title,
                    };
                    chat.Save(db, insert: true);
                }
            }

            Console.WriteLine(response);
        }
    }
}
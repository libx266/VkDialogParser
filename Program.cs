using VkDialogParser.Database;
using VkDialogParser.Database.Models;
using VkDialogParser.VkUtils;

namespace VkDialogParser
{
    internal class Program
    {
        static void Main(string[] args) => Parse("vk1.a.33rhfHLwCwgKSFEaszV4qHycUIg3RGYoe9PqKy6VxBp2qn6uUZEKrTty-78-wFfMY11Enwa2gp1C5f01q7kjlkGCSSd3GR-PGBs3MYOXaiygeidxZbELP37DzbRfdUGwQFjD8K729TL_cJEfS7wbV-J71wfT0kr2rHJTZZQzO4lpu_sHCSyJICEjeuahLtECBO7PJQZLXyW8J-9Us3KQOQ").Wait();

        static async Task Parse(string token)
        {
            var client = new VkHttpProvider(token);

            using (var db = new EfModel())
            {
                await client.ParseMessages(new HttpClient(), new ChatModel { VkId = 2000000926, Name = "Kurwa" },400).ForEachAsync(chat => chat.Save(db, insert: true));
            }
        }
    }
}
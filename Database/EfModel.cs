using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkDialogParser.Database.Models;

namespace VkDialogParser.Database
{
    internal sealed class EfModel : DbContext
    {
        internal EfModel() : base() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Storage.db");
            base.OnConfiguring(optionsBuilder);
        }


        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<FileContentModel> Files { get; set; }
        public DbSet<ChatModel> Chats { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkDialogParser.Database.Models
{
    public class MessageModel : BaseModel
    {
        public long VkId { get; set; }

        public UserModel Sender { get; set; }

        public long VkDate { get; set; }

        [MaxLength(UInt16.MaxValue)]
        public string? Text { get; set; }

        
        public virtual ChatModel Chat { get; set; }

        public virtual MessageModel? Replay { get; set; }


        internal override bool Save(EfModel database, bool insert = false)
        {
            insert = insert && !database.Messages.Any(msg => msg.Chat.Id == Chat.Id && msg.VkId == VkId);
            bool result = base.Save(database, insert);
            if (result && insert)
            {
                foreach (var attach in Attachments)
                    attach.Save(database, true);
            }
            return result;
        }

        [NotMapped]
        public HashSet<FileContentModel> Attachments { get; set; } = new();

    }
}

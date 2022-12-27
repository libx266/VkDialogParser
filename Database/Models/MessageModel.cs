using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkDialogParser.Database.Models
{
    public class MessageModel : BaseModel
    {
        public long VkId { get; set; }

        public long SenderVkId { get; set; }

        public string SenderName { get; set; }

        public DateTime VkDate { get; set; }

        [MaxLength(UInt16.MaxValue)]
        public string? Text { get; set; }

        
        public virtual ChatModel Chat { get; set; }

        public virtual FileContentModel? Content { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkDialogParser.Database.Models
{
    public class ChatModel : BaseModel
    {
        public long VkId { get; set; }

        public string Name { get; set; }

    }
}

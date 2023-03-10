using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkDialogParser.Database.Models
{
    public class FileContentModel : BaseModel
    {
        [JsonIgnore]
        public MessageModel Message { get; set; } 

        public string FileName { get; set; }

        [JsonIgnore]
        public byte[] Content { get; set; }
    }
}

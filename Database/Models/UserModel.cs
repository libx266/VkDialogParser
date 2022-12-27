using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkDialogParser.Database.Models
{
    public class UserModel : BaseModel
    {
        public long VkId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string? Status { get; set; }

        public string? City { get; set; }

        public string? DateOfBirth { get; set; }

        public byte[]? Photo { get; set; }


        internal override bool Save(EfModel database, bool insert = false)
        {
            insert = insert && database.Users.Any(user => user.VkId == VkId);
            return base.Save(database, insert);
        }

    }
}

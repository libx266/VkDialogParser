using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkDialogParser.Database
{
    public abstract class BaseModel
    {
        public long Id { get; set; }


        private protected virtual bool DeleteActions(EfModel database)
        {
            database.Remove(this);
            return true;
        }

        internal bool Delete(EfModel database)
        {
            try
            {
                lock (this)
                {
                    bool result = DeleteActions(database);
                    if (result) database.SaveChanges();
                    return result;
                }

            }
            catch (Exception ex) { return ex.Log(); }
        }

        internal virtual bool Save(EfModel database, bool insert = false)
        {
            try
            {
                lock(this)
                {
                    if (insert) database.Add(this);
                    database.SaveChanges();
                }

                return true;
            }
            catch (Exception ex) { return ex.Log(); }
        }


        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

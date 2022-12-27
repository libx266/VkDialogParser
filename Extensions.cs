using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkDialogParser
{
    internal static class Extensions
    {
        internal static bool Log(this Exception ex)
        {
            Console.WriteLine(JsonConvert.SerializeObject(ex));
            return false;
        }
    }
}

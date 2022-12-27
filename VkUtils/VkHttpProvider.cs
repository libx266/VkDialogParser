using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VkDialogParser.VkUtils
{
    internal sealed class VkHttpProvider : IDisposable
    {
        private HttpClient _http;
        private readonly string _token;
        private readonly string _version;

        private readonly int delay;
        private DateTime LastAccess;


        internal VkHttpProvider(string token, string version = "5.131", int milisecondsDelay = 1200)
        {
            _http = new HttpClient();
            _http.BaseAddress = new Uri("https://api.vk.com/method/");

            _token = token;
            _version = version;

            delay = milisecondsDelay;
            LastAccess = DateTime.Now;
        }

        internal async Task<dynamic?> GetAsync(string method, Dictionary<string, string>? args = null)
        {
            int period = (int)(DateTime.Now - LastAccess).TotalMilliseconds;
            if (period < delay) await Task.Delay(delay - period);

            LastAccess = DateTime.Now;

            args ??= new();
            args.Add("access_token", _token);
            args.Add("v", _version);

            string urlArgs = string.Join("&", args.Select(kvp =>
            string.Format("{0}={1}", kvp.Key, kvp.Value)));

            string url = string.Format(method + "?{0}", urlArgs);

            var response = await _http.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        public void Dispose() => _http?.Dispose();
    }
}

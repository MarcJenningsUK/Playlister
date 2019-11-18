using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Playlister
{
    class Program
    {
        private const string home_assistant_update_method = "http://192.168.1.21:8123/api/services/input_select/set_options";
        private const string lms_address = "http://192.168.1.21:9000/jsonrpc.js";
        private const string home_assistant_access_token = "your token here";

        static void Main(string[] args)
        {
            var file = GetData(lms_address, "{\"id\":1,\"method\":\"slim.request\",\"params\":[\"\",[\"playlists\",0,10]]}").Result;
            ExtractLists(file);
            Console.WriteLine(file);
            System.IO.File.WriteAllText("file.json", file);
        }

        private static void ExtractLists(string json)
        {
            var temp = JsonConvert.DeserializeObject<PlaylistResult>(json);
            var RequestText = "[";
            foreach (var item in temp.result.playlists_loop)
            {
                RequestText += String.Format("\"[{0}] {1}\",", item.id, item.playlist);
            }
            RequestText = RequestText.Substring(0, RequestText.Length - 1) + "]";
            var output = "{\"entity_id\": \"input_select.playlists\",\"options\":" + RequestText + "}";

            Console.WriteLine(output);

            var result = GetData(home_assistant_update_method, output, home_assistant_access_token).Result;
            Console.WriteLine(result);
        }

        private static async Task<string> GetData(string Uri, string content, string AccessToken = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Uri);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if(AccessToken !=  null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                }

                var request = new HttpRequestMessage(HttpMethod.Post, Uri);
                request.Content = new StringContent(content,
                                                    Encoding.UTF8,
                                                    "application/json");

                var result = await client.SendAsync(request);
                var theResult = await result.Content.ReadAsStringAsync();
                return theResult;
            }
        }
    }

    public class PlaylistResult
    {
        public object[] _params { get; set; }
        public string method { get; set; }
        public Result result { get; set; }
        public int id { get; set; }
    }

    public class Result
    {
        public Playlists_Loop[] playlists_loop { get; set; }
        public int count { get; set; }
    }

    public class Playlists_Loop
    {
        public int id { get; set; }
        public string playlist { get; set; }
    }

}

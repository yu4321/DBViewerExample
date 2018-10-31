using Newtonsoft.Json.Linq;
using System.Text;

namespace DBViewerExample.Model
{
    public static class GlobalJSONData
    {
        public static string filepath;
        public static string prevURL = "";

        public static JObject contentJObject;

        public static JArray contentJArray;

        public static int Type = 0;

        public static Encoding nowencoding = Encoding.UTF8;
    }
}
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Steam_Icons_Fix
{
    public static class Steam
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task DownloadIconAsync(string appId, string iconHash, string savePath)
        {
            string iconUrl = $"https://cdn.cloudflare.steamstatic.com/steamcommunity/public/images/apps/{appId}/{iconHash}.ico";

            HttpResponseMessage response = await httpClient.GetAsync(iconUrl);
            response.EnsureSuccessStatusCode();

            using (Stream stream = await response.Content.ReadAsStreamAsync())
            {
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
    }
}

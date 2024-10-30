using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AscomPayPG.Helpers.HTTPHelper
{
    public class RequestHelper
    {
        private static HttpClient BuildHeader(HttpClient httpClient, Dictionary<string, string> headerNamesAndKeys)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (headerNamesAndKeys != null)
            {
                foreach (var item in headerNamesAndKeys)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            return httpClient;
        }

        private static HttpClient BuildPayload(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        public static async Task<HttpResponseMessage> PostWithRouteParametre(string url, Dictionary<string, string> headerNamesAndKeys)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildHeader(httpClient, headerNamesAndKeys);
                HttpResponseMessage response = await client.PostAsync(url, null);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostWithRouteParametresAndNoHeader(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildPayload(httpClient);
                HttpResponseMessage response = await client.PostAsync(url, null);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> Get(string url, Dictionary<string, string> headerNamesAndKeys)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildHeader(httpClient, headerNamesAndKeys);
                HttpResponseMessage response = await client.GetAsync(url);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostWithBody(string url, Dictionary<string, string> headerNamesAndValue, HttpContent body)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildHeader(httpClient, headerNamesAndValue);
                HttpResponseMessage response = await client.PostAsync(url, body);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostWithBody(string url, HttpContent body, Dictionary<string, string> Header)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildHeader(httpClient, Header);
                HttpResponseMessage response = await client.PostAsync(url, body);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostWithBody(string url, HttpContent body)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildHeader(httpClient, null);
                HttpResponseMessage response = await client.PostAsync(url, body);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> GetWithQuery(string urlWithQuery, Dictionary<string, string> headerNamesAndKeys)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildHeader(httpClient, headerNamesAndKeys);
                HttpResponseMessage response = await client.GetAsync(urlWithQuery);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> GetWithQuery(string urlWithQuery)
        {
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(urlWithQuery);
                return response;
            }
        }
        public static async Task<HttpResponseMessage> Patch(string urlWithQuery)
        {
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.PatchAsync(urlWithQuery, null);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> Patch(string url, HttpContent body, Dictionary<string, string> headerNamesAndKeys)
        {
            using (var httpClient = new HttpClient())
            {
                var client = BuildHeader(httpClient, headerNamesAndKeys);
                HttpResponseMessage response = await httpClient.PatchAsync(url, body);
                return response;
            }
        }
    }
}

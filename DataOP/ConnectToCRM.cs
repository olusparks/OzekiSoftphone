using System;
using System.Net.Http;

namespace zoiper_sdk.DataOP
{
    class ConnectToCRM
    {
        public static HttpClient TheHttpClient(string apiVersion)
        {
            Configuration config = new Configuration("CrmOnline");
            Authentication auth = new Authentication(config);

            string url = config.ServiceUrl;

            //string url = new Configuration("CrmOnline").ServiceUrl.ToString();

            string accessToken = Authentication.AcquireToken().AccessToken;

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url + apiVersion);
            httpClient.Timeout = new TimeSpan(0, 2, 0);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }
    }
}

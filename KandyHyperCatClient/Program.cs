using System;
using System.IO;
using System.Windows.Forms;
using RestSharp;

namespace KandyHyperCatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string appDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            string path = $"{appDirectory}{"\\Hypercat\\sensors.json"}";
            string sensorsCatalouge = File.ReadAllText(path);

            // Fetch a resource
            var catalougeName = "temperature";
            FetchHypercatCatalouge(catalougeName);

            // Create a resource
            CreateHypercatCatalouge("sensor1", sensorsCatalouge);

            // Delete a resource
            catalougeName = "temperature";
            DeleteHypercatCatalouge(catalougeName);

            Console.ReadLine();
        }

        /// <summary>
        /// Fetching the Hypercat Catalouge Resource from 
        /// RavenDB using Hypercat Server
        /// </summary>
        /// <param name="catalougeName">Catalouge Name</param>
        private static void FetchHypercatCatalouge(string catalougeName)
        {
            var client = new RestClient("http://localhost:53850/");
            var url = $"{"api/cats/"}{catalougeName}";

            Console.WriteLine("Requesting Hypercat Catalouge: " + catalougeName);
            var request = new RestRequest(url, Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string
            Console.WriteLine("Hypercat Catalouge Response for " + catalougeName);
            Console.WriteLine(content);
        }

        /// <summary>
        /// Create a new Hypercat Catalouge by Catalouge Name and Content (JSON Hypercat Catalouge)
        /// </summary>
        /// <param name="catalougeName">Catalouge Name</param>
        /// <param name="catalouge">Hypercat Catalouge Content</param>
        private static void CreateHypercatCatalouge(string catalougeName, string catalouge)
        {
            var client = new RestClient("http://localhost:53850/");
            var url = $"{"api/cats?catalougeName="}{catalougeName}";

            var request = new RestRequest(url, Method.POST);
            request.AddParameter("application/json", catalouge, 
                ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        /// <summary>
        /// Delete Hypercat Catalouge by it's name
        /// </summary>
        /// <param name="catalougeName">CatalougeName</param>
        private static void DeleteHypercatCatalouge(string catalougeName)
        {
            var client = new RestClient("http://localhost:53850/");
            var url = $"{"api/cats/"}{catalougeName}";

            var request = new RestRequest(url, Method.DELETE);
            IRestResponse response = client.Execute(request);
        }
    }
}

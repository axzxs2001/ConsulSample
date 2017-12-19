using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsulSharp
{
    /// <summary>
    /// Service Govern
    /// </summary>
    public class ServiceGovern
    {
        string _baseAddress;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">Base Address</param>
        public ServiceGovern(string baseAddress = "http://localhost:8500")
        {
            _baseAddress = baseAddress;
        }

        /// <summary>
        /// get service name by a datacetnter
        /// </summary>    
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param>
        /// <returns></returns>
        public async Task<string[]> GetServiceNames(string requestUrl = "/v1/catalog/services", string dataCenter = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{_baseAddress}{(!string.IsNullOrEmpty(dataCenter) ? $"?dc={dataCenter}" : "")}");

            var response = await client.GetAsync(requestUrl);
            var json = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    var services = new List<string>();
                    foreach (var serviceCheck in jsonObj)
                    {
                        services.Add(serviceCheck.Path);
                    }
                    return services.ToArray();
                }
                catch (JsonReaderException)
                {
                    throw new ApplicationException($"back content is error formatter:{json}");
                }
            }
            else
            {
                throw new ApplicationException($"back content is empty.");
            }

        }
        /// <summary>
        /// get service address
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param>
        /// <param name="serviceState">service state(enable or disable)</param>
        /// <returns></returns>
        public async Task<string[]> GetServices(string requestUrl = "/v1/catalog/services", string dataCenter = null, ServiceState serviceState = ServiceState.Full)
        {
            var serviceNames = await GetServiceNames(requestUrl, dataCenter);
            var services = new List<string>();
            foreach (var serviceName in serviceNames)
            {
                if(serviceName.ToLower()== "consul")
                {
                    continue;
                }
                var client = new HttpClient();
                client.BaseAddress = new Uri($"{_baseAddress}{(!string.IsNullOrEmpty(dataCenter) ? $"?dc={dataCenter}" : "")}");
                var response = await client.GetAsync($"/v1/catalog/service/{serviceName}");
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        dynamic jsonObj = JsonConvert.DeserializeObject(json);
                        foreach (var service in jsonObj)
                        {
                            services.Add($"{service.ServiceAddress}:{service.ServicePort}");
                        }
                    }
                    catch (JsonReaderException)
                    {
                        throw new ApplicationException($"back content is error formatter:{json}");
                    }
                }
                else
                {
                    throw new ApplicationException($"back content is empty.");
                }
            }
            return services.ToArray();
        }
    }
}

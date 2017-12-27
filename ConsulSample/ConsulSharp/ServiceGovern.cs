using ConsulSharp.Health;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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
        #region base method
        /// <summary>
        /// base call method
        /// </summary>
        /// <typeparam name="T">back type</typeparam>
        /// <param name="url">request url</param>
        /// <param name="dataCenter">datacenter</param>
        /// <returns></returns>
        private async Task<T> CallConsulAPI<T>(string url, string dataCenter = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{_baseAddress}{(!string.IsNullOrEmpty(dataCenter) ? $"?dc={dataCenter}" : "")}");
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var entity = JsonConvert.DeserializeObject<T>(json);
                    return entity;
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
        /// call url back json
        /// </summary>
        /// <param name="url">request url</param>
        /// <param name="dataCenter">datacenter</param>
        /// <returns></returns>
        public async Task<string> CallConsulReturnJson(string url,string dataCenter=null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{_baseAddress}{(!string.IsNullOrEmpty(dataCenter) ? $"?dc={dataCenter}" : "")}");
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        #endregion

        #region catalog
        /// <summary>
        /// get catalog datacenter
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> CatalogDatacenters()
        {           
            var json = await CallConsulReturnJson("/v1/catalog/datacenters");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    var services = new List<string>();
                    foreach (var serviceCheck in jsonObj)
                    {
                        services.Add(serviceCheck.Value);
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
        /// get catalog nodes
        /// </summary>
        /// <returns></returns>
        public async Task<HealthCatalogNode[]> CatalogNodes()
        {
            return await CallConsulAPI<HealthCatalogNode[]>("/v1/catalog/nodes");
        }
        /// <summary>
        /// get catalog node by name
        /// </summary>
        /// <returns></returns>
        public async Task<NodeService> CatalogNodeByName(string nodeName)
        {
            return await CallConsulAPI<NodeService>($"/v1/catalog/node/{nodeName}");
        }
        /// <summary>
        /// get catalog services
        /// </summary>    
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string[]>> CatalogServices(string dataCenter = null)
        {           

            var json = await CallConsulReturnJson("/v1/catalog/services", dataCenter);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    var services = new Dictionary<string, string[]>();
                    foreach (var serviceCheck in jsonObj)
                    {
                        var names = new List<string>();
                        foreach (var child in serviceCheck.Value)
                        {
                            names.Add(child.Value);
                        }
                        services.Add(serviceCheck.Name, names.ToArray());
                    }
                    return services;
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
        /// get catalog service by service name
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param> 
        /// <returns></returns>
        public async Task<CatalogService[]> CatalogServiceByName(string serviceName, string dataCenter = null)
        {
            return await CallConsulAPI<CatalogService[]>($"/v1/catalog/service/{serviceName}", dataCenter);
        }
        #endregion

        #region health

        /// <summary>
        /// get health services by service name
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param>
        /// <param name="serviceState">service state(enable or disable)</param>
        /// <returns></returns>
        public async Task<QueryHealthService[]> HealthServiceByName(string serviceName, string dataCenter = null)
        {
            return await CallConsulAPI<QueryHealthService[]>($"/v1/health/service/{serviceName}", dataCenter);           
        }

        #endregion

        #region register and deregister service
        /// <summary>
        /// register service
        /// </summary>
        /// <returns></returns>
        /// <param name="service">service</param>
        public async Task<(bool result, string backJson)> RegisterServices(Service service)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
            var json = JsonConvert.SerializeObject(service);
            var stream = new MemoryStream(Encoding.Default.GetBytes(json));
            var content = new StreamContent(stream);
            var response = await client.PutAsync($"/v1/agent/service/register", content);
            var backJson = await response.Content.ReadAsStringAsync();
            return (result: response.StatusCode == System.Net.HttpStatusCode.OK, backJson: backJson);
        }
        /// <summary>
        /// deregister service
        /// </summary>
        /// <returns></returns>
        /// <param name="serviceID">service ID</param>
        public async Task<(bool result, string backJson)> UnRegisterServices(string serviceID)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
            var response = await client.PutAsync($"/v1/agent/service/deregister/" + serviceID, null);
            var backJson = await response.Content.ReadAsStringAsync();
            return (result: response.StatusCode == System.Net.HttpStatusCode.OK, backJson: backJson);
        }
        #endregion

    }
}

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

        /// <summary>
        /// get service name by a datacetnter
        /// </summary>    
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param>
        /// <returns></returns>
        public async Task<string[]> GetServiceNames(string dataCenter = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{_baseAddress}{(!string.IsNullOrEmpty(dataCenter) ? $"?dc={dataCenter}" : "")}");

            var response = await client.GetAsync("/v1/catalog/services");
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
        /// <returns></returns>
        public async Task<string[]> GetServices(string dataCenter = null)
        {
            var serviceNames = await GetServiceNames(dataCenter);
            var services = new List<string>();
            foreach (var serviceName in serviceNames)
            {
                if (serviceName.ToLower() == "consul")
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

        /// <summary>
        /// get service address
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param> 
        /// <returns></returns>
        public async Task<string[]> GetServices(string serviceName, string dataCenter = null)
        {
            var services = new List<string>();
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

            return services.ToArray();
        }

        /// <summary>
        /// get service address by check service name
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param>
        /// <param name="serviceState">service state(enable or disable)</param>
        /// <returns></returns>
        public async Task<string[]> GetCheckServices(string dataCenter = null, ServiceState serviceState = ServiceState.Enable)
        {
            var serviceNames = await GetServiceNames(dataCenter);
            var services = new List<string>();
            foreach (var serviceName in serviceNames)
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri($"{_baseAddress}{(!string.IsNullOrEmpty(dataCenter) ? $"?dc={dataCenter}" : "")}");
                var response = await client.GetAsync($"/v1/health/service/{serviceName}");
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        dynamic jsonObj = JsonConvert.DeserializeObject(json);
                        foreach (var service in jsonObj)
                        {
                            switch (serviceState)
                            {
                                case ServiceState.Full:
                                    services.Add($"{service.Service.Address}:{service.Service.Port}");
                                    break;
                                case ServiceState.Enable:

                                    foreach (var check in service.Checks)
                                    {
                                        if (check.ServiceName == serviceName && check.Status == "passing")
                                        {
                                            services.Add($"{service.Service.Address}:{service.Service.Port}");
                                        }
                                    }
                                    break;
                                case ServiceState.Disable:
                                    foreach (var check in service.Checks)
                                    {
                                        if (check.ServiceName == serviceName && check.Status != "passing")
                                        {
                                            services.Add($"{service.Service.Address}:{service.Service.Port}");
                                        }
                                    }
                                    break;
                            }
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

        /// <summary>
        /// get service address by check service name
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        /// <param name="requestUrl">Request Url</param>
        /// <param name="dataCenter">Data Center Name</param>
        /// <param name="serviceState">service state(enable or disable)</param>
        /// <returns></returns>
        public async Task<string[]> GetCheckServices(string serviceName,string dataCenter = null, ServiceState serviceState = ServiceState.Enable)
        {
            var services = new List<string>();
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{_baseAddress}{(!string.IsNullOrEmpty(dataCenter) ? $"?dc={dataCenter}" : "")}");
            var response = await client.GetAsync($"/v1/health/service/{serviceName}");
            var json = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    foreach (var service in jsonObj)
                    {
                        switch (serviceState)
                        {
                            case ServiceState.Full:
                                services.Add($"{service.Service.Address}:{service.Service.Port}");
                                break;
                            case ServiceState.Enable:

                                foreach (var check in service.Checks)
                                {
                                    if (check.ServiceName == serviceName && check.Status == "passing")
                                    {
                                        services.Add($"{service.Service.Address}:{service.Service.Port}");
                                    }
                                }
                                break;
                            case ServiceState.Disable:
                                foreach (var check in service.Checks)
                                {
                                    if (check.ServiceName == serviceName && check.Status != "passing")
                                    {
                                        services.Add($"{service.Service.Address}:{service.Service.Port}");
                                    }
                                }
                                break;
                        }
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
            return services.ToArray();
        }


        /// <summary>
        /// Register Services
        /// </summary>
        /// <returns></returns>
        /// <param name="service">service</param>
        public async Task RegisterServices(Service service)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
            var json = JsonConvert.SerializeObject(service);
            var stream = new MemoryStream(Encoding.Default.GetBytes(json));
            var content = new StreamContent(stream);
            var response = await client.PutAsync($"/v1/agent/service/register", content);
            var backJson = await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 注销服务
        /// </summary>
        /// <returns></returns>
        /// <param name="serviceID">service ID</param>
        public async Task UnRegisterServices(string serviceID)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
            var json = $"{{service_id:{serviceID}}}";
            var stream = new MemoryStream(Encoding.Default.GetBytes(json));
            var content = new StreamContent(stream);
            var response = await client.PutAsync($"/v1/agent/service/deregister", content);
            var backJson = await response.Content.ReadAsStringAsync();
        }
            
    }
}

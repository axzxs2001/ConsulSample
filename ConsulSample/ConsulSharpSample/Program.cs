using System;
using System.Text;
using ConsulSharp;

namespace ConsulSharpSample
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1、注册管理  2、Catalog查询  3、Health查询   按e退出");
                switch (Console.ReadLine())
                {
                    case "1":
                        ServiceManage();
                        break;
                    case "2":
                        CatalogQuery();
                        break;
                    case "3":
                        HealthQuery();
                        break;
                    case "e":
                        return;
                }
            }
        }


        #region Health查询
        /// <summary>
        /// Health查询
        /// </summary>
        static void HealthQuery()
        {
            while (true)
            {
                Console.WriteLine("1、查询全部Catalog服务  2、按名称查Catalog服务  按e退出");
                switch (Console.ReadLine())
                {
                    case "1":

                        break;
                    case "2":
                        QueryHealthServicesByName();
                        break;
                    case "e":
                        return;
                }
            }
        }


        /// <summary>
        /// 按名称查询健康的服务 
        /// </summary>
        private static void QueryHealthServicesByName()
        {
            Console.WriteLine("请输入服务名称：");
            var serviceName = Console.ReadLine();
            var serviceGovern = new ServiceGovern();
            foreach (var healthService in serviceGovern.HealthServiceByName(serviceName: serviceName).GetAwaiter().GetResult())
            {
                Console.WriteLine($"服务名称：{healthService.Service.Service} {healthService.Service.Address}:{healthService.Service.Port}");

                foreach (var check in healthService.Checks)
                {
                    Console.WriteLine($"   CheckID:{check.CheckID}  状态：{check.Status} {check.Output}");
                }
            }
        }
        #endregion
        #region Catalog查询
        /// <summary>
        /// Catalog查询
        /// </summary>
        static void CatalogQuery()
        {
            while (true)
            {
                Console.WriteLine("1、查询Catalog数据中心  2、查询全部Catalog服务  3、按名称查Catalog服务   按e退出");
                switch (Console.ReadLine())
                {
                    case "1":
                        QueryCatalogDatacenters();
                        break;
                    case "2":
                        QueryCatalogServices();
                        break;
                    case "3":
                        QueryCatalogServiceByName();
                        break;
                    case "e":
                        return;
                }
            }
        }
        /// <summary>
        /// 查旬Catalog的数据中心
        /// </summary>
        private static void QueryCatalogDatacenters()
        {
            var serviceGovern = new ServiceGovern();
            var i = 1;
            foreach (var service in serviceGovern.CatalogDatacenters().GetAwaiter().GetResult())
            {
                Console.WriteLine($"{i++ }、{service}");
            }
        }

        /// <summary>
        /// 查旬Catalog的服务 
        /// </summary>
        private static void QueryCatalogServices()
        {
            var serviceGovern = new ServiceGovern();
            var i = 1;
            foreach (var service in serviceGovern.CatalogServices().GetAwaiter().GetResult())
            {
                var content = new StringBuilder($"{i++ }、{service.Key}:");

                foreach (var value in service.Value)
                {
                    content.Append($"{value}，");
                }
                Console.WriteLine(content.ToString().TrimEnd('，'));
            }
        }

        /// <summary>
        /// 按服务名称查询Catalog服务
        /// </summary>
        private static void QueryCatalogServiceByName()
        {
            Console.WriteLine("请输入服务名称：");
            var serviceName = Console.ReadLine();
            var serviceGovern = new ServiceGovern();
            foreach (var service in serviceGovern.CatalogServiceByName(serviceName).GetAwaiter().GetResult())
            {
                Console.WriteLine($"Node:{service.Node }  服务名称：{service.ServiceName} 地址：{service.ServiceAddress}:{service.ServicePort}");
            }
        }

        #endregion

        #region 注册，注销服务
        /// <summary>
        /// 服务注册，注销
        /// </summary>
        static void ServiceManage()
        {
            while (true)
            {
                Console.WriteLine("1、注册服务  2、注销服务  按e退出");
                switch (Console.ReadLine())
                {
                    case "1":
                        RegisterService();
                        break;
                    case "2":
                        UnRegisterService();
                        break;
                    case "e":
                        return;
                }
            }
        }

        /// <summary>
        /// Deregister Service注销服务 
        /// </summary>
        private static void UnRegisterService()
        {
            var serviceGovern = new ServiceGovern();
            var result = serviceGovern.UnRegisterServices("newservice001").GetAwaiter().GetResult();
            Console.WriteLine(result.backJson);
            Console.WriteLine(result.result);

        }

        /// <summary>
        /// Register Service注册服务
        /// </summary>
        private static void RegisterService()
        {
            var service = new Service();
            service.ID = "newservice001";
            service.Name = "newservice001";
            service.Address = "192.168.1.110";
            service.Port = 5005;
            service.Checks = new HttpCheck[1];
            service.Checks[0] = new HttpCheck { ID = "check1", Name = "check1", Http = "http://192.168.1.110:5005/health", Interval = "10s" };
            service.Tags = new string[] { "newservice001" };

            var serviceGovern = new ServiceGovern();
            var result = serviceGovern.RegisterServices(service).GetAwaiter().GetResult();
            Console.WriteLine(result.backJson);
            Console.WriteLine(result.result);

        }

        #endregion
    }
}

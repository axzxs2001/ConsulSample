using System;
using ConsulSharp;

namespace ConsulSharpSample
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1、注册服务  2、注销服务  3、查询服务");
                switch (Console.ReadLine())
                {
                    case "1":
                        RegisterService();
                        break;
                    case "2":
                        UnRegisterService();
                        break;
                    case "3":
                        QueryFullServices();
                        break;
                }
            }
        }

        private static void QueryFullServices()
        {
            var serviceGovern = new ServiceGovern();
            foreach (var address in serviceGovern.GetServices().GetAwaiter().GetResult())
            {
                Console.WriteLine(address);
            }
        }

        private static void UnRegisterService()
        {
            var serviceGovern = new ServiceGovern();
            var result = serviceGovern.UnRegisterServices("newservice001").GetAwaiter().GetResult();
            Console.WriteLine(result);
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
            service.Checks = new Check[1];
            service.Checks[0] = new HttpCheck { ID = "check1", Name = "check1", Http = "http://192.168.1.110:5005/health", Interval = "10s" };
            service.Tags = new string[] { "newservice001" };

            var serviceGovern = new ServiceGovern();
            var result = serviceGovern.RegisterServices(service).GetAwaiter().GetResult();
            Console.WriteLine(result);

        }
    }
}

using Config;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading;

namespace Centaline.Fyq.LogAnalyze.ElasticSearch
{
    public static class RedisHelper
    {
        private static string WriteHosts
        {
            get
            {
                return Config.ConfigHelper._RedisConfig.Host;
            }
        }
        private static object lockobj = new object();

        public static ConnectionMultiplexer ClientManagers
        {
            get
            {
                if (_ClientManagers == null)
                {
                    lock (lockobj)
                    {
                        if (_ClientManagers == null)
                        {
                            _ClientManagers = ConnectionMultiplexer.Connect(string.Format("password={0}", WriteHosts.Replace('@', ',')));
                        }
                    }
                }
                return _ClientManagers;
            }
        }
        /// <summary>
        /// 发布订阅提供者
        /// </summary>
        public static ISubscriber SubServer
        {
            get
            {
                return ClientManagers.GetSubscriber();
            }
        }
        static ConnectionMultiplexer _ClientManagers = null;
        public static T ListRightPop<T>(out string logStr)
        {
            try
            {
                return GetDataFromRedis<T>(out logStr);
            }
            catch (System.Exception ex)
            {
                Thread.Sleep(3000);
                return ListRightPop<T>(out logStr);
            }
        }

        public static T GetDataFromRedis<T>(out string logStr)
        {
            var strs = ClientManagers.GetDatabase().ListRightPop(ConfigHelper.AppName + ConfigHelper._RedisConfig.Key);
            logStr = string.Empty;
            if (strs.HasValue)
            {
                logStr = strs.ToString();
                var log = JsonConvert.DeserializeObject<T>(logStr);
                return log;
            }
            return default(T);
        }
    }
}

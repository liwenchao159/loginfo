using Config;
using Newtonsoft.Json;
using StackExchange.Redis;

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

        public static ConnectionMultiplexer ClientManager(string writeHosts)
        {

            if (_ClientManagers == null)
            {
                lock (lockobj)
                {
                    if (_ClientManagers == null)
                    {
                        _ClientManagers = ConnectionMultiplexer.Connect(string.Format("password={0}", writeHosts.Replace('@', ',')));
                    }
                }
            }
            return _ClientManagers;
        }
        static ConnectionMultiplexer _ClientManagers = null;


        public static T ListRightPop<T>(out string logStr)
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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Config
{
    public static class ConfigHelper
    {
        private static IServiceCollection services = new ServiceCollection();
        public static IConfigurationRoot _config;
        private static IConfigurationRoot GetConfigBuilder()
        {
            if (_config == null)
            {
                _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true).Build();
            }
            return _config;
        }
        /// <summary>
        /// 获取配置文件信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private static T GetAppSettings<T>(string key) where T : class, new()
        {
            return services.AddOptions().Configure<T>(GetConfigBuilder().GetSection(key)).BuildServiceProvider().GetService<IOptions<T>>().Value;
        }
        /// <summary>
        /// 环境参数
        /// </summary>
        public static string AppName
        {
            get
            {
                return GetConfigBuilder()["AppName"];
            }
        }
        /// <summary>
        /// es配置
        /// </summary>
        public static ElasticConfig _ElasticConfig
        {
            get
            {
                return GetAppSettings<ElasticConfig>("ElasticSearch");
            }
        }
        /// <summary>
        /// redis配置
        /// </summary>
        public static RedisConfig _RedisConfig
        {
            get
            {
                return GetAppSettings<RedisConfig>("Redis");
            }
        }

    }
    public class ElasticConfig
    {
        public string Host { get; set; }
        public string Index { get; set; }
    }

    public class RedisConfig
    {
        public string Host { get; set; }
        public string Key { get; set; }
    }
}

using Centaline.Fyq.LogAnalyze;
using Config;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ElasticSearch
{
    /// <summary>
    /// 配置获取
    /// </summary>
    public static class ElasticSearchHelper
    {
        private static object lockobj = new object();
        private static string LogIndex
        {
            get
            {
                return ConfigHelper.AppName.ToLower() + ConfigHelper._ElasticConfig.Index.ToLower();
            }
        }
        private static IConnectionPool _pool
        {
            get
            {
                var esHosts = ConfigHelper._ElasticConfig.Host.Split(';');
                var uris = esHosts.AsEnumerable().Select(t => new Uri(t));
                var nodess = uris.Select(t => new Node(t));
                var pools = new SniffingConnectionPool(nodess);
                return pools;
            }
        }
        private static ElasticClient _client;

        private static ElasticClient GetElasticClient()
        {
            if (_client == null)
            {
                lock (lockobj)
                {
                    if (_client == null)
                    {
                        var settings = new ConnectionSettings(_pool).DefaultIndex(LogIndex).DisableDirectStreaming(true);
                        _client = new ElasticClient(settings);
                    }
                }
            }
            return _client;
        }
        public static void CreateLogIndex()
        {
            var result = GetElasticClient().IndexExists(LogIndex);
            if (!result.Exists)
            {
                var indexDesc = new CreateIndexDescriptor(LogIndex).Settings(s => s.NumberOfShards(4).NumberOfShards(0)).
                    Mappings(ms => ms.Map<LogInfoDto>(m => m.AutoMap()));
                GetElasticClient().CreateIndex(indexDesc);
            }
        }
        /// <summary>
        /// 索引插入
        /// </summary>
        /// <param name="loginfo"></param>
        public static void InSertElastic(LogInfoDto loginfo)
        {
            try
            {
                if (loginfo.LogStatus == "UPDATE")
                {
                    InsertData(loginfo);
                }
                else
                {
                    var log = GetDataById(loginfo.RequestId);
                    if ((log != null && log.LogStatus == "INSERT") && log == null)
                    {
                        InsertData(log);
                    }
                    log = null;
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(3000);
                InSertElastic(loginfo);
            }
        }
        public static void InsertData(LogInfoDto loginfo)
        {
          //  CreateLogIndex();
            GetElasticClient().Index(new IndexRequest<LogInfoDto>(loginfo, LogIndex, "loginfo", loginfo.RequestId));
        }
        public static LogInfoDto GetDataById(string id)
        {
            var searchRequest = new SearchRequest<LogInfoDto>()
            {
                Query = new TermQuery { Field = new Field("_id"), Value = id }
            };
            return GetElasticClient().Search<LogInfoDto>(searchRequest).Documents.FirstOrDefault();
        }
    }
}

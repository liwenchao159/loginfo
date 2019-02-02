﻿using Centaline.Fyq.LogAnalyze;
using Config;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElasticSearch
{
    /// <summary>
    /// 配置获取
    /// </summary>
    public static class ElasticSearchHelper
    {
        private static ConcurrentDictionary<string, ElasticClient> _elasticClients = new ConcurrentDictionary<string, ElasticClient>();

        private static string LogIndex
        {
            get
            {
                return ConfigHelper.AppName.ToLower() + ConfigHelper._ElasticConfig.Index.ToLower();
            }
        }
        private static ElasticClient GetElasticClient()
        {
            ElasticClient _elasticClient;
            var eclient = _elasticClients.TryGetValue(ConfigHelper._ElasticConfig.Host, out _elasticClient);
            if (_elasticClient == null)
            {
                _elasticClients = new ConcurrentDictionary<string, ElasticClient>();
                var esHosts = ConfigHelper._ElasticConfig.Host.Split(';');
                var uris = esHosts.AsEnumerable().Select(t => new Uri(t));
                var nodess = uris.Select(t => new Node(t));
                var pools = new SniffingConnectionPool(nodess);
                var settings = new ConnectionSettings(pools).DefaultIndex(LogIndex).DisableDirectStreaming(true);
                _elasticClient = new ElasticClient(settings);
                _elasticClients.TryAdd(ConfigHelper._ElasticConfig.Host, _elasticClient);
            }
            return _elasticClient;
        }
        /// <summary>
        /// 集群是否可用
        /// </summary>
        public static bool ClusterIsValid
        {
            get
            {
                var status = GetElasticClient().ClusterState(t => new ClusterStateRequest());

                return GetElasticClient().ClusterHealth(t => new ClusterHealthRequest(LogIndex)).IsValid;
            }
        }
        public static void CreateLogIndex()
        {
            var result = GetElasticClient().IndexExists(LogIndex);
            if (!result.Exists)
            {
                var indexDesc = new CreateIndexDescriptor(LogIndex).Settings(s => s.NumberOfReplicas(4).NumberOfReplicas(0)).
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
            }
        }
        public static void InsertData(LogInfoDto loginfo)
        {
            CreateLogIndex();
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

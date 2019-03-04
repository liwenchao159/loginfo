using Centaline.Fyq.LogAnalyze.ElasticSearch;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Centaline.Fyq.LogAnalyze;
using ElasticSearch;
using Config;
using System.Net.NetworkInformation;

namespace Centaline.Fyq.LogAnalyze
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 发布订阅
            //var channel = "FYQ" + "logs_publish_channel";
            //RedisHelp.SubServer.Subscribe(channel, new Action<RedisChannel, RedisValue>((chan, message) =>
            //{
            //    Console.WriteLine(chan + "订阅到的消息是:" + message);
            //}));
            #endregion
            var logDirectionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            var path = Path.Combine(logDirectionPath, string.Format("log{0}.txt", DateTime.Now.ToString("yyyyMMdd")));
            var errpath = Path.Combine(logDirectionPath, string.Format("errlog{0}.txt", DateTime.Now.ToString("yyyyMMdd")));
            if (!Directory.Exists(logDirectionPath)) Directory.CreateDirectory(logDirectionPath);
            if (!File.Exists(path)) File.CreateText(path);
            if (!File.Exists(errpath)) File.CreateText(errpath);
            int i = 0;

            while (true)
            {
                   Console.WriteLine("数据查询中...testcore111....{0}", i);
                var str = string.Empty;
                i++;
                try
                {
                    var loginfo = RedisHelper.ListRightPop<LogInfoDto>(out str);
                    if (loginfo != null)
                    {
                        ElasticSearchHelper.InSertElastic(loginfo);
                        if (ConfigHelper.AppName == "FYQ")
                        {
                            // WriteInfoToFile(str, path);
                        }
                    }
                    else
                    {
                        Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    Thread.Sleep(3000);
                    WriteInfoToFile("ErrorLog_" + DateTime.Now.ToString() + ex.Message, errpath);
                }
                Thread.Sleep(10);
            }
        }

        public static void WriteInfoToFile(string str, string path)
        {
            if (!string.IsNullOrEmpty(str))
            {
                Console.WriteLine(str);
                //using (StreamWriter fs = new StreamWriter(path, true))
                //{
                //    fs.WriteLine(str);
                //}
            }
        }

    }
}

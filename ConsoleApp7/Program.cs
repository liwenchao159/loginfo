using Centaline.Fyq.LogAnalyze.ElasticSearch;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Centaline.Fyq.LogAnalyze;
using ElasticSearch;
using Config;

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
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                    var logDirectionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                    var path = Path.Combine(logDirectionPath, string.Format("log{0}.txt", DateTime.Now.ToString("yyyyMMdd")));
                    var errpath = Path.Combine(logDirectionPath, string.Format("errlog{0}.txt", DateTime.Now.ToString("yyyyMMdd")));
                    if (!Directory.Exists(logDirectionPath)) Directory.CreateDirectory(logDirectionPath);
                    if (!File.Exists(path)) File.CreateText(path);
                    if (!File.Exists(errpath)) File.CreateText(errpath);
                    var str = string.Empty;
                    LogInfoDto loginfo = null;
                    try
                    {
                        loginfo = RedisHelper.ListRightPop<LogInfoDto>(out str);
                        if (loginfo != null)
                        {
                            ElasticSearchHelper.InSertElastic(loginfo);
                            if (ConfigHelper.AppName == "FYQ")
                                WriteInfoToFile(str, path);
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteInfoToFile(str, errpath);
                    }
                }
            });
            Console.Read();
        }

        public static void WriteInfoToFile(string str, string path)
        {
            if (!string.IsNullOrEmpty(str))
            {
                Console.WriteLine(str);
                FileInfo fi = new FileInfo(path);
                using (StreamWriter fs = new StreamWriter(path, true))
                {
                    fs.WriteLine(str);
                }
            }
        }
    }
}

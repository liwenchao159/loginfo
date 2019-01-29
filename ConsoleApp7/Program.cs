using Centaline.Fyq.LogAnalyze.ElasticSearch;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Centaline.Fyq.LogAnalyze;
using ElasticSearch;

namespace ConsoleApp7
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
                    var path = AppDomain.CurrentDomain.BaseDirectory + string.Format("Log/log{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
                    var errpath = AppDomain.CurrentDomain.BaseDirectory + string.Format("Log/errlog{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
                    if (!File.Exists(path))
                    {
                        File.CreateText(path);
                    }
                    if (!File.Exists(errpath))
                    {
                        File.CreateText(errpath);
                    }
                    var str = string.Empty;
                    LogInfoDto loginfo = null;
                    try
                    {
                        loginfo = RedisHelper.ListRightPop<LogInfoDto>(out str);
                    }
                    catch (Exception ex)
                    {
                        WriteInfoToFile(str, errpath);
                        continue;
                    }
                    if (loginfo != null)
                    {

                        if (loginfo.Paramters.Contains("&"))
                        {
                            path = errpath;
                            continue;
                        }
                        else
                        {
                            ElasticSearchHelper.InSertElastic(loginfo);
                        }
                        WriteInfoToFile(str, path);
                    }
                    else
                    {
                        Thread.Sleep(100);
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

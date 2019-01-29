using Nest;
using Newtonsoft.Json;
using System;

namespace Centaline.Fyq.LogAnalyze
{

    [ElasticsearchType(Name = "loginfo")]
    public class LogInfoDto
    {
        /// <summary>
        /// 经纪人（用户类型）
        /// </summary>
        [Number]
        public int UserType { get; set; }
        /// <summary>
        /// 当前登录人
        /// </summary>
        [Keyword]
        public string EmpName { get; set; }
        /// <summary>
        /// 部门编号
        /// </summary>
        [Keyword]
        public string DeptId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Keyword]
        public string Mobile { get; set; }
        /// <summary>
        /// EmpId
        /// </summary>
        [Keyword]
        public string UserId { get; set; }
        /// <summary>
        /// 请求城市参数
        /// </summary>

        [Keyword]
        public string City { get; set; }
        /// <summary>
        /// 控制器
        /// </summary>
        [Keyword]
        public string Controller { get; set; }
        /// <summary>
        /// Action
        /// </summary>
        [Keyword]
        public string Action { get; set; }
        /// <summary>
        /// 请求参数
        /// {
        ///     GET:{}
        ///     POST:{}
        /// }
        /// </summary>
        [Text]
        public string Paramters { get; set; }
        [Object]
        public ParameterDto ParamDto
        {
            get
            {
                return JsonConvert.DeserializeObject<ParameterDto>(this.Paramters);
            }
        }
        /// <summary>
        /// 头文件参数
        /// </summary>
        [Text]
        public string HeaderInfo { get; set; }
        /// <summary>
        /// header => AuthToken
        /// </summary>
        [Object]
        public AuthTokenHeader AuthToken { get; set; }
        /// <summary>
        /// 请求开始时间
        /// </summary>
        [Date]
        public DateTime DoTime { get; set; }
        /// <summary>
        /// 请求数据状态
        /// </summary>
        [Keyword]
        public string Status { get; set; }
        /// <summary>
        /// 接口状态
        /// </summary>
        [Keyword]
        public string RequestStatus { get; set; }

        /// <summary>
        /// 异常描述（Message）
        /// </summary>
        [Text]
        public string Exception { get; set; }
        /// <summary>
        /// 接口执行时间
        /// </summary>
        [Number]
        public long TotalExecTime { get; set; }
        /// <summary>
        /// 请求Url
        /// </summary>
        [Text]
        public string RawUrl { get; set; }
        /// <summary>
        /// 请求唯一Id（服务端生成）
        /// </summary>
        [Keyword]
        public string RequestId { get; set; }
        /// <summary>
        /// INSERT，UPDATE
        /// UPDATE数据权重大于INSERT
        /// </summary>
        [Keyword]
        public string LogStatus { get; set; }
    }
    [ElasticsearchType(Name = "authtokenheader")]
    public class AuthTokenHeader
    {
        /// <summary>
        /// 授权码
        /// </summary>
        [Keyword]
        public Guid Token { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        [Keyword]
        public PlatformEnum Platform { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [Keyword]
        public string ClientVersion { get; set; }

        /// <summary>
        /// 更新号
        /// </summary>
        [Keyword]
        public int UpdateCode { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        [Keyword]
        public string DeviceId { get; set; }

        /// <summary>
        /// 其他信息
        /// </summary>
        [Text]
        public string OtherInfo { get; set; }
        /// <summary>
        /// 当前页面
        /// </summary>
        [Keyword]
        public string CurrentPage { get; set; }

        /// <summary>
        /// 是否单点登录
        /// </summary>
        [Number]
        public int IsSimpleLogin { get; set; }
        /// <summary>
        /// 是否是APP请求
        /// </summary>
        [Boolean]
        public bool IsApp
        {
            get
            {
                return Platform == PlatformEnum.android || Platform == PlatformEnum.ios;
            }
        }

        /// <summary>
        /// 是否是APP请求
        /// </summary>
        [Boolean]
        public bool IsMobile
        {
            get
            {
                return Platform == PlatformEnum.android || Platform == PlatformEnum.ios || Platform == PlatformEnum.wap;
            }
        }

        /// <summary>
        /// 公司信息
        /// </summary>
        [Text]
        public string CompanyPath { get; set; }


        /// <summary>
        /// 功能模块
        /// </summary>
        [Keyword]
        public string FunctionModule { get; set; }
        /// <summary>
        /// 页面名字
        /// </summary>
        [Text]
        public string PageName { get; set; }
        /// <summary>
        /// 上次请求ID，客户端（header）传递
        /// </summary>
        [Keyword]
        public string ParentRequestId { get; set; }


    }
    public enum PlatformEnum
    {
        ios,
        android,
        adminweb,
        wap
    }
}

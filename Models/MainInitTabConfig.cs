using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolapk_UWP.Models
{
    /// <summary>
    /// 主页侧边栏tab配置信息，记录了tab对应名称，子项目列表以及对应的Page页面信息，用于
    /// 点击即可打开对应的Page页面
    /// </summary>
    public class MainInitTabConfig
    {
        public string Title;
        [JsonProperty("page_name")]
        public string PageName;
        public string Url;
        public string Logo;
        [JsonProperty("entities")]
        public IList<MainInitTabConfig> SubTabs;
    }
}

using Coolapk_UWP.Controls;
using Coolapk_UWP.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Coolapk_UWP.ViewModels
{
    public class SearchResultViewModel
    {

        public List<TabInfo> GetTabItems(ICoolapkApis apis, String queryText)
        {
            var zhDataList = new FeedsDataList();
            zhDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, page: config.Page)).Data;


            // https://api.coolapk.com/v6/search?type=product&searchValue=a&page=1&showAnonymous=-1
            // lastItem maybe has a bug
            var digitalDataList = new FeedsDataList();
            digitalDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, type: "product", page: config.Page, lastItem: config.LastItem)).Data;


            var userDataList = new FeedsDataList();
            userDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, type: "user", page: config.Page, lastItem: config.LastItem)).Data;

            // TODO: sort selector, feed type selector
            var feedDataList = new FeedsDataList();
            feedDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, type: "feed", feedType: "all", sort: "default", page: config.Page, lastItem: config.LastItem)).Data;

            var topicDataList = new FeedsDataList();
            topicDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, type: "feedTopic", page: config.Page, lastItem: config.LastItem)).Data;

            var goodsDataList = new FeedsDataList();
            goodsDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, type: "goods", page: config.Page, lastItem: config.LastItem)).Data;

            // URL	https://api.coolapk.com/v6/search?type=ershou&sort=default&searchValue=a&status=1&deal_type=0&city_code=&is_link=&exchange_type=&ershou_type=&product_id=&page=1
            var ershouDataList = new FeedsDataList();
            ershouDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, type: "ershou", page: config.Page, lastItem: config.LastItem)).Data;

            // TODO: feed type selector
            var askDataList = new FeedsDataList();
            askDataList.CustomFetchDataEvent += async (config) => (await apis.Search(queryText, type: "ask", feedType: "all", page: config.Page, lastItem: config.LastItem)).Data;

            var tabItems = new List<TabInfo>() {
                    new TabInfo
                    {
                        Header = "动态",
                        Content = feedDataList
                    },
                    new TabInfo
                    {
                        Header = "综合",
                        Content = zhDataList
                    },
                    new TabInfo
                    {
                        Header = "数码",
                        Content = digitalDataList
                    },
                    new TabInfo
                    {
                        Header = "用户",
                        Content = userDataList
                    },
                    new TabInfo
                    {
                        Header = "话题",
                        Content = topicDataList
                    },
                    new TabInfo
                    {
                        Header = "好物",
                        Content = goodsDataList
                    },
                    new TabInfo
                    {
                        Header = "二手",
                        Content = ershouDataList
                    },
                    new TabInfo
                    {
                        Header = "问答",
                        Content = askDataList
                    }
                };

            return tabItems;
        }

        /// <summary>
        /// 获取标签页的标题与搜索结果页面
        /// </summary>
        /// <param name="apis"></param>
        /// <param name="queryText"></param>
        /// <returns></returns>

        public class TabInfo
        {
            private string header;

            private UserControl content;

            public string Header { get => header; set => header = value; }

            public UserControl Content { get => content; set => content = value; }

        }

    }
}

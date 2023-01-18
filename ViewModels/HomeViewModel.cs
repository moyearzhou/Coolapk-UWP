using Coolapk_UWP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Coolapk_UWP.Controls;
using System.Collections;

namespace Coolapk_UWP.ViewModels
{

    public class MenuItem 
    {
        public String Name { get; set; }
        public Symbol Icon { get; set; }
    }

    public class HomeMenuItem
    {
        public String Name { get; set; }
        public Symbol Icon { get; set; }
        public String Logo { get; set; }

        /// <summary>
        /// 点击item默认打开的页面的配置信息
        /// </summary>
        public MainInitTabConfig DefaultConfig { get; set; }
        public MainInitTabConfig Config { get; set; }
        public IList<HomeMenuItem> Children { get; set; }
    }

    /// <summary>
    /// 页面的类型，有：主页，数码，发现等
    /// </summary>
    public enum PageType
    {
        Home,
        Digital,
        Discovery
    }

    //public class SpecialHomeMenuItem : HomeMenuItem
    //{
    //    public string Tag;
    //}

    public class HomeNavigationViewMenuTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NavItemTemplate { get; set; }

        public DataTemplate NoIconTemplate { get; set; }
        public DataTemplate IconTemplate { get; set; }
        public DataTemplate LogoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is MenuItem)
            {
                return NavItemTemplate;
            } 
            else if (item is HomeMenuItem)
            {
                var _item = (HomeMenuItem)item;
                if (_item == null) return NoIconTemplate;
                if (_item.Config == null) return IconTemplate;
                else if (_item.Logo != null && _item.Logo.Length > 7) return LogoTemplate;
                else return NoIconTemplate;
            }

            return NoIconTemplate;
            //return null;
        }
    }

    public class HomeViewModel : AsyncLoadViewModel<IList<MainInit>>
    {
        // 主页tab页面
        public IList<MainInitTabConfig> HomeTabsConfig => Data?[1]?.Tabs;
        // 数码页面
        public IList<MainInitTabConfig> DigitalTabsConfig => Data?[3]?.Tabs;
        public IList<MainInitTabConfig> DiscoveryTabsConfig => Data?[2]?.Tabs;

        //当前页面的类型，默认为主页
        public PageType curPageType = PageType.Home;

        public IList<MenuItem> FootMenu = new List<MenuItem>()
        {
            new MenuItem() { Name = "发布动态", Icon = Symbol.Edit},
            new MenuItem() { Name = "账户", Icon = Symbol.Contact},
        };

        public IList<HomeMenuItem> Tabs {
            get
            {
                if (tabs == null) return convertConfigToTabs(HomeTabsConfig);
                return tabs;
            }
            set {
                tabs = value;
                NotifyChanged();
            } 
        }

        private IList<HomeMenuItem> tabs;

        public IList<HomeMenuItem> convertConfigToTabs(IList<MainInitTabConfig> config)
        {
            return config.Select(tab => new HomeMenuItem()
            {
                Name = tab.Title,
                Config = tab,
                Logo = tab.Logo,
                DefaultConfig = (tab.SubTabs?.Count ?? 0) > 0 ? tab.SubTabs[0] : null,
                Children = tab.SubTabs?.Select(stab => new HomeMenuItem()
                {
                    Name = stab.Title,
                    Config = stab,
                    Logo = stab.Logo,
                })?.ToList(),
            }).ToList();
        }

        private IList<HomeMenuItem> convertToTabs(PageType type)
        {
            switch (type)
            {
                case PageType.Home: return convertConfigToTabs(HomeTabsConfig);
                case PageType.Digital: return convertConfigToTabs(DigitalTabsConfig);
                case PageType.Discovery: return convertConfigToTabs(DiscoveryTabsConfig);

                default: return convertConfigToTabs(HomeTabsConfig);
            }
        }

        /// <summary>
        /// 设置当前页面的类型
        /// </summary>
        /// <param name="type"></param>
        public void setCurrentTabType(PageType type)
        {
            if (curPageType == type) return;

            Tabs = convertToTabs(type);
            curPageType= type;
        }

        /// <summary>
        /// 异步获取主页tab信息
        /// </summary>
        /// <returns></returns>
        public override async Task<RespBase<IList<MainInit>>> OnLoadAsync()
        {
            try
            {
                //todo: 
                var config = await CoolapkApis.GetMainInit();

                config.Data[1].Tabs[0].SubTabs.RemoveAt(0); // 关注分组 

                var jsonStr = JsonConvert.SerializeObject(config.Data);
                var tempFolder = ApplicationData.Current.TemporaryFolder;
                var mainInitConfigFile = await tempFolder.CreateFileAsync("mainInitConfigFile.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(mainInitConfigFile, jsonStr);

                return config;
            }
            catch (Exception exception)
            {
                var tempFolder = ApplicationData.Current.TemporaryFolder;
                try
                {
                    var mainInitConfigFile = await tempFolder.GetFileAsync("mainInitConfigFile.json");
                    return new RespBase<IList<MainInit>>
                    {
                        Data = JsonConvert.DeserializeObject<IList<MainInit>>(await FileIO.ReadTextAsync(mainInitConfigFile))
                    };
                }
                catch (Exception _)
                {
                    throw exception;
                }
            }
        }
    }
}


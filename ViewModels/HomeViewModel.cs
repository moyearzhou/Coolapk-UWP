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
        public IList<MainInitTabConfig> HomeTabs => Data?[1]?.Tabs;
        // 数码页面
        public IList<MainInitTabConfig> DigitalTabs => Data?[3]?.Tabs;
        public IList<MainInitTabConfig> DiscoveryTabs => Data?[2]?.Tabs;

        public IList<MenuItem> FootMenu = new List<MenuItem>()
        {
            new MenuItem() { Name = "发布动态", Icon = Symbol.Edit},
            new MenuItem() { Name = "账户", Icon = Symbol.Contact},
        };

       /// <summary>
       /// NavgationView页面的tab信息
       /// </summary>
        public IList<HomeMenuItem> Tabs
        {
            get
            {
                return HomeTabs == null ? new List<HomeMenuItem>() : new List<HomeMenuItem>() {
                    new HomeMenuItem() {
                        Name = "首页",
                        Icon = Symbol.Home,
                        //DefaultConfig = HomeTabs[1], // 头条
                        Children = HomeTabs.Select(tab => new HomeMenuItem(){
                            Name = tab.Title,
                            Config = tab,
                            Logo = tab.Logo,
                            DefaultConfig = (tab.SubTabs?.Count ?? 0) > 0 ? tab.SubTabs[0]: null,
                            Children = tab.SubTabs?.Select(stab => new HomeMenuItem(){
                                Name = stab.Title,
                                Config = stab,
                                Logo = stab.Logo,
                            })?.ToList(),
                        }).ToList(),
                    },
                    new HomeMenuItem() {
                        Name = "数码",
                        Icon = Symbol.CellPhone,
                        //DefaultConfig = DigitalTabs[0],
                        Children = DigitalTabs.Select(tab => new HomeMenuItem(){
                            Name = tab.Title,
                            Config = tab,
                            Logo = tab.Logo,
                        }).ToList(),
                    },
                    new HomeMenuItem() {
                        Name = "发现",
                        Icon = Symbol.Find,
                        //DefaultConfig = DiscoveryTabs[0],
                        Children = DiscoveryTabs.Select(tab => new HomeMenuItem(){
                            Name = tab.Title,
                            Config = tab,
                            Logo = tab.Logo,
                        }).ToList(),
                    }
                };
            }
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


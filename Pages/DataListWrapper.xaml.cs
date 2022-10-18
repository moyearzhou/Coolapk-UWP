using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Coolapk_UWP.ViewModels;
using Coolapk_UWP.Controls;
using Windows.ApplicationModel.Core;
using Coolapk_UWP.Other;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Coolapk_UWP.Pages
{

    // 为首页设计的，不周全
    public sealed partial class DataListWrapper : Page
    {
        public static String MODE_TOUTIAO = "toutiao";

        FeedsDataList CurDataList;
        HomeMenuItem PrePage;

        public DataListWrapper()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter == null) return;

            switch(e.Parameter)
            {
                case String mode:
                    if (mode == MODE_TOUTIAO)
                    {
                        FeedsDataList PageView = new FeedsDataList();
                        //DataList.SetValue(DataList.titleProperty, param.Config.Title);
                        //DataList.SetValue(DataList.urlProperty, param.Config.Url);
                        PageView.SetValue(FeedsDataList.PaddingProperty, new Thickness { Top = CoreApplication.GetCurrentView().TitleBar.Height + 12 });

                        PageView.SetValue(FeedsDataList.toutiaoMode, true);

                        Content = PageView;
                    }
                    break;
                case HomeMenuItem homeMenuItem:
                    if (PrePage != homeMenuItem && homeMenuItem.Config != null && e.NavigationMode != NavigationMode.Back)
                    {
                        PrePage = homeMenuItem;

                        FeedsDataList DataList = ConvertPage(homeMenuItem);

                        CurDataList = DataList;

                        Content = DataList;
                    }
                    break;
            }

      
        }

        private FeedsDataList ConvertPage(HomeMenuItem param)
        {
            FeedsDataList DataList = new FeedsDataList();
            DataList.SetValue(FeedsDataList.titleProperty, param.Config.Title);
            DataList.SetValue(FeedsDataList.urlProperty, param.Config.Url);
            DataList.SetValue(FeedsDataList.PaddingProperty, new Thickness { Top = CoreApplication.GetCurrentView().TitleBar.Height + 12 });
            if (param.Name == "头条")
                DataList.SetValue(FeedsDataList.toutiaoMode, true);

            return DataList;
        }

    }
}

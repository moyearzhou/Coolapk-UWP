using Coolapk_UWP.Controls;
using Coolapk_UWP.Network;
using Coolapk_UWP.Other;
using Coolapk_UWP.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NavigationViewItem = Windows.UI.Xaml.Controls.NavigationViewItem;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Coolapk_UWP.Pages
{
    public sealed partial class SearchResult
    {

        public SearchResultViewModel viewModel = new SearchResultViewModel();

        public SearchResult()
        {
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            this.InitializeComponent();
            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            //tabView.Resources["TabViewHeaderPadding"] = new Thickness { Top = coreTitleBar.Height };
        }

        private void CoreTitleBar_LayoutMetricsChanged(Windows.ApplicationModel.Core.CoreApplicationViewTitleBar sender, object args)
        {
            //tabView.SetValue(TabViewHeaderPadding, new Thickness { Top = sender.Height });
            //tabView.Resources["TabViewHeaderPadding"] = new Thickness { Top = sender.Height };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string queryText)
            {
                //tabView.TabItems.Clear();
                //tabView.IsAddTabButtonVisible = false;

                tabPivot.Items.Clear();

                var apis = App.AppViewModel.CoolapkApis;

                var tabItems = viewModel.GetTabItems(apis, queryText);

                foreach (var itemInfo in tabItems)
                {
                    //TabViewItem item = new TabViewItem();

                    PivotItem item = new PivotItem();
                    item.Header = itemInfo.Header;
                    item.Content = itemInfo.Content;

                    tabPivot.Items.Add(item);

                    //item.IsClosable = false;
                    //item.Padding = new Thickness { Top = 20 };
                    //tabView.TabItems.Add(item);
                }

                //设置默认选中第一个tab
                //((TabViewItem)tabView.TabItems[0]).IsSelected = true;
            }
        }

        

    }
   
}

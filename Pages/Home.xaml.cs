using Coolapk_UWP.Models;
using Coolapk_UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

namespace Coolapk_UWP.Pages
{
    public sealed partial class Home : Page
    {
        private HomeMenuItem CurrentMenuItem;
        Microsoft.UI.Xaml.Controls.NavigationView HomeNavigationView;

        /// <summary>
        ///  不带标签栏的内容布局容器Frame
        /// </summary>
        Frame ContentFrame;

        public Home()
        {
            this.InitializeComponent();
            ((HomeViewModel)DataContext).Reload();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null && e.Parameter is PageType pageType) {
                HomeViewModel viewModel = DataContext as HomeViewModel;

                if (ViewModel.curPageType != pageType)
                {
                    viewModel.setCurrentTabType(pageType);
                    //刷新页面
                    viewModel.Reload();
                }
            }
        }


        public void AsyncLoadStateControl_Retry(object sender, RoutedEventArgs a)
        {
            ((HomeViewModel)DataContext).Reload();
        }

        private void ContentFrameControl_Loaded(object sender, RoutedEventArgs e)
        {
           //App.AppViewModel.HomeContentFrame = sender as Frame;
           ContentFrame = sender as Frame;
            if ((sender as Frame).BackStackDepth == 0)
            {
                //默认加载头条列表
                ContentFrame.Navigate(typeof(DataListWrapper), DataListWrapper.MODE_TOUTIAO);
            }

        }

        private void NavigationViewControl_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            //if (App.AppViewModel.HomeContentFrame.CanGoBack) App.AppViewModel.HomeContentFrame.GoBack();
            if (ContentFrame.CanGoBack) ContentFrame.GoBack();
        }

        private void NavigationViewControl_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            switch (args.SelectedItem)
            {
                case HomeMenuItem menuItem:
                    var frame = ContentFrame;
                    MainInitTabConfig target;
                    if (menuItem.Children != null && menuItem.Children.Count > 0 && CurrentMenuItem == null)
                    {
                        //target = menuItem.DefaultConfig ?? menuItem.Children[0].Config;
                        //var targetItem = menuItem.Children.First(child => child.Config == target);
                        //if (targetItem == null) return;
                        //_ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        //{
                        //    sender.SelectedItem = targetItem;
                        //});
                    }
                    else
                    { // 最终目标tab
                        target = menuItem.Config;
                        if (CurrentMenuItem != menuItem)
                        {
                            frame.Navigate(typeof(DataListWrapper), menuItem);
                        }
                        CurrentMenuItem = menuItem;

                    }
                    break;
            }
        }

        private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            HomeNavigationView = sender as Microsoft.UI.Xaml.Controls.NavigationView;

        }

    }
}

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
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;
using Windows.UI.Xaml.Media.Animation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Coolapk_UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {

        private HomeMenuItem CurrentMenuItem;
        Microsoft.UI.Xaml.Controls.NavigationView HomeNavigationView;

        public HomePage()
        {

            this.InitializeComponent();
            ((HomeViewModel)DataContext).Reload();

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            Window.Current.SetTitleBar(AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            Window.Current.Activated += Current_Activated;
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Windows.UI.Xaml.Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Windows.UI.Xaml.Application.Current.Resources["TextFillColorDisabledBrush"];
            AppTitle.Foreground = e.WindowActivationState == CoreWindowActivationState.Deactivated ? inactiveForegroundBrush : defaultForegroundBrush;
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        public void AsyncLoadStateControl_Retry(object sender, RoutedEventArgs a)
        {
            ((HomeViewModel)DataContext).Reload();
        }

        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;
            // 如果返回按钮未显示，则削减TitleBar的空间
            if (HomeNavigationView == null) return;
            if (HomeNavigationView.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                // 如果是Top Mode
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Left, currMargin.Right);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                // 如果是 minimal
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        private void ContentFrameControl_Loaded(object sender, RoutedEventArgs e)
        {
            App.AppViewModel.HomeContentFrame = sender as Frame;
            if ((sender as Frame).BackStackDepth == 0)
            {
                //跳转到导航页面
                App.AppViewModel.HomeContentFrame.Navigate(typeof(LaunchPad));
            }

        }

        private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            HomeNavigationView = sender as Microsoft.UI.Xaml.Controls.NavigationView;
        }

        /// <summary>
        /// 后退导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavigationViewControl_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (App.AppViewModel.HomeContentFrame.CanGoBack) App.AppViewModel.HomeContentFrame.GoBack();
        }

        private void NavigationViewControl_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            switch (args.SelectedItem)
            {
                case HomeMenuItem menuItem:
                    var frame = App.AppViewModel.HomeContentFrame;
                    MainInitTabConfig target;
                    if (menuItem.Children != null && menuItem.Children.Count > 0 && CurrentMenuItem == null)
                    {
                        target = menuItem.DefaultConfig ?? menuItem.Children[0].Config;
                        var targetItem = menuItem.Children.First(child => child.Config == target);
                        if (targetItem == null) return;
                        _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            sender.SelectedItem = targetItem;
                        });
                    }
                    else
                    { // 最终目标tab
                        target = menuItem.Config;
                        if (CurrentMenuItem != menuItem)
                            frame.Navigate(typeof(DataListWrapper), menuItem);
                        CurrentMenuItem = menuItem;
                    }
                    break;
            }
        }


        private void SearchInput_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            App.AppViewModel.HomeContentFrame.Navigate(typeof(SearchResult), args.QueryText);
        }

        private void NavigationViewControl_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                // 设置按钮被点击
                App.AppViewModel.HomeContentFrame.Navigate(typeof(Pages.SettingPage));
            } else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();

                //var itemName = args.InvokedItemContainer.Name;

                switch (navItemTag)
                {
                    case "发布动态":
                        App.AppViewModel.HomeContentFrame.Navigate(typeof(Pages.CreateFeed));
                        break;
                    case "账户":
                        bool isLoged = App.AppViewModel.IsLogged;

                        if (isLoged)
                        {
                            App.AppViewModel.HomeContentFrame.Navigate(typeof(Pages.LaunchPad));

                            //var dialog = new ContentDialog();
                            //dialog.Title = "已登录";
                            //await dialog.ShowAsync();
                        } else
                        {
                            //App.AppViewModel.LoadLoginState();
                            App.AppViewModel.HomeContentFrame.Navigate(typeof(Pages.Login));
                        }
                        break;
                }
            }
        }

    }
}

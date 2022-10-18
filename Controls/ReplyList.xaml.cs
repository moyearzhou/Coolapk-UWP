﻿using Coolapk_UWP.Models;
using Coolapk_UWP.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Coolapk_UWP.Controls {
  
    public sealed partial class ReplyList : UserControl, INotifyPropertyChanged {
        
        // 对于GetDataList，需要Title参数
        public static readonly DependencyProperty FeedIdProperty = DependencyProperty.Register(
            "FeedId",
            typeof(uint),
            typeof(FeedsDataList),
            null
        );

        public uint FeedId {
            get { return (uint)GetValue(FeedIdProperty); }
            set { SetValue(FeedIdProperty, value); }
        }

        public IncrementalLoadingEntityCollection<FeedReply> Entities;

        public ReplyList() {
            this.InitializeComponent();

            UpdateRelay();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) {
            if (Equals(storage, value)) {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
        /// <summary>
        /// 刷新评论列表。该操作会异步进行，当执行完成后自动刷新评论列表。
        /// </summary>
        public void UpdateRelay()
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                Set(ref Entities, new IncrementalLoadingEntityCollection<FeedReply>(async config => {
                    var resp = await App.AppViewModel.CoolapkApis.GetFeedReplyList(
                        FeedId,
                        config.Page,
                        firstItem: config.FirstItem,
                        lastItem: config.LastItem
                    );
                    return resp.Data;
                })
                {
                    AutoCast = false // 关闭该功能
                }, "Entities");
            });
        }
    }
}

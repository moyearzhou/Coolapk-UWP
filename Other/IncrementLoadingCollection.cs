﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Coolapk_UWP.Models;
using Newtonsoft.Json;
using Windows.Foundation.Metadata;
using System.ComponentModel;

namespace Coolapk_UWP.Other {

    public class IncrementalLoadingEntityCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading where T : Entity {
        public uint Page = 1;
        public uint Size = 5;
        public bool _hasMoreItems = true;
        public bool HasMoreItems { get { return _hasMoreItems; } set { Set(ref _hasMoreItems, value); } }
        private Exception _error;
        public Exception Error { get { return _error; } set { Set(ref _error, value); } }
        private bool _loading = false;
        public bool Loading { get { return _loading; } set { Set(ref _loading, value); } }
        public Func<uint, Task<ICollection<T>>> LoadMoreItemsAsyncFunc;

        public IncrementalLoadingEntityCollection(Func<uint, Task<ICollection<T>>> _loadMoreItemsAsyncFunc) {
            LoadMoreItemsAsyncFunc = _loadMoreItemsAsyncFunc;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) {
            return AsyncInfo.Run(async cancelToken => {
                try {
                    Loading = true;
                    var loadedItem = await LoadMoreItemsAsyncFunc(count);
                    var itemList = loadedItem.ToList();
                    if (itemList.Count < Size) HasMoreItems = false;
                    for (var i = 0; i < itemList.Count; i++)
                        Add(itemList[i]);
                    Page += 1;
                    return new LoadMoreItemsResult { Count = (uint)loadedItem.Count };
                } catch (Exception err) {
                    Error = err;
                    return new LoadMoreItemsResult { Count = 0 };
                } finally {
                    Loading = false;
                }
            });
        }

        bool ISupportIncrementalLoading.HasMoreItems => HasMoreItems && Error == null;

        public async void Reload() {
            Error = null;
            Clear();
            Page = 1;
            await LoadMoreItemsAsync(Size);
        }

        // 重写InsertItem使它在插入得时候根据EntityType和EntityTemplate分配正确的Model
        protected override void InsertItem(int index, T entity) {
            switch (entity.EntityType) {
                case "card":
                    switch (entity.EntityTemplate) {
                        case "configCard":
                            entity = entity.Cast<ConfigCard>() as T;
                            break;
                        case "imageCarouselCard_1":
                            entity = entity.Cast<ImageCarouselCard>() as T;
                            break;
                    }
                    break;
            }
            base.InsertItem(index, entity);
        }

        protected void Set<VT>(ref VT storage, VT value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) {
            if (Equals(storage, value)) { return; }
            storage = value;
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }

    [Deprecated("这里是Demo实现，请用IncrementalLoadingEntityCollection", DeprecationType.Deprecate, 1)]
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading {
        public uint page = 1;
        public bool HasMoreItems { get { return page < 1000; } } //maximum
        Func<uint, Task<Collection<T>>> LoadMoreItemsAsyncFunc;

        public IncrementalLoadingCollection(Func<uint, Task<Collection<T>>> _loadMoreItemsAsyncFunc) {
            LoadMoreItemsAsyncFunc = _loadMoreItemsAsyncFunc;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) {
            return AsyncInfo.Run(async cancelToken => {
                try {
                    var loadedItem = await LoadMoreItemsAsyncFunc(count);
                    for (var i = 0; i < loadedItem.Count; i++)
                        Add(loadedItem[i]);
                    page += 1;
                    return new LoadMoreItemsResult { Count = (uint)loadedItem.Count };
                } catch (Exception err) {
                    return new LoadMoreItemsResult { Count = 0 };
                }
            });
        }
    }
}

using Coolapk_UWP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Coolapk_UWP.ViewModels {

    public class FeedDetailViewModel : AsyncLoadViewModel<FeedDetail> {
        public uint FeedId = 18484842;

        public uint AutherId = 0;

        public UserAction UserAction { get; set; } = new UserAction { Like = false };

        //public FeedDetailViewModel() {
        //    this.Reload();
        //}

        public override async Task<RespBase<FeedDetail>> OnLoadAsync() {
            var resp = await CoolapkApis.GetFeedDetail(FeedId);
            AutherId = resp.Data.UserInfo.Uid;
            
            return resp;
        }

        public override string[] NotifyChangedProperties() {
            return base.NotifyChangedProperties();
        }

        /// <summary>
        /// 切换“关注”与“取消关注”
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionResp<Entity>> ToggleFollow()
        {
            if (AutherId == 0) throw new Exception("获取用户uid失败");

            CollectionResp<Entity> resp;
            var old = UserAction?.Follow ?? false;
            try
            {
                if (UserAction.Follow)
                {
                    UserAction.Follow = false;
                    resp = await App.AppViewModel.CoolapkApis.Unfollow(AutherId);
                }
                else
                {
                    UserAction.Follow = true;
                    resp = await App.AppViewModel.CoolapkApis.Follow(AutherId);
                    
                }
                if (resp.Message != null) throw new Exception(resp.Message);
                return resp;
            }
            catch (Exception err)
            {
                UserAction.Follow = old;
                throw err;
            }
        }

        public void UpdateFeedInfo(uint FeedId)
        {
            this.FeedId = FeedId;
            //AutherId = Data.UserInfo.Uid;
        }

      
    }
}

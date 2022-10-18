using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace Coolapk_UWP.Utils
{
    public class ApUtils
    {
        /// <summary>
        /// 设置自定义标题栏
        /// </summary>
        /// <param name="coreTitleBar"></param>
        /// <param name="window"></param>
        /// <param name="appTitleBar"></param>
        public static void SetCustomizeAppTitleBar(CoreApplicationViewTitleBar coreTitleBar, Window window, UIElement appTitleBar)
        {
            //设置自定义标题栏
            window.SetTitleBar(appTitleBar);
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }
    }
}

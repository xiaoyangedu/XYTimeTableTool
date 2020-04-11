using GalaSoft.MvvmLight.Threading;
using OSKernel.Presentation.Core;
using OSKernel.Presentation.Core.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Arranging
{
    /// <summary>
    /// 签名逻辑验证
    /// </summary>
    public class SignLogic
    {
        public static bool SignCheck()
        {
            var secret = OSHttpClient.Instance.SetSecret(CacheManager.Instance.LoginUser.PublickKey, 0, CacheManager.Instance.LoginUser.AccessToken);
            if (!secret.Item1)
            {
                CustomControl.DialogWindowHelper.ShowDialog("提示信息", secret.Item2, CustomControl.Enums.DialogSettingType.OnlyOkButton, CustomControl.Enums.DialogType.Warning);

                System.Diagnostics.Process.Start(System.IO.Path.Combine(Environment.CurrentDirectory, "Client.exe"));

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Environment.Exit(0);
                });
            }
            return secret.Item1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Models.Enums
{
    /// <summary>
    /// 版本类型枚举
    /// </summary>
    public enum VersionTypeEnum
    {
        /// <summary>
        /// 此版本表示该软件在此阶段主要是以实现软件功能为主，通常只在软件开发者内部交流，一般而言，该版本软件的Bug较多，需要继续修改
        /// </summary>
        Alpha = 1,
        /// <summary>
        /// 该版本相对于α版已有了很大的改进，消除了严重的错误，但还是存在着一些缺陷，需要经过多次测试来进一步消除，此版本主要的修改对像是软件的UI
        /// </summary>
        Beta = 2,
        /// <summary>
        /// 该版本已经相当成熟了，基本上不存在导致错误的BUG，与即将发行的正式版相差无几
        /// </summary>
        RC = 3,
        /// <summary>
        /// 该版本意味“最终版本”，在前面版本的一系列测试版之后，终归会有一个正式版本，是最终交付用户使用的一个版本。
        /// 该版本有时也称为标准版。一般情况下，Release 不会以单词形式出现在软件封面上，取而代之的是符号(R)。
        /// </summary>
        Release = 4

    }
}

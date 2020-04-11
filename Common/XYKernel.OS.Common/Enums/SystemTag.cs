using System;
using System.ComponentModel;

namespace XYKernel.OS.Common.Enums
{
    public sealed class SystemTag
    {
        private readonly string name;
        private readonly int value;

        /// <summary>
        /// 教案齐头
        /// </summary>
        public static readonly SystemTag XYTag1 = new SystemTag("XYTag1", 1);
        /// <summary>
        /// 课时分散
        /// </summary>
        public static readonly SystemTag XYTagN = new SystemTag("XYTagN", 2);

        private SystemTag(string name, int value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return name;
        }
    }
}

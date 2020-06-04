using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OSKernel.Presentation.Utilities.XY.Common
{
    /// <summary>
    /// RSA工具类
    /// </summary>
    public static class RSAUtil
    {
        static readonly byte[] SeqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        static private readonly byte[] Version = new byte[] { 0x02, 0x01, 0x00 };
        /// <summary>
        /// 生成密钥对，[0]=公钥;[1]=私钥
        /// </summary>
        public static string[] GenerateSuiteKeys(bool shortKey = true, bool withPKCS8 = false)
        {
            return GenerateSuiteKeys(shortKey ? 1024 : 2048, withPKCS8);
        }

        /// <summary>
        /// 生成密钥对，[0]=公钥;[1]=私钥
        /// </summary>
        public static string[] GenerateSuiteKeys(int keySize, bool withPKCS8 = false)
        {
            string[] keypairs = new string[2];
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = keySize;

                using (var ms = new MemoryStream())
                {

                    /****生成公钥****/
                    var param = rsa.ExportParameters(false);


                    //写入总字节数，不含本段长度，额外需要24字节的头，后续计算好填入
                    ms.WriteByte(0x30);
                    var index1 = (int)ms.Length;

                    //固定内容
                    // encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                    //ms.writeAll(SeqOID);
                    ms.Write(SeqOID, 0, SeqOID.Length);

                    //从0x00开始的后续长度
                    ms.WriteByte(0x03);
                    var index2 = (int)ms.Length;
                    ms.WriteByte(0x00);

                    //后续内容长度
                    ms.WriteByte(0x30);
                    var index3 = (int)ms.Length;

                    //写入Modulus
                    ms.WriteBlock(param.Modulus);

                    //写入Exponent
                    ms.WriteBlock(param.Exponent);


                    //计算空缺的长度
                    var byts = ms.ToArray();

                    byts = ms.WriteLen(index3, byts);
                    byts = ms.WriteLen(index2, byts);
                    byts = ms.WriteLen(index1, byts);

                    keypairs[0] = Convert.ToBase64String(byts);
                }

                using (var ms = new MemoryStream())
                {
                    /****生成私钥****/
                    var param = rsa.ExportParameters(true);

                    //写入总字节数，后续写入
                    ms.WriteByte(0x30);
                    var index1 = (int)ms.Length;

                    //写入版本号
                    //ms.writeAll(_Ver);
                    ms.Write(Version, 0, Version.Length);

                    //PKCS8 多一段数据
                    var index2 = -1;
                    var index3 = -1;
                    if (withPKCS8)
                    {
                        //固定内容
                        ms.Write(SeqOID, 0, SeqOID.Length);

                        //后续内容长度
                        ms.WriteByte(0x04);
                        index2 = (int)ms.Length;

                        //后续内容长度
                        ms.WriteByte(0x30);
                        index3 = (int)ms.Length;

                        //写入版本号
                        ms.Write(Version, 0, Version.Length);
                    }

                    //写入数据
                    ms.WriteBlock(param.Modulus);
                    ms.WriteBlock(param.Exponent);
                    ms.WriteBlock(param.D);
                    ms.WriteBlock(param.P);
                    ms.WriteBlock(param.Q);
                    ms.WriteBlock(param.DP);
                    ms.WriteBlock(param.DQ);
                    ms.WriteBlock(param.InverseQ);


                    //计算空缺的长度
                    var byts = ms.ToArray();

                    if (index2 != -1)
                    {
                        byts = ms.WriteLen(index3, byts);
                        byts = ms.WriteLen(index2, byts);
                    }
                    byts = ms.WriteLen(index1, byts);


                    keypairs[1] = Convert.ToBase64String(byts);
                }
            }

            return keypairs;
        }

        /// <summary>
        /// 根据私钥创建RSA
        /// </summary>
        public static RSA FromPrivateKey(string privateKey)
        {
            NotNullOrWhiteSpace(privateKey, "privateKey");

            var privateKeyBits = Convert.FromBase64String(privateKey);

            return FromPrivateKey(privateKeyBits);
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的字符串不能是 null 或 空白字符串。
        /// </summary>
        public static void NotNullOrWhiteSpace(string @string, string variableName)
        {
            if (string.IsNullOrWhiteSpace(@string))
            {
                throw new ArgumentException("the param is null or whitespace.", variableName);
            }
        }

        /// <summary>
        /// 根据私钥创建RSA
        /// </summary>
        public static RSA FromPrivateKey(byte[] privateKeyBits)
        {
            //var privateKeyBits = Convert.FromBase64String(privateKey);
            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                {
                    binr.ReadByte();
                }
                else if (twobytes == 0x8230)
                {
                    binr.ReadInt16();
                }
                else
                {
                    throw new CryptographicException("Unexpected value read binr.ReadUInt16()");
                }

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                {
                    throw new CryptographicException("Unexpected version");
                }

                bt = binr.ReadByte();
                if (bt != 0x00)
                {
                    throw new CryptographicException("Unexpected value read binr.ReadByte()");
                }

                rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(rsaParameters);
            return rsa;
        }

        static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = binr.ReadByte();
            if (bt != 0x02)
            {
                return 0;
            }

            bt = binr.ReadByte();

            int count;
            if (bt == 0x81)
            {
                count = binr.ReadByte();
            }
            else
            if (bt == 0x82)
            {
                var highbyte = binr.ReadByte();
                var lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        //根据后续内容长度写入长度数据
        static byte[] WriteLen(this MemoryStream ms, int index, byte[] bytes)
        {
            var len = bytes.Length - index;

            ms.SetLength(0);
            ms.Write(bytes, 0, index);
            ms.WriteLenByte(len);
            ms.Write(bytes, index, len);

            return ms.ToArray();
        }

        //写入一块数据
        static void WriteBlock(this MemoryStream ms, byte[] bytes)
        {
            var addZero = (bytes[0] >> 4) >= 0x8;
            ms.WriteByte(0x02);
            var len = bytes.Length + (addZero ? 1 : 0);
            ms.WriteLenByte(len);

            if (addZero)
            {
                ms.WriteByte(0x00);
            }
            ms.Write(bytes, 0, bytes.Length);
        }

        //写入一个长度字节码
        static void WriteLenByte(this MemoryStream ms, int len)
        {
            if (len < 0x80)
            {
                ms.WriteByte((byte)len);
            }
            else if (len <= 0xff)
            {
                ms.WriteByte(0x81);
                ms.WriteByte((byte)len);
            }
            else
            {
                ms.WriteByte(0x82);
                ms.WriteByte((byte)(len >> 8 & 0xff));
                ms.WriteByte((byte)(len & 0xff));
            }
        }
    }
}

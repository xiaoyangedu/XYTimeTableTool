using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OSKernel.Presentation.Utilities
{
    /// <summary>
    /// 文件序列化类
    /// </summary>
    public static class FileSerializer
    {
        public static void SaveToXml<T>(this string filePath, T sourceObj, string xmlRootName = "")
        {
            if (!string.IsNullOrWhiteSpace(filePath) && sourceObj != null)
            {
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    XmlSerializer xmlSerializer = string.IsNullOrWhiteSpace(xmlRootName) ?
                        new XmlSerializer(typeof(T)) :
                        new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootName));
                    xmlSerializer.Serialize(writer, sourceObj);
                }
            }
        }

        /// <summary>
        /// 从XML文件加载对象
        /// </summary>
        /// <param name="path">XML文件路径</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static T LoadFromXml<T>(this string path) where T : class
        {
            try
            {
                T result = null;
                if (System.IO.File.Exists(path))
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
                    {
                        System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                        result = xmlSerializer.Deserialize(reader) as T;
                    }
                }
                return result;
            }
            catch
            {
                // TODO 异常处理
                return null;
            }
        }


        /// <summary>
        /// 将文件反序列化
        /// </summary>
        /// <param name="FilePath">文件路径(必须是经过当前序列化后的文件)</param>
        /// <returns>返回 null 表示序列反解失败或者目标文件不存在</returns>
        public static object FileDeSerialize(this string FilePath)
        {
            FileStream fs = null;
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryFormatter sl = new BinaryFormatter();
                    object obg = sl.Deserialize(fs);
                    return obg;
                }
                catch
                {
                    // TODO 处理异常
                    return null;
                }
                finally
                {
                    if (fs != null)
                    {
                        try
                        {
                            fs.Close();
                        }
                        catch
                        {
                            // TODO 处理异常
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 将对象序列化到指定文件
        /// </summary>
        /// <param name="FilePath">文件(支持绝大多数数据类型)</param>
        /// <param name="obj">要序列化的对象(如哈希表,数组等等)</param>
        public static string FileSerialize(this string FilePath, object obj)
        {
            string message = string.Empty;

            if (File.Exists(FilePath)) { File.Delete(FilePath); }

            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(FilePath, FileMode.Create);
                BinaryFormatter sl = new BinaryFormatter();
                sl.Serialize(fs, obj);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                if (fs != null)
                {
                    try
                    {
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                }
            }
            return message;
        }

        /// <summary>
        /// object to json
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        public static void SerializeObjectToJson(this string path, object obj)
        {
            try
            {
                var jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings
                {
                    StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default
                });

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                File.WriteAllText(path, jsonMessage);

                //path.FileSerialize(jsonMessage);
            }
            catch (Exception ex)
            {
                // TODO
            }
        }

        /// <summary>
        /// json to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DeSerializeObjectFromJson<T>(this string path) where T : class
        {            
            try
            {
                var jsonString = File.ReadAllText(path);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
                return result;
            }
            catch (Exception ex)
            {
                // TODO
                return null;
            }
        }

        /// <summary>
        /// 将xml反序列化为类
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T XmlDeserailize<T>(this string xml) where T : class
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            xml = "<" + xml.Substring(xml.IndexOf('<') + 1);

            StringReader reader = new StringReader(xml.Trim());

            XmlTextReader xmlReader = new XmlTextReader(reader);

            T obj = serializer.Deserialize(xmlReader) as T;

            xmlReader.Close();
            reader.Close();
            return obj;
        }

        /// <summary>
        /// 将类序列化为xml文本
        /// </summary>
        /// <param name="type">要被序列化的对象的type</param>
        /// <param name="isDflg">序列化时候是否进行缩进格式化</param>
        /// <returns></returns>
        public static string Xmlserailize(this object obj, bool isIndent = false)
        {
            if (obj == null) return "";

            string xml = string.Empty;

            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();

            xmlns.Add(string.Empty, string.Empty);

            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            MemoryStream ms = new MemoryStream();

            Encoding utf8 = new UTF8Encoding(false);

            XmlTextWriter writer = new XmlTextWriter(ms, utf8);
            writer.Formatting = isIndent ? Formatting.Indented : Formatting.None;

            serializer.Serialize(writer, obj, xmlns);

            xml = utf8.GetString(ms.ToArray());

            writer.Flush();
            writer.Close();
            ms.Close();

            return xml;
        }
    }
}

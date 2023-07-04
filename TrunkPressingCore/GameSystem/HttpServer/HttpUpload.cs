using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkPressingCore.GameModel;

namespace TrunkPressingCore 
{
    public class HttpUpload
    {
        public static string PostFromData(string url, List<FromDataItemModel> itemModels, CookieContainer cookieContainer = null, string refererUrl = null, Encoding encoding = null, int timeOut = 20000)
        {
            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                #region 初始化请求对象
                request.Method = "POST";
                request.Timeout = timeOut;
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
                request.UseDefaultCredentials = true;
                if (!string.IsNullOrEmpty(refererUrl))
                {
                    request.Referer = refererUrl;
                }
                if (cookieContainer != null)
                {
                    request.CookieContainer = cookieContainer;
                }
                #endregion 
                string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                //请求流
                var postStream = new MemoryStream();
                #region 处理Form表单请求内容
                var uploadFile = itemModels != null && itemModels.Count > 0;
                if (uploadFile)
                {
                    string jpgFormdataTemplate =
                        "\r\n--" + boundary +
                        "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"" +
                        "\r\nContent-Type: image/jpeg" +
                        "\r\n\r\n";
                    //png图片数据模板
                    string pngFormdataTemplate =
                        "\r\n--" + boundary +
                        "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"" +
                        "\r\nContent-Type: image/png" +
                        "\r\n\r\n";

                    //MP4视频数据模板
                    string videoFormdataTemplate =
                        "\r\n--" + boundary +
                        "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"" +
                        "\r\nContent-Type: video/mpeg4" +
                        "\r\n\r\n";

                    //txt文本数据模板
                    string textFormdataTemplate = "\r\n--" + boundary +
                        "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"" +
                        "\r\nContent-Type: text/plain" +
                        "\r\n\r\n";

                    //data数据模板
                    string dataFormdataTemplate =
                        "\r\n--" + boundary +
                        "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                        "\r\n\r\n{1}";
                    foreach (var item in itemModels)
                    {
                        string datas = null;
                        if (item.IsFile)
                        {
                            if (item.key == "images")
                            {
                                string[] st = item.FileName.Split('.');
                                if (st[1] == "jpg")
                                {
                                    datas = string.Format(jpgFormdataTemplate, item.key, item.FileName);
                                }
                                else if (st[1] == "png")
                                {
                                    datas = string.Format(pngFormdataTemplate, item.key, item.FileName);
                                }
                            }
                            else if (item.key == "videos")
                            {
                                datas = string.Format(videoFormdataTemplate, item.key, item.FileName);
                            }
                            else if (item.key == "text")
                            {
                                datas = String.Format(textFormdataTemplate, item.key, item.FileName);
                            }
                        }
                        else
                        {
                            datas = String.Format(dataFormdataTemplate, item.key, item.value);
                        }
                        // 统一处理
                        byte[] bytes = null;
                        if (postStream.Length == 0)
                            bytes = Encoding.UTF8.GetBytes(datas.Substring(2, datas.Length - 2));
                        else
                        {
                            bytes = Encoding.UTF8.GetBytes(datas);
                        }
                        postStream.Write(bytes, 0, bytes.Length);
                        //写入文件内容
                        if (item.FileContent != null && item.FileContent.Length > 0)
                        {
                            using (var streams = item.FileContent)
                            {
                                byte[] sy = new byte[1024];
                                int byteRead = 0;
                                while ((byteRead = streams.Read(sy, 0, sy.Length)) != -1)
                                {
                                    postStream.Write(sy, 0, byteRead);
                                }
                            }
                        }
                    }
                    // 结尾
                    var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                    postStream.Write(footer, 0, footer.Length);
                }

                else
                {
                    request.ContentType = "application/x-www-form-urlencoded";

                }
                #endregion 
                request.ContentLength = postStream.Length;
                #region 输入二进制数据流
                if (postStream != null)
                {
                    postStream.Position = 0;
                    //直接写入流
                    Stream reqstream = request.GetRequestStream();
                    byte[] buff = new byte[1024];
                    int bys = 0;
                    while ((bys = postStream.Read(buff, 0, buff.Length)) != 0)
                    {
                        reqstream.Write(buff, 0, bys);
                    }

                    postStream.Close();

                }
                #endregion
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                if (cookieContainer != null)
                {
                    httpWebResponse.Cookies = cookieContainer.GetCookies(httpWebResponse.ResponseUri);

                }
                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        retString = streamReader.ReadToEnd();
                    }
                }
            }

            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            return retString;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ldic"></param>
        /// <returns></returns>
        public static bool downList2Excel(string path, List<Dictionary<string, string>> ldic)
        {
            bool result = false;
            /* List<string> title = new List<string>();
             for (int i = 0; i < listView1.Columns.Count; i++)
             {
                 string text = listView1.Columns[i].Text;
                 title.Add(text);
             }
             for (int i = 0; i < listView1.Items.Count; i++)
             {
                 Dictionary<string, string> dic = new Dictionary<string, string>();
                 for (int j = 0; j < title.Count; j++)
                 {
                     dic.Add(title[j], listView1.Items[i].SubItems[j].Text);
                 }
                 ldic.Add(dic);
             }

             result = ExcelUtils.OutPutExcel(ldic, path);*/
            return result;
        }
    }
}

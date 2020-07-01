using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace com.lw.qrobot.Code.App
{
    public class Translater : App
    {
        private string transUrl = "http://fanyi.youdao.com/translate?&doctype=xml&type=AUTO&i={0}";
        private bool finished = true;
        private List<string> outputMsg = new List<string>();

        public override bool Finished 
        { 
            get
            {
                return finished;
            }
        }

        public override List<string> OutputMsg
        {
            get
            {
                return outputMsg;
            }
        }

        public override void Update(string msg)
        {
            bool msgLegal = false;
            string word = msg.Substring(msg.IndexOf("词") + 1, msg.Length - 3);

            if (word.Length > 0)
            {
                msgLegal = true;
            }

            if (msgLegal)
            {
                WebClient webClient = new WebClient();                      //获取网页
                webClient.Encoding = Encoding.UTF8;
                string response = webClient.DownloadString(String.Format(transUrl, word));

                XmlDocument doc = new XmlDocument();                        //转为xml，用xpath获取翻译结果
                doc.LoadXml(response);

                XmlNodeList nodes = doc.SelectNodes("/response/translation");
                foreach (XmlNode item in nodes)
                {
                    outputMsg.Add(item.InnerText);
                }

                /*string wordRef = "\"tgt\":\"[^\"]*\"";                    //用正则获取结果
                Regex regex = new Regex(wordRef);
                var matches = regex.Matches(response);

                if (matches.Count == 0)
                {
                    outputMsg.Add("没有查到相关的意思哦，是不是打错啦?");
                }
                else
                {
                    string meaning = "";
                    foreach (Match match in matches)                        //处理多次单词组
                    {
                        string tgtStr = match.Value;
                        meaning += tgtStr.Substring(tgtStr.IndexOf(":") + 2, tgtStr.Length - 8) + " ";
                    }

                    outputMsg.Add(meaning);
                }*/
            }
            else
            {
                outputMsg.Add("您是不是打漏了要查的单词呀！");
            }
        }

        public override void ClearOutputMsg()
        {
            outputMsg.Clear();
        }
    }
}

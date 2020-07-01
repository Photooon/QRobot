using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using com.lw.qrobot.Code.App;

namespace com.lw.qrobot.Code
{
    public enum AppState
    {
        Sleep,      //未启动应用
        Punch,      //启动了猜拳应用
        Translater, //翻译应用
    }

    public class User
    {
        public AppState appState;
        public QQ qq;
        public Group group;
        public long id;
        public CQCode atName;
        public string inputMsg;         //用户输入的信息
        public List<string> outputMsg;      //等待输出的信息

        public Punch punch;
        public Translater translater;

        public User(QQ q, Group g = null)
        {
            appState = AppState.Sleep;
            qq = q;
            group = g;
            id = q.Id;
            atName = q.CQCode_At();
            outputMsg = new List<string>();

            outputMsg.Add("(｡･∀･)ﾉﾞ嗨");

            punch = new Punch();
            translater = new Translater();
        }

        public void Update(CQPrivateMessageEventArgs e)
        {
            inputMsg = e.Message;
            e.Handler = true;       // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q

            CallApps();
        }

        public void Update(CQGroupMessageEventArgs e)
        {
            inputMsg = e.Message;
            e.Handler = true;       // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q

            CallApps();
        }

        public void CallApps()
        {
            if (appState == AppState.Sleep)
            {
                if (inputMsg == "猜拳")
                {
                    appState = AppState.Punch;
                    punch.Update("#Start");
                    outputMsg.AddRange(punch.OutputMsg);
                    punch.ClearOutputMsg();

                    if (punch.Finished)
                    {
                        appState = AppState.Sleep;
                    }
                }
                else if (inputMsg.StartsWith("查单词"))
                {
                    appState = AppState.Translater;
                    translater.Update(inputMsg);
                    outputMsg.AddRange(translater.OutputMsg);
                    translater.ClearOutputMsg();

                    if (translater.Finished)
                    {
                        appState = AppState.Sleep;
                    }
                }
                //...其他应用
            }
            else
            {
                switch (appState)       //将输入信息传给应用
                {
                    case AppState.Punch:
                        punch.Update(inputMsg);
                        outputMsg.AddRange(punch.OutputMsg);
                        punch.ClearOutputMsg();
                        if (punch.Finished)
                        {
                            appState = AppState.Sleep;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void OutputPrivateMsg()
        {
            foreach (var msg in outputMsg)
            {
                qq.SendPrivateMessage(msg);         // 发送一条群消息, 下列对象会合并成一个字符串发送
            }

            outputMsg.Clear();
        }

        public void OutputGroupMsg()
        {
            foreach (var msg in outputMsg)
            {
                if (group != null)
                {
                    group.SendGroupMessage(msg);    // 发送一条群消息, 下列对象会合并成一个字符串发送
                }    
            }

            outputMsg.Clear();
        }
    }
}


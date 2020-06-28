using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;

namespace com.lw.qrobot.Code
{
    public enum App
    {
        Sleep,      //未启动应用
        Punch,      //启动了猜拳应用
    }

    public class User
    {
        public App appState;
        public QQ qq;
        public long id;
        public CQCode atName;
        public string inputMsg;         //用户输入的信息
        public List<string> outputMsg;      //等待输出的信息

        public Punch punch;

        public User(QQ q)
        {
            appState = App.Sleep;
            qq = q;
            id = q.Id;
            atName = q.CQCode_At();
            outputMsg = new List<string>();

            outputMsg.Add("(｡･∀･)ﾉﾞ嗨");

            punch = new Punch();
        }

        public void Update(CQPrivateMessageEventArgs e)
        {
            inputMsg = e.Message;
            e.Handler = true;       // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q

            if (appState == App.Sleep)
            {
                if (inputMsg == "猜拳")
                {
                    appState = App.Punch;
                    punch.Start();
                    outputMsg.AddRange(punch.outputMsg);
                    punch.outputMsg.Clear();
                }
                //...其他应用
            }
            else
            {
                switch (appState)       //将输入信息传给应用
                {
                    case App.Punch:
                        punch.Update(inputMsg);
                        outputMsg.AddRange(punch.outputMsg);
                        punch.outputMsg.Clear();
                        if (punch.finished)
                        {
                            appState = App.Sleep;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void Output()
        {
            foreach (var msg in outputMsg)
            {
                SendMsg(msg);
            }

            outputMsg.Clear();
        }

        public void SendMsg(params object[] message)
        {
            qq.SendPrivateMessage(message);     // 往来源群发送一条群消息, 下列对象会合并成一个字符串发送
        }
    }
}


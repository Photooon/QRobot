using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using com.lw.qrobot.Code;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;

namespace com.lw.qrobot.Code
{
    public class Event_PrivateMessage: IPrivateMessage
    {

        public Dictionary<long, User> users = new Dictionary<long, User>();

        /// <summary>
        /// 收到私聊消息
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            User user;
            if (!users.ContainsKey(e.FromQQ.Id))
            {
                user = new User(e.FromQQ);
                users.Add(e.FromQQ.Id, user);
                user.SendMsg("欢迎使用海螺！");
            }
            else
            {
                user = users[e.FromQQ.Id];
                user.Update(e);
                user.Output();
            }
        }
    }

    public enum State
    {
        Sleep,              //睡眠
        Error,              //发生错误
        WaitForPunchInput,        //等待用户出拳
        WaitForPunchOutput,       //等待电脑出拳
        WaitForPunchScoreOutput,  //等待查看猜拳分数
    }

    public class Punch
    {
        public int score = 0;
        public string[] punchStr = { "石头", "剪刀", "布" };
        public int input = 0;
        public int output = 0;
        public bool isStone = false;
        public bool isScissor = false;
        public bool isCloth = false;

        public bool isLegalInput(string m)
        {
            isStone = m.Contains("石头");
            isScissor = m.Contains("剪刀") || m.Contains("✂");
            isCloth = m.Contains("布");

            if (Convert.ToInt32(isStone) + Convert.ToInt32(isScissor) + Convert.ToInt32(isCloth) == 1)
            {
                input = 1 * Convert.ToInt32(isScissor) + 2 * Convert.ToInt32(isCloth);

                return true;
            }

            return false;
        }

        public string GetPunch()
        {
            Random rd = new Random();
            output = rd.Next(3);

            if (isUserWin())
                score += 1;

            return punchStr[output];
        }

        public bool isUserWin()
        {
            if (input == 2 && output == 0)
            {
                return true;
            }
            else if (input < output)
            {
                return true;
            }

            return false;
        }
    }

    public class User
    {
        public State state;
        public QQ qq;
        public long id;
        public CQCode atName;
        public string msg;
        public string errMsg;

        public Punch punch;

        public User(QQ q)
        {
            state = State.Sleep;
            qq = q;
            id = q.Id;
            atName = q.CQCode_At();

            punch = new Punch();
        }

        public void Update(CQPrivateMessageEventArgs e)
        {
            msg = e.Message;
            e.Handler = true;   // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q

            if (!Recognition())      //优先根据指令变更状态，然后处理正常流程
            {
                switch (state)      //当msg不是新指令时，将msg作为前一个state的输入
                {
                    case State.WaitForPunchInput:
                        if (punch.isLegalInput(msg))
                        {
                            state = State.WaitForPunchOutput;
                        }
                        else
                        {
                            state = State.Error;
                            errMsg = "不能耍赖！😣";
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void Output()
        {
            switch (state)
            {
                case State.Error:
                    SendMsg(errMsg);
                    state = State.Sleep;
                    break;
                case State.WaitForPunchInput:
                    SendMsg("准备好了，出拳吧😁");
                    break;
                case State.WaitForPunchOutput:
                    SendMsg(punch.GetPunch());
                    state = State.WaitForPunchInput;
                    break;
                case State.WaitForPunchScoreOutput:
                    SendMsg("当前得分：" + punch.score.ToString());
                    state = State.Sleep;
                    break;
                default:
                    break;
            }
        }

        public bool Recognition()      //识别指令
        {
            if (msg == "猜拳来")
            {
                punch.score = 0;
                state = State.WaitForPunchInput;
                return true;
            }
            else if(msg == "不猜拳了")
            {
                state = State.WaitForPunchScoreOutput;
                return true;
            }

            return false;
        }

        public void SendMsg(params object[] message)
        {
            qq.SendPrivateMessage(message);     // 往来源群发送一条群消息, 下列对象会合并成一个字符串发送
        }
    }
}

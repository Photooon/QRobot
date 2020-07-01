using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.lw.qrobot.Code.App
{
    public enum PunchState
    {
        Sleep,
        WaitForInput,
        OutputPunch,
    }

    public class Punch: App
    {
        private int score = 0;
        private string[] punchStr = { "👊", "✌", "🖐" };
        private PunchState state = PunchState.Sleep;
        private int input = 0;
        private int output = 0;
        private bool finished = false;
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
            switch (state)      //这里的state是上一个状态，结合msg信息更新状态并给出回复
            {
                case PunchState.Sleep:
                    if (msg == "#Start")
                    {
                        score = 0;
                        outputMsg.Add("我准备好了，出拳吧！😊");
                        finished = false;
                        state = PunchState.WaitForInput;
                    }
                    break;
                case PunchState.WaitForInput:       //上一个状态时等待输入，那么这次的msg就是输入
                    if (msg.Contains("不猜拳"))
                    {
                        outputMsg.Add(String.Format("上一轮的得分是：{0}分", score));
                        score = 0;
                        finished = true;
                        state = PunchState.Sleep;
                        break;
                    }
                    if (isLegalInput(msg))
                    {
                        outputMsg.Add(GetPunch());

                        if (isUserWin())
                        {
                            score += 1;
                        }

                        state = PunchState.OutputPunch;     //这里显式地表达了状态机的转换，因为output状态不能保持，所以自动跳到下一状态
                        state = PunchState.WaitForInput;
                    }
                    else
                    {
                        outputMsg.Add("不能耍赖噢，亲");
                        outputMsg.Add("出拳吧！😊");
                        state = PunchState.WaitForInput;
                    }
                    break;
                default:
                    break;
            }
        }

        public override void ClearOutputMsg()
        {
            outputMsg.Clear();
        }

        public bool isLegalInput(string m)
        {
            bool isStone = m.Contains("石头");
            bool isScissor = m.Contains("剪刀") || m.Contains("✂");
            bool isCloth = m.Contains("布");

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
}

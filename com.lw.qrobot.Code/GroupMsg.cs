using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.lw.qrobot.Code
{
    public class Event_GroupMessage : IGroupMessage
    {
        public long[] legalId = { 854057585 };

        /// <summary>
        /// 收到群消息
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            var groupId = e.FromGroup.Id;
            bool reply = false;

            foreach (var id in legalId)
            {
                if (groupId == id)
                {
                    reply = true;
                }
            }

            if (reply)      //测试阶段，只提供部分群的服务
            {
                // 获取 At 某人对象
                CQCode cqat = e.FromQQ.CQCode_At();
                // 往来源群发送一条群消息, 下列对象会合并成一个字符串发送
                e.FromGroup.SendGroupMessage(cqat, " 您发送了一条消息: ", e.Message);
                // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q
                e.Handler = true;
            }
        }
    }
}

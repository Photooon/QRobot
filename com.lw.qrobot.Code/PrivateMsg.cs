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
            }
            else
            {
                user = users[e.FromQQ.Id];
                user.Update(e);
            }

            user.OutputPrivateMsg();
        }
    }
}

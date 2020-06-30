﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.lw.qrobot.Code.App
{
    public abstract class App
    {
        public abstract bool Finished { get; set; }            //功能是否完成，每次Update之后消息循环类通过此值判断是否结束功能

        public abstract void Update(string msg);      //功能在开启状态下，每次新的消息都会被消息循环传入正在使用的功能，既本方法
    }
}

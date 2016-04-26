﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TAS.Remoting.Client;
using TAS.Server.Interfaces;

namespace TAS.Client.Model
{
    public class PlayoutServer : ProxyBase, IPlayoutServer
    {
        public IAnimationDirectory AnimationDirectory { get { return Get<AnimationDirectory>(); } protected set { Set(value); } }

        public List<IPlayoutServerChannel> Channels { get { return Get<List<PlayoutServerChannel>>().Cast<IPlayoutServerChannel>().ToList(); } protected set { Set(value); } }

        public ulong Id { get { return Get<ulong>(); }  set { Set(value); } }

        public bool IsConnected { get { return Get<bool>(); } protected set { Set(value); } }

        public IServerDirectory MediaDirectory { get { return Get<ServerDirectory>(); } protected set { Set(value); } }

        public string MediaFolder { get { return Get<string>(); } set { Set(value); } }

        public string ServerAddress { get { return Get<string>(); } set { Set(value); } }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}

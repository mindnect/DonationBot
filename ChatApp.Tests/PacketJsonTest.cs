#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChatAppLib.Extensions;
using ChatAppLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#endregion

namespace ChatApp.Tests
{
    [TestClass]
    public class PacketJsonTest
    {
        [TestMethod]
        public void Packet_Json_Test()
        {
            var testPacket1 = PacketHelper.CreateChat(new User("이순신",Platform.KAKAO, "등급이다."), "이것은 메세지 입니다.");
            var result = testPacket1.Serialize();
            Console.WriteLine(result);
        }
    }
}
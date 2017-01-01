using System;
using ChatAppLib.Extensions;
using ChatAppLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChatApp.Tests
{
    [TestClass]
    public class PacketJsonTest
    {
        [TestMethod]
        public void Packet_Json_Test()
        {
            var testPacket1 = PacketHelper.CreateChat(new User("이순신", Platform.KAKAO, "등급이다."), "이것은 메세지 입니다.");
            var result = testPacket1.Serialize();
            Console.WriteLine(result);
        }
    }
}
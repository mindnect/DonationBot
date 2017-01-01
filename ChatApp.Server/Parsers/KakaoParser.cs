using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChatAppLib.Models;
using ChatAppLib.Win32;
using HtmlAgilityPack;
using mshtml;
using static ChatApp.Extension.HtmlExtension;

namespace ChatAppLib.Parsers
{
    public class KakaoParser
    {
        private const Platform Platform = Models.Platform.KAKAO;

        //protected HTMLDocument document;
        protected HTMLDocument document;
        protected int lastLength;
        protected Dictionary<string, User> mapUser;
        protected Stack<IHTMLElement> parseStack;
        protected bool readyToUpdate;

        public Dictionary<IntPtr, string> PlayerList { get; protected set; }
        public IntPtr SelectedPlayerHwnd { get; protected set; } = IntPtr.Zero;

        public void SelectPotPlayer()
        {
            if (PlayerList.Count == 1)
            {
                SelectedPlayerHwnd = PlayerList.ElementAt(0).Key;
            }
            else if (PlayerList.Count > 1)
            {
                Console.WriteLine("하나 이상의 팟플레이어 실행중");
                for (var i = 0; i < PlayerList.Count; i++)
                {
                    Console.WriteLine(i + 1 + " : " + PlayerList.ElementAt(i).Value);
                }

                var userInput = Console.ReadKey(true);
                var index = int.Parse(userInput.KeyChar.ToString()) - 1;
                SelectedPlayerHwnd = PlayerList.ElementAt(index).Key;
            }
            else
            {
                // 플레이어 없음
                Console.Write(".");
            }
        }

        public void Update()
        {
            if (readyToUpdate)
            {
                try
                {
                    UpdateRead();
                    UpdateParse();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Error] " + e.Message);
                    readyToUpdate = false;
                }
            }
            else
            {
                Init();
            }
        }


        public void RefreshPlayerList()
        {
            PlayerList = WinAPI.FindAllWindowsWithClassNames("PotPlayer", "PotPlayer64");
        }

        protected virtual void OnUserAdded(User newUser)
        {
            Console.Write("New");
        }


        protected User GetUserData(string nickname, string grade = null)
        {
            if (mapUser.ContainsKey(nickname))
                return mapUser[nickname];

            var user = new User(nickname, Platform, grade);

            mapUser.Add(nickname, user);
            OnUserAdded(user);
            return user;
        }

        private void UpdateRead()
        {
            var currLength = document.body.children.Length;

            if (currLength != lastLength)
            {
                for (var index = currLength - 1; index >= 0; index--) // reverse loop
                {
                    var isParsed = GetIsParsedOfElement(document.body.children.item(index));
                    if (isParsed) break;
                    IHTMLElement element = document.body.children.item(index);
                    parseStack.Push(element);
                    SetElemenentAsParsed(element);
                }
                lastLength = currLength;
            }
        }

        private void UpdateParse()
        {
            while (parseStack.Count > 0)
            {
                var element = parseStack.Pop();
                var linearize = Regex.Replace(element.innerHTML, @"\r\n?|\n", "");
                if (!string.IsNullOrEmpty(linearize))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(linearize);

                    var packet = Parse(doc);

                    if (packet != null)
                    {
                        Console.WriteLine(packet);
                        Server.SendMessage(packet);
                    }
                }
            }
        }

        private Packet Parse(HtmlDocument doc)
        {
            Packet packet = null;

            var node = doc.DocumentNode.FirstChild;
            var currAttr = node.GetAttributeValue("class", "");
            var childs = node.GetRecursiveAttributes("class").ToDictionary(x => x.Value.Split()[0], x => x.OwnerNode.SelectSingleNode("text()")?.InnerText);
            // 알림
            if (currAttr.Contains("txt_notice"))
            {
                // 후원
                if (currAttr.Equals("txt_notice area_space box_sticker"))
                {
                    var nickname = childs.GetValueOrDefault("txt_spon", "").Split('님')[0];
                    var amount = childs.GetValueOrDefault("txt_money", "0").Replace(",", "");
                    var message = childs.GetValueOrDefault("txt_message", "").Trim();
                    var user = GetUserData(nickname);

                    packet = PacketHelper.CreateSpon(user, message, amount);
                }

                // todo : 입장, 퇴장, 임명, 해임, 닉변경, 도배경고 파싱
                else
                {
                    var message = node.SelectSingleNode("text()").InnerText;
                    if (message.Contains("입장"))
                    {
                        var nickname = message.Split('(')[0].Trim();
                        var user = GetUserData(nickname);
                        packet = PacketHelper.CreateEnter(user);
                    }
                    else if (message.Contains("퇴장"))
                    {
                        var nickname = message.Split('(')[0].Trim();
                        var user = GetUserData(nickname);
                        packet = PacketHelper.CreateExit(user);
                    }
                    else
                    {
                        /*
	                        철수(hpch****)님이 매니저로 임명 되었습니다.
	                        철수님의 닉네임이 '순이'(으)로 변경되었습니다.
                        */
                        packet = PacketHelper.CreateLog(message);
                    }
                }
            }

            // 메세지
            else if (currAttr.Contains("area_chat"))
            {
                // 채팅
                if (childs.ContainsKey("tit_name"))
                {
                    var nickname = childs.GetValueOrDefault("tit_name", "").Split('(')[0].Trim();
                    var grade = childs.GetValueOrDefault("ico_label", "");
                    var message = childs.GetValueOrDefault("info_words", "").Trim();
                    var user = GetUserData(nickname, grade);
                    packet = PacketHelper.CreateChat(user, message);
                }

                // 쪽지
                else if (childs.ContainsKey("tit_whisper"))
                {
                    var nickname = childs.GetValueOrDefault("tit_whisper", "").Split('(')[0].Trim();
                    var message = childs.GetValueOrDefault("info_whisper", "").Trim();
                    var user = GetUserData(nickname);
                    packet = PacketHelper.CreateChat(user, message);
                }
            }

            return packet;
        }

        private void Init()
        {
            RefreshPlayerList();
            if (PlayerList.ContainsKey(SelectedPlayerHwnd))
            {
                document = FindChatRoot(SelectedPlayerHwnd);
                if (document != null)
                {
                    parseStack = new Stack<IHTMLElement>();
                    mapUser = new Dictionary<string, User>();
                    lastLength = document.body.children.Length;
                    SetElemenentAsParsed((IHTMLElement) document.body.children[lastLength - 1]); // set last html element as parsed
                    readyToUpdate = true;
                    Console.WriteLine("준비 완료");
                }
            }
            else
            {
                SelectPotPlayer();
            }
        }

        private static HTMLDocument FindChatRoot(IntPtr hwnd)
        {
            // case of seperated chat window
            var currInstance = WinAPI.GetWndInstance(hwnd);
            var seperatedChatHwnds = WinAPI.FindAllWindowsWithCaption("채팅/덧글");
            foreach (var currChatHwnd in seperatedChatHwnds)
            {
                var currChatWndInstance = WinAPI.GetWndInstance(currChatHwnd.Key);
                if (currInstance == currChatWndInstance)
                {
                    var schilds = WinAPI.GetChildsWithClassName(currChatHwnd.Key, "Internet Explorer_Server");
                    foreach (var intPtr in schilds)
                    {
                        var doc = WinAPI.GetHtmlDocumentOfWnd(intPtr);

                        var t = FindElementWithClassName(doc, "wrap_chat");
                        if (t != null)
                        {
                            return doc;
                        }
                    }
                }
            }

            // case of embeded chat window 
            var childs = WinAPI.GetChildsWithClassName(hwnd, "Internet Explorer_Server");
            foreach (var intPtr in childs)
            {
                var doc = WinAPI.GetHtmlDocumentOfWnd(intPtr);
                var tempElem = FindElementWithClassName(doc, "wrap_chat");
                if (tempElem != null)
                {
                    return doc;
                }
            }
            return null;
        }
    }
}
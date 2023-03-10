using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChatApp.Server.Models;
using ChatApp.Server.Win32;
using HtmlAgilityPack;
using mshtml;
using static ChatApp.Server.Extensions.HtmlExtension;

namespace ChatApp.Server.Parsers
{
    public class KakaoParser
    {
        public KakaoParser()
        {
            SelectPotPlayer();
        }

        private const Platform Platform = Models.Platform.kakao;
        private Dictionary<IntPtr, string> _playerList;
        protected int lastLength;
        protected Dictionary<string, User> mapUser;
        protected Stack<IHTMLElement> parseStack;

        public HTMLDocument Document { get; protected set; }

        public bool Ready { get; protected set; }

        protected Dictionary<IntPtr, string> PlayerList
        {
            get
            {
                _playerList = WinAPI.FindAllWindowsWithClassNames("PotPlayer", "PotPlayer64");
                return _playerList;
            }
        }

        protected IntPtr SelectedPlayerHwnd { get; set; } = IntPtr.Zero;

        public void Update()
        {
            if (Ready)
                try
                {
                    UpdateRead();
                    UpdateParse();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Error] " + e.Message);
                    Ready = false;
                }
            else
                Init();
        }

        public void Init()
        {
            if (PlayerList.ContainsKey(SelectedPlayerHwnd))
            {
                Document = FindChatRoot(SelectedPlayerHwnd);
                if (Document != null)
                {
                    parseStack = new Stack<IHTMLElement>();
                    mapUser = new Dictionary<string, User>();
                    lastLength = Document.body.children.Length;
                    SetElemenentAsParsed((IHTMLElement) Document.body.children[lastLength - 1]); // set last html element as parsed
                    Ready = true;
                    Console.WriteLine("준비 완료");
                }
            }
            else
            {
                SelectPotPlayer();
            }
        }

        protected void SelectPotPlayer()
        {
            if (PlayerList.Count == 1)
            {
                SelectedPlayerHwnd = PlayerList.ElementAt(0).Key;
            }
            else if (PlayerList.Count > 1)
            {
                Console.WriteLine("하나 이상의 팟플레이어 실행중");
                for (var i = 0; i < PlayerList.Count; i++)
                    Console.WriteLine(i + 1 + " : " + PlayerList.ElementAt(i).Value);

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

        protected static HTMLDocument FindChatRoot(IntPtr hwnd)
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
                            return doc;
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
                    return doc;
            }
            return null;
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

        protected void UpdateRead()
        {
            var currLength = Document.body.children.Length;

            if (currLength != lastLength)
            {
                for (var index = currLength - 1; index >= 0; index--) // reverse loop
                {
                    var isParsed = GetIsParsedOfElement(Document.body.children.item(index));
                    if (isParsed) break;
                    IHTMLElement element = Document.body.children.item(index);
                    parseStack.Push(element);
                    SetElemenentAsParsed(element);
                }
                lastLength = currLength;
            }
        }

        protected void UpdateParse()
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

        protected Packet Parse(HtmlDocument doc)
        {
            Packet packet = null;

            var node = doc.DocumentNode.FirstChild;
            var currAttr = node.GetAttributeValue("class", "");
            var childs = node.GetRecursiveAttributes("class").ToDictionary(x => x.Value.Split()[0], x => x.OwnerNode.SelectSingleNode("text()")?.InnerText);
            // 알림
            if (currAttr.Contains("txt_notice"))
                if (currAttr.Equals("txt_notice area_space box_sticker"))
                {
                    var nickname = childs.GetValueOrDefault("txt_spon", "").Split('님')[0];
                    var amount = childs.GetValueOrDefault("txt_money", "0").Replace(",", "");
                    var message = childs.GetValueOrDefault("txt_message", "").Trim();
                    var user = GetUserData(nickname);

                    packet = Packet.CreateSpon(user, message, amount);
                }

                // todo : 입장, 퇴장, 임명, 해임, 닉변경, 도배경고 파싱
                else
                {
                    var message = node.SelectSingleNode("text()").InnerText;
                    if (message.Contains("입장"))
                    {
                        var nickname = message.Split('(')[0].Trim();
                        var user = GetUserData(nickname);
                        packet = Packet.CreateEnter(user);
                    }
                    else if (message.Contains("퇴장"))
                    {
                        var nickname = message.Split('(')[0].Trim();
                        var user = GetUserData(nickname);
                        packet = Packet.CreateExit(user);
                    }
                    else
                    {
                        /*
	                        철수(hpch****)님이 매니저로 임명 되었습니다.
	                        철수님의 닉네임이 '순이'(으)로 변경되었습니다.
                        */
                        packet = Packet.CreateLog(message);
                    }
                }

            // 메세지
            else if (currAttr.Contains("area_chat"))
                if (childs.ContainsKey("tit_name"))
                {
                    var nickname = childs.GetValueOrDefault("tit_name", "").Split('(')[0].Trim();
                    var grade = childs.GetValueOrDefault("ico_label", "");
                    var message = childs.GetValueOrDefault("info_words", "").Trim();
                    var user = GetUserData(nickname, grade);
                    packet = Packet.CreateChat(user, message);
                }

                // 쪽지
                else if (childs.ContainsKey("tit_whisper"))
                {
                    var nickname = childs.GetValueOrDefault("tit_whisper", "").Split('(')[0].Trim();
                    var message = childs.GetValueOrDefault("info_whisper", "").Trim();
                    var user = GetUserData(nickname);
                    packet = Packet.CreateChat(user, message);
                }

            return packet;
        }
    }
}
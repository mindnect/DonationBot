using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatApp.Extension;
using ChatApp.Parser;
using ChatApp.WPF.Win32;
using ChatAppLib;
using ChatAppLib.Models;
using HtmlAgilityPack;
using mshtml;

namespace ChatApp.WPF.Parser
{
    public class KakaoParser : BaseParser
    {
        private IHTMLElement _chatRoot;
        private int _currIndex;
        private int _selectedPotPlayerIdx;
        private List<WinInfo> _playerList;

        public List<WinInfo> GetPlayerList()
        {
            return _playerList;
        }

        public void SelectPlayerWithIndex(int index)
        {
            _selectedPotPlayerIdx = index;
        }

        public void Init()
        {
            base.Init("Daum");
            _selectedPotPlayerIdx = -1;
            _currIndex = -1;
        }

        public HTMLDocument GetHtmlDocument(IntPtr hWndExplorer)
        {
            HTMLDocument htmlDocument = null;
            int num1;
            num1 = Win.Win32.RegisterWindowMessage("WM_HTML_GETOBJECT");
            if (num1 != 0)
            {
                //Console.WriteLine("Get Explorer Document");
                int num;
                Win.Win32.SendMessageTimeout(hWndExplorer, num1, 0, 0, 2, 1000, out num);
                if (num != 0)
                {
                    Win.Win32.ObjectFromLresult(num, ref Win.Win32.IID_IHTMLDocument, 0, ref htmlDocument);
                    if (htmlDocument == null) Console.WriteLine("채팅 오브젝트를 찾을 수 없습니다.");
                    // MessageBox.Show("Couldn't Found Document", "Warning");
                }
            }

            Console.WriteLine("HTML 문서를 찾았습니다.");
            if (!string.IsNullOrEmpty(htmlDocument?.title))
                Console.WriteLine(htmlDocument.title);
            return htmlDocument;
        }

        public bool FindPotPlayer()
        {
            Console.Write(".");
            var ret = false;

            var num = 10;
            var intPtrArray = new IntPtr[num];
            var found = 0;
            var zero = IntPtr.Zero;
            for (var i = 0; i < num; i++)
            {
                zero = Win.Win32.FindWindowEx(IntPtr.Zero, zero, "PotPlayer", null);
                if (zero == IntPtr.Zero)
                    break;
                intPtrArray[i] = zero;
                found++;
            }
            for (var j = found; j < num; j++)
            {
                zero = Win.Win32.FindWindowEx(IntPtr.Zero, zero, "PotPlayer64", null);
                if (zero == IntPtr.Zero)
                    break;
                intPtrArray[j] = zero;
                found++;
            }
            _playerList = new List<WinInfo>(found);
            var stringBuilder = new StringBuilder(260);
            for (var k = 0; k < found; k++)
            {
                Win.Win32.GetWindowText(intPtrArray[k], stringBuilder, 260);
                uint num2;
                Win.Win32.GetWindowThreadProcessId(intPtrArray[k], out num2);
                var  t = new WinInfo
                {
                    hWnd = intPtrArray[k],
                    caption = stringBuilder.ToString(),
                    pid = num2
                };
                _playerList.Add(t);
                ret = true;
            }
            return ret;
        }

        public override void Reset()
        {
            base.Reset();
            _chatRoot = null;
            _currIndex = -1;
            isReady = false;
        }


        public override bool Update()
        {
            if (!base.Update())
                return false;

            int length;
            try
            {
                length = (int) _chatRoot.children.length;
            }
            catch
            {
                _currIndex = -1;
                _chatRoot = null;
                isReady = false;
                PrepareUpdate();
                return false;
            }

            if (_currIndex == -1)
                _currIndex = length - 1;

            var currUniqueNumber = GetElementUniqueNumber((IHTMLElement) _chatRoot.children.Item(_currIndex)); // 80
            var lastUniqueNumber = GetElementUniqueNumber((IHTMLElement) _chatRoot.children.Item(length - 1)); // 101

            while (currUniqueNumber < lastUniqueNumber)
            {
                _currIndex = length - (lastUniqueNumber - currUniqueNumber);
                var element = (IHTMLElement) _chatRoot.children.Item(_currIndex);
                var linearize = Regex.Replace(element.innerHTML, @"\r\n?|\n", "");
                if (!string.IsNullOrEmpty(linearize))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(linearize);

                    var packet = Parse(doc);
                    Console.WriteLine(packet);
                    if (packet != null) Server.SendMessage(packet);
                }
                currUniqueNumber++;
            }
            return true;
        }


        protected override void PrepareUpdate()
        {
            base.PrepareUpdate();
            if (!FindPotPlayer()) return;
            FindChatRoot();
        }

        private void FindChatRoot()
        {
            Console.WriteLine("채팅방 찾는중");

            while (true)
            {
                var intPtr = _playerList[_selectedPotPlayerIdx].hWnd;
                var intPtr1 = FindExplorerHandleByChatWindow(FindChatWindowHandle());
                if (intPtr1 == IntPtr.Zero)
                {
                    foreach (var childWindow in Win.Win32.GetChildWindows(intPtr))
                    {
                        if (Win.Win32.GetWinClass(childWindow) != "Internet Explorer_Server")
                            continue;
                        document = GetHtmlDocument(childWindow);
                        if (document == null) continue;
                        var variable = FindChatRoot(document, 50);
                        if ((variable == null) || (_chatRoot != null))
                            continue;
                        _chatRoot = variable.parentElement;
                        isReady = true;
                        return;
                        //FormCoreWindow.inst.SetChatStatTVPot(ChatStat.Run);
                    }
                }
                else
                {
                    var document1 = GetHtmlDocument(intPtr1);
                    if (document1 != null)
                    {
                        var variable1 = FindChatRoot(document1, 50);
                        if (variable1 != null)
                        {
                            document = document1;
                            _chatRoot = variable1.parentElement;
                            isReady = true;
                            return;
                            //FormCoreWindow.inst.SetChatStatTVPot(ChatStat.Run);
                        }
                    }
                }
            }
        }

        private IHTMLElement FindChatRoot(HTMLDocument tempDocument, int maxAttemptCount)
        {
            foreach (var child in (IEnumerable) tempDocument.body.children)
            {
                var variable = (IHTMLElement) child;
                while (maxAttemptCount > 0)
                {
                    if (variable.className == "wrap_chat")
                    {
                        Console.WriteLine("채팅 문서를 찾았습니다.");
                        return variable;
                    }
                    maxAttemptCount--;
                }
                return null;
            }
            return null;
        }

        private IntPtr FindChatWindowHandle()
        {
            var zero = IntPtr.Zero;
            const int num = 5;
            for (var i = 0; i < num; i++)
            {
                zero = Win.Win32.FindWindowEx(IntPtr.Zero, zero, null, "채팅/덧글");
                if (zero == IntPtr.Zero)
                    break;
                uint num1;
                Win.Win32.GetWindowThreadProcessId(zero, out num1);
                if (num1 == _playerList[_selectedPotPlayerIdx].pid)
                    return zero;
            }
            return IntPtr.Zero;
        }

        private IntPtr FindExplorerHandleByChatWindow(IntPtr hWndChatRoot)
        {
            var intPtr = hWndChatRoot;
            var zero = IntPtr.Zero;
            while (true)
            {
                zero = Win.Win32.FindWindowEx(intPtr, IntPtr.Zero, "Internet Explorer_Server", null);
                if (zero == IntPtr.Zero)
                {
                    zero = Win.Win32.FindWindowEx(intPtr, IntPtr.Zero, null, "");
                    if (zero == IntPtr.Zero)
                        break;
                    intPtr = zero;
                }
                else
                {
                    Console.WriteLine("익스플로러를 찾을 수 없습니다.");
                    break;
                }
            }

            return zero;
        }


        // @습격
        // @습격 죽어라
        // @습격 @200 죽어라
        private MessageModel Parse(HtmlDocument doc)
        {
            MessageModel @base = null;
            PacketType packetType;

            var node = doc.DocumentNode.FirstChild;
            var currAttr = node.GetAttributeValue("class", "");
            var childs = node.GetRecursiveAttributes("class").ToDictionary(x => x.Value.Split()[0], x => x.OwnerNode.SelectSingleNode("text()")?.InnerText);

            // 알림
            if (currAttr.Contains("txt_notice"))
            {
                packetType = PacketType.NOTICE;
                // 후원
                if (currAttr.Equals("txt_notice area_space box_sticker"))
                {
                    packetType |= PacketType.SPON;
                    var nickname = childs.GetValueOrDefault("txt_spon", "").Split('님')[0];
                    var amount = childs.GetValueOrDefault("txt_money", "0").Replace(",", "");
                    var message = childs.GetValueOrDefault("txt_message", "").Trim();
                    var param ="";

                    if (message.StartsWith("@")) // 게임 명령어
                    {
                        packetType |= PacketType.COMMAND;
                        param = ParseCommand(ref message);
                    }

                    @base = new MessageModel(packetType, GetUserData(nickname), message, amount, param);
                }

                // todo : 입장, 퇴장, 임명, 해임, 닉변경, 도배경고 파싱
                else
                {
                    var msg = node.SelectSingleNode("text()").InnerText;
                    //Console.WriteLine("TestLog : " + msg);
                    @base = new MessageModel(PacketType.NOTICE, null, msg);
                }
            }

            // 메세지
            else if (currAttr.Contains("area_chat"))
            {
                packetType = PacketType.MESSAGE;
                var nickname = "";
                var rank = "";
                var message = "";

                // 채팅
                if (childs.ContainsKey("tit_name"))
                {
                    nickname = childs.GetValueOrDefault("tit_name", "").Split('(')[0].Trim();
                    rank = childs.GetValueOrDefault("ico_label", "");
                    message = childs.GetValueOrDefault("info_words", "").Trim();
                }

                // 쪽지
                else if (childs.ContainsKey("tit_whisper"))
                {
                    packetType |= PacketType.WHISPER;
                    nickname = childs.GetValueOrDefault("tit_whisper", "").Split('(')[0].Trim();
                    message = childs.GetValueOrDefault("info_whisper", "").Trim();
                }

                if (message == "") return null; // 이모티콘등과 같이 메세지 없을경우

                // 명령어
                var param ="";
                if (message.StartsWith("@")) // 게임 명령어
                {
                    packetType |= PacketType.COMMAND;
                    param = ParseCommand(ref message);
                }

                @base = new MessageModel(packetType, GetUserData(nickname, rank), message, param);
            }

            return @base;
        }
    }
}
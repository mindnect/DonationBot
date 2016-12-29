using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatApp.Extension;
using ChatApp.Win;
using ChatAppLib.Models;
using ChatAppLib.Window;
using HtmlAgilityPack;
using mshtml;

namespace ChatAppLib.Brokers
{
    public class KakaoBroker : Broker
    {
        private IHTMLElement _chatRoot;

        private IHTMLElement _currElement;
        private int _currUniqueIndex;
        private int _selectedPotPlayerIdx;
        private WindowInfo[] _windowPotPlayers;

        public void Init()
        {
            Console.WriteLine("다음팟 후킹 초기화 중");
            base.Init(Platform.KAKAO);
            _selectedPotPlayerIdx = -1;
            _currUniqueIndex = -1;
            Console.WriteLine("다음팟 후킹 초기화 완료");
        }

        public void SelectPotPlayer()
        {
            Console.WriteLine("\n팟플레이어 선택중");

            if (_windowPotPlayers.Length < 1)
                _selectedPotPlayerIdx = -1;
            else if (_windowPotPlayers.Length == 1)
                _selectedPotPlayerIdx = 0;
            else if (_windowPotPlayers.Length > 1)
                ConsoleSelectPotPlayer();
        }

        public HTMLDocument GetHtmlDocument(IntPtr hWndExplorer)
        {
            HTMLDocument htmlDocument = null;
            var num1 = Win32.RegisterWindowMessage("WM_HTML_GETOBJECT");
            if (num1 != 0)
            {
                //Console.WriteLine("Get Explorer Document");
                int num;
                Win32.SendMessageTimeout(hWndExplorer, num1, 0, 0, 2, 1000, out num);
                if (num != 0)
                {
                    Win32.ObjectFromLresult(num, ref Win32.iidIhtmlDocument, 0, ref htmlDocument);
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
            var num1 = 0;
            var zero = IntPtr.Zero;
            for (var i = 0; i < num; i++)
            {
                zero = Win32.FindWindowEx(IntPtr.Zero, zero, "PotPlayer", null);
                if (zero == IntPtr.Zero)
                    break;
                intPtrArray[i] = zero;
                num1++;
            }
            for (var j = num1; j < num; j++)
            {
                zero = Win32.FindWindowEx(IntPtr.Zero, zero, "PotPlayer64", null);
                if (zero == IntPtr.Zero)
                    break;
                intPtrArray[j] = zero;
                num1++;
            }
            _windowPotPlayers = new WindowInfo[num1];
            var stringBuilder = new StringBuilder(260);
            for (var k = 0; k < num1; k++)
            {
                Win32.GetWindowText(intPtrArray[k], stringBuilder, 260);
                uint num2;
                Win32.GetWindowThreadProcessId(intPtrArray[k], out num2);
                _windowPotPlayers[k] = new WindowInfo
                {
                    hWnd = intPtrArray[k],
                    caption = stringBuilder.ToString(),
                    pid = num2
                };
                Console.Write("\n팟플레이어를 찾았습니다.");
                ret = true;
            }
            return ret;
        }

        public override void Reset()
        {
            base.Reset();
            _chatRoot = null;
            _currUniqueIndex = -1;
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
                _currUniqueIndex = -1;
                _chatRoot = null;
                isReady = false;
                PrepareUpdate();
                return false;
            }

            if (_currElement == null)
            {
                _currElement = (IHTMLElement) _chatRoot.children.Item(length - 1);
            }
            var lastElem = (IHTMLElement) _chatRoot.children.Item(length - 1); // 81
            if (_currElement == lastElem) return true;

            var stack = new Stack<IHTMLElement>();
            var temp = lastElem;
            for (var i = length - 1; _currElement != temp; i--)
            {
                stack.Push(temp);
                temp = _chatRoot.children.item(i-1);
            }
            _currElement = lastElem;

            while (stack.Count > 0)
            {
                var element = stack.Pop();
                var linearize = Regex.Replace(element.innerHTML, @"\r\n?|\n", "");
                if (!string.IsNullOrEmpty(linearize))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(linearize);

                    var packet = Parse(doc);
                    Console.WriteLine(packet);
                    if (packet != null) Server.SendMessage(packet);
                }
            }


            //if (_currUniqueIndex == -1)
            //    _currUniqueIndex = length - 1;

            //var currUniqueNumber = GetElementUniqueNumber((IHTMLElement) _chatRoot.children.Item(_currUniqueIndex)); // 80
            //var lastUniqueNumber = GetElementUniqueNumber((IHTMLElement) _chatRoot.children.Item(length-1)); // 81


            //while (currUniqueNumber < lastUniqueNumber)
            //{
            //    _currUniqueIndex = length - (lastUniqueNumber - currUniqueNumber);
            //    Console.WriteLine(_currUniqueIndex + " " + currUniqueNumber + " " + lastUniqueNumber + " " + length);
            //    var element = (IHTMLElement) _chatRoot.children.Item(_currUniqueIndex);
            //    var linearize = Regex.Replace(element.innerHTML, @"\r\n?|\n", "");
            //    if (!string.IsNullOrEmpty(linearize))
            //    {
            //        var doc = new HtmlDocument();
            //        doc.LoadHtml(linearize);

            //        var packet = Parse(doc);
            //        if (packet == null)
            //        {
            //          Console.WriteLine(doc);  
            //        }
            //        //Console.WriteLine(packet);
            //        if (packet != null) Server.SendMessage(packet);
            //    }
            //    currUniqueNumber++;
            //}
            return true;
        }


        protected override void PrepareUpdate()
        {
            base.PrepareUpdate();
            if (!FindPotPlayer()) return;
            SelectPotPlayer();
            FindChatRoot();
        }

        private void FindChatRoot()
        {
            Console.WriteLine("채팅방 찾는중");

            while (true)
            {
                var intPtr = _windowPotPlayers[_selectedPotPlayerIdx].hWnd;
                var intPtr1 = FindExplorerHandleByChatWindow(FindChatWindowHandle());
                if (intPtr1 == IntPtr.Zero)
                {
                    foreach (var childWindow in Win32.GetChildWindows(intPtr))
                    {
                        if (Win32.GetWinClass(childWindow) != "Internet Explorer_Server")
                            continue;
                        document = GetHtmlDocument(childWindow);
                        if (document == null) continue;
                        var variable = FindChatRoot(document, 50);
                        if ((variable == null) || (_chatRoot != null))
                            continue;
                        _chatRoot = variable.parentElement;
                        isReady = true;
                        return;
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
                zero = Win32.FindWindowEx(IntPtr.Zero, zero, null, "채팅/덧글");
                if (zero == IntPtr.Zero)
                    break;
                uint num1;
                Win32.GetWindowThreadProcessId(zero, out num1);
                if (num1 == _windowPotPlayers[_selectedPotPlayerIdx].pid)
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
                zero = Win32.FindWindowEx(intPtr, IntPtr.Zero, "Internet Explorer_Server", null);
                if (zero == IntPtr.Zero)
                {
                    zero = Win32.FindWindowEx(intPtr, IntPtr.Zero, null, "");
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

        private void ConsoleSelectPotPlayer()
        {
            Console.WriteLine("하나 이상의 팟플레이어 실행중");
            for (var i = 0; i < _windowPotPlayers.Length; i++)
            {
                var windowPotPlayer = _windowPotPlayers[i];
                Console.WriteLine(i + 1 + " : " + windowPotPlayer.caption);
            }
            var userInput = Console.ReadKey(true);
            _selectedPotPlayerIdx = int.Parse(userInput.KeyChar.ToString()) - 1;
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
                        var username = message.Split('(')[0];
                        var user = GetUserData(username);
                        packet = PacketHelper.CreateEnter(user);
                    }
                    else if (message.Contains("퇴장"))
                    {
                        var username = message.Split('(')[0];
                        var user = GetUserData(username);
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
    }
}
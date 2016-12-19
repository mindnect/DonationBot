﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Database;
using HtmlAgilityPack;
using mshtml;

namespace ChatApp.Chat
{
    internal class ChatTVPot : ChatBase
    {
        private IHTMLElement _chatRoot;
        private int _lastElementIdx;
        private int _lastUniqueNumber;
        private int _selectedPotPlayerIdx;
        private bool _waitForSelect;
        private WindowInfo[] _windowPotPlayers;

        public void Init()
        {
            Console.WriteLine("다음팟 후킹 초기화");
            Initialize("Daum");
            _selectedPotPlayerIdx = -1;
            _lastUniqueNumber = -1;
            _lastElementIdx = -1;
            _waitForSelect = false;
        }

        public void FindPotplayer()
        {
            if (_waitForSelect)
                return;
            if (_selectedPotPlayerIdx == -1)
            {
                RefreshPotPlayerHandleList();
                if (_windowPotPlayers.Length == 1)
                {
                    _selectedPotPlayerIdx = 0;
                }
                else if (_windowPotPlayers.Length > 1)
                {
                    _waitForSelect = true;
                    ConsoleSelectPotPlayer();
                    //new FormSelectPotPlayer().Show();
                }
            }


            if (_selectedPotPlayerIdx == -1)
                return;
            FindChatRoot();
        }

        public HTMLDocument GetDocument(IntPtr hWndExplorer)
        {
            HTMLDocument variable = null;
            var num1 = 0;
            num1 = Win32.RegisterWindowMessage("WM_HTML_GETOBJECT");
            if (num1 != 0)
            {
                //Console.WriteLine("Get Explorer Document");
                int num;
                Win32.SendMessageTimeout(hWndExplorer, num1, 0, 0, 2, 1000, out num);
                if (num != 0)
                {
                    Win32.ObjectFromLresult(num, ref Win32.IID_IHTMLDocument, 0, ref variable);
                    if (variable == null) Console.WriteLine("Couldn't Found Document");
                    // MessageBox.Show("Couldn't Found Document", "Warning");
                }
            }
            Console.WriteLine("Find Document");
            return variable;
        }

        public void RefreshPotPlayerHandleList()
        {
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
                uint num2 = 0;
                Win32.GetWindowThreadProcessId(intPtrArray[k], out num2);
                _windowPotPlayers[k] = new WindowInfo();
                _windowPotPlayers[k].hWnd = intPtrArray[k];
                _windowPotPlayers[k].caption = stringBuilder.ToString();
                _windowPotPlayers[k].pid = num2;
            }
        }

        public override void Reset()
        {
            base.Reset();
            _chatRoot = null;
            _lastUniqueNumber = -1;
            _lastElementIdx = -1;
            isReady = false;
        }


        public override bool Update()
        {
            if (!base.Update())
                return false;
            UpdateChat();
            return true;
        }

        protected override void OnUserAdded(UserData newUserData)
        {
            base.OnUserAdded(newUserData);
        }

        protected override void PrepareUpdate()
        {
            FindPotplayer();
            base.PrepareUpdate();
        }

        private void FindChatRoot()
        {
            if (_selectedPotPlayerIdx != -1)
            {
                var intPtr = _windowPotPlayers[_selectedPotPlayerIdx].hWnd;
                var intPtr1 = FindExplorerHandleByChatWindow(FindChatWindowHandle());
                if (intPtr1 == IntPtr.Zero)
                {
                    foreach (var childWindow in Win32.GetChildWindows(intPtr))
                    {
                        if (Win32.GetWinClass(childWindow) != "Internet Explorer_Server")
                            continue;
                        document = GetDocument(childWindow);
                        if (document == null)
                            continue;
                        var variable = FindChatRoot(document, 50);
                        if ((variable == null) || (_chatRoot != null))
                            continue;
                        _chatRoot = variable.parentElement;
                        isReady = true;
                        //FormCoreWindow.inst.SetChatStatTVPot(ChatStat.Run);
                        return;
                    }
                }
                else
                {
                    var document1 = GetDocument(intPtr1);
                    if (document1 != null)
                    {
                        var variable1 = FindChatRoot(document1, 50);
                        if (variable1 != null)
                        {
                            document = document1;
                            _chatRoot = variable1.parentElement;
                            isReady = true;
                            //FormCoreWindow.inst.SetChatStatTVPot(ChatStat.Run);
                        }
                    }
                }
            }
        }

        private IHTMLElement FindChatRoot(HTMLDocument tempDocument, int maxAttemptCount)
        {
            var num = maxAttemptCount;
            foreach (var child in (IEnumerable) tempDocument.body.children)
            {
                var variable = (IHTMLElement) child;
                num--;
                if (num > 0)
                {
                    if (variable.className != "wrap_chat")
                        continue;
                    var num1 = 0;
                    foreach (var obj in (IEnumerable) variable.children)
                        num1++;
                    return variable;
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
                uint num1 = 0;
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
                    Console.WriteLine("Control Found");
                    break;
                }
            }
            return zero;
        }

        
        private void ConsoleSelectPotPlayer()
        {
            Console.WriteLine("한 개 이상 실행중.");
            for (var i = 0; i < _windowPotPlayers.Length; i++)
            {
                var windowPotPlayer = _windowPotPlayers[i];
                Console.WriteLine(i + 1 + " : " + windowPotPlayer.caption);
            }
            var userInput = Console.ReadKey(true);
            _waitForSelect = false;
            _selectedPotPlayerIdx = int.Parse(userInput.KeyChar.ToString()) - 1;
        }

   

        private void Parse(HtmlDocument doc)
        {
            var node = doc.DocumentNode.FirstChild;
            var attrStr = node.GetAttributeValue("class", "");
            var childAttrs = node.GetRecursiveAttributes("class");


            if (attrStr.Contains("txt_notice"))
                // 후원
                if (attrStr.Equals("txt_notice area_space box_sticker"))
                {
                    var amount = node.SelectSingleNode("span[@class='area_sticker type_01']").InnerText.Replace(",", "");
                    var message = node.SelectSingleNode("p[@class='txt_message box_type01']").InnerText.Trim();
                    var nickname = node.SelectSingleNode("p[@class='txt_spon']").InnerText.Split('님')[0];
                    Console.WriteLine(nickname + "\t" + message + "\t" + amount);
                }

                // todo : 입장, 퇴장, 임명, 해임, 닉변경, 도배경고 파싱
                else
                {
                    var msg = node.SelectSingleNode("text()").InnerText;
                    Console.WriteLine(msg);
                }

            else if (attrStr.Contains("area_chat"))
            {
                if (childAttrs.Any(x => x.Value == "tit_whisper"))
                {
                    // 귓속말
                    //var nickname = node.SelectSingleNode("strong/text()").InnerText.Split('(')[0].Trim();
                    //var rankNode = node.SelectSingleNode("strong/span");
                    //var rank = "";
                    //if (rankNode.Attributes["class"].Value.Contains("ico_label"))
                    //    rank = rankNode.InnerText;

                    //var message = node.SelectSingleNode("div").InnerText.Trim();

                    //if (message == "") return; // 이모티콘등과 같이 메세지 없을경우
                    //Console.WriteLine(rank + "\t" + nickname + "\t" + message + "\t");

                }
                else if(childAttrs.Any(x => x.Value == "tit_name"))
                {
                    var nickname = node.SelectSingleNode("strong/text()").InnerText.Split('(')[0].Trim();

                    var rank = "";
                    rank = node.SelectSingleNode(@"strong/span[starts-with(@class,'ico_label')]")?.InnerText;
                    var message = node.SelectSingleNode("div").InnerText.Trim();
                    if (message == "") return; // 이모티콘등과 같이 메세지 없을경우
                    Console.WriteLine(rank + "\t" + nickname + "\t" + message + "\t");
                }

            }
        }

        private void UpdateChat()
        {
            UpdateChatRoom(_chatRoot);
        }
        
        private bool UpdateChatRoom(IHTMLElement root)
        {
            bool flag;
            var num = 0;
            try
            {
                num = (int) root.children.length;
                goto Label0;
            }
            catch
            {
                _lastElementIdx = -1;
                _lastUniqueNumber = -1;
                _chatRoot = null;
                FindChatRoot();
                flag = false;
            }
            return flag;

            Label0:
            if (_lastElementIdx == -1) _lastElementIdx = num - 1;
            if (_lastElementIdx > root.children.length - 1)
            {
                Console.WriteLine("호출되면 안됩니다.");
                var num1 = 50;
                var num2 = num - 1;
                while (num2 >= num - 1 - num1)
                    if (GetElementUniqueNumber((IHTMLElement) root.children.Item(num2)) != _lastUniqueNumber)
                    {
                        num2--;
                    }
                    else
                    {
                        _lastElementIdx = num2 + 1;
                        break;
                    }
            }


            for (var i = _lastElementIdx + 1; i < num; i++)
            {
                var element = (IHTMLElement) root.children.Item(i);
                if (element == null) break;

                var linearize = Regex.Replace(element.innerHTML, @"\r\n?|\n", "");

                var doc = new HtmlDocument();
                doc.LoadHtml(linearize);
                Parse(doc);
            }

            _lastElementIdx = num - 1;
            try
            {
                var variable4 = (IHTMLElement) root.children.Item(num - 1);

                _lastUniqueNumber = GetElementUniqueNumber(variable4);
            }
            catch
            {
            }
            return false;
        }
    }
}
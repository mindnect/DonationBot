using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Database;
using HtmlAgilityPack;
using mshtml;

namespace ChatApp.Chat
{
    internal class ChatTVPot : ChatBase
    {
        private IHTMLElement _chatRoot;
        private int _currIndex;
        private int _selectedPotPlayerIdx;
        private WindowInfo[] _windowPotPlayers;

        public void Init()
        {
            Console.WriteLine("다음팟 후킹 초기화 중");
            Initialize("Daum");
            _selectedPotPlayerIdx = -1;
            _currIndex = -1;
            Console.WriteLine("다음팟 후킹 초기화 완료");
        }

        public void SelectPotPlayer()
        {
            Console.WriteLine("팟플레이어 선택중");

            if (_windowPotPlayers.Length < 1)
            {
                _selectedPotPlayerIdx = -1;
            }
            else if (_windowPotPlayers.Length == 1)
            {
                _selectedPotPlayerIdx = 0;
            }
            else if (_windowPotPlayers.Length > 1)
            {
                ConsoleSelectPotPlayer();
                //new FormSelectPotPlayer().Show();
            }
        }

        public HTMLDocument GetHtmlDocument(IntPtr hWndExplorer)
        {
            HTMLDocument htmlDocument = null;
            var num1 = 0;
            num1 = Win32.RegisterWindowMessage("WM_HTML_GETOBJECT");
            if (num1 != 0)
            {
                //Console.WriteLine("Get Explorer Document");
                int num;
                Win32.SendMessageTimeout(hWndExplorer, num1, 0, 0, 2, 1000, out num);
                if (num != 0)
                {
                    Win32.ObjectFromLresult(num, ref Win32.IID_IHTMLDocument, 0, ref htmlDocument);
                    if (htmlDocument == null) Console.WriteLine("채팅 오브젝트를 찾을 수 없습니다.");
                    // MessageBox.Show("Couldn't Found Document", "Warning");
                }
            }

            Console.WriteLine("HTML 문서를 찾았습니다.");
            if (!string.IsNullOrEmpty(htmlDocument?.title))
                Console.WriteLine(htmlDocument.title);
            return htmlDocument;
        }

        public void FindPotPlayer()
        {
            Console.Write("팟플레이어 찾는중");
            while (true)
            {
                Thread.Sleep(100);
                Console.Write(".");

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
                    Console.WriteLine("\n팟플레이어를 찾았습니다.");

                    return;
                }
            }
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

            var length = -1;
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

            var lastIndex = length - 1;
            if (_currIndex == -1)
                _currIndex = lastIndex;

            while (_currIndex <= lastIndex)
            {
                var element = (IHTMLElement) _chatRoot.children.Item(_currIndex);
                var linearize = Regex.Replace(element.innerHTML, @"\r\n?|\n", "");
                if (!string.IsNullOrEmpty(linearize))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(linearize);
                    try
                    {
                        Parse(doc);
                    }
                    catch
                    {
                        Console.WriteLine("Error : " + element.innerHTML);
                        Console.WriteLine("Error : " + linearize);
                    }
                }
                _currIndex++;
            }
            return true;
        }

        protected override void OnUserAdded(UserData newUserData)
        {
            base.OnUserAdded(newUserData);
        }

        protected override void PrepareUpdate()
        {
            base.PrepareUpdate();
            FindPotPlayer();
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

                    Console.WriteLine("채팅 문서를 찾았습니다.");
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
                    Console.WriteLine("익스플로러를 찾을 수 없습니다.");
                    break;
                }
            }
            
            return zero;
        }


        private void ConsoleSelectPotPlayer()
        {
            Console.WriteLine("\n하나 이상의 팟플레이어 실행중");
            for (var i = 0; i < _windowPotPlayers.Length; i++)
            {
                var windowPotPlayer = _windowPotPlayers[i];
                Console.WriteLine(i + 1 + " : " + windowPotPlayer.caption);
            }
            var userInput = Console.ReadKey(true);
            _selectedPotPlayerIdx = int.Parse(userInput.KeyChar.ToString()) - 1;
        }


        private void Parse(HtmlDocument doc)
        {
            var node = doc.DocumentNode.FirstChild;
            var attrStr = node.GetAttributeValue("class", "");
            var childs = node.GetRecursiveAttributes("class").ToDictionary(x => x.Value.Split()[0], x => x.OwnerNode.SelectSingleNode("text()")?.InnerText);
            // 알림
            if (attrStr.Contains("txt_notice"))
                // 후원
                if (attrStr.Equals("txt_notice area_space box_sticker"))
                {
                    var nickname = childs.GetValueOrDefault("txt_spon", "").Split('님')[0];
                    var amount = childs.GetValueOrDefault("txt_money", "").Replace(",", "");
                    var message = childs.GetValueOrDefault("txt_message", "").Trim();

                    Console.WriteLine(nickname + "\t" + message + "\t" + amount);
                }

                // todo : 입장, 퇴장, 임명, 해임, 닉변경, 도배경고 파싱
                else
                {
                    var msg = node.SelectSingleNode("text()").InnerText;
                    Console.WriteLine(msg);
                }

            // 메세지
            else if (attrStr.Contains("area_chat"))
            {
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
                    nickname = childs.GetValueOrDefault("tit_whisper", "").Split('(')[0].Trim();
                    message = childs.GetValueOrDefault("info_whisper", "").Trim();
                }

                if (message == "") return; // 이모티콘등과 같이 메세지 없을경우

                Console.WriteLine(rank + "\t" + nickname + "\t" + message + "\t");
            }
        }
    }
}
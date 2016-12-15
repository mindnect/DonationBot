using System;
using System.Collections;
using System.Text;
using mshtml;

namespace ChatApp
{
    internal class ChatTVPot : ChatBase
    {
        public WindowInfo[] windowPotPlayers;

        public int selectedPotPlayerIdx;

        private int lastUniqueNumber;

        private int lastElementIdx;

        private bool waitForSelect;

        private IHTMLElement chatRoot;

        private void ParseHtmlElement(IHTMLElement element , ChatData ret)
        {
            foreach (var child in element.children)
            {
                var current = (IHTMLElement) child;
                ParseHtmlElement(current, ret);

                string message;

                switch (current.className)
                {
                    case "txt_notice": // 알림
                        ret.isNotice = true;
                        break;

                    case "tit_whisper": // 귓속말 아이디
                    case "tit_name": // 채팅 아이디
                        if (current.className == "tit_whisper") ret.isWhisper = true;

                        message = current.outerText;
                        message = message.Split(' ','(')[0];
                        ret.user.platform = "Daum";

                        if (message[0] == 'P')
                        {
                            ret.user.rank = "PD";
                            message = message.Replace("PD\r\n", "");
                        }
                        else if (message[0] == 'A')
                        {
                            ret.user.rank = "AD";
                            message = message.Replace("AD\r\n", "");
                        }
                        ret.user.nickName = message;
                        break;

                    case "info_words": // 채팅 메세지
                    case "txt_message box_type01": // 후원 메세지
                    case "info_whisper": // 귓속말 메세지
                        message = current.outerText;
                        message = message.Replace("\n", ""); // 후원 메세지 들어올떄 \n 들어옴
                        ret.message = message;
                        break;

                    case "txt_money txt_type01": // 후원 금액
                        ret.isDonation = true;
                        var amountStr = current.outerText;
                        amountStr = amountStr.Replace(",", "");
                        Console.WriteLine(amountStr);
                        ret.amount = int.Parse(amountStr);
                        break;

                    case "txt_spon": // 후원자
                        var nickName = current.outerText.Substring(0, current.outerText.Length - 11);
                        ret.user.nickName = nickName;
                        break;
                }
            }
        }




        private void FindChatRoot()
        {
            if (selectedPotPlayerIdx != -1)
            {
                var intPtr = windowPotPlayers[selectedPotPlayerIdx].hWnd;
                var intPtr1 = FindExplorerHandleByChatWindow(FindChatWindowHandle());
                if (intPtr1 == IntPtr.Zero)
                {
                    foreach (var childWindow in Win32.GetChildWindows(intPtr))
                    {
                        if (Win32.GetWinClass(childWindow) != "Internet Explorer_Server")
                            continue;
                        var document = GetDocument(childWindow);
                        if (document == null)
                            continue;
                        var variable = FindChatRoot(document, 50);
                        if ((variable == null) || (chatRoot != null))
                            continue;
                        this.document = document;
                        chatRoot = variable.parentElement;
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
                            chatRoot = variable1.parentElement;
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
                    {
                        var variable1 = (IHTMLElement) obj;
                        num1++;
                    }
                    return variable;
                }
                return null;
            }
            return null;
        }

        private IntPtr FindChatWindowHandle()
        {
            var zero = IntPtr.Zero;
            var num = 5;
            for (var i = 0; i < num; i++)
            {
                zero = Win32.FindWindowEx(IntPtr.Zero, zero, null, "채팅/덧글");
                if (zero == IntPtr.Zero)
                    break;
                uint num1 = 0;
                Win32.GetWindowThreadProcessId(zero, out num1);
                if (num1 == windowPotPlayers[selectedPotPlayerIdx].pid)
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

        public void FindPotplayer()
        {
            if (waitForSelect)
                return;
            if (selectedPotPlayerIdx == -1)
            {
                RefreshPotPlayerHandleList();
                if (windowPotPlayers.Length == 1)
                {
                    selectedPotPlayerIdx = 0;
                }
                else if (windowPotPlayers.Length > 1)
                {
                    waitForSelect = true;
                    ConsoleSelectPotPlayer();
                    //new FormSelectPotPlayer().Show();
                }
            }
            Console.WriteLine(windowPotPlayers[selectedPotPlayerIdx].caption + " 후킹중.");
            if (selectedPotPlayerIdx == -1)
                return;
            FindChatRoot();
        }

        private void ConsoleSelectPotPlayer()
        {
            Console.WriteLine("한 개 이상 실행중.");
            for (var i = 0; i < windowPotPlayers.Length; i++)
            {
                var windowPotPlayer = windowPotPlayers[i];
                Console.WriteLine(i + 1 + " : " + windowPotPlayer.caption);
            }
            var UserInput = Console.ReadKey(true);
            waitForSelect = false;
            selectedPotPlayerIdx = int.Parse(UserInput.KeyChar.ToString()) - 1;
        }

        public HTMLDocument GetDocument(IntPtr hWndExplorer)
        {
            int num;
            HTMLDocument variable = null;
            var num1 = 0;
            num1 = Win32.RegisterWindowMessage("WM_HTML_GETOBJECT");
            if (num1 != 0)
            {
                //Console.WriteLine("Get Explorer Document");
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

        public override void Initialize()
        {
            base.Initialize();
            selectedPotPlayerIdx = -1;
            lastUniqueNumber = -1;
            lastElementIdx = -1;
            waitForSelect = false;
        }

        protected override void OnUserAdded(UserData newUser)
        {
            base.OnUserAdded(newUser);
            newUser.platform = "tvpot";
        }

        protected override void PrepareUpdate()
        {
            FindPotplayer();
            base.PrepareUpdate();
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
            windowPotPlayers = new WindowInfo[num1];
            var stringBuilder = new StringBuilder(260);
            for (var k = 0; k < num1; k++)
            {
                Win32.GetWindowText(intPtrArray[k], stringBuilder, 260);
                uint num2 = 0;
                Win32.GetWindowThreadProcessId(intPtrArray[k], out num2);
                windowPotPlayers[k] = new WindowInfo();
                windowPotPlayers[k].hWnd = intPtrArray[k];
                windowPotPlayers[k].caption = stringBuilder.ToString();
                windowPotPlayers[k].pid = num2;
            }
        }

        public override void Reset()
        {
            base.Reset();
            chatRoot = null;
            lastUniqueNumber = -1;
            lastElementIdx = -1;
            isReady = false;
        }

        //public void SelectPotPlayerWindow(int idxWindow)
        //{
        //    Reset();
        //    selectedPotPlayerIdx = idxWindow;
        //    waitForSelect = false;
        //}

        public override bool Update()
        {
            if (!base.Update())
                return false;
            UpdateChat();
            return true;
        }

        private void UpdateChat()
        {
            UpdateChatRoom(chatRoot);
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
                lastElementIdx = -1;
                lastUniqueNumber = -1;
                chatRoot = null;
                FindChatRoot();
                flag = false;
            }
            return flag;

            Label0:
            if (lastElementIdx == -1) lastElementIdx = num - 1;
            if (lastElementIdx > root.children.length - 1)
            {
                Console.WriteLine("호출되면 안됩니다.");
                var num1 = 50;
                var num2 = num - 1;
                while (num2 >= num - 1 - num1)
                    if (GetElementUniqueNumber((IHTMLElement) root.children.Item(num2)) != lastUniqueNumber)
                    {
                        num2--;
                    }
                    else
                    {
                        lastElementIdx = num2 + 1;
                        break;
                    }
            }


            for (var i = lastElementIdx + 1; i < num; i++)
            {
                var htmlElement = (IHTMLElement) root.children.Item(i);
                if (htmlElement == null) break;


                var htmlChild = (IHTMLElement) htmlElement.children.Item(0);
                var chat  = new ChatData();
                ParseHtmlElement(htmlChild,chat);
                AddChatData(chat);
            }

            lastElementIdx = num - 1;
            try
            {
                var variable4 = (IHTMLElement) root.children.Item(num - 1);
                lastUniqueNumber = GetElementUniqueNumber(variable4);
            }
            catch
            {
            }
            return false;
        }
    }
}
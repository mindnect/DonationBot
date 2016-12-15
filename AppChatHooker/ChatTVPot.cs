using System;
using System.Collections;
using System.Text;
using mshtml;

namespace AlcoholV
{
    internal class ChatTVPot : ChatBase
    {
        public WindowInfo[] windowPotPlayers;

        public int selectedPotPlayerIdx;

        private int lastUniqueNumber;

        private int lastElementIdx;

        private bool waitForSelect;

        private IHTMLElement chatRoot;



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

        private IHTMLElement FindChatRoot(IHTMLDocument2 tempDocument, int maxAttemptCount)
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
                    Console.Write("Control Found");
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
                    selectedPotPlayerIdx = ConsoleSelectPotPlayer();
                    //new FormSelectPotPlayer().Show();
                }
            }
            if (selectedPotPlayerIdx == -1)
                return;
            FindChatRoot();
        }

        private int ConsoleSelectPotPlayer()
        {
            Console.WriteLine("한 개 이상 실행중.");
            for (var i = 1; i < windowPotPlayers.Length; i++)
            {
                var windowPotPlayer = windowPotPlayers[i-1];
                Console.WriteLine(i + " : " + windowPotPlayer.caption);
            }
            var UserInput = Console.ReadKey();
            waitForSelect = false;
            return int.Parse(UserInput.KeyChar.ToString())-1;
            
        }

        public IHTMLDocument2 GetDocument(IntPtr hWndExplorer)
        {
            int num;
            IHTMLDocument2 variable = null;
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
                IHTMLElement variable = (IHTMLElement)((dynamic)root.children).Item(i);
                if (variable == null) break;

                IHTMLElement variable1 = (IHTMLElement)((dynamic)variable.children).Item(0);
                var length = variable1.className.Length;

                if (((length == 9) || (length == 20)) && variable1.className.Contains("area_chat"))
                {
                    var variable2 = FindClassFromChild(variable1, "tit_name");

                    if (variable2 != null)
                    {
                        var str = variable2.outerText;
                        var num3 = -1;
                        for (var j = 0; j < str.Length; j++)
                            if (str[j] == '(')
                                num3 = j;
                        var nickName = str.Substring(0, num3);
                        if (nickName[0] == 'P')
                            nickName = nickName.Replace("PD\r\n", "");
                        else if (nickName[0] == 'A')
                            nickName = nickName.Replace("AD\r\n", "");
                        if (nickName[nickName.Length - 1] == ' ')
                            nickName = nickName.Remove(nickName.Length - 1);
                        var variable3 = FindClassFromChild(variable1, "info_words");
                        if (variable3 != null)
                        {
                            var message = variable3.outerText;
                            var chatDatum = new ChatData
                            {
                                user = GetUserData(nickName),
                                message = message
                            };
                            AddChatData(chatDatum);
                        }
                    }
                }

                else if ((variable1.className != null) && variable1.className.Contains("txt_notice"))
                {
                    var var3 = FindClassFromChild(variable1, "txt_spon");
                    if (var3 != null)
                    {
                        var variable2 = FindClassFromChild(variable1, "txt_spon");
                        if (variable2 != null)
                        {
                            var nickName = variable2.outerText.Substring(0, variable2.outerText.Length - 11);
                            var message = "";
                            var messageElement = FindClassFromChild(variable1, "txt_message box_type01");
                            if (messageElement != null)
                            {
                                message = messageElement.outerText;
                                message = message.Replace("\r\n", "");
                            }


                            var amountTxt = FindClassFromChild(variable1, "area_sticker type_01").outerText;
                            amountTxt = amountTxt.Replace(" ", "");
                            amountTxt = amountTxt.Replace(",", "");

                            var amount = int.Parse(amountTxt);

                            var chatDatum = new ChatData
                            {
                                isSpon = true,
                                user = GetUserData(nickName),
                                message = message,
                                amount = amount
                            };
                            AddChatData(chatDatum);
                        }
                    }
                }
            }

            lastElementIdx = num - 1;
            try
            {
                IHTMLElement variable4 = (IHTMLElement)((dynamic)root.children).Item(num - 1);
                lastUniqueNumber = GetElementUniqueNumber(variable4);
            }
            catch
            {
            }
            return false;
        }
    }
}
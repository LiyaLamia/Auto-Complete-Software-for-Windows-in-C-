using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/*
 * 
 * static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
            kelogger keylogger = new kelogger();            
            keylogger.start();
            int count = 0;
            if (count == 0)
                return;
            //Application.Run(new Form1());
            
        } 
 * 
 */


namespace key_unlogger
{

    class kelogger
    {

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder textOut, int count);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint ToUnicodeEx(uint wVirtkey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)]StringBuilder pwszBuff, int cchBuff, uint wflags, uint dwhkl);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        [DllImport("user32")]
        private extern static int CreateCaret(IntPtr hwnd, IntPtr hBitmap, int width, int height);

        /*
        [DllImport("user32")]
        private extern static int GetCaretPos(out Point p);
        [DllImport("user32")]
        private extern static int SetCaretPos(int x, int y);
        [DllImport("user32")]
        private extern static bool ShowCaret(IntPtr hwnd);
        */

        const int WM_SETTEXT = 12;
        const int WM_GETTEXT = 13;
        const int EM_SETSEL = 0x00B1;

        private const int MAX_STRING_BUILDER = 256;
        string path;
        Form1 f;
        Label t;
        ListBox L;
        Label Info;
        IntPtr hWindowHandle;

        System.Collections.ObjectModel.Collection<String> s;
        System.Collections.ObjectModel.Collection<String> s_meaning;

        public kelogger()
        {
      
            f = new Form1();
            f.Width = 200;
            f.Height = 150;
            f.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            f.Text = "AutoComplete";
            
            t = new Label();
            t.Width = 180;
            t.Height = 30;
            t.Padding = new Padding(5);
            t.ForeColor = System.Drawing.Color.Black;
            t.Font = new System.Drawing.Font("Times New Roman", 12);
            
            
            s = new System.Collections.ObjectModel.Collection<string>();
           // s.Add("Bangladesh");
           // s.Add("India");
           // s.Add("Tajmahal");

            s_meaning = new System.Collections.ObjectModel.Collection<string>();
            // s_meaning.Add("Country");
            // s_meaning.Add("Country");
            // s_meaning.Add("Wonder");

            path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "autocomplete";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            path += "\\dataset.txt";


            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                char[] spl = { ' ' };
                string[] l_s = lines[i].Split(spl);
                s.Add(l_s[0]);

                string m = "";
                for (int j = 1; j < l_s.Length; j++)
                {
                    m = m + l_s[j] + " ";
                }
                s_meaning.Add(m);
            }

            t.TextChanged += T_TextChanged;

            L = new ListBox();
            L.Width = 160;
            L.Height = 80;
            L.Left = 10;
            L.Top = 35;
            L.SelectedIndexChanged += L_SelectedIndexChanged;
            

            Info = new Label();
            Info.Left = 180;
            Info.Width = 150;
            Info.Height = 150;
            Info.Padding = new Padding(15);
            Info.Font = new System.Drawing.Font("Times New Roman", 14);
           
            f.Controls.Add(Info);
            f.Controls.Add(t);
            f.Controls.Add(L);


            //f.Show();
        }

        private void L_SelectedIndexChanged(object sender, EventArgs e)
        {
            f.Width = 330;
            String s_item = (String)L.SelectedItem;
            s_item = s_item.ToLower();
            for (int i = 0; i < s.Count; i++)
            {
                String r_item = s[i].ToLower();
                if (r_item.Contains(s_item))
                {
                    Info.Text = s_meaning[i];
                    break;
                }
            }
        }

        private void T_TextChanged(object sender, EventArgs e)
        {
            L.Items.Clear();
            String t_s = t.Text.ToLower();
            for (int I = 0; I < s.Count; I++)
            {
                String s_s = s[I].ToLower();
                if (s_s.Contains(t_s))
                {
                    L.Items.Add(s[I]);
                }
            }

        }

        private string GetActiveWindowTitle(IntPtr h)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
           
            if (GetWindowText(h, Buff, nChars).ToInt32() > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        internal void start()
        {
         
            Keys key;

                while (true)
                {
                    Application.DoEvents();

                    for (Int32 i = 0; i < 1000; i++)
                    {
              
                        int value = GetAsyncKeyState(i);

                        if ((value & 0x8000) == 0 || (value & 0x1) == 0)
                            continue;
                        key = (Keys)i;

                        switch (key)
                        {
                            case Keys.LButton:
                                break;
                            case Keys.MButton:
                                break;
                            case Keys.RButton:
                                break;
                            case Keys.Back:
                                if (t.Text != "")
                                {
                                    t.Text = t.Text.Substring(0, t.Text.Length - 1);
                                    f.Update();
                                }
                                else
                                {
                                    f.Width = 200;
                                    Info.Text = "";
                                    f.Hide();
                                    f.Update();
                                }
                                break;
                            case Keys.Down:
                                f.Focus();
                                L.Focus();
                                break;
                            case Keys.ShiftKey:
                            case Keys.Shift:

                            case Keys.LShiftKey:

                            case Keys.RShiftKey:

                            case Keys.Capital:
                                break;
                            case Keys.Enter:
                                if (hWindowHandle == null)
                                    break;
                             
                                string w_name = GetActiveWindowTitle(hWindowHandle);
                               
                  
                                if (w_name.IndexOf("Notepad") > 0)
                                {

                                    IntPtr notepadTextbox = FindWindowEx(hWindowHandle, IntPtr.Zero, "Edit", null);
                                    //sending the message to the textbox

                                    IntPtr Handle = Marshal.AllocHGlobal(500);
                                    int NumText = (int)SendMessage(notepadTextbox, WM_GETTEXT, (IntPtr)50, Handle);
                                    // copy the characters from the unmanaged memory to a managed string
                                    string Text = Marshal.PtrToStringUni(Handle);
                                    // Display the string using a label
                                    string sub_text = Text.Substring(0, Text.Length - t.Text.Length);
                                    sub_text = sub_text + (String)L.SelectedItem;
                                    
                                    SendMessage(notepadTextbox, WM_SETTEXT, 0, sub_text);
                                    SendMessage(notepadTextbox, EM_SETSEL, sub_text.Length,sub_text.Length);
                                    
                              
                                } else if (w_name.IndexOf("WordPad") > 0)
                                {
                                    IntPtr notepadTextbox = FindWindowEx(hWindowHandle, IntPtr.Zero, "RICHEDIT50W", null);
                                    //sending the message to the textbox
                                    SendMessage(notepadTextbox, WM_SETTEXT, 0, "This is the new Text!!!");
                                }

                                t.Text = "";
                                f.Width = 200;
                                Info.Text = "";
                                f.Hide();
                                break;
                            case Keys.Space:
                                t.Text = "";
                       
                                f.Hide();
                                break;
                            case Keys.Tab:
                                break;
                            case Keys.Escape:
                                break;



                            default:

                                ShowWindow(f.Handle, 8);
                                // f.Show();

                                IntPtr Pt = GetForegroundWindow();

                                if (Pt != f.Handle)
                                    hWindowHandle = Pt;
                                   
                                if (hWindowHandle != null)
                                {
                                    uint dwProcessId;
                                    uint dwThreadId = GetWindowThreadProcessId(hWindowHandle, out dwProcessId);
                                    byte[] kState = new byte[256];
                                    GetKeyboardState(kState);
                                    uint HKL = GetKeyboardLayout(dwThreadId);
                                    StringBuilder keyName = new StringBuilder();
                                    ToUnicodeEx((uint)i, (uint)i, kState, keyName, 16, 0, HKL);

                                    t.Text = t.Text + keyName.ToString();
                                    f.Update();
                                }

                       
                                break;

                        }
                }
            }
        }


        private string getCurrentWindowText()
        {
            IntPtr handle = GetForegroundWindow();
            StringBuilder title = new StringBuilder(MAX_STRING_BUILDER);
            GetWindowText(handle, title, MAX_STRING_BUILDER);
            return title.ToString();

        }


    }
}


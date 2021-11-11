using Binarysharp.MemoryManagement;
using Jupiter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace m2gbot
{
    public partial class Form1 : Form
    {
        MemoryModule memoryModule = new MemoryModule();
        int index = 0;
        Dictionary<int, float[,]> dict = new Dictionary<int, float[,]>();
        Keys[] keyB = new Keys[] {Keys.F1, Keys.F2, Keys.F3,Keys.F4,Keys.F5,Keys.F6,
        Keys.F7,Keys.F8,Keys.F9,Keys.F10,Keys.F11,Keys.F12};
        IntPtr[] patternkord;
        bool durum = true;
        bool hataDurum = false;
        IntPtr[] telePattern;
        IntPtr[] ptk;
        MemorySharp sharp;
        IntPtr[] game;
        int msg = 0;
        float uzaklik;
        int pos;
        float kordx, kordy;
        int state = -1;
        float[] xyz;
        IntPtr[] patternAddresses;
        bool msjKontrol = true;
        int indis;
        Sending s = new Sending();
        public Form1()
        {
            InitializeComponent();
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            int a = 4;
            sharp = new MemorySharp(Process.GetProcessById(Convert.ToInt32(textBox1.Text)));

            int length_game = await gamePattern();
            int length_tel = await telPattern();
            int length_ptk = await ptkPattern();

            if (length_tel > 0 && length_ptk == 1 && length_game == 1)
            {
               
                await kordAl();
                
            }
            else
            {
                //timer1.Enabled = false;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            durum = true;
            await stoneFind();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            durum = false;
            sharp.Write<int>(ptk[0] + 0x64, 0, false);

        }

        Task findP()
        {
            return Task.Run(() =>
            {
                float[] telkord;
                indis = 0;
                for (int i = 0; i < telePattern.Length; i++)
                {
                    telkord = sharp.Read<float>(telePattern[i] + 0x2C1, 2, false);
                    if ((int)telkord[0] == kordx && (int)telkord[1] == kordy)
                    {

                        indis = i;
                        break;
                    }
                }
                //MessageBox.Show(indis.ToString());
            });
        }

        //Task<int> kordPattern()
        //{
        //    Task<int> pattern = Task.Run(() =>
        //    {
        //        patternkord = memoryModule.PatternScan(Convert.ToInt32(textBox1.Text), IntPtr.Zero, "DC ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 A3 D8");
        //        return patternkord.Length;
        //    });
        //    return pattern;
        //}

        Task<int> gamePattern()
        {
            Task<int> pattern = Task.Run(() =>
            {
                game = memoryModule.PatternScan(Convert.ToInt32(textBox1.Text), IntPtr.Zero, "00 00 00 0F 00 00 00 47 61 6D 65 00 6E 67");
                //var deger = sharp.ReadString(game[0] + 0x7, false,40);

                //MessageBox.Show(result);
                return game.Length;
            });
            return pattern;
        }

     

        Task mesajAl()
        {
            return Task.Run(async () =>
            {

                while (true)
                {

                    if (msjKontrol == true && kordx!=0 && kordy!=0)
                    {
                        await Task.Delay(2000);
                        var healthPtr = new IntPtr(0x019A02F0);
                        int[] offsets = { 0x8, 0x0 };
                        healthPtr = sharp[healthPtr].Read<IntPtr>();

                        //healthPtr = m[healthPtr + offsets[0], false].Read<IntPtr>(); // false is here to avoid rebasing
                        //healthPtr = m[healthPtr + offsets[1], false].Read<IntPtr>();

                        foreach (var offset in offsets)
                        {
                            healthPtr = sharp[healthPtr + offset, false].Read<IntPtr>(); // false is here to avoid rebasing
                        }

                        msg = sharp[healthPtr + 0x28, false].Read<int>();
                        label5.Text = msg.ToString();
                        if (msg > 0)
                        {
                            this.BackColor = Color.Red; // this should be pink-ish

                        }
                        else if (msg == 0)
                        {
                            this.BackColor = Color.Ivory; // this should be pink-ish

                        }
                        /// mesaj al
                    }
                    else if (msjKontrol == false)
                    {
                        break;
                    }
                }
            });
        }     

        Task<int> telPattern()
        {
            Task<int> pattern = Task.Run(() =>
            {
                telePattern = memoryModule.PatternScan(Convert.ToInt32(textBox1.Text), IntPtr.Zero, "46 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01");
                return telePattern.Length;
            });
            return pattern;
        }

        Task kordAl()
        {
            return Task.Run(async () =>
            {

                while (true)
                {
                    try
                    {
                        if (hataDurum == true)
                        {
                            await Task.Delay(2000);
                            int length_tel = await telPattern();
                            int length_ptk = await ptkPattern();
                            if (length_tel > 0 && length_ptk == 1)
                            {
                                hataDurum = false;
                            }
                            else
                            {
                                //var healthPtr2 = new IntPtr(0x01784CB4);

                                //healthPtr2 = sharp[healthPtr2].Read<IntPtr>();
                                throw new Win32Exception();
                            }
                        }
                        else if (hataDurum == false)
                        {
                            await Task.Delay(300);
                            var healthPtr = new IntPtr(0x019A0258);
                            int[] offsets = { 0x48C, 0x18, 0x3A8 };
                            healthPtr = sharp[healthPtr].Read<IntPtr>();

                            //healthPtr = m[healthPtr + offsets[0], false].Read<IntPtr>(); // false is here to avoid rebasing
                            //healthPtr = m[healthPtr + offsets[1], false].Read<IntPtr>();

                            foreach (var offset in offsets)
                            {
                                healthPtr = sharp[healthPtr + offset, false].Read<IntPtr>(); // false is here to avoid rebasing
                            }

                            kordx = sharp[healthPtr + 0x10, false].Read<int>();
                            kordy = sharp[healthPtr + 0x10 + 0x4, false].Read<int>();
                            kordy = kordy * -1;
                            if (kordx == 0 && kordy == 0)
                            {
                                throw new Win32Exception();
                            }
                            label1.Text = kordx.ToString();
                            label2.Text = kordy.ToString();


                        }
                    }
                    catch (Win32Exception e)
                    {
                        label1.Text = "0";
                        label2.Text = "0";
                        kordx = 0;
                        kordy = 0;
                        label3.Text = "0";
                        durum = false;
                        await Task.Delay(2000);
                        var deger = sharp.Read<byte>(game[0] + 0x7, 4, false);
                        string result = System.Text.Encoding.UTF8.GetString(deger);
                        int a = 5;
                        label4.Text = result;
                        if (result == "OffL")
                        {
                            sharp.Windows.MainWindow.Activate();
                            await Task.Delay(2000);
                            //result = "";
                            s.Sendings(keyB[selectIndex], false);
                        }

                        else if (result == "Sele")
                        {
                            sharp.Windows.MainWindow.Activate();
                            await Task.Delay(2000);
                            //result = "";
                            s.Sendings(Keys.Enter, false);
                            await Task.Delay(2000);
                        }

                        else if (result == "Load")
                        {
                            
                            await Task.Delay(2000);
                            //result = "";
                            deger = sharp.Read<byte>(game[0] + 0x7, 4, false);
                            result = System.Text.Encoding.UTF8.GetString(deger);
                            label4.Text = result;
                            await Task.Delay(2000);
                        }

                        else if (result == "Game")
                        {
                            hataDurum = true;
                            result = "";

                            await Task.Delay(2000);
                            durum = true;
                            //state = -1;
                            if (checkBox1.Checked == true)
                            {
                                button2.PerformClick();
                                
                            }
                            if (checkBox2.Checked == true)
                            {
                                button4.PerformClick();

                            }
                        }

                    }

                }
            });
        }

        Task<int> ptkPattern()
        {
            Task<int> pattern = Task.Run(() =>
            {
                ptk = memoryModule.PatternScan(Convert.ToInt32(textBox1.Text), IntPtr.Zero, "34 35 B1 01 EC 35 B1 01");
                return ptk.Length;
            });
            return pattern;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            checkedListBox1.CheckOnClick = true;
            this.TopMost = true;
        }
        int selectIndex;
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectIndex = checkedListBox1.SelectedIndex;
            MessageBox.Show(selectIndex.ToString());
        }

       

 

        private async void button4_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true) {
                msjKontrol = true;
                await mesajAl();
            }
            else
            {
                msjKontrol = false;
            }

        }

        Task stoneFind()
        {
            return Task.Run(async () =>
            {

                while (true)
                {

                    await Task.Delay(500);
                    if (durum == true)
                    {
                        if (kordx != 0 && kordy != 0)
                        {
                            if (state == -1)
                            {
                                await findP();
                                state = 0;
                                await Task.Delay(2000);
                            }
                            if (state == 0)
                            {
                                pos = 0;
                                index = 0;
                                dict.Clear();
                                patternAddresses = memoryModule.PatternScan(Convert.ToInt32(textBox1.Text), IntPtr.Zero, "48 1F 00 00 48 1F");
                                int a = 4;

                                //IntPtr value = new IntPtr(0x20);
                                // Read an array of 3 integers 

                                for (int i = 0; i < patternAddresses.Length; i++)
                                {
                                    int[] adres = sharp.Read<int>(patternAddresses[i], 1, false);
                                    if (adres[0] != 0)
                                    {
                                        xyz = sharp.Read<float>(patternAddresses[i] + 0x78, 3, false);
                                        uzaklik = Convert.ToInt32(Math.Sqrt((Math.Pow(xyz[0] - kordx, 2) + Math.Pow(xyz[1] - kordy, 2))));
                                        float[,] deger = { { xyz[0], xyz[1], xyz[2], uzaklik, i } };
                                        dict.Add(index, deger);
                                        index++;
                                    }
                                }
                                if (dict.Count > 0)
                                {
                                    for (int i = 0; i < dict.Count; i++)
                                    {
                                        if (dict[i][0, 3] < dict[pos][0, 3]) { pos = i; }
                                    }
                                    //MessageBox.Show(dict[0][0, 0].ToString());
                                    //MessageBox.Show(pos.ToString());
                                    state = 1;
                                }
                            }
                            if (state == 1)
                            {
                                //sharp.Write<int>(targetPattern[0] + 0xA8, dict[pos][0, 0], false);
                                // Thread.Sleep(5000);
                                //targetKontrol = sharp.Read<int>(targetPattern[0] + 0xAC, 1, false);

                                //if (targetKontrol[0] == 0)
                                //{
                                //MessageBox.Show("isin");
                                //sharp.Write<int>(targetPattern[0] + 0xA8, dict[pos][0, 0], false);
                                var deneme = sharp.Read<float>(telePattern[0] + 0x2C1, 2, false);
                                //MessageBox.Show(deneme[0].ToString() + "+" + deneme[1].ToString());
                                sharp.Write<float>(telePattern[indis] + 0x2C1, dict[pos][0, 0], false);
                                sharp.Write<float>(telePattern[indis] + 0x2C5, dict[pos][0, 1], false);
                                sharp.Write<float>(telePattern[indis] + 0x2C9, dict[pos][0, 2], false);
                                //sharp.Write<float>(wPattern[0] - 0x13C, 1, false);
                                //sharp.Write<float>(wPattern[0] - 0x138, 65792, false);
                                //sharp.Write<float>(wPattern[0] - 0x13C, 0, false);
                                //sharp.Write<float>(wPattern[0] - 0x138, 65536, false);
                                sharp.Write<int>(ptk[0] + 0x64, 1, false);
                                //targetKontrol[0] = 5;
                                await Task.Delay(1000);
                                if(kordx!= (int)dict[pos][0, 0]&& kordy != (int)dict[pos][0, 1])
                                {
                                    state = 0;
                                }
                                //while (kontrol) {
                                //    await Task.Delay(1000);
                                //    var kontrol=sharp.Read<int>(patternAddresses[pos], 1, false);

                                //    label3.Text = kontrol[0].ToString();
                                //    if (kontrol[0] == 0)
                                //    {
                                //        sharp.Write<int>(ptk[0] + 0x64, 0, false);
                                //        break;
                                //    }
                                //}
                                //}

                                //sharp.read
                                state = 2;

                            }
                            if (state == 2)
                            {
                                //
                                if (kordx != 0 && kordy != 0)
                                {

                                    var kontrol = sharp.Read<int>(patternAddresses[pos], 1, false);

                                    label3.Text = kontrol[0].ToString();
                                    if (kontrol[0] != 8008 && kontrol[0] != 0)
                                    {
                                        sharp.Write<int>(ptk[0] + 0x64, 0, false);
                                        state = 0;
                                    }
                                    if (kontrol[0] == 0)
                                    {
                                        sharp.Write<int>(ptk[0] + 0x64, 0, false);

                                        using (var t = sharp.Assembly.BeginTransaction())
                                        {
                                            t.AddLine("mov ecx,[0x01DA0248]");
                                            t.AddLine("call 0x597DD0");
                                            t.AddLine("retn");
                                        }
                                        state = 0;
                                    }
                                } //
                            }

                            //durum = false;
                        }
                    }
                    else
                    {
                        state = -1;
                        //sharp.Write<int>(ptk[0] + 0x64, 0, false);
                        break;

                    }
                }
            });
        }
    }
}
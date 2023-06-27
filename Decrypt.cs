using System;
using System.Collections.Generic;
using System.Linq;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(GetButton1());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            label1.Text = fish.decrypt(textBox1.Text);
        }
    }

    class fish
    {
        static Dictionary<char, int> EXTRA = new Dictionary<char, int>();
        static List<char> NUM_ALPHA = new List<char>();
        static Dictionary<char, int> ALPHA_NUM = new Dictionary<char, int>();

        static List<List<int>> ENCODING = new List<List<int>>
        {
            new List<int> { 1, 4, 32 },
            new List<int> { 1, 16, 32 },
            new List<int> { 1, 8, 32 },
            new List<int> { 1, 64 },
            new List<int> { 1, 32 },
            new List<int> { 1, 4, 16, 128 },
            new List<int> { 1, 32, 64 }
        };

        public static string decrypt(string crypt)
        {
            string[] FAMILY = { "QzF3n6/9CAtpu0O", "B1IREhcSyrleKvMW8LXx", "7N-dVbwsY2g4oaJZGUDj", "iHkq.mPf5T" };

            int x = 0;
            foreach (var item in FAMILY)
            {
                foreach (var c in item)
                {
                    EXTRA[c] = 3 - x;
                    NUM_ALPHA.Add(c);
                    ALPHA_NUM[c] = NUM_ALPHA.Count - 1;
                }
                x++;
            }


            return JuniperDecrypt(crypt);
        }

        public static string JuniperDecrypt(string crypt)
        {
            string[] parts = crypt.Split(new[] { "$9$" }, StringSplitOptions.None);
            string chars = parts[1];
            char first = chars[0];
            chars = chars.Substring(1);
            string toss = chars.Substring(0, EXTRA[first]);
            chars = chars.Substring(EXTRA[first]);
            char prev = first;
            string decrypt = "";

            while (chars.Length > 0)
            {
                var decode = ENCODING[decrypt.Length % ENCODING.Count];
                string nibble = chars.Substring(0, decode.Count);
                chars = chars.Substring(decode.Count);
                List<int> gaps = new List<int>();
                foreach (char i in nibble)
                {
                    int g = (_Gap(prev, i) + NUM_ALPHA.Count - 1) % NUM_ALPHA.Count;
                    prev = i;
                    gaps.Add(g);
                }
                decrypt += _GapDecode(gaps, decode);
            }

            return decrypt;
        }

        static int _Gap(char c1, char c2)
        {
            return (ALPHA_NUM[c2] - ALPHA_NUM[c1] + NUM_ALPHA.Count - 1) % (NUM_ALPHA.Count - 1);
        }

        static char _GapDecode(List<int> gaps, List<int> dec)
        {
            int num = 0;
            if (gaps.Count != dec.Count)
            {
                Console.WriteLine("Nibble and decode size not the same!");
                Environment.Exit(1);
            }
            for (int i = 0; i < gaps.Count; i++)
            {
                num += gaps[i] * dec[i];
            }
            return (char)(num % 256);
        }
    }
}
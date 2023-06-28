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
            Juniper.Decrypt9_setup();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void button1_Click(object sender, System.EventArgs e)
        {
            label1.Text = Juniper.Decrypt9(textBox1.Text);
        }
    }

    class Juniper
    {
        private static Dictionary<char, int> SALT_LEN = new();
        private static List<char> NUM_ALPHA = new();
        private static Dictionary<char, int> ALPHA_NUM = new();

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

        public static void Decrypt9_setup()
        {
            string[] FAMILY = { "QzF3n6/9CAtpu0O", "B1IREhcSyrleKvMW8LXx", "7N-dVbwsY2g4oaJZGUDj", "iHkq.mPf5T" };

            int x = 0;
            foreach (var foo in FAMILY)
            {
                foreach (var bar in foo)
                {
                    SALT_LEN[bar] = 3 - x;
                    NUM_ALPHA.Add(bar); //FAMILY concatinated
                }
                x++;
            }

            for (int y = 0; y < NUM_ALPHA.Count; y++)
            {
                ALPHA_NUM[NUM_ALPHA[y]] = y;
            }
        }

        public static string Decrypt9(string crypt)
        {   // the crypt is of the form $9$ + salt_len_indicator + salt + hash

            if (crypt.Substring(0, 3) != "$9$") return "Error: incorrect format"; //making sure the crypt is the correct form/type


            string salt_n_hash = crypt.Split(new[] { "$9$" }, StringSplitOptions.None)[1]; //removing $9$
            char salt_len_indicator = salt_n_hash[0]; 
            string hash = salt_n_hash.Substring(SALT_LEN[salt_len_indicator]+1); //removing the salt

            // decrypting the hash
            char prev = salt_len_indicator;
            string decrypt = "";
            while (hash.Length > 0)
            {
                var decode = ENCODING[decrypt.Length % 7];
                string nibble = hash.Substring(0, decode.Count); //each nibble will result in a character
                hash = hash.Substring(decode.Count); // removing the nibble from the hash
                List<int> gaps = new();
                foreach (char i in nibble)
                {
                    int g = Gap(prev, i);
                    prev = i;
                    gaps.Add(g);
                }
                decrypt += (char) GapDecode(gaps, decode);
            }

            return decrypt;
        }


        static int Gap(char c1, char c2)
        {
            return ((ALPHA_NUM[c2] - ALPHA_NUM[c1] + NUM_ALPHA.Count) % NUM_ALPHA.Count) - 1;
        }

        static char GapDecode(List<int> gaps, List<int> dec)
        {
            int num = 0;
            if (gaps.Count != dec.Count)
            {
                Console.WriteLine("Nibble and decode size not the same!");
                Environment.Exit(1);
            }
            for (int i = 0; i < gaps.Count; i++)
            {
                num += (gaps[i]*dec[i]);
            }

            int ASCII = num % 256;
            System.Diagnostics.Debug.WriteLine(ASCII.ToString());
            return (char)(ASCII);
        }
    }
}
using GreenBox.CParsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Scanner s = new Scanner();
            s.PushSource("(main)", new StreamReader("test.c"));

            List<Token> tokens = new List<Token>();
           // StreamWriter sw = new StreamWriter("log.txt");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                Token tk = s.Next();

                if (tk.Type == TokenType.EOF || tk.Type == TokenType.Null)
                    break;

               // tokens.Add(tk);
                /*string str = tk.Type.ToString();

                if (tk.Text != null)
                    str += " " + tk.Text;

                sw.WriteLine(str);*/
            }
            sw.Stop();

            //sw.Close();
        }
    }
}

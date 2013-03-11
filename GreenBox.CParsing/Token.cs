using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.CParsing
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string File { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Text { get; set; }

        public Token(string file, int line, int column)
        {
            Type = TokenType.Null;
            Line = line;
            Column = column;
        }

        public Token(TokenType type, string file, int line, int column)
        {
            Type = type;
            Line = line;
            Column = column;
        }

        public Token(TokenType type, string text, string file, int line, int column)
        {
            Type = type;
            Line = line;
            Text = text;
            Column = column;
        }

        public override string ToString()
        {
            // TODO: Improve this
            string str = Line + ":" + Column + ": " + Type.ToString();

            if (Text != null)
                str += " [" + Text + "]";

            return str;
        }
    }
}

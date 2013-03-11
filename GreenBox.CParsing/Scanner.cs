using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.CParsing
{
    public class Scanner
    {
        private class State
        {
            public TextReader Source;
            public string File;
            public int Line, Column;
            public char Cur, La;
        }

        private Stack<State> _stateStack;
        private State _s;
        private bool _isEof;

        public Scanner()
        {
            _stateStack = new Stack<State>();
            _s = null;
        }

        public void PushSource(string filename)
        {
            PushSource(filename, new StreamReader(filename));
        }

        public void PushSource(string filename, string text)
        {
            PushSource(filename, new StringReader(text));
        }

        public void PushSource(string filename, TextReader reader)
        {
            if (_s != null)
                _stateStack.Push(_s);

            _s = new State()
            {
                Source = reader,
                File = filename,
                Line = 1,
                Column = 0,
            };

            GetChar(false);
        }

        public Token Next()
        {
            SkipWhitespace();

            Token token = new Token(_s.File, _s.Line, _s.Column);

            if (_isEof)
            {
                token.Type = TokenType.EOF;
                return token;
            }

            if (char.IsLetter(_s.Cur) || _s.Cur == '_')
            {
                ParseIdentifier(token);
                return token;
            }
            else if (char.IsNumber(_s.Cur))
            {
                ParseNumber(token);
                return token;
            }
            else if (_s.Cur == '"')
            {
                ParseString(token);
                return token;
            }
            else if (_s.Cur == '\'')
            {
                ParseChar(token);
                return token;
            }

            switch (_s.Cur)
            {
                case ';':
                    token.Type = TokenType.Semicolon;
                    break;
                case '{':
                    token.Type = TokenType.LeftBracket;
                    break;
                case '}':
                    token.Type = TokenType.RightBracket;
                    break;
                case ',':
                    token.Type = TokenType.Comma;
                    break;
                case ':':
                    token.Type = TokenType.Colon;
                    break;
                case '=':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.EqOp;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Assign;
                    break;
                case '(':
                    token.Type = TokenType.LeftPar;
                    break;
                case ')':
                    token.Type = TokenType.RightPar;
                    break;
                case '[':
                    token.Type = TokenType.LeftBracket;
                    break;
                case ']':
                    token.Type = TokenType.RightBracket;
                    break;
                case '.':
                    if (_s.La == '.')
                    {
                        GetChar();
                        if (_s.La == '.')
                        {
                            token.Type = TokenType.Ellipsis;
                            GetChar();
                        }
                        else
                            throw new Exception(""); // TODO: Where is my error handling
                    }
                    else
                        token.Type = TokenType.Dot;
                    break;
                case '&':
                    if (_s.La == '&')
                    {
                        token.Type = TokenType.AndOp;
                        GetChar();
                    }
                    else if (_s.La == '=')
                    {
                        token.Type = TokenType.AndAssign;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Ampersand;
                    break;
                case '!':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.NotAssign;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Exclamation;
                    break;
                case '~':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.NotAssign;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Tilde;
                    break;
                case '+':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.AddAssign;
                        GetChar();
                    }
                    else if (_s.La == '+')
                    {
                        token.Type = TokenType.IncOp;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Plus;
                    break;
                case '-':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.SubAssign;
                        GetChar();
                    }
                    else if (_s.La == '-')
                    {
                        token.Type = TokenType.DecOp;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Minus;
                    break;
                case '*':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.MulAssign;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Star;
                    break;
                case '/':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.DivAssign;
                        GetChar();
                    }
                    else if (_s.La == '/' || _s.La == '*')
                    {
                        ParseComment();
                        return Next();
                    }
                    else
                        token.Type = TokenType.Slash;
                    break;
                case '%':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.ModAssign;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Percent;
                    break;
                case '<':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.LeOp;
                        GetChar();
                    }
                    else if (_s.La == '<')
                    {
                        GetChar();
                        if (_s.La == '=')
                        {
                            token.Type = TokenType.LeftAssign;
                            GetChar();
                        }
                        else
                            token.Type = TokenType.LeftOp;
                    }
                    else
                        token.Type = TokenType.Lt;
                    break;
                case '>':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.GeOp;
                        GetChar();
                    }
                    else if (_s.La == '>')
                    {
                        GetChar();
                        if (_s.La == '=')
                        {
                            token.Type = TokenType.RightAssign;
                            GetChar();
                        }
                        else
                            token.Type = TokenType.RightOp;
                    }
                    else
                        token.Type = TokenType.Gt;
                    break;
                case '^':
                    if (_s.La == '=')
                    {
                        token.Type = TokenType.XorAssign;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Xor;
                    break;
                case '|':
                    if (_s.La == '|')
                    {
                        token.Type = TokenType.OrOp;
                        GetChar();
                    }
                    else if (_s.La == '=')
                    {
                        token.Type = TokenType.OrAssign;
                        GetChar();
                    }
                    else
                        token.Type = TokenType.Bar;
                    break;
                case '?':
                    token.Type = TokenType.Question;
                    break;
                case '#':
                    ParsePreprocessor();
                    return Next();
                case '\\':
                    GetChar();
                    return Next();
                default:
                    throw new Exception(""); // TODO: Where is my error handling
            }

            GetChar();

            return token;
        }

        private void ParsePreprocessor()
        {
            int line = _s.Line;

            do
            {
                bool hue = _s.Cur == '\\';

                GetChar();

                if (hue && line != _s.Line)
                    line = _s.Line;
            }
            while (line == _s.Line);

            // TODO: Report
        }

        private void GetChar(bool checkEof = false)
        {
            if (_s == null)
                throw new InvalidOperationException("PushSource must be called first");

            if (_isEof)
                return;

            int ch = _s.Source.Read();

            if (ch == -1)
            {
                if (_stateStack.Count == 0)
                {
                    _isEof = true;
                    _s.Cur = '\0';
                    _s.La = '\0';

                    if (checkEof)
                        throw new Exception("Unexpected EOF"); // TODO: Where is my error handling
                }
                else
                {
                    _s = _stateStack.Pop();
                    GetChar();
                }
            }
            else if (ch == '\n')
            {
                _s.Line++;
                _s.Column = 0;
                GetChar();
            }
            else if (ch == '\r')
            {
                // We just ignore CR
                GetChar();
            }
            else
            {
                _s.Column++;
                _s.Cur = (char)ch;
                _s.La = (char)_s.Source.Peek();
            }
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_s.Cur) && !_isEof)
                GetChar();
        }

        private void ParseIdentifier(Token token)
        {
            string str = "";

            do
            {
                str += _s.Cur;
                GetChar();
            } while (char.IsLetterOrDigit(_s.Cur) || _s.Cur == '_');

            token.Text = str;

            switch (str)
            {
                case "auto":
                    token.Type = TokenType.Auto;
                    break;
                case "break":
                    token.Type = TokenType.Break;
                    break;
                case "case":
                    token.Type = TokenType.Case;
                    break;
                case "const":
                    token.Type = TokenType.Const;
                    break;
                case "continue":
                    token.Type = TokenType.Continue;
                    break;
                case "default":
                    token.Type = TokenType.Default;
                    break;
                case "do":
                    token.Type = TokenType.Do;
                    break;
                case "else":
                    token.Type = TokenType.Else;
                    break;
                case "enum":
                    token.Type = TokenType.Enum;
                    break;
                case "extern":
                    token.Type = TokenType.Extern;
                    break;
                case "for":
                    token.Type = TokenType.For;
                    break;
                case "goto":
                    token.Type = TokenType.Goto;
                    break;
                case "if":
                    token.Type = TokenType.If;
                    break;
                case "return":
                    token.Type = TokenType.Return;
                    break;
                case "signed":
                    token.Type = TokenType.Signed;
                    break;
                case "sizeof":
                    token.Type = TokenType.SizeOf;
                    break;
                case "static":
                    token.Type = TokenType.Static;
                    break;
                case "struct":
                    token.Type = TokenType.Struct;
                    break;
                case "switch":
                    token.Type = TokenType.Switch;
                    break;
                case "typedef":
                    token.Type = TokenType.Typedef;
                    break;
                case "union":
                    token.Type = TokenType.Union;
                    break;
                case "unsigned":
                    token.Type = TokenType.Unsigned;
                    break;
                case "volatile":
                    token.Type = TokenType.Volatile;
                    break;
                case "while":
                    token.Type = TokenType.While;
                    break;
                default:
                    token.Type = TokenType.Identifier;
                    break;
            }
        }

        private void ParseComment()
        {
            bool isMultiline = false;
            string str = "";
            int line = _s.Line;
            int column = _s.Column;

            if (_s.Cur == '/' && _s.La == '*')
                isMultiline = true;

            if (isMultiline)
            {
                while (!(_s.Cur == '*' && _s.La == '/'))
                {
                    str += _s.Cur;
                    GetChar();
                }

                str += "*/";
                GetChar();
                GetChar();
            }
            else
            {
                while (line == _s.Line)
                {
                    str += _s.Cur;
                    GetChar();
                }
            }

            // TODO: Report comment
        }

        private char ParseEscape()
        {
            char c;

            if (_s.Cur == '\\')
            {
                GetChar(true);

                switch (_s.Cur)
                {
                    case 'n':
                        c = '\n';
                        break;
                    case 'r':
                        c = '\r';
                        break;
                    case 'b':
                        c = '\b';
                        break;
                    case 'a':
                        c = '\a';
                        break;
                    case 't':
                        c = '\t';
                        break;
                    default:
                        c = _s.Cur;
                        break;
                }
            }
            else
            {
                c = _s.Cur;
            }

            GetChar(true);
            return c;
        }

        private void ParseString(Token token)
        {
            // TODO: Parse escapes
            string str = "";

            GetChar(true);
            while (_s.Cur != '"')
                str += ParseEscape();
            GetChar();
            
            token.Text = str;
            token.Type = TokenType.StringLiteral;
        }

        private void ParseChar(Token token)
        {
            // TODO: Parse escapes
            string str = "";

            GetChar(true);
            str = ParseEscape().ToString();

            if (_s.Cur != '\'')
                throw new Exception(""); // TODO: Where is my error handling
            GetChar();

            token.Text = str;
            token.Type = TokenType.CharLiteral;
        }

        private void ParseNumber(Token token)
        {
            string str = "";
            bool isHex = false;
            bool isDecimal = false;
            bool isFloat = false;

            if (_s.Cur == '0' && _s.La == 'x')
            {
                str += "0x";
                isHex = true;
                GetChar();
                GetChar();
            }
            else if (_s.Cur == '.' && char.IsNumber(_s.La))
            {
                str += ".";
                isDecimal = true;
                GetChar();
            }

            if (isHex)
            {
                if (!(char.IsNumber(_s.Cur) || (_s.Cur >= 'a' && _s.Cur <= 'f') || (_s.Cur >= 'A' && _s.Cur <= 'F')))
                    throw new Exception(""); // TODO: Where is my error handling

                do
                {
                    str += _s.Cur;
                    GetChar();
                } while (char.IsNumber(_s.Cur) || (_s.Cur >= 'a' && _s.Cur <= 'f') || (_s.Cur >= 'A' && _s.Cur <= 'F'));

            }
            else
            {
                if (!char.IsNumber(_s.Cur))
                    throw new Exception(""); // TODO: Where is my error handling

                do
                {
                    str += _s.Cur;
                    GetChar();

                    if (_s.Cur == '.')
                    {
                        if (!isDecimal && char.IsNumber(_s.La))
                        {
                            str += ".";
                            isDecimal = true;
                            GetChar();
                        }
                        else
                        {
                            throw new Exception(""); // TODO: Where is my error handling
                        }
                    }
                    else if (_s.Cur == 'f' || _s.Cur == 'F')
                    {
                        str += _s.Cur;
                        isDecimal = true;
                        isFloat = true;
                        GetChar();
                        break;
                    }
                } while (char.IsNumber(_s.Cur));
            }

            token.Text = str;

            if (isDecimal)
                token.Type = isFloat ? TokenType.SingleDecimal : TokenType.Decimal;
            else if (isHex)
                token.Type = TokenType.HexNumber;
            else
                token.Type = TokenType.Number;
        }
    }
}

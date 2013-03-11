using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.CParsing
{
    public enum TokenType
    {
        Null,

        // Keyword
        Auto,
        Break,
        Case,
        Const,
        Continue,
        Default,
        Do,
        Else,
        Enum,
        Extern,
        For,
        Goto,
        If,
        Return,
        Signed,
        SizeOf,
        Static,
        Struct,
        Switch,
        Typedef,
        Union,
        Unsigned,
        Volatile,
        While,

        // Constants
        Identifier,
        Number,
        HexNumber,
        OctalNumber,
        Decimal,
        SingleDecimal,
        StringLiteral,
        CharLiteral,

        RightAssign,
        LeftAssign,
        AddAssign,
        SubAssign,
        MulAssign,
        DivAssign,
        ModAssign,
        AndAssign,
        XorAssign,
        NotAssign,
        OrAssign,
        RightOp,
        LeftOp,
        IncOp,
        DecOp,
        PtrOp,
        AndOp,
        OrOp,
        LeOp,
        GeOp,
        EqOp,
        NeOp,

        // Symbols
        Semicolon,
        RightBracket,
        LeftBracket,
        Comma,
        Colon,
        Assign,
        RightPar,
        LeftPar,
        RightCol,
        Dot,
        Ampersand,
        Exclamation,
        Tilde,
        Minus,
        Plus,
        Star,
        Slash,
        Percent,
        Lt,
        Gt,
        Xor,
        Bar,
        Question,
        Ellipsis,

        EOF
    }
}

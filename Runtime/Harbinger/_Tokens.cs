using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        public enum TokenType
        {
            Identifier,
            Number,
            String,
            If, Else,
            LParen, RParen,
            LBrace, RBrace,
            Semicolon,
            Equals, Greater, Less,
            Print,
            EOF
        }

        public class Token
        {
            public TokenType Type;
            public string Value;
            public Token(TokenType type, string value = "")
            {
                Type = type;
                Value = value;
            }
        }
        static List<Token> Tokenize(string src)
        {
            var tokens = new List<Token>();
            int i = 0;
            while (i < src.Length)
            {
                char c = src[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (char.IsLetter(c))
                {
                    int start = i;
                    while (i < src.Length && (char.IsLetterOrDigit(src[i]) || src[i] == '_')) i++;
                    string word = src.Substring(start, i - start);
                    if (word == "if") tokens.Add(new Token(TokenType.If));
                    else if (word == "else") tokens.Add(new Token(TokenType.Else));
                    else if (word == "print") tokens.Add(new Token(TokenType.Print));
                    else tokens.Add(new Token(TokenType.Identifier, word));
                    continue;
                }

                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < src.Length && char.IsDigit(src[i])) i++;
                    tokens.Add(new Token(TokenType.Number, src.Substring(start, i - start)));
                    continue;
                }

                if (c == '"')
                {
                    int start = ++i;
                    while (i < src.Length && src[i] != '"') i++;
                    string str = src.Substring(start, i - start);
                    i++; // skip closing "
                    tokens.Add(new Token(TokenType.String, str));
                    continue;
                }

                switch (c)
                {
                    case ';': tokens.Add(new Token(TokenType.Semicolon)); i++; break;
                    case '=': tokens.Add(new Token(TokenType.Equals)); i++; break;
                    case '{': tokens.Add(new Token(TokenType.LBrace)); i++; break;
                    case '}': tokens.Add(new Token(TokenType.RBrace)); i++; break;
                    case '(': tokens.Add(new Token(TokenType.LParen)); i++; break;
                    case ')': tokens.Add(new Token(TokenType.RParen)); i++; break;
                    case '>': tokens.Add(new Token(TokenType.Greater)); i++; break;
                    case '<': tokens.Add(new Token(TokenType.Less)); i++; break;
                    default: i++; break; // skip unknown chars
                }
            }
            tokens.Add(new Token(TokenType.EOF));
            return tokens;
        }
    }
}
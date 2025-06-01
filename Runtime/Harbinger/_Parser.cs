using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        class Parser
        {
            List<Token> tokens; int pos = 0;

            Token Peek() => tokens[pos];
            Token Next() => tokens[pos++];

            public List<Node> Parse(List<Token> toks)
            {
                tokens = toks; pos = 0;
                var nodes = new List<Node>();
                while (Peek().Type != TokenType.EOF)
                {
                    if (Peek().Type == TokenType.If) nodes.Add(ParseIf());
                    else if (Peek().Type == TokenType.Print) nodes.Add(ParsePrint());
                    else if (Peek().Type == TokenType.Identifier) nodes.Add(ParseAssign());
                    else { Next(); } // skip
                }
                return nodes;
            }

            Node ParseAssign()
            {
                var id = Next();
                Next(); // =
                var val = Next();
                Next(); // ;
                return new AssignNode(id.Value, val.Value);
            }
            Node ParsePrint()
            {
                Next(); // print
                var str = Next();
                Next(); // ;
                return new PrintNode(str.Value);
            }
            Node ParseIf()
            {
                Next(); // if
                Next(); // (
                var left = Next(); var op = Next(); var right = Next();
                Next(); // )
                Next(); // {
                var body = new List<Node>();
                while (Peek().Type != TokenType.RBrace)
                {
                    if (Peek().Type == TokenType.Print) body.Add(ParsePrint());
                    else if (Peek().Type == TokenType.Identifier) body.Add(ParseAssign());
                    else Next();
                }
                Next(); // }
                return new IfNode(new ConditionNode(left.Value, op.Type.ToString(), right.Value), body);
            }
        }
    }
}
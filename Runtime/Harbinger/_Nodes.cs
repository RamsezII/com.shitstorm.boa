using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        public abstract class Node { }
        public class AssignNode : Node
        {
            public string Name; public string Value;
            public AssignNode(string n, string v) { Name = n; Value = v; }
        }
        public class PrintNode : Node
        {
            public string Message;
            public PrintNode(string msg) { Message = msg; }
        }
        public class IfNode : Node
        {
            public Node Condition;
            public List<Node> Body;
            public IfNode(Node cond, List<Node> body) { Condition = cond; Body = body; }
        }
        public class ConditionNode : Node
        {
            public string Left, Op, Right;
            public ConditionNode(string l, string op, string r) { Left = l; Op = op; Right = r; }
        }
    }
}
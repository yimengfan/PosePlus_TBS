using System;
using System.Collections.Generic;
using System.Text;

namespace MyMathf
{
    static public class RPN
    {
        static Dictionary<char, int> priorities = null;
        const string operators = "+-*/%^";

        static RPN()
        {
            priorities = new Dictionary<char, int>();
            priorities.Add('#', -1);
            priorities.Add('+', 0);
            priorities.Add('-', 0);
            priorities.Add('*', 1);
            priorities.Add('/', 1);
            priorities.Add('%', 1);
            priorities.Add('^', 2);
        }

        static double Compute(double leftNum, double rightNum, char op)
        {
            switch (op)
            {
                case '+': return leftNum + rightNum;
                case '-': return leftNum - rightNum;
                case '*': return leftNum * rightNum;
                case '/': return leftNum / rightNum;
                case '%': return leftNum % rightNum;
                case '^': return Math.Pow(leftNum, rightNum);
                default: return 0;
            }
        }

        static bool IsOperator(char op)
        {
            return operators.IndexOf(op) >= 0;
        }

        static bool IsLeftAssoc(char op)
        {
            return op == '+' || op == '-' || op == '*' || op == '/' || op == '%';
        }

        static Queue<object> PreOrderToPostOrder(string expression)
        {
            var result = new Queue<object>();
            var operatorStack = new Stack<char>();
            operatorStack.Push('#');
            char top, cur,tempChar;
            string tempNum;
            if (expression[0] == '-') expression = '0' + expression;

            for (int i = 0,j; i < expression.Length; )
            {
                cur = expression[i++];
                top = operatorStack.Peek();

                if (cur == '(')
                {
                    operatorStack.Push(cur);
                }
                else
                {
                    if (IsOperator(cur))
                    {
                        while(IsOperator(top) && ((IsLeftAssoc(cur)&& priorities[cur] <= priorities[top])) ||(!IsLeftAssoc(cur)&&priorities[cur]<priorities[top]))
                        {
                            result.Enqueue(operatorStack.Pop());
                            top = operatorStack.Peek();
                        }
                        operatorStack.Push(cur);
                    }
                    else if (cur == ')')
                    {
                        while (operatorStack.Count > 0 && (tempChar = operatorStack.Pop()) != '(')
                        {
                            result.Enqueue(tempChar);
                        }
                    }
                    else
                    {
                        tempNum = "" + cur;
                        j = i;
                        while (j < expression.Length && (expression[j] == '.' || (expression[j] >= '0' && expression[j] <= '9')))
                        {
                            tempNum += expression[j++];
                        }
                        i = j;
                        result.Enqueue(tempNum);
                    }
                }
            }
            while (operatorStack.Count > 0)
            {
                cur = operatorStack.Pop();
                if (cur == '#') continue;
                if (operatorStack.Count > 0)
                {
                    top = operatorStack.Peek();
                }
                
                result.Enqueue(cur);
            }
            
            return result;
        }

        public static double Calucate(string expression)
        {
            try
            {
                var rpn = PreOrderToPostOrder(expression);
                var operandStack = new Stack<double>();
                double left, right;
                object cur;
                while (rpn.Count > 0)
                {
                    cur = rpn.Dequeue();
                    if (cur is char)
                    {
                        right = operandStack.Pop();
                        left = operandStack.Pop();
                        operandStack.Push(Compute(left, right, (char)cur));
                    }
                    else
                    {
                        operandStack.Push(double.Parse(cur.ToString()));
                    }
                }
                return operandStack.Pop();
            }
            catch
            {
                throw new Exception("表达式格式不正确！");
            }            
        }

       
    }
}

using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DapperRepository.Providers.MySql
{
    public class WhereBuilder
    {
        public string Translate<T>(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                return string.Empty;
            return Recurse(expression.Body, typeof(T).Name);
        }

        private string Recurse(Expression expression, string tableName, bool isUnary = false)
        {
            if (expression is UnaryExpression unary)
            {
                var right = Recurse(unary.Operand, tableName, true);
                return string.Format("`{0}`.`{1}` @{2}", tableName, NodeTypeToString(unary.NodeType, right == "NULL"), right);
            }
            else if (expression is BinaryExpression body)
            {
                string right = Recurse(body.Left, tableName);
                return string.Format("`{0}`.`{1}` {2} @{3}", tableName, Recurse(body.Left, tableName), NodeTypeToString(body.NodeType, right == "NULL"), right);
            }
            else if (expression is ConstantExpression constant)
            {
                return ValueToString(constant.Value, isUnary);
            }
            else if (expression is MemberExpression member)
            {
                if (member.Member is PropertyInfo property)
                {
                    if (isUnary && member.Type == typeof(bool))
                    {
                        return string.Format("(`{0}`.`{1}` != 0)", tableName, property.Name);
                    }

                    return property.Name;
                }
                else if (member.Member is FieldInfo)
                {
                    return ValueToString(GetValue(member), isUnary);
                }

                throw new Exception($"Expression does not refer to a property or field: {expression}");
            }
            else if (expression is MethodCallExpression methodCall)
            {
                // LIKE queries:
                if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) }))
                {
                    return string.Format("(`{0}`.`{1}` LIKE '%@{2}%')", tableName, Recurse(methodCall.Object, tableName), Recurse(methodCall.Arguments[0], tableName));
                }
                if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
                {
                    return string.Format("((`{0}`.`{1}` LIKE '@{2}%')", tableName, Recurse(methodCall.Object, tableName), Recurse(methodCall.Arguments[0], tableName));
                }
                if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
                {
                    return string.Format("(`{0}`.`{1}` LIKE '%@{2}')", tableName, Recurse(methodCall.Object, tableName), Recurse(methodCall.Arguments[0], tableName));
                }
                // IN queries:
                if (methodCall.Method.Name == "Contains")
                {
                    Expression collection;
                    Expression property;
                    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                    {
                        collection = methodCall.Arguments[0];
                        property = methodCall.Arguments[1];
                    }
                    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                    {
                        collection = methodCall.Object;
                        property = methodCall.Arguments[0];
                    }
                    else
                    {
                        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                    }

                    var values = (IEnumerable)GetValue(collection);
                    var concated = string.Empty;
                    foreach (var e in values)
                    {
                        concated += ValueToString(e, false) + ", ";
                    }

                    if (concated == string.Empty)
                    {
                        return ValueToString(false, true);
                    }

                    return string.Format("(`{0}`.`{1}` IN ({2}))", tableName, Recurse(property, tableName), concated.Substring(0, concated.Length - 2));
                }

                throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            }

            throw new Exception("Unsupported expression: " + expression.GetType().Name);
        }

        public string ValueToString(object value, bool isUnary)
        {
            if (value is bool)
            {
                if (isUnary)
                    return (bool)value ? "(1 = 1)" : "(1 = 0)";

                return (bool)value ? "1" : "0";
            }

            return value.ToString();
        }

        private static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            return getterLambda.Compile();
        }

        private static object NodeTypeToString(ExpressionType nodeType, bool rightIsNull)
        {
            switch (nodeType)
            {
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return rightIsNull ? "IS" : "=";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Negate:
                    return "-";
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Convert:
                    return "";
            }

            throw new Exception($"Unsupported node type: {nodeType}");
        }
    }
}

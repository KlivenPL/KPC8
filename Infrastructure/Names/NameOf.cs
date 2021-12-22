using System;
using System.Linq.Expressions;

namespace _Infrastructure.Names {
    public static class NameOf<TSource> {
        public static string Full<TObj>(Expression<Func<TSource, TObj>> expression) {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null) {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
                    memberExpression = unaryExpression.Operand as MemberExpression;
            }

            var result = memberExpression.ToString();
            result = result.Substring(result.IndexOf('.') + 1);

            return result;
        }

        public static string Full<TObj>(string sourceFieldName, Expression<Func<TSource, TObj>> expression) {
            var result = Full(expression);
            result = string.IsNullOrEmpty(sourceFieldName) ? result : sourceFieldName + "." + result;
            return result;
        }
    }
}

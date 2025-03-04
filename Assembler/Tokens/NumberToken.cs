using Assembler.Readers;
using System;
using System.Text;

namespace Assembler.Tokens {
    class NumberToken : TokenBase<ushort> {
        public NumberToken() { }

        public NumberToken(ushort value, int position, int line, string filePath) {
            Value = value;
            AddDebugData(position, line, filePath);
        }

        public override ushort Value { get; protected set; }
        public override TokenClass Class => TokenClass.Number;

        public override IToken DeepCopy() {
            return new NumberToken(Value, CodePosition, LineNumber, FilePath);
        }

        public override bool TryAccept(CodeReader reader) {
            // Check if we have an expression enclosed in { }
            if (reader.Current == '{') {
                // Consume the '{'
                reader.Read();
                var exprBuilder = new StringBuilder();
                while (reader.Current != '}' && reader.Current != '\0') {
                    exprBuilder.Append(reader.LowerCurrent);
                    reader.Read();
                }
                if (reader.Current != '}') {
                    // No closing brace found.
                    return false;
                }
                // Consume the closing '}'
                reader.Read();
                var expr = exprBuilder.ToString();

                // Check for trailing 'f' indicating floating-point evaluation.
                bool useFloating = false;
                if (reader.Current == 'f') {
                    useFloating = true;
                    reader.Read();
                }
                if (useFloating) {
                    if (TryEvaluateExpressionFloat(expr, out double floatResult)) {
                        if (floatResult < short.MinValue || floatResult > ushort.MaxValue)
                            return false;
                        Value = (ushort)floatResult;
                        return true;
                    }
                    return false;
                } else {
                    if (TryEvaluateExpression(expr, out int evaluated)) {
                        if (evaluated < short.MinValue || evaluated > ushort.MaxValue)
                            return false;
                        Value = (ushort)evaluated;
                        return true;
                    }
                    return false;
                }
            }

            // Existing handling for numeric literals (decimal, hex, binary)
            if (char.IsDigit(reader.Current) || reader.Current == '-') {
                var sb = new StringBuilder(reader.Current.ToString());

                while (reader.Read() && (char.IsDigit(reader.Current) || IsCharacterAccepted(reader.LowerCurrent))) {
                    sb.Append(reader.Current);
                }

                var input = sb.ToString().ToLower();
                if (input[0] == '-') {
                    // Negative numbers allowed only in decimal.
                    if (short.TryParse(input, out var signedResult)) {
                        Value = (ushort)signedResult;
                        return true;
                    }
                }

                if (input.StartsWith("0x")) {
                    if (ushort.TryParse(input[2..], System.Globalization.NumberStyles.AllowHexSpecifier, null, out var unsignedHexResult)) {
                        Value = unsignedHexResult;
                        return true;
                    }
                }

                if (input.StartsWith("0b")) {
                    if (TryBinToDec(input[2..], out var binaryResult)) {
                        Value = binaryResult;
                        return true;
                    }
                }

                if (ushort.TryParse(input, out var unsignedResult)) {
                    Value = unsignedResult;
                    return true;
                }
            }
            return false;
        }

        private bool IsCharacterAccepted(char c) {
            return c switch {
                'a' => true,
                'b' => true,
                'c' => true,
                'd' => true,
                'e' => true,
                'f' => true,
                'x' => true,
                _ => false,
            };
        }

        private bool TryBinToDec(string val, out ushort result) {
            result = 0;
            if (val.Length == 0)
                return false;
            var tmpSum = 0;
            for (int i = val.Length - 1; i >= 0; i--) {
                if (val[i] == '0' || val[i] == '1') {
                    var numVal = int.Parse(val[i].ToString());
                    tmpSum += numVal == 1 ? 1 << (val.Length - i - 1) : 0;
                } else {
                    return false;
                }
            }
            if (tmpSum >= 0 && tmpSum <= ushort.MaxValue) {
                result = (ushort)tmpSum;
                return true;
            }
            return false;
        }

        // Integer arithmetic expression evaluator.
        private bool TryEvaluateExpression(string expr, out int result) {
            result = 0;
            try {
                int pos = 0;
                void SkipWhitespace() {
                    while (pos < expr.Length && char.IsWhiteSpace(expr[pos]))
                        pos++;
                }
                int ParseNumber() {
                    SkipWhitespace();
                    bool negative = false;
                    if (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-')) {
                        char sign = expr[pos];
                        pos++;
                        SkipWhitespace();
                        // Disallow multiple consecutive unary signs.
                        if (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
                            throw new Exception("Multiple consecutive unary operators not allowed");
                        if (sign == '-') negative = true;
                    }
                    SkipWhitespace();
                    if (pos >= expr.Length)
                        throw new Exception("Expected number");
                    if (pos + 1 < expr.Length && expr[pos] == '0' &&
                        (expr[pos + 1] == 'x' || expr[pos + 1] == 'b')) {
                        if (negative)
                            throw new Exception("Negative hex or binary not allowed");
                        char prefix = expr[pos + 1];
                        pos += 2;
                        int value = 0;
                        if (prefix == 'x') {
                            while (pos < expr.Length && IsHexDigit(expr[pos])) {
                                value = value * 16 + HexValue(expr[pos]);
                                pos++;
                            }
                        } else { // 'b'
                            while (pos < expr.Length && (expr[pos] == '0' || expr[pos] == '1')) {
                                value = value * 2 + (expr[pos] - '0');
                                pos++;
                            }
                        }
                        return value;
                    } else {
                        int value = 0;
                        int start = pos;
                        while (pos < expr.Length && char.IsDigit(expr[pos])) {
                            value = value * 10 + (expr[pos] - '0');
                            pos++;
                        }
                        if (pos == start)
                            throw new Exception("No digits found");
                        return negative ? -value : value;
                    }
                }
                int ParseFactor() {
                    SkipWhitespace();
                    if (pos < expr.Length && expr[pos] == '(') {
                        pos++; // consume '('
                        int value = ParseExpression();
                        SkipWhitespace();
                        if (pos >= expr.Length || expr[pos] != ')')
                            throw new Exception("Missing closing parenthesis");
                        pos++; // consume ')'
                        return value;
                    }
                    return ParseNumber();
                }
                int ParseTerm() {
                    int value = ParseFactor();
                    SkipWhitespace();
                    while (pos < expr.Length && (expr[pos] == '*' || expr[pos] == '/')) {
                        char op = expr[pos];
                        pos++;
                        int next = ParseFactor();
                        if (op == '*')
                            value *= next;
                        else {
                            if (next == 0)
                                throw new Exception("Division by zero");
                            value /= next;
                        }
                        SkipWhitespace();
                    }
                    return value;
                }
                int ParseExpression() {
                    int value = ParseTerm();
                    SkipWhitespace();
                    while (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-')) {
                        char op = expr[pos];
                        pos++;
                        int next = ParseTerm();
                        value = op == '+' ? value + next : value - next;
                        SkipWhitespace();
                    }
                    return value;
                }
                int evaluated = ParseExpression();
                SkipWhitespace();
                if (pos != expr.Length)
                    throw new Exception("Invalid characters in expression");
                result = evaluated;
                return true;
            } catch {
                result = 0;
                return false;
            }
        }

        // Floating-point arithmetic expression evaluator.
        private bool TryEvaluateExpressionFloat(string expr, out double result) {
            result = 0;
            try {
                int pos = 0;
                void SkipWhitespace() {
                    while (pos < expr.Length && char.IsWhiteSpace(expr[pos]))
                        pos++;
                }
                double ParseNumber() {
                    SkipWhitespace();
                    bool negative = false;
                    if (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-')) {
                        char sign = expr[pos];
                        pos++;
                        SkipWhitespace();
                        // Disallow multiple consecutive unary signs.
                        if (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
                            throw new Exception("Multiple consecutive unary operators not allowed");
                        if (sign == '-') negative = true;
                    }
                    SkipWhitespace();
                    if (pos >= expr.Length)
                        throw new Exception("Expected number");
                    if (pos + 1 < expr.Length && expr[pos] == '0' &&
                        (expr[pos + 1] == 'x' || expr[pos + 1] == 'b')) {
                        if (negative)
                            throw new Exception("Negative hex or binary not allowed");
                        char prefix = expr[pos + 1];
                        pos += 2;
                        double value = 0;
                        if (prefix == 'x') {
                            while (pos < expr.Length && IsHexDigit(expr[pos])) {
                                value = value * 16 + HexValue(expr[pos]);
                                pos++;
                            }
                        } else {
                            while (pos < expr.Length && (expr[pos] == '0' || expr[pos] == '1')) {
                                value = value * 2 + (expr[pos] - '0');
                                pos++;
                            }
                        }
                        return value;
                    } else {
                        double value = 0;
                        int start = pos;
                        while (pos < expr.Length && char.IsDigit(expr[pos])) {
                            value = value * 10 + (expr[pos] - '0');
                            pos++;
                        }
                        if (pos == start)
                            throw new Exception("No digits found");
                        return negative ? -value : value;
                    }
                }
                double ParseFactor() {
                    SkipWhitespace();
                    if (pos < expr.Length && expr[pos] == '(') {
                        pos++; // consume '('
                        double value = ParseExpression();
                        SkipWhitespace();
                        if (pos >= expr.Length || expr[pos] != ')')
                            throw new Exception("Missing closing parenthesis");
                        pos++; // consume ')'
                        return value;
                    }
                    return ParseNumber();
                }
                double ParseTerm() {
                    double value = ParseFactor();
                    SkipWhitespace();
                    while (pos < expr.Length && (expr[pos] == '*' || expr[pos] == '/')) {
                        char op = expr[pos];
                        pos++;
                        double next = ParseFactor();
                        if (op == '*')
                            value *= next;
                        else {
                            if (next == 0)
                                throw new Exception("Division by zero");
                            value /= next;
                        }
                        SkipWhitespace();
                    }
                    return value;
                }
                double ParseExpression() {
                    double value = ParseTerm();
                    SkipWhitespace();
                    while (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-')) {
                        char op = expr[pos];
                        pos++;
                        double next = ParseTerm();
                        value = op == '+' ? value + next : value - next;
                        SkipWhitespace();
                    }
                    return value;
                }
                double evaluated = ParseExpression();
                SkipWhitespace();
                if (pos != expr.Length)
                    throw new Exception("Invalid characters in expression");
                result = evaluated;
                return true;
            } catch {
                result = 0;
                return false;
            }
        }

        private bool IsHexDigit(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f');
        }

        private int HexValue(char c) {
            if (c >= '0' && c <= '9') return c - '0';
            return c - 'a' + 10;
        }
    }
}

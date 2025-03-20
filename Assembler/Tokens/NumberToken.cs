using Assembler._Infrastructure;
using Assembler.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.Tokens {
    class NumberToken : TokenBase<ushort> {
        private class UnresolvedExpression {
            public ushort? ResolvedValue { get; set; }
            public string Expression { get; set; }
            public List<IdentifierToken> UnresolvedIdentifiers { get; set; } = new();
            public bool UseFloating { get; set; }

            public void AddUnresolvedIdentifier(IdentifierToken identifierToken) {
                if (!UnresolvedIdentifiers.Any(x => x.Value == identifierToken.Value)) {
                    UnresolvedIdentifiers.Add(identifierToken);
                }
            }
        }

        private UnresolvedExpression _unresolvedExpression = null;
        private ushort value;

        public NumberToken() { }

        public NumberToken(ushort value, int position, int line, string filePath) {
            Value = value;
            AddDebugData(position, line, filePath);
        }

        public override NumberToken DeepCopy() {
            var clone = new NumberToken(value, CodePosition, LineNumber, FilePath);
            if (_unresolvedExpression != null) {
                clone._unresolvedExpression = _unresolvedExpression;
            }
            return clone;
        }

        public override ushort Value {
            get {
                if (!IsResolved) {
                    NumberTokenResolveRescue.ResolveOrThrow(this);
                }

                if (!IsResolved) {
                    throw new Exception("That should not happen");
                }

                if (_unresolvedExpression != null) {
                    return _unresolvedExpression.ResolvedValue.Value;
                }

                return value;
            }

            protected set {
                this.value = value;
            }
        }

        public bool IsResolved => _unresolvedExpression == null || !_unresolvedExpression.UnresolvedIdentifiers.Any();
        public override TokenClass Class => TokenClass.Number;

        public bool TryResolve(Func<string, NumberToken> getNumberToken, HashSet<NumberToken> visitedNumberTokens = null) {
            if (IsResolved) {
                //if (_unresolvedExpression != null) {
                //    _unresolvedExpression.ResolvedValue = value;
                //}
                return true;
            }

            if (visitedNumberTokens == null) {
                visitedNumberTokens = new();
            }

            if (visitedNumberTokens.Contains(this)) {
                throw ParserException.Create($"Cyclic dependecy on number token detected: {_unresolvedExpression.Expression}", this);
            }

            visitedNumberTokens.Add(this);

            List<IdentifierToken> resolvedIdentifiers = new();
            var exprCopy = _unresolvedExpression.Expression;
            foreach (var identifierToken in _unresolvedExpression.UnresolvedIdentifiers) {
                var numberToken = getNumberToken(identifierToken.Value);

                if (numberToken.TryResolve(getNumberToken, visitedNumberTokens)) {
                    exprCopy = exprCopy.Replace(identifierToken.Value, numberToken.Value.ToString());
                    resolvedIdentifiers.Add(identifierToken);
                }
            }

            _unresolvedExpression.UnresolvedIdentifiers =
                _unresolvedExpression.UnresolvedIdentifiers
                .Except(resolvedIdentifiers)
                .ToList();

            if (_unresolvedExpression.UnresolvedIdentifiers.Any()) {
                return false;
            }

            var exprLc = exprCopy.ToLower();

            if (_unresolvedExpression.UseFloating) {
                if (TryEvaluateExpressionFloat(exprLc, out double floatResult)) {
                    value = (ushort)floatResult;
                    _unresolvedExpression.ResolvedValue = value;
                    if (floatResult < short.MinValue || floatResult > ushort.MaxValue)
                        throw ParserException.Create($"Expression value out of range: {_unresolvedExpression.Expression} = {floatResult:0.00}", this);
                    return true;
                }
                _unresolvedExpression.ResolvedValue = 0;
                throw ParserException.Create($"Unresolvable expression: {_unresolvedExpression.Expression}", this);

            } else {
                if (TryEvaluateExpression(exprLc, out int evaluated)) {
                    value = (ushort)evaluated;
                    _unresolvedExpression.ResolvedValue = value;
                    if (evaluated < short.MinValue || evaluated > ushort.MaxValue)
                        throw ParserException.Create($"Expression value out of range: {_unresolvedExpression.Expression} = {evaluated}", this);
                    return true;
                }
                _unresolvedExpression.ResolvedValue = 0;
                throw ParserException.Create($"Unresolvable expression: {_unresolvedExpression.Expression}", this);
            }
        }

        public override bool TryAccept(CodeReader reader) {
            // Check if we have an expression enclosed in { }
            if (reader.Current == '{') {
                // Consume the '{'
                reader.Read();
                var exprBuilder = new StringBuilder();
                var unresolvedExprBuilder = new StringBuilder();
                while (reader.Current != '}' && reader.Current != '\0') {
                    if (char.IsLetter(reader.Current) || reader.Current == '@') {
                        // identifier found, read it.
                        var identifierToken = new IdentifierToken();
                        identifierToken.AddDebugData(reader.Position, reader.Line, reader.FilePath);
                        if (identifierToken.TryAccept(reader)) {
                            _unresolvedExpression ??= new();
                            _unresolvedExpression.AddUnresolvedIdentifier(identifierToken);
                            unresolvedExprBuilder.Append(identifierToken.Value);
                        }
                    } else {
                        unresolvedExprBuilder.Append(reader.Current);
                    }
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

                if (_unresolvedExpression != null) {
                    _unresolvedExpression.Expression = unresolvedExprBuilder.ToString();
                    _unresolvedExpression.UseFloating = useFloating;
                    Value = 0;
                    return true;
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

        public override string ToString() {
            if (!IsResolved) {
                return _unresolvedExpression.Expression;
            }

            if (_unresolvedExpression?.ResolvedValue != null) {
                return _unresolvedExpression.ResolvedValue.Value.ToString();
            }

            return Value.ToString();
        }
    }
}

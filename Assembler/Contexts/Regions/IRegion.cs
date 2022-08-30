using Assembler.Contexts.Labels;
using Assembler.Tokens;

namespace Assembler.Contexts.Regions {
    internal interface IRegion {
        string Name { get; }
        bool IsReserved { get; }
        LabelInfo GetLabel(string labelName);
        void InsertToken(string name, IToken token);
        TokenInfo GetToken(string tokenName);
    }
}

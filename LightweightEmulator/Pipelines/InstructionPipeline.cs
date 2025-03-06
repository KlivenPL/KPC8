using LightweightEmulator.Kpc;

namespace LightweightEmulator.Pipelines {
    internal class InstructionPipeline {
        public void Execute(LwKpcBuild kpc, LightweightInstruction instrReg, ImmediateInstruction instrImm) {

            var regDest = kpc.ProgrammerRegisters[instrReg.RegDestIndex];
            var regA = kpc.ProgrammerRegisters[instrReg.RegAIndex];
            var regB = kpc.ProgrammerRegisters[instrReg.RegBIndex];
            var imm = instrImm.ImmediateValue;

            switch (instrReg.Type) {
                case KpcInstructionType.Nop:
                    break;
                case KpcInstructionType.Lbrom:
                    regA.LowValue = kpc.Rom.ReadByte(regB.WordValue);
                    break;
                case KpcInstructionType.Lbromo:
                    regDest.LowValue = kpc.Rom.ReadByte((ushort)(regA.WordValue + regB.WordValue));
                    break;
                case KpcInstructionType.Lwrom:
                    regA.WordValue = kpc.Rom.ReadWord(regB.WordValue);
                    break;
                case KpcInstructionType.Lwromo:
                    regDest.WordValue = kpc.Rom.ReadWord((ushort)(regA.WordValue + regB.WordValue));
                    break;
                case KpcInstructionType.Lbram:
                    regA.LowValue = kpc.Ram.ReadByte(regB.WordValue);
                    break;
                case KpcInstructionType.Lbramo:
                    regDest.LowValue = kpc.Ram.ReadByte((ushort)(regA.WordValue + regB.WordValue));
                    break;
                case KpcInstructionType.Lwram:
                    regA.WordValue = kpc.Ram.ReadWord(regB.WordValue);
                    break;
                case KpcInstructionType.Lwramo:
                    regDest.WordValue = kpc.Ram.ReadWord((ushort)(regA.WordValue + regB.WordValue));
                    break;
                case KpcInstructionType.Popb:
                    regA.LowValue = kpc.Ram.ReadByte(--regB.WordValue);
                    break;
                case KpcInstructionType.Popw:
                    regA.LowValue = kpc.Ram.ReadByte(--regB.WordValue);
                    regA.HighValue = kpc.Ram.ReadByte(--regB.WordValue);
                    break;
                case KpcInstructionType.Lbext:
                    break;
                case KpcInstructionType.Sbram:
                    kpc.Ram.WriteByte(regA.LowValue, regB.WordValue);
                    break;
                case KpcInstructionType.SbramI:
                    kpc.Ram.WriteByte(imm, regDest.WordValue);
                    break;
                case KpcInstructionType.Sbramo:
                    kpc.Ram.WriteByte(regDest.LowValue, (ushort)(regA.WordValue + regB.WordValue));
                    break;
                case KpcInstructionType.Swram:
                    kpc.Ram.WriteWord(regA.WordValue, regB.WordValue);
                    break;
                case KpcInstructionType.Swramo:
                    kpc.Ram.WriteWord(regDest.WordValue, (ushort)(regA.WordValue + regB.WordValue));
                    break;
                case KpcInstructionType.Pushb:
                    kpc.Ram.WriteByte(regA.LowValue, regB.WordValue++);
                    break;
                case KpcInstructionType.Pushw:
                    kpc.Ram.WriteByte(regA.HighValue, regB.WordValue++);
                    kpc.Ram.WriteByte(regA.LowValue, regB.WordValue++);
                    break;
                case KpcInstructionType.Sbext:
                    break;
                case KpcInstructionType.Add:
                    break;
                case KpcInstructionType.AddI:
                    break;
                case KpcInstructionType.Sub:
                    break;
                case KpcInstructionType.SubI:
                    break;
                case KpcInstructionType.Addw:
                    break;
                case KpcInstructionType.Negw:
                    break;
                case KpcInstructionType.Not:
                    break;
                case KpcInstructionType.Or:
                    break;
                case KpcInstructionType.And:
                    break;
                case KpcInstructionType.Xor:
                    break;
                case KpcInstructionType.Sll:
                    break;
                case KpcInstructionType.Srl:
                    break;
                case KpcInstructionType.Set:
                    break;
                case KpcInstructionType.SetI:
                    break;
                case KpcInstructionType.Seth:
                    break;
                case KpcInstructionType.SethI:
                    break;
                case KpcInstructionType.Setw:
                    break;
                case KpcInstructionType.Setloh:
                    break;
                case KpcInstructionType.Swap:
                    break;
                case KpcInstructionType.Swaph:
                    break;
                case KpcInstructionType.Swapw:
                    break;
                case KpcInstructionType.Swaploh:
                    break;
                case KpcInstructionType.Jr:
                    break;
                case KpcInstructionType.Jro:
                    break;
                case KpcInstructionType.Jas:
                    break;
                case KpcInstructionType.JpcaddI:
                    break;
                case KpcInstructionType.JpcsubI:
                    break;
                case KpcInstructionType.Irrex:
                    break;
                case KpcInstructionType.Irrret:
                    break;
                case KpcInstructionType.Irren:
                    break;
                case KpcInstructionType.Irrdis:
                    break;
                case KpcInstructionType.Jwz:
                    break;
                case KpcInstructionType.Jwnotz:
                    break;
                case KpcInstructionType.Jwn:
                    break;
                case KpcInstructionType.Jwnotn:
                    break;
                case KpcInstructionType.Jzf:
                    break;
                case KpcInstructionType.Jnf:
                    break;
                case KpcInstructionType.Jcf:
                    break;
                case KpcInstructionType.Jof:
                    break;
                default:
                    break;
            }

        }
    }
}

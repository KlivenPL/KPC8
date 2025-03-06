using LightweightEmulator.Components;
using LightweightEmulator.Kpc;

namespace LightweightEmulator.Pipelines {
    internal class LwInstructionProcessor {
        public void Execute(LwKpcBuild kpc, LightweightInstruction lwInstr) {
            Register16 regDest = kpc.ProgrammerRegisters[lwInstr.RegDestIndex];
            Register16 regA = kpc.ProgrammerRegisters[lwInstr.RegAIndex];
            Register16 regB = kpc.ProgrammerRegisters[lwInstr.RegBIndex];
            byte imm = lwInstr.ImmediateValue;
            Register4 flags = kpc.Flags;
            Register16 pc = kpc.Pc;
            Register16 mar = kpc.Mar;

            mar.WordValue = kpc.Pc.WordValue;
            pc.WordValue += 2;
            mar.WordValue += 1;

            switch (lwInstr.Type) {
                case KpcInstructionType.Nop:
                    ExecuteNop();
                    break;

                // --- Load instructions ---

                case KpcInstructionType.Lbrom:
                    ExecuteLbrom(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.Lbromo:
                    ExecuteLbromo(kpc, regDest, regA, regB, mar, flags);
                    break;

                case KpcInstructionType.Lwrom:
                    ExecuteLwrom(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.Lwromo:
                    ExecuteLwromo(kpc, regDest, regA, regB, mar, flags);
                    break;

                case KpcInstructionType.Lbram:
                    ExecuteLbram(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.Lbramo:
                    ExecuteLbramo(kpc, regDest, regA, regB, mar, flags);
                    break;

                case KpcInstructionType.Lwram:
                    ExecuteLwram(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.Lwramo:
                    ExecuteLwramo(kpc, regDest, regA, regB, mar, flags);
                    break;

                case KpcInstructionType.Popb:
                    ExecutePopb(kpc, regA, regB, mar, flags);
                    break;

                case KpcInstructionType.Popw:
                    ExecutePopw(kpc, regA, regB, mar, flags);
                    break;

                // --- Store instructions ---

                case KpcInstructionType.Sbram:
                    ExecuteSbram(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.SbramI:
                    ExecuteSbramI(kpc, regDest, imm, mar);
                    break;

                case KpcInstructionType.Sbramo:
                    ExecuteSbramo(kpc, regDest, regA, regB, mar, flags);
                    break;

                case KpcInstructionType.Swram:
                    ExecuteSwram(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.Swramo:
                    ExecuteSwramo(kpc, regDest, regA, regB, mar, flags);
                    break;

                case KpcInstructionType.Pushb:
                    ExecutePushb(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.Pushw:
                    ExecutePushw(kpc, regA, regB, mar);
                    break;

                case KpcInstructionType.Sbext:
                    ExecuteSbext();
                    break;

                // --- Regs instructions ---

                case KpcInstructionType.Set:
                    ExecuteSet(regA, regB);
                    break;

                case KpcInstructionType.SetI:
                    ExecuteSetI(regDest, imm);
                    break;

                case KpcInstructionType.Seth:
                    ExecuteSeth(regA, regB);
                    break;

                case KpcInstructionType.SethI:
                    ExecuteSethI(regDest, imm);
                    break;

                case KpcInstructionType.Setw:
                    ExecuteSetw(regA, regB);
                    break;

                case KpcInstructionType.Setloh:
                    ExecuteSetloh(regA, regB);
                    break;

                case KpcInstructionType.Swap:
                    ExecuteSwap(regA, regB);
                    break;

                case KpcInstructionType.Swaph:
                    ExecuteSwaph(regA, regB);
                    break;

                case KpcInstructionType.Swapw:
                    ExecuteSwapw(regA, regB);
                    break;

                case KpcInstructionType.Swaploh:
                    ExecuteSwaploh(regA, regB);
                    break;

                // --- Math Instructions ---

                case KpcInstructionType.Add:
                    ExecuteAdd(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.AddI:
                    ExecuteAddI(regDest, imm, flags);
                    break;

                case KpcInstructionType.Sub:
                    ExecuteSub(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.SubI:
                    ExecuteSubI(regDest, imm, flags);
                    break;

                case KpcInstructionType.Addw:
                    ExecuteAddw(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.Negw:
                    ExecuteNegw(regA, regB, flags);
                    break;

                // --- Logic instructions ---

                case KpcInstructionType.Not:
                    ExecuteNot(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.Or:
                    ExecuteOr(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.And:
                    ExecuteAnd(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.Xor:
                    ExecuteXor(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.Sll:
                    ExecuteSll(regDest, regA, regB, flags);
                    break;

                case KpcInstructionType.Srl:
                    ExecuteSrl(regDest, regA, regB, flags);
                    break;

                // --- Jump procedural instructions ---

                case KpcInstructionType.Jr:
                    ExecuteJr(regB, pc);
                    break;

                case KpcInstructionType.Jro:
                    ExecuteJro(regA, regB, pc, flags);
                    break;

                case KpcInstructionType.Jas:
                    ExecuteJas(regA, regB, pc);
                    break;

                case KpcInstructionType.JpcaddI:
                    ExecuteJpcaddI(imm, pc, mar, flags);
                    break;

                case KpcInstructionType.JpcsubI:
                    ExecuteJpcsubI(imm, pc, mar, flags);
                    break;

                // --- Jump conditional instructions ---

                case KpcInstructionType.Jwz:
                    ExecuteJwz(regA, regB, pc, flags);
                    break;

                case KpcInstructionType.Jwnotz:
                    ExecuteJwnotz(regA, regB, pc, flags);
                    break;

                case KpcInstructionType.Jwn:
                    ExecuteJwn(regA, regB, pc, flags);
                    break;

                case KpcInstructionType.Jwnotn:
                    ExecuteJwnotn(regA, regB, pc, flags);
                    break;

                case KpcInstructionType.Jzf:
                    ExecuteJzf(regB, pc, flags);
                    break;

                case KpcInstructionType.Jnf:
                    ExecuteJnf(regB, pc, flags);
                    break;

                case KpcInstructionType.Jcf:
                    ExecuteJcf(regB, pc, flags);
                    break;

                case KpcInstructionType.Jof:
                    ExecuteJof(regB, pc, flags);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void ExecuteNop() {
            // No operation.
        }

        #region Load instructions

        private void ExecuteLbrom(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            regA.LowValue = kpc.Rom.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteLbromo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.LowValue = kpc.Rom.ReadByte(tmpReg.WordValue);
            mar.WordValue = tmpReg.WordValue;
        }

        private void ExecuteLwrom(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            regA.WordValue = kpc.Rom.ReadWord(regB.WordValue);
            mar.WordValue = (ushort)(regB.WordValue + 1);
        }

        private void ExecuteLwromo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.WordValue = kpc.Rom.ReadWord(tmpReg.WordValue);
            mar.WordValue = (ushort)(tmpReg.WordValue + 1);
        }

        private void ExecuteLbram(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            regA.LowValue = kpc.Ram.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteLbramo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.LowValue = kpc.Ram.ReadByte(tmpReg.WordValue);
            mar.WordValue = tmpReg.WordValue;
        }

        private void ExecuteLwram(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            regA.WordValue = kpc.Ram.ReadWord(regB.WordValue);
            mar.WordValue = (ushort)(regB.WordValue + 1);
        }

        private void ExecuteLwramo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.WordValue = kpc.Ram.ReadWord(tmpReg.WordValue);
            mar.WordValue = (ushort)(tmpReg.WordValue + 1);
        }

        private void ExecutePopb(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            Register16 minusOneReg = new(-1);
            ExecuteAddw(tmpReg, regB, minusOneReg, flags);
            regB.WordValue--;
            regA.LowValue = kpc.Ram.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecutePopw(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            Register16 minusTwoReg = new(-2);
            ExecuteAddw(tmpReg, regB, minusTwoReg, flags);
            regB.WordValue--;
            regA.LowValue = kpc.Ram.ReadByte(regB.WordValue);
            regB.WordValue--;
            regA.HighValue = kpc.Ram.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        #endregion

        #region Store instructions

        private void ExecuteSbram(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            kpc.Ram.WriteByte(regA.LowValue, regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteSbramI(LwKpcBuild kpc, Register16 regDest, byte imm, Register16 mar) {
            kpc.Ram.WriteByte(imm, regDest.WordValue);
            mar.WordValue = regDest.WordValue;
        }

        private void ExecuteSbramo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            kpc.Ram.WriteByte(regDest.LowValue, tmpReg.WordValue);
            mar.WordValue = tmpReg.WordValue;
        }

        private void ExecuteSwram(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            kpc.Ram.WriteWord(regA.WordValue, regB.WordValue);
            mar.WordValue = (ushort)(regB.WordValue + 1);
        }

        private void ExecuteSwramo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            kpc.Ram.WriteWord(regDest.WordValue, tmpReg.WordValue);
            mar.WordValue = (ushort)(tmpReg.WordValue + 1);
        }

        private void ExecutePushb(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            kpc.Ram.WriteByte(regA.LowValue, regB.WordValue++);
            mar.WordValue = regB.WordValue;
        }

        private void ExecutePushw(LwKpcBuild kpc, Register16 regA, Register16 regB, Register16 mar) {
            kpc.Ram.WriteByte(regA.HighValue, regB.WordValue++);
            kpc.Ram.WriteByte(regA.LowValue, regB.WordValue++);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteSbext() {
            throw new NotImplementedException();
        }

        #endregion

        #region Regs instructions

        private void ExecuteSet(Register16 regA, Register16 regB) {
            regA.LowValue = regB.LowValue;
        }

        private void ExecuteSetI(Register16 regDest, byte imm) {
            regDest.LowValue = imm;
        }

        private void ExecuteSeth(Register16 regA, Register16 regB) {
            regA.HighValue = regB.HighValue;
        }

        private void ExecuteSethI(Register16 regA, byte imm) {
            regA.HighValue = imm;
        }

        private void ExecuteSetw(Register16 regA, Register16 regB) {
            regA.LowValue = regB.LowValue;
            regA.HighValue = regB.HighValue;
        }

        private void ExecuteSetloh(Register16 regA, Register16 regB) {
            Register16 regBCopy = new Register16 {
                WordValue = regB.WordValue
            };

            regA.LowValue = regBCopy.HighValue;
            regA.HighValue = regBCopy.LowValue;
        }

        private void ExecuteSwap(Register16 regA, Register16 regB) {
            byte temp = regA.LowValue;
            regA.LowValue = regB.LowValue;
            regB.LowValue = temp;
        }

        private void ExecuteSwaph(Register16 regA, Register16 regB) {
            byte temp = regA.HighValue;
            regA.HighValue = regB.HighValue;
            regB.HighValue = temp;
        }

        private void ExecuteSwapw(Register16 regA, Register16 regB) {
            byte tempLow = regA.LowValue;
            regA.LowValue = regB.LowValue;
            regB.LowValue = tempLow;

            byte tempHigh = regA.HighValue;
            regA.HighValue = regB.HighValue;
            regB.HighValue = tempHigh;
        }

        private void ExecuteSwaploh(Register16 regA, Register16 regB) {
            byte tempALow = regA.LowValue;
            byte tempAHigh = regA.HighValue;

            byte tempBLow = regB.LowValue;
            byte tempBHigh = regB.HighValue;

            regA.LowValue = tempBHigh;
            regA.HighValue = tempBLow;

            regB.LowValue = tempAHigh;
            regB.HighValue = tempALow;
        }

        #endregion

        #region Math instructions

        private void ExecuteAdd(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            regDest.LowValue = (byte)(a + b);

            UpdateFlags(flags, false, a, b, regDest.LowValue, false);
        }

        private void ExecuteAddI(Register16 regDest, byte imm, Register4 flags) {
            byte a = regDest.LowValue;

            regDest.LowValue = (byte)(a + imm);

            UpdateFlags(flags, false, a, imm, regDest.LowValue, false);
        }

        private void ExecuteSub(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            regDest.LowValue = (byte)(a - b);

            UpdateFlags(flags, true, a, b, regDest.LowValue, true);
        }

        private void ExecuteSubI(Register16 regDest, byte imm, Register4 flags) {
            byte a = regDest.LowValue;

            regDest.LowValue = (byte)(a - imm);

            UpdateFlags(flags, true, a, imm, regDest.LowValue, true);
        }

        private void ExecuteAddw(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.HighValue;
            byte b = regB.HighValue;

            int low = regA.LowValue + regB.LowValue;
            byte lowResult = (byte)low;
            int carry = low >> 8;
            int high = regA.HighValue + regB.HighValue + carry;

            regDest.LowValue = lowResult;
            regDest.HighValue = (byte)high;

            UpdateFlags(flags, carry > 0, a, b, regDest.HighValue, false);
        }

        private void ExecuteNegw(Register16 regA, Register16 regB, Register4 flags) {
            bool lowCarry = (regB.LowValue + 255) > 255;
            byte a = regB.HighValue;

            ushort value = regB.WordValue;
            ushort negated = (ushort)((~value) + 1);

            regA.WordValue = negated;

            UpdateFlags(flags, lowCarry, a, 255, regA.HighValue, false);
        }

        #endregion

        #region Logic instructions

        private void ExecuteNot(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            regDest.LowValue = (byte)~(a + b);

            UpdateFlags(flags, false, a, b, regDest.LowValue, false);
        }

        private void ExecuteOr(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            regDest.LowValue = (byte)(a | b);

            UpdateFlags(flags, false, a, b, regDest.LowValue, false, true);
        }

        private void ExecuteAnd(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            regDest.LowValue = (byte)(a & b);

            UpdateFlags(flags, false, a, b, regDest.LowValue, false, true);
        }

        private void ExecuteXor(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            regDest.LowValue = (byte)(a ^ b);

            UpdateFlags(flags, false, a, b, regDest.LowValue, false, true);
        }

        private void ExecuteSll(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            regDest.LowValue = (byte)((a + b) << 1);

            UpdateFlags(flags, false, a, b, regDest.LowValue, false, false);
        }

        private void ExecuteSrl(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            byte a = regA.LowValue;
            byte b = regB.LowValue;

            uint combined = (byte)(regA.LowValue + regB.LowValue);
            regDest.LowValue = (byte)(combined >> 1);

            UpdateFlags(flags, false, a, b, regDest.LowValue, false, false);
        }

        #endregion

        #region Jump procedural instructions

        private void ExecuteJr(Register16 regB, Register16 pc) {
            pc.WordValue = regB.WordValue;
        }

        private void ExecuteJro(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            pc.WordValue = tmpReg.WordValue;
        }

        private void ExecuteJas(Register16 regA, Register16 regB, Register16 pc) {
            regB.WordValue = pc.WordValue;
            pc.WordValue = regA.WordValue;
        }

        private void ExecuteJpcaddI(byte imm, Register16 pc, Register16 mar, Register4 flags) {
            Register16 immReg = new(imm);
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, pc, immReg, flags);
            pc.WordValue = tmpReg.WordValue;
            mar.WordValue = 0;
        }

        private void ExecuteJpcsubI(byte imm, Register16 pc, Register16 mar, Register4 flags) {
            Register16 immReg = new(-imm);
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, pc, immReg, flags);
            pc.WordValue = tmpReg.WordValue;

            var zero = 0;
            mar.WordValue = (ushort)(zero - 1);
        }

        #endregion

        #region Jump conditional instructions
        private void ExecuteJwz(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            Register16 regALowReg = new(regA.LowValue);
            Register16 regAHighReg = new(regA.HighValue);

            ExecuteOr(tmpReg, regALowReg, regAHighReg, flags);

            var kpcFlags = (KpcFlag)flags.Value;
            if (kpcFlags.HasFlag(KpcFlag.Zf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJwnotz(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            Register16 regALowReg = new(regA.LowValue);
            Register16 regAHighReg = new(regA.HighValue);

            ExecuteOr(tmpReg, regALowReg, regAHighReg, flags);

            var kpcFlags = (KpcFlag)flags.Value;
            if (!kpcFlags.HasFlag(KpcFlag.Zf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJwn(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            Register16 regAHighReg = new(regA.HighValue);

            ExecuteOr(tmpReg, tmpReg, regAHighReg, flags);

            var kpcFlags = (KpcFlag)flags.Value;
            if (kpcFlags.HasFlag(KpcFlag.Nf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJwnotn(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            Register16 regAHighReg = new(regA.HighValue);

            ExecuteOr(tmpReg, tmpReg, regAHighReg, flags);

            var kpcFlags = (KpcFlag)flags.Value;
            if (!kpcFlags.HasFlag(KpcFlag.Nf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJzf(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (KpcFlag)flags.Value;
            if (kpcFlags.HasFlag(KpcFlag.Zf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJnf(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (KpcFlag)flags.Value;
            if (kpcFlags.HasFlag(KpcFlag.Nf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJcf(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (KpcFlag)flags.Value;
            if (kpcFlags.HasFlag(KpcFlag.Cf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJof(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (KpcFlag)flags.Value;
            if (kpcFlags.HasFlag(KpcFlag.Of)) {
                pc.WordValue = regB.WordValue;
            }
        }

        #endregion

        private void UpdateFlags(
            Register4 regFlags,
            bool carryIn,
            byte a,
            byte b,
            byte operationResult,
            bool isSubtraction,
            bool skipCf = false) {
            KpcFlag calcFlag = KpcFlag.None;

            if (operationResult == 0) {
                calcFlag |= KpcFlag.Zf;
            }

            if ((operationResult & 0b10000000) != 0) {
                calcFlag |= KpcFlag.Nf;
            }

            if (!skipCf) {
                int sum = a + (isSubtraction ? (byte)~b : b) + (carryIn ? 1 : 0);

                if (sum > 255) {
                    calcFlag |= KpcFlag.Cf;
                }
            }

            if (isSubtraction) {
                if ((((a ^ b) & 0x80) != 0) && (((a ^ operationResult) & 0x80) != 0)) {
                    calcFlag |= KpcFlag.Of;
                }
            } else {
                if ((((a ^ b) & 0x80) == 0) && (((a ^ operationResult) & 0x80) != 0)) {
                    calcFlag |= KpcFlag.Of;
                }
            }

            regFlags.Value = (byte)calcFlag;
        }
    }
}

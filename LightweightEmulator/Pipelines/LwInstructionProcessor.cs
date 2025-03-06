using LightweightEmulator.Components;
using LightweightEmulator.Kpc;

namespace LightweightEmulator.Pipelines {
    internal class LwInstructionProcessor {
        // Define the index for the T1 register (adjust as appropriate).
        private const int T1_INDEX = 7;

        public void Execute(LwKpcBuild kpc, LightweightInstruction lwInstr) {
            var regDest = kpc.ProgrammerRegisters[lwInstr.RegDestIndex];
            var regA = kpc.ProgrammerRegisters[lwInstr.RegAIndex];
            var regB = kpc.ProgrammerRegisters[lwInstr.RegBIndex];
            var imm = lwInstr.ImmediateValue;
            var flags = kpc.Flags;

            switch (lwInstr.Type) {
                // --- Memory & Data Movement Operations ---
                case KpcInstructionType.Nop:
                    break;
                case KpcInstructionType.Lbrom:
                    ExecuteLbrom(kpc, regA, regB);
                    break;
                case KpcInstructionType.Lbromo:
                    ExecuteLbromo(kpc, regDest, regA, regB);
                    break;
                case KpcInstructionType.Lwrom:
                    ExecuteLwrom(kpc, regA, regB);
                    break;
                case KpcInstructionType.Lwromo:
                    ExecuteLwromo(kpc, regDest, regA, regB);
                    break;
                case KpcInstructionType.Lbram:
                    ExecuteLbram(kpc, regA, regB);
                    break;
                case KpcInstructionType.Lbramo:
                    ExecuteLbramo(kpc, regDest, regA, regB);
                    break;
                case KpcInstructionType.Lwram:
                    ExecuteLwram(kpc, regA, regB);
                    break;
                case KpcInstructionType.Lwramo:
                    ExecuteLwramo(kpc, regDest, regA, regB);
                    break;
                case KpcInstructionType.Popb:
                    ExecutePopb(kpc, regA, regB);
                    break;
                case KpcInstructionType.Popw:
                    ExecutePopw(kpc, regA, regB);
                    break;
                case KpcInstructionType.Sbram:
                    kpc.Ram.WriteByte(regA.LowValue, regB.WordValue);
                    break;
                case KpcInstructionType.SbramI:
                    kpc.Ram.WriteByte((byte)imm, regDest.WordValue);
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
                    throw new NotImplementedException();
                    break;

                // --- Register Set Operations ---
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

                // --- Math Operations ---
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

                // --- Logic Operations ---
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

                //// --- Load Operations ---
                //case KpcInstructionType.Lbext:
                //    ExecuteLbext(kpc, regA);
                //    break;

                //// --- Jump Operations ---
                //case KpcInstructionType.Jr:
                //    ExecuteJr(kpc, regB);
                //    break;
                //case KpcInstructionType.Jro:
                //    ExecuteJro(kpc, regA, regB);
                //    break;
                //case KpcInstructionType.Jas:
                //    ExecuteJas(kpc, regA, regB);
                //    break;
                //case KpcInstructionType.JpcaddI:
                //    ExecuteJpcaddI(kpc, imm);
                //    break;
                //case KpcInstructionType.JpcsubI:
                //    ExecuteJpcsubI(kpc, imm);
                //    break;

                //// --- Conditional Jump Operations ---
                //case KpcInstructionType.Jwz:
                //    ExecuteJwz(kpc, regB, flags);
                //    break;
                //case KpcInstructionType.Jwnotz:
                //    ExecuteJwnotz(kpc, regB, flags);
                //    break;
                //case KpcInstructionType.Jwn:
                //    ExecuteJwn(kpc, regB, flags);
                //    break;
                //case KpcInstructionType.Jwnotn:
                //    ExecuteJwnotn(kpc, regB, flags);
                //    break;
                //case KpcInstructionType.Jzf:
                //    ExecuteJzf(kpc, regB, flags);
                //    break;
                //case KpcInstructionType.Jnf:
                //    ExecuteJnf(kpc, regB, flags);
                //    break;
                //case KpcInstructionType.Jcf:
                //    ExecuteJcf(kpc, regB, flags);
                //    break;
                //case KpcInstructionType.Jof:
                //    ExecuteJof(kpc, regB, flags);
                //    break;

                //// --- Interrupt Operations ---
                //case KpcInstructionType.Irrex:
                //    ExecuteIrrex(kpc, regB, imm);
                //    break;
                //case KpcInstructionType.Irrret:
                //    ExecuteIrrret(kpc);
                //    break;
                //case KpcInstructionType.Irren:
                //    kpc.InterruptEnabled = true;
                //    break;
                //case KpcInstructionType.Irrdis:
                //    kpc.InterruptEnabled = false;
                //    break;

                default:
                    throw new NotImplementedException();
            }

            kpc.Pc.WordValue += 2;
        }

        #region Memory & Data Movement Methods
        private void ExecuteLbrom(LwKpcBuild kpc, Register16 regA, Register16 regB) {
            regA.LowValue = kpc.Rom.ReadByte(regB.WordValue);
        }

        private void ExecuteLbromo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB) {
            regDest.LowValue = kpc.Rom.ReadByte((ushort)(regA.WordValue + regB.WordValue));
        }

        private void ExecuteLwrom(LwKpcBuild kpc, Register16 regA, Register16 regB) {
            regA.WordValue = kpc.Rom.ReadWord(regB.WordValue);
        }

        private void ExecuteLwromo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB) {
            regDest.WordValue = kpc.Rom.ReadWord((ushort)(regA.WordValue + regB.WordValue));
        }

        private void ExecuteLbram(LwKpcBuild kpc, Register16 regA, Register16 regB) {
            regA.LowValue = kpc.Ram.ReadByte(regB.WordValue);
        }

        private void ExecuteLbramo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB) {
            regDest.LowValue = kpc.Ram.ReadByte((ushort)(regA.WordValue + regB.WordValue));
        }

        private void ExecuteLwram(LwKpcBuild kpc, Register16 regA, Register16 regB) {
            regA.WordValue = kpc.Ram.ReadWord(regB.WordValue);
        }

        private void ExecuteLwramo(LwKpcBuild kpc, Register16 regDest, Register16 regA, Register16 regB) {
            regDest.WordValue = kpc.Ram.ReadWord((ushort)(regA.WordValue + regB.WordValue));
        }

        private void ExecutePopb(LwKpcBuild kpc, Register16 regA, Register16 regB) {
            regA.LowValue = kpc.Ram.ReadByte(--regB.WordValue);
        }

        private void ExecutePopw(LwKpcBuild kpc, Register16 regA, Register16 regB) {
            regA.LowValue = kpc.Ram.ReadByte(--regB.WordValue);
            regA.HighValue = kpc.Ram.ReadByte(--regB.WordValue);
        }

        //private void ExecuteLbext(KpcBuild kpc, Register16 regA) {
        //    regA.LowValue = kpc.External.ReadByte();
        //}
        #endregion

        #region Register Set Methods
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
            var regBCopy = new Register16 { WordValue = regB.WordValue };
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

        #region Math Methods
        private void ExecuteAdd(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)(regA.LowValue + regB.LowValue);
            UpdateFlags(flags, false, a, b, regDest.LowValue, false);
        }

        private void ExecuteAddI(Register16 regDest, byte imm, Register4 flags) {
            var a = regDest.LowValue;

            regDest.LowValue = (byte)(regDest.LowValue + imm);
            UpdateFlags(flags, false, a, imm, regDest.LowValue, false);
        }

        private void ExecuteSub(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)(regA.LowValue - regB.LowValue);
            UpdateFlags(flags, true, a, b, regDest.LowValue, true);
        }

        private void ExecuteSubI(Register16 regDest, byte imm, Register4 flags) {
            var a = regDest.LowValue;

            regDest.LowValue = (byte)(regDest.LowValue - imm);
            UpdateFlags(flags, true, a, imm, regDest.LowValue, true);
        }

        private void ExecuteAddw(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.HighValue;
            var b = regB.HighValue;

            int low = regA.LowValue + regB.LowValue;
            byte lowResult = (byte)low;
            int carry = low >> 8;
            int high = regA.HighValue + regB.HighValue + carry;
            regDest.LowValue = lowResult;
            regDest.HighValue = (byte)high;

            UpdateFlags(flags, carry > 0, a, b, regDest.HighValue, false);
        }

        private void ExecuteNegw(Register16 regA, Register16 regB, Register4 flags) {
            var lowCarry = (regB.LowValue + 255) > 255;
            var a = regB.HighValue;
            ushort value = regB.WordValue;
            ushort negated = (ushort)((~value) + 1);
            regA.WordValue = negated;

            UpdateFlags(flags, lowCarry, a, 255, regA.HighValue, false);
        }
        #endregion

        #region Logic Methods
        private void ExecuteNot(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)~(regA.LowValue + regB.LowValue);
            UpdateFlags(flags, false, a, b, regDest.LowValue, false);
        }

        private void ExecuteOr(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)(regA.LowValue | regB.LowValue);
            UpdateFlags(flags, false, a, b, regDest.LowValue, false, true);
        }

        private void ExecuteAnd(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)(regA.LowValue & regB.LowValue);
            UpdateFlags(flags, false, a, b, regDest.LowValue, false, true);
        }

        private void ExecuteXor(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)(regA.LowValue ^ regB.LowValue);
            UpdateFlags(flags, false, a, b, regDest.LowValue, false, true);
        }

        private void ExecuteSll(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)((regA.LowValue + regB.LowValue) << 1);
            UpdateFlags(flags, false, a, b, regDest.LowValue, false, false);
        }

        private void ExecuteSrl(Register16 regDest, Register16 regA, Register16 regB, Register4 flags) {
            var a = regA.LowValue;
            var b = regB.LowValue;

            regDest.LowValue = (byte)(((uint)(byte)(regA.LowValue + regB.LowValue)) >> 1);
            UpdateFlags(flags, false, a, b, regDest.LowValue, false, false);
        }
        #endregion

        private void UpdateFlags(Register4 regFlags, bool carryIn, byte a, byte b, byte operationResult, bool isSubtraction, bool skipCf = false) {
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


        //#region Jump Methods
        //private void ExecuteJr(KpcBuild kpc, Register16 regB) {
        //    kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJro(KpcBuild kpc, Register16 regA, Register16 regB) {
        //    int lowSum = regA.LowValue + regB.LowValue;
        //    byte lowResult = (byte)lowSum;
        //    int carry = lowSum >> 8;
        //    int highSum = regA.HighValue + regB.HighValue + carry;
        //    ushort newPc = (ushort)((highSum << 8) | lowResult);
        //    kpc.Pc.WordValue = newPc;
        //}

        //private void ExecuteJas(KpcBuild kpc, Register16 regA, Register16 regB) {
        //    regB.WordValue = kpc.Pc.WordValue;
        //    kpc.Pc.WordValue = regA.WordValue;
        //}

        //private void ExecuteJpcaddI(KpcBuild kpc, int imm) {
        //    int lowSum = kpc.Pc.LowValue + imm;
        //    byte lowResult = (byte)lowSum;
        //    int carry = lowSum >> 8;
        //    int highSum = kpc.Pc.HighValue + carry;
        //    ushort newPc = (ushort)((highSum << 8) | lowResult);
        //    kpc.Pc.WordValue = newPc;
        //}

        //private void ExecuteJpcsubI(KpcBuild kpc, int imm) {
        //    int lowDiff = kpc.Pc.LowValue - imm;
        //    int borrow = 0;
        //    if (lowDiff < 0) {
        //        lowDiff += 256;
        //        borrow = 1;
        //    }
        //    int highDiff = kpc.Pc.HighValue - borrow;
        //    ushort newPc = (ushort)((highDiff << 8) | (lowDiff & 0xFF));
        //    kpc.Pc.WordValue = newPc;
        //}
        //#endregion

        //#region Conditional Jump Methods
        //private void ExecuteJwz(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (flags.HasFlag(CpuFlag.Zf))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJwnotz(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (!flags.HasFlag(CpuFlag.Zf))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJwn(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (flags.HasFlag(CpuFlag.Nf))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJwnotn(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (!flags.HasFlag(CpuFlag.Nf))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJzf(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (flags.HasFlag(CpuFlag.Zf))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJnf(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (flags.HasFlag(CpuFlag.Nf))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJcf(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (flags.HasFlag(CpuFlag.Cf))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}

        //private void ExecuteJof(KpcBuild kpc, Register16 regB, CpuFlag flags) {
        //    if (flags.HasFlag(CpuFlag.Of))
        //        kpc.Pc.WordValue = regB.WordValue;
        //}
        //#endregion

        //#region Interrupt Methods
        //private void ExecuteIrrex(KpcBuild kpc, Register16 regB, int imm) {
        //    kpc.ToggleInterruptAck();

        //    // Set MAR to 0xFF00 and store FLAGS.
        //    kpc.Ram.WriteByte((byte)kpc.Flags, 0xFF00);

        //    // Store (PC - 1) into addresses 0xFF00 (low) and 0xFF01 (high).
        //    ushort pcMinusOne = (ushort)(kpc.Pc.WordValue - 1);
        //    kpc.Ram.WriteByte((byte)(pcMinusOne & 0xFF), 0xFF00);
        //    kpc.Ram.WriteByte((byte)(pcMinusOne >> 8), 0xFF01);

        //    // Store T1 register into 0xFF02 (high) and 0xFF03 (low).
        //    var t1 = kpc.Registers[T1_INDEX];
        //    kpc.Ram.WriteByte(t1.HighValue, 0xFF02);
        //    kpc.Ram.WriteByte(t1.LowValue, 0xFF03);

        //    // Set new PC: high byte from regB.LowValue and low byte from the immediate.
        //    kpc.Pc.HighValue = regB.LowValue;
        //    kpc.Pc.LowValue = (byte)imm;
        //}

        //private void ExecuteIrrret(KpcBuild kpc) {
        //    // Set MAR to 0xFF00 and retrieve stored FLAGS.
        //    byte storedFlags = kpc.Ram.ReadByte(0xFF00);

        //    // Retrieve saved PC from addresses 0xFF00 and 0xFF01.
        //    byte savedPCLow = kpc.Ram.ReadByte(0xFF00);
        //    byte savedPCHigh = kpc.Ram.ReadByte(0xFF01);
        //    ushort savedPC = (ushort)((savedPCHigh << 8) | savedPCLow);

        //    // Subtract 1 from the saved PC.
        //    ushort newPC = (ushort)(savedPC - 1);
        //    kpc.Pc.WordValue = newPC;

        //    // Restore FLAGS.
        //    kpc.Flags = (CpuFlag)storedFlags;

        //    // Restore T1 register from 0xFF02 and 0xFF03.
        //    var t1 = kpc.Registers[T1_INDEX];
        //    t1.HighValue = kpc.Ram.ReadByte(0xFF02);
        //    t1.LowValue = kpc.Ram.ReadByte(0xFF03);

        //    kpc.ToggleInterruptAck();
        //}
        //#endregion
    }
}

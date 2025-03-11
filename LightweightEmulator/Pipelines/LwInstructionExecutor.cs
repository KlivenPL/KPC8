using LightweightEmulator.Components;
using LightweightEmulator.ExternalDevices;
using LightweightEmulator.Kpc;

namespace LightweightEmulator.Pipelines {
    internal class LwInstructionExecutor {
        private readonly LwKpcBuild _kpcBuild;

        public LwInstructionExecutor(
            LwKpcBuild kpcBuild) {

            _kpcBuild = kpcBuild;
        }

        public void Execute(LwInstruction lwInstr) {
            var regDest = _kpcBuild.ProgrammerRegisters[lwInstr.RegDestIndex];
            var regA = _kpcBuild.ProgrammerRegisters[lwInstr.RegAIndex];
            var regB = _kpcBuild.ProgrammerRegisters[lwInstr.RegBIndex];
            var imm = lwInstr.ImmediateValue;
            var flags = _kpcBuild.Flags;
            var pc = _kpcBuild.Pc;
            var mar = _kpcBuild.Mar;
            var rom = _kpcBuild.Rom;
            var ram = _kpcBuild.Ram;
            var extDeviceAdapter = _kpcBuild.ExtDeviceAdapter;
            var irrManager = _kpcBuild.IrrManager;

            mar.WordValue = _kpcBuild.Pc.WordValue;
            pc.WordValue += 2;
            mar.WordValue += 1;

            if (_kpcBuild.IrrManager.ShouldProcessInterrupt(out ushort? irrAddress, out var handleIrrex)) {
                var t1 = _kpcBuild.ProgrammerRegisters[4];
                ExecuteIrrex(irrAddress!.Value, handleIrrex!, t1, ram, pc, mar, flags);
                return;
            }

            switch (lwInstr.Type) {
                case LwInstructionType.Nop:
                    // No operation
                    break;

                // --- Load instructions ---

                case LwInstructionType.Lbrom:
                    ExecuteLbrom(regA, regB, rom, mar);
                    break;

                case LwInstructionType.Lbromo:
                    ExecuteLbromo(regDest, regA, regB, rom, mar, flags);
                    break;

                case LwInstructionType.Lwrom:
                    ExecuteLwrom(regA, regB, rom, mar);
                    break;

                case LwInstructionType.Lwromo:
                    ExecuteLwromo(regDest, regA, regB, mar, rom, flags);
                    break;

                case LwInstructionType.Lbram:
                    ExecuteLbram(regA, regB, ram, mar);
                    break;

                case LwInstructionType.Lbramo:
                    ExecuteLbramo(regDest, regA, regB, ram, mar, flags);
                    break;

                case LwInstructionType.Lwram:
                    ExecuteLwram(regA, regB, ram, mar);
                    break;

                case LwInstructionType.Lwramo:
                    ExecuteLwramo(regDest, regA, regB, ram, mar, flags);
                    break;

                case LwInstructionType.Popb:
                    ExecutePopb(regA, regB, ram, mar, flags);
                    break;

                case LwInstructionType.Popw:
                    ExecutePopw(regA, regB, ram, mar, flags);
                    break;

                case LwInstructionType.Lbext:
                    ExecuteLbext(extDeviceAdapter, regA, regB);
                    break;

                // --- Store instructions ---

                case LwInstructionType.Sbram:
                    ExecuteSbram(regA, regB, ram, mar);
                    break;

                case LwInstructionType.SbramI:
                    ExecuteSbramI(regDest, imm, ram, mar);
                    break;

                case LwInstructionType.Sbramo:
                    ExecuteSbramo(regDest, regA, regB, ram, mar, flags);
                    break;

                case LwInstructionType.Swram:
                    ExecuteSwram(regA, regB, ram, mar);
                    break;

                case LwInstructionType.Swramo:
                    ExecuteSwramo(regDest, regA, regB, ram, mar, flags);
                    break;

                case LwInstructionType.Pushb:
                    ExecutePushb(regA, regB, ram, mar);
                    break;

                case LwInstructionType.Pushw:
                    ExecutePushw(regA, regB, ram, mar);
                    break;

                case LwInstructionType.Sbext:
                    ExecuteSbext(extDeviceAdapter, regA, regB);
                    break;

                // --- Regs instructions ---

                case LwInstructionType.Set:
                    ExecuteSet(regA, regB);
                    break;

                case LwInstructionType.SetI:
                    ExecuteSetI(regDest, imm);
                    break;

                case LwInstructionType.Seth:
                    ExecuteSeth(regA, regB);
                    break;

                case LwInstructionType.SethI:
                    ExecuteSethI(regDest, imm);
                    break;

                case LwInstructionType.Setw:
                    ExecuteSetw(regA, regB);
                    break;

                case LwInstructionType.Setloh:
                    ExecuteSetloh(regA, regB);
                    break;

                case LwInstructionType.Swap:
                    ExecuteSwap(regA, regB);
                    break;

                case LwInstructionType.Swaph:
                    ExecuteSwaph(regA, regB);
                    break;

                case LwInstructionType.Swapw:
                    ExecuteSwapw(regA, regB);
                    break;

                case LwInstructionType.Swaploh:
                    ExecuteSwaploh(regA, regB);
                    break;

                // --- Math Instructions ---

                case LwInstructionType.Add:
                    ExecuteAdd(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.AddI:
                    ExecuteAddI(regDest, imm, flags);
                    break;

                case LwInstructionType.Sub:
                    ExecuteSub(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.SubI:
                    ExecuteSubI(regDest, imm, flags);
                    break;

                case LwInstructionType.Addw:
                    ExecuteAddw(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.Negw:
                    ExecuteNegw(regA, regB, flags);
                    break;

                // --- Logic instructions ---

                case LwInstructionType.Not:
                    ExecuteNot(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.Or:
                    ExecuteOr(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.And:
                    ExecuteAnd(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.Xor:
                    ExecuteXor(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.Sll:
                    ExecuteSll(regDest, regA, regB, flags);
                    break;

                case LwInstructionType.Srl:
                    ExecuteSrl(regDest, regA, regB, flags);
                    break;

                // --- Jump procedural instructions ---

                case LwInstructionType.Jr:
                    ExecuteJr(regB, pc);
                    break;

                case LwInstructionType.Jro:
                    ExecuteJro(regA, regB, pc, flags);
                    break;

                case LwInstructionType.Jas:
                    ExecuteJas(regA, regB, pc);
                    break;

                case LwInstructionType.JpcaddI:
                    ExecuteJpcaddI(imm, pc, mar, flags);
                    break;

                case LwInstructionType.JpcsubI:
                    ExecuteJpcsubI(imm, pc, mar, flags);
                    break;

                // --- Jump conditional instructions ---

                case LwInstructionType.Jwz:
                    ExecuteJwz(regA, regB, pc, flags);
                    break;

                case LwInstructionType.Jwnotz:
                    ExecuteJwnotz(regA, regB, pc, flags);
                    break;

                case LwInstructionType.Jwn:
                    ExecuteJwn(regA, regB, pc, flags);
                    break;

                case LwInstructionType.Jwnotn:
                    ExecuteJwnotn(regA, regB, pc, flags);
                    break;

                case LwInstructionType.Jzf:
                    ExecuteJzf(regB, pc, flags);
                    break;

                case LwInstructionType.Jnf:
                    ExecuteJnf(regB, pc, flags);
                    break;

                case LwInstructionType.Jcf:
                    ExecuteJcf(regB, pc, flags);
                    break;

                case LwInstructionType.Jof:
                    ExecuteJof(regB, pc, flags);
                    break;

                // --- Interrupts ---

                case LwInstructionType.Irrex:
                    throw new InvalidOperationException("Irrex is a hardware instruction");

                case LwInstructionType.Irrret:
                    var t1 = _kpcBuild.ProgrammerRegisters[4];
                    ExecuteIrrret(irrManager, t1, ram, pc, mar, flags);
                    break;

                case LwInstructionType.Irren:
                    ExecuteIrren(irrManager);
                    break;

                case LwInstructionType.Irrdis:
                    ExecuteIrrdis(irrManager);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #region Load instructions

        private void ExecuteLbrom(Register16 regA, Register16 regB, Memory rom, Register16 mar) {
            regA.LowValue = rom.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteLbromo(Register16 regDest, Register16 regA, Register16 regB, Memory rom, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.LowValue = rom.ReadByte(tmpReg.WordValue);
            mar.WordValue = tmpReg.WordValue;
        }

        private void ExecuteLwrom(Register16 regA, Register16 regB, Memory rom, Register16 mar) {
            regA.WordValue = rom.ReadWord(regB.WordValue);
            mar.WordValue = (ushort)(regB.WordValue + 1);
        }

        private void ExecuteLwromo(Register16 regDest, Register16 regA, Register16 regB, Register16 mar, Memory rom, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.WordValue = rom.ReadWord(tmpReg.WordValue);
            mar.WordValue = (ushort)(tmpReg.WordValue + 1);
        }

        private void ExecuteLbram(Register16 regA, Register16 regB, Memory ram, Register16 mar) {
            regA.LowValue = ram.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteLbramo(Register16 regDest, Register16 regA, Register16 regB, Memory ram, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.LowValue = ram.ReadByte(tmpReg.WordValue);
            mar.WordValue = tmpReg.WordValue;
        }

        private void ExecuteLwram(Register16 regA, Register16 regB, Memory ram, Register16 mar) {
            regA.WordValue = ram.ReadWord(regB.WordValue);
            mar.WordValue = (ushort)(regB.WordValue + 1);
        }

        private void ExecuteLwramo(Register16 regDest, Register16 regA, Register16 regB, Memory ram, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            regDest.WordValue = ram.ReadWord(tmpReg.WordValue);
            mar.WordValue = (ushort)(tmpReg.WordValue + 1);
        }

        private void ExecutePopb(Register16 regA, Register16 regB, Memory ram, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            Register16 minusOneReg = new(-1);
            ExecuteAddw(tmpReg, regB, minusOneReg, flags);
            regB.WordValue--;
            regA.LowValue = ram.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecutePopw(Register16 regA, Register16 regB, Memory ram, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            Register16 minusTwoReg = new(-2);
            ExecuteAddw(tmpReg, regB, minusTwoReg, flags);
            regB.WordValue--;
            regA.LowValue = ram.ReadByte(regB.WordValue);
            regB.WordValue--;
            regA.HighValue = ram.ReadByte(regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteLbext(LwExternalDevicesAdapter extDevAdapter, Register16 regA, Register16 regB) {
            regA.LowValue = extDevAdapter.HandleLbext(regB.WordValue);
        }

        #endregion

        #region Store instructions

        private void ExecuteSbram(Register16 regA, Register16 regB, Memory ram, Register16 mar) {
            ram.WriteByte(regA.LowValue, regB.WordValue);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteSbramI(Register16 regDest, byte imm, Memory ram, Register16 mar) {
            ram.WriteByte(imm, regDest.WordValue);
            mar.WordValue = regDest.WordValue;
        }

        private void ExecuteSbramo(Register16 regDest, Register16 regA, Register16 regB, Memory ram, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            ram.WriteByte(regDest.LowValue, tmpReg.WordValue);
            mar.WordValue = tmpReg.WordValue;
        }

        private void ExecuteSwram(Register16 regA, Register16 regB, Memory ram, Register16 mar) {
            ram.WriteWord(regA.WordValue, regB.WordValue);
            mar.WordValue = (ushort)(regB.WordValue + 1);
        }

        private void ExecuteSwramo(Register16 regDest, Register16 regA, Register16 regB, Memory ram, Register16 mar, Register4 flags) {
            Register16 tmpReg = new();
            ExecuteAddw(tmpReg, regA, regB, flags);
            ram.WriteWord(regDest.WordValue, tmpReg.WordValue);
            mar.WordValue = (ushort)(tmpReg.WordValue + 1);
        }

        private void ExecutePushb(Register16 regA, Register16 regB, Memory ram, Register16 mar) {
            ram.WriteByte(regA.LowValue, regB.WordValue++);
            mar.WordValue = regB.WordValue;
        }

        private void ExecutePushw(Register16 regA, Register16 regB, Memory ram, Register16 mar) {
            ram.WriteByte(regA.HighValue, regB.WordValue++);
            ram.WriteByte(regA.LowValue, regB.WordValue++);
            mar.WordValue = regB.WordValue;
        }

        private void ExecuteSbext(LwExternalDevicesAdapter extDevAdapter, Register16 regA, Register16 regB) {
            extDevAdapter.HandleSbext(regB.WordValue, regA.LowValue);
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

            var kpcFlags = (LwKpcFlag)flags.Value;
            if (kpcFlags.HasFlag(LwKpcFlag.Zf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJwnotz(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            Register16 regALowReg = new(regA.LowValue);
            Register16 regAHighReg = new(regA.HighValue);

            ExecuteOr(tmpReg, regALowReg, regAHighReg, flags);

            var kpcFlags = (LwKpcFlag)flags.Value;
            if (!kpcFlags.HasFlag(LwKpcFlag.Zf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJwn(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            Register16 regAHighReg = new(regA.HighValue);

            ExecuteOr(tmpReg, tmpReg, regAHighReg, flags);

            var kpcFlags = (LwKpcFlag)flags.Value;
            if (kpcFlags.HasFlag(LwKpcFlag.Nf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJwnotn(Register16 regA, Register16 regB, Register16 pc, Register4 flags) {
            Register16 tmpReg = new();
            Register16 regAHighReg = new(regA.HighValue);

            ExecuteOr(tmpReg, tmpReg, regAHighReg, flags);

            var kpcFlags = (LwKpcFlag)flags.Value;
            if (!kpcFlags.HasFlag(LwKpcFlag.Nf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJzf(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (LwKpcFlag)flags.Value;
            if (kpcFlags.HasFlag(LwKpcFlag.Zf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJnf(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (LwKpcFlag)flags.Value;
            if (kpcFlags.HasFlag(LwKpcFlag.Nf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJcf(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (LwKpcFlag)flags.Value;
            if (kpcFlags.HasFlag(LwKpcFlag.Cf)) {
                pc.WordValue = regB.WordValue;
            }
        }

        private void ExecuteJof(Register16 regB, Register16 pc, Register4 flags) {
            var kpcFlags = (LwKpcFlag)flags.Value;
            if (kpcFlags.HasFlag(LwKpcFlag.Of)) {
                pc.WordValue = regB.WordValue;
            }
        }

        #endregion

        #region Interrupt instructions
        private void ExecuteIrrex(ushort irrAddress, Action handleIrrex, Register16 t1, Memory ram, Register16 pc, Register16 mar, Register4 flags) {
            handleIrrex();

            ram.WriteByte(flags.Value, 0xFF00);

            var tmpReg = new Register16();
            Register16 minusOneReg = new(-1);
            ExecuteAddw(tmpReg, pc, minusOneReg, flags);

            ram.WriteByte(tmpReg.LowValue, 0xFF01);     // warning - LE convention
            ram.WriteByte(tmpReg.HighValue, 0xFF02);    // warning - LE convention

            ram.WriteWord(t1.WordValue, 0xFF03);

            mar.WordValue = 0xFF04;
            pc.WordValue = irrAddress;
        }

        private void ExecuteIrrret(LwInterruptsManager irrManager, Register16 t1, Memory ram, Register16 pc, Register16 mar, Register4 flags) {
            var pcLo = (ushort)ram.ReadByte(0xFF01);
            var pcHi = (ushort)ram.ReadByte(0xFF02);

            var tmpReg = new Register16((pcHi << 8) | pcLo);
            Register16 minusOneReg = new(-1);
            ExecuteAddw(tmpReg, tmpReg, minusOneReg, flags);

            pc.WordValue = tmpReg.WordValue;
            flags.Value = ram.ReadByte(0xFF00);
            t1.WordValue = ram.ReadWord(0xFF03);

            mar.WordValue = 0xFF04;

            irrManager.HandleIrrret();
        }

        private void ExecuteIrren(LwInterruptsManager irrManager) {
            irrManager.HandleIrren();
        }

        private void ExecuteIrrdis(LwInterruptsManager irrManager) {
            irrManager.HandleIrrdis();
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
            LwKpcFlag calcFlag = LwKpcFlag.None;

            if (operationResult == 0) {
                calcFlag |= LwKpcFlag.Zf;
            }

            if ((operationResult & 0b10000000) != 0) {
                calcFlag |= LwKpcFlag.Nf;
            }

            if (!skipCf) {
                int sum = a + (isSubtraction ? (byte)~b : b) + (carryIn ? 1 : 0);

                if (sum > 255) {
                    calcFlag |= LwKpcFlag.Cf;
                }
            }

            if (isSubtraction) {
                if ((((a ^ b) & 0x80) != 0) && (((a ^ operationResult) & 0x80) != 0)) {
                    calcFlag |= LwKpcFlag.Of;
                }
            } else {
                if ((((a ^ b) & 0x80) == 0) && (((a ^ operationResult) & 0x80) != 0)) {
                    calcFlag |= LwKpcFlag.Of;
                }
            }

            regFlags.Value = (byte)calcFlag;
        }
    }
}

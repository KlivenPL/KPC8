using _Infrastructure.BitArrays;
using Components.Sequencers;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Linq;
using Tests._Infrastructure;
using Tests.Adapters;
using Xunit;

namespace Tests.ComponentTests {
    public class HLIRRSequencerTests : TestBase {

        [Fact]
        public void InterruptFlagsCorrectlySet() {
            var zero = BitArrayHelper.FromString("0000");
            var irrCode = BitArrayHelper.FromString("0101");

            using var register = CreateSequencer(out var inputs, out var outputs, out var IRRRQ, out var RDY, out var EN, out var Irr_a, out var Irr_b, out var Ir_le, out var Ic_clr, out var shouldProcessInterrupt, out var busy);

            Enable(Irr_b);
            inputs.Write(irrCode);

            IRRRQ.Value = true;
            MakeTickAndWait();

            Assert.False(shouldProcessInterrupt);
            Assert.False(RDY);
            Assert.True(busy);
            Assert.True(EN);
            BitAssert.Equality(zero, outputs);

            Enable(Ic_clr);
            MakeTickAndWait();

            Assert.False(shouldProcessInterrupt);
            Assert.False(RDY);
            Assert.True(busy);
            Assert.True(EN);
            BitAssert.Equality(zero, outputs);

            Enable(Ir_le);
            MakeTickAndWait();

            Assert.True(shouldProcessInterrupt);
            Assert.True(EN);
            Assert.False(RDY);
            Assert.True(busy);
            BitAssert.Equality(irrCode, outputs);

            Enable(Ir_le);
            MakeTickAndWait();

            Assert.True(shouldProcessInterrupt);
            Assert.True(EN);
            Assert.False(RDY);
            Assert.True(busy);

            Enable(Irr_a);
            Enable(Irr_b);
            MakeTickAndWait();

            IRRRQ.Value = false;
            MakeTickAndWait();

            Enable(Irr_a);
            Enable(Irr_b);
            MakeTickAndWait();

            for (int j = 0; j < 3; j++) {
                inputs.Write(irrCode);
                IRRRQ.Value = true;

                Enable(Ic_clr);
                MakeTickAndWait();

                Enable(Ir_le);
                MakeTickAndWait();

                Assert.True(shouldProcessInterrupt);
                Assert.True(EN);
                Assert.False(RDY);
                Assert.True(busy);
                BitAssert.Equality(irrCode, outputs);

                Enable(Irr_a);
                Enable(Irr_b);
                MakeTickAndWait();

                Assert.False(shouldProcessInterrupt);
                Assert.True(EN);
                Assert.False(RDY);
                Assert.True(busy);

                for (int i = 0; i < 3; i++) {
                    Enable(Ir_le);
                    MakeTickAndWait();

                    Assert.False(shouldProcessInterrupt);
                    Assert.True(EN);
                    Assert.False(RDY);
                    Assert.True(busy);
                }

                Enable(Irr_a);
                Enable(Irr_b);
                MakeTickAndWait();

                Assert.False(shouldProcessInterrupt);
                Assert.True(EN);
                Assert.True(RDY);
                Assert.True(busy);

                for (int i = 0; i < 3; i++) {
                    Enable(Ir_le);
                    MakeTickAndWait();

                    Assert.False(shouldProcessInterrupt);
                    Assert.True(EN);
                    Assert.True(RDY);
                    Assert.True(busy);
                }

                IRRRQ.Value = false;

                Enable(Ir_le);
                MakeTickAndWait();

                Assert.False(shouldProcessInterrupt);
                Assert.True(EN);
                Assert.False(RDY);
                Assert.False(busy);
            }
        }

        [Fact]
        public void Interrupt_Disabled() {
            var irrCode = BitArrayHelper.FromString("0101");

            using var register = CreateSequencer(out var inputs, out var outputs, out var IRRRQ, out var RDY, out var EN, out var Irr_a, out var Irr_b, out var Ir_le, out var Ic_clr, out var shouldProcessInterrupt, out var busy);

            Enable(Irr_a);
            inputs.Write(irrCode);

            IRRRQ.Value = true;
            MakeTickAndWait();

            Assert.False(shouldProcessInterrupt);
            Assert.False(RDY);
            Assert.False(EN);
            Assert.True(busy);

            Enable(Ic_clr);
            MakeTickAndWait();

            Enable(Ir_le);
            MakeTickAndWait();

            Enable(Ir_le);
            MakeTickAndWait();

            Assert.False(shouldProcessInterrupt);
            Assert.False(EN);
            Assert.False(RDY);
            Assert.True(busy);
        }

        [Fact]
        public void DeviceAbort() {
            var zero = BitArrayHelper.FromString("0000");
            var irrCode = BitArrayHelper.FromString("0101");

            using var register = CreateSequencer(out var inputs, out var outputs, out var IRRRQ, out var RDY, out var EN, out var Irr_a, out var Irr_b, out var Ir_le, out var Ic_clr, out var shouldProcessInterrupt, out var busy);

            Enable(Irr_b);
            inputs.Write(irrCode);

            for (int j = 0; j < 3; j++) {
                inputs.Write(irrCode);
                IRRRQ.Value = true;
                MakeTickAndWait();

                Enable(Ic_clr);
                MakeTickAndWait();

                Enable(Ir_le);
                MakeTickAndWait();

                Assert.True(shouldProcessInterrupt);
                Assert.True(EN);
                Assert.False(RDY);
                BitAssert.Equality(irrCode, outputs);
                Assert.True(busy);

                Enable(Irr_a);
                Enable(Irr_b);
                MakeTickAndWait();

                Assert.False(shouldProcessInterrupt);
                Assert.True(EN);
                Assert.False(RDY);
                Assert.True(busy);

                for (int i = 0; i < 3; i++) {
                    MakeTickAndWait();

                    Assert.False(shouldProcessInterrupt);
                    Assert.True(EN);
                    Assert.False(RDY);
                    Assert.True(busy);
                }

                IRRRQ.Value = false; // device abort

                for (int i = 0; i < 3; i++) {
                    MakeTickAndWait();

                    Assert.False(shouldProcessInterrupt);
                    Assert.True(EN);
                    Assert.False(RDY);
                    Assert.True(busy);
                }

                IRRRQ.Value = true; // other device coming in

                for (int i = 0; i < 3; i++) {
                    Enable(Ic_clr);
                    MakeTickAndWait();

                    Enable(Ir_le);
                    MakeTickAndWait();

                    Assert.False(shouldProcessInterrupt);
                    Assert.True(EN);
                    Assert.False(RDY);
                    Assert.True(busy);
                }

                Enable(Irr_a);
                Enable(Irr_b);
                MakeTickAndWait();

                Assert.False(shouldProcessInterrupt);
                Assert.True(EN);
                Assert.False(RDY);
                Assert.True(busy);

                for (int i = 0; i < 5; i++) {
                    MakeTickAndWait();

                    Assert.False(shouldProcessInterrupt);
                    Assert.True(EN);
                    Assert.False(RDY);
                    Assert.True(busy);
                }
            }
        }

        private HLIRRSequencer CreateSequencer(out Signal[] inputs, out Signal[] outputs, out Signal IRRRQ, out Signal RDY, out Signal EN, out Signal Irr_a, out Signal Irr_b, out Signal Ir_le, out Signal Ic_clr, out Signal shouldProcessInterrupt, out Signal busy) {
            var sequencer = new HLIRRSequencer("HLIRRSequencer");
            sequencer.MainClockBar.PlugIn(_testClock.Clk);

            inputs = sequencer.CreateSignalAndPlugInInputs().ToArray();
            outputs = sequencer.CreateSignalAndPlugInOutputs().ToArray();

            IRRRQ = sequencer.CreateSignalAndPlugInPort(r => r.IRRRQ);
            RDY = sequencer.CreateSignalAndPlugInPort(r => r.RDY);
            EN = sequencer.CreateSignalAndPlugInPort(r => r.EN);
            Irr_a = sequencer.CreateSignalAndPlugInPort(r => r.Irr_a);
            Irr_b = sequencer.CreateSignalAndPlugInPort(r => r.Irr_b);
            Ir_le = sequencer.CreateSignalAndPlugInPort(r => r.Ir_le);
            Ic_clr = sequencer.CreateSignalAndPlugInPort(r => r.Ic_clr);

            shouldProcessInterrupt = sequencer.CreateSignalAndPlugInPort(r => r.ShouldProcessInterrupt);
            busy = sequencer.CreateSignalAndPlugInPort(r => r.BUSY);

            return sequencer;
        }
    }
}

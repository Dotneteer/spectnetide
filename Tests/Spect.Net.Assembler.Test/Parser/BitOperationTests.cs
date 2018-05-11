using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class BitOperationTests : ParserTestBed
    {
        [TestMethod]
        public void RegisterBitOperationsWorkAsExpected()
        {
            RegisterBitOpWorks("rlc a", "RLC", "A");
            RegisterBitOpWorks("rlc b", "RLC", "B");
            RegisterBitOpWorks("rlc c", "RLC", "C");
            RegisterBitOpWorks("rlc d", "RLC", "D");
            RegisterBitOpWorks("rlc e", "RLC", "E");
            RegisterBitOpWorks("rlc h", "RLC", "H");
            RegisterBitOpWorks("rlc l", "RLC", "L");
            RegisterBitOpWorks("rlc (hl)", "RLC", "(HL)");

            RegisterBitOpWorks("rrc a", "RRC", "A");
            RegisterBitOpWorks("rrc b", "RRC", "B");
            RegisterBitOpWorks("rrc c", "RRC", "C");
            RegisterBitOpWorks("rrc d", "RRC", "D");
            RegisterBitOpWorks("rrc e", "RRC", "E");
            RegisterBitOpWorks("rrc h", "RRC", "H");
            RegisterBitOpWorks("rrc l", "RRC", "L");
            RegisterBitOpWorks("rrc (hl)", "RRC", "(HL)");

            RegisterBitOpWorks("rl a", "RL", "A");
            RegisterBitOpWorks("rl b", "RL", "B");
            RegisterBitOpWorks("rl c", "RL", "C");
            RegisterBitOpWorks("rl d", "RL", "D");
            RegisterBitOpWorks("rl e", "RL", "E");
            RegisterBitOpWorks("rl h", "RL", "H");
            RegisterBitOpWorks("rl l", "RL", "L");
            RegisterBitOpWorks("rl (hl)", "RL", "(HL)");

            RegisterBitOpWorks("rr a", "RR", "A");
            RegisterBitOpWorks("rr b", "RR", "B");
            RegisterBitOpWorks("rr c", "RR", "C");
            RegisterBitOpWorks("rr d", "RR", "D");
            RegisterBitOpWorks("rr e", "RR", "E");
            RegisterBitOpWorks("rr h", "RR", "H");
            RegisterBitOpWorks("rr l", "RR", "L");
            RegisterBitOpWorks("rr (hl)", "RR", "(HL)");

            RegisterBitOpWorks("sla a", "SLA", "A");
            RegisterBitOpWorks("sla b", "SLA", "B");
            RegisterBitOpWorks("sla c", "SLA", "C");
            RegisterBitOpWorks("sla d", "SLA", "D");
            RegisterBitOpWorks("sla e", "SLA", "E");
            RegisterBitOpWorks("sla h", "SLA", "H");
            RegisterBitOpWorks("sla l", "SLA", "L");
            RegisterBitOpWorks("sla (hl)", "SLA", "(HL)");

            RegisterBitOpWorks("sra a", "SRA", "A");
            RegisterBitOpWorks("sra b", "SRA", "B");
            RegisterBitOpWorks("sra c", "SRA", "C");
            RegisterBitOpWorks("sra d", "SRA", "D");
            RegisterBitOpWorks("sra e", "SRA", "E");
            RegisterBitOpWorks("sra h", "SRA", "H");
            RegisterBitOpWorks("sra l", "SRA", "L");
            RegisterBitOpWorks("sra (hl)", "SRA", "(HL)");

            RegisterBitOpWorks("sll a", "SLL", "A");
            RegisterBitOpWorks("sll b", "SLL", "B");
            RegisterBitOpWorks("sll c", "SLL", "C");
            RegisterBitOpWorks("sll d", "SLL", "D");
            RegisterBitOpWorks("sll e", "SLL", "E");
            RegisterBitOpWorks("sll h", "SLL", "H");
            RegisterBitOpWorks("sll l", "SLL", "L");
            RegisterBitOpWorks("sll (hl)", "SLL", "(HL)");

            RegisterBitOpWorks("srl a", "SRL", "A");
            RegisterBitOpWorks("srl b", "SRL", "B");
            RegisterBitOpWorks("srl c", "SRL", "C");
            RegisterBitOpWorks("srl d", "SRL", "D");
            RegisterBitOpWorks("srl e", "SRL", "E");
            RegisterBitOpWorks("srl h", "SRL", "H");
            RegisterBitOpWorks("srl l", "SRL", "L");
            RegisterBitOpWorks("srl (hl)", "SRL", "(HL)");
        }

        [TestMethod]
        public void RlcIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("rlc (ix),a", "RLC", "A", "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a),a", "RLC", "A", "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04),a", "RLC", "A", "IX", "-");
            RegisterIndexedBitOpWorks("rlc (ix),b", "RLC", "B", "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a),b", "RLC", "B", "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04),b", "RLC", "B", "IX", "-");
            RegisterIndexedBitOpWorks("rlc (ix),c", "RLC", "C", "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a),c", "RLC", "C", "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04),c", "RLC", "C", "IX", "-");
            RegisterIndexedBitOpWorks("rlc (ix),d", "RLC", "D", "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a),d", "RLC", "D", "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04),d", "RLC", "D", "IX", "-");
            RegisterIndexedBitOpWorks("rlc (ix),e", "RLC", "E", "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a),e", "RLC", "E", "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04),e", "RLC", "E", "IX", "-");
            RegisterIndexedBitOpWorks("rlc (ix),h", "RLC", "H", "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a),h", "RLC", "H", "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04),h", "RLC", "H", "IX", "-");
            RegisterIndexedBitOpWorks("rlc (ix),l", "RLC", "L", "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a),l", "RLC", "L", "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04),l", "RLC", "L", "IX", "-");
            RegisterIndexedBitOpWorks("rlc (ix)", "RLC", null, "IX", null);
            RegisterIndexedBitOpWorks("rlc (ix+#0a)", "RLC", null, "IX", "+");
            RegisterIndexedBitOpWorks("rlc (ix-#04)", "RLC", null, "IX", "-");
        }

        [TestMethod]
        public void RrcIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("rrc (ix),a", "RRC", "A", "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a),a", "RRC", "A", "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04),a", "RRC", "A", "IX", "-");
            RegisterIndexedBitOpWorks("rrc (ix),b", "RRC", "B", "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a),b", "RRC", "B", "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04),b", "RRC", "B", "IX", "-");
            RegisterIndexedBitOpWorks("rrc (ix),c", "RRC", "C", "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a),c", "RRC", "C", "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04),c", "RRC", "C", "IX", "-");
            RegisterIndexedBitOpWorks("rrc (ix),d", "RRC", "D", "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a),d", "RRC", "D", "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04),d", "RRC", "D", "IX", "-");
            RegisterIndexedBitOpWorks("rrc (ix),e", "RRC", "E", "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a),e", "RRC", "E", "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04),e", "RRC", "E", "IX", "-");
            RegisterIndexedBitOpWorks("rrc (ix),h", "RRC", "H", "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a),h", "RRC", "H", "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04),h", "RRC", "H", "IX", "-");
            RegisterIndexedBitOpWorks("rrc (ix),l", "RRC", "L", "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a),l", "RRC", "L", "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04),l", "RRC", "L", "IX", "-");
            RegisterIndexedBitOpWorks("rrc (ix)", "RRC", null, "IX", null);
            RegisterIndexedBitOpWorks("rrc (ix+#0a)", "RRC", null, "IX", "+");
            RegisterIndexedBitOpWorks("rrc (ix-#04)", "RRC", null, "IX", "-");
        }

        [TestMethod]
        public void RlIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("rl (ix),a", "RL", "A", "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a),a", "RL", "A", "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04),a", "RL", "A", "IX", "-");
            RegisterIndexedBitOpWorks("rl (ix),b", "RL", "B", "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a),b", "RL", "B", "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04),b", "RL", "B", "IX", "-");
            RegisterIndexedBitOpWorks("rl (ix),c", "RL", "C", "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a),c", "RL", "C", "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04),c", "RL", "C", "IX", "-");
            RegisterIndexedBitOpWorks("rl (ix),d", "RL", "D", "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a),d", "RL", "D", "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04),d", "RL", "D", "IX", "-");
            RegisterIndexedBitOpWorks("rl (ix),e", "RL", "E", "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a),e", "RL", "E", "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04),e", "RL", "E", "IX", "-");
            RegisterIndexedBitOpWorks("rl (ix),h", "RL", "H", "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a),h", "RL", "H", "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04),h", "RL", "H", "IX", "-");
            RegisterIndexedBitOpWorks("rl (ix),l", "RL", "L", "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a),l", "RL", "L", "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04),l", "RL", "L", "IX", "-");
            RegisterIndexedBitOpWorks("rl (ix)", "RL", null, "IX", null);
            RegisterIndexedBitOpWorks("rl (ix+#0a)", "RL", null, "IX", "+");
            RegisterIndexedBitOpWorks("rl (ix-#04)", "RL", null, "IX", "-");
        }

        [TestMethod]
        public void RrIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("rr (ix),a", "RR", "A", "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a),a", "RR", "A", "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04),a", "RR", "A", "IX", "-");
            RegisterIndexedBitOpWorks("rr (ix),b", "RR", "B", "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a),b", "RR", "B", "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04),b", "RR", "B", "IX", "-");
            RegisterIndexedBitOpWorks("rr (ix),c", "RR", "C", "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a),c", "RR", "C", "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04),c", "RR", "C", "IX", "-");
            RegisterIndexedBitOpWorks("rr (ix),d", "RR", "D", "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a),d", "RR", "D", "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04),d", "RR", "D", "IX", "-");
            RegisterIndexedBitOpWorks("rr (ix),e", "RR", "E", "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a),e", "RR", "E", "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04),e", "RR", "E", "IX", "-");
            RegisterIndexedBitOpWorks("rr (ix),h", "RR", "H", "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a),h", "RR", "H", "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04),h", "RR", "H", "IX", "-");
            RegisterIndexedBitOpWorks("rr (ix),l", "RR", "L", "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a),l", "RR", "L", "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04),l", "RR", "L", "IX", "-");
            RegisterIndexedBitOpWorks("rr (ix)", "RR", null, "IX", null);
            RegisterIndexedBitOpWorks("rr (ix+#0a)", "RR", null, "IX", "+");
            RegisterIndexedBitOpWorks("rr (ix-#04)", "RR", null, "IX", "-");
        }

        [TestMethod]
        public void SlaIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("sla (ix),a", "SLA", "A", "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a),a", "SLA", "A", "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04),a", "SLA", "A", "IX", "-");
            RegisterIndexedBitOpWorks("sla (ix),b", "SLA", "B", "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a),b", "SLA", "B", "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04),b", "SLA", "B", "IX", "-");
            RegisterIndexedBitOpWorks("sla (ix),c", "SLA", "C", "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a),c", "SLA", "C", "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04),c", "SLA", "C", "IX", "-");
            RegisterIndexedBitOpWorks("sla (ix),d", "SLA", "D", "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a),d", "SLA", "D", "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04),d", "SLA", "D", "IX", "-");
            RegisterIndexedBitOpWorks("sla (ix),e", "SLA", "E", "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a),e", "SLA", "E", "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04),e", "SLA", "E", "IX", "-");
            RegisterIndexedBitOpWorks("sla (ix),h", "SLA", "H", "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a),h", "SLA", "H", "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04),h", "SLA", "H", "IX", "-");
            RegisterIndexedBitOpWorks("sla (ix),l", "SLA", "L", "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a),l", "SLA", "L", "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04),l", "SLA", "L", "IX", "-");
            RegisterIndexedBitOpWorks("sla (ix)", "SLA", null, "IX", null);
            RegisterIndexedBitOpWorks("sla (ix+#0a)", "SLA", null, "IX", "+");
            RegisterIndexedBitOpWorks("sla (ix-#04)", "SLA", null, "IX", "-");
        }

        [TestMethod]
        public void SraIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("sra (ix),a", "SRA", "A", "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a),a", "SRA", "A", "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04),a", "SRA", "A", "IX", "-");
            RegisterIndexedBitOpWorks("sra (ix),b", "SRA", "B", "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a),b", "SRA", "B", "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04),b", "SRA", "B", "IX", "-");
            RegisterIndexedBitOpWorks("sra (ix),c", "SRA", "C", "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a),c", "SRA", "C", "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04),c", "SRA", "C", "IX", "-");
            RegisterIndexedBitOpWorks("sra (ix),d", "SRA", "D", "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a),d", "SRA", "D", "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04),d", "SRA", "D", "IX", "-");
            RegisterIndexedBitOpWorks("sra (ix),e", "SRA", "E", "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a),e", "SRA", "E", "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04),e", "SRA", "E", "IX", "-");
            RegisterIndexedBitOpWorks("sra (ix),h", "SRA", "H", "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a),h", "SRA", "H", "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04),h", "SRA", "H", "IX", "-");
            RegisterIndexedBitOpWorks("sra (ix),l", "SRA", "L", "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a),l", "SRA", "L", "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04),l", "SRA", "L", "IX", "-");
            RegisterIndexedBitOpWorks("sra (ix)", "SRA", null, "IX", null);
            RegisterIndexedBitOpWorks("sra (ix+#0a)", "SRA", null, "IX", "+");
            RegisterIndexedBitOpWorks("sra (ix-#04)", "SRA", null, "IX", "-");
        }

        [TestMethod]
        public void SllIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("sll (ix),a", "SLL", "A", "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a),a", "SLL", "A", "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04),a", "SLL", "A", "IX", "-");
            RegisterIndexedBitOpWorks("sll (ix),b", "SLL", "B", "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a),b", "SLL", "B", "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04),b", "SLL", "B", "IX", "-");
            RegisterIndexedBitOpWorks("sll (ix),c", "SLL", "C", "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a),c", "SLL", "C", "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04),c", "SLL", "C", "IX", "-");
            RegisterIndexedBitOpWorks("sll (ix),d", "SLL", "D", "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a),d", "SLL", "D", "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04),d", "SLL", "D", "IX", "-");
            RegisterIndexedBitOpWorks("sll (ix),e", "SLL", "E", "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a),e", "SLL", "E", "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04),e", "SLL", "E", "IX", "-");
            RegisterIndexedBitOpWorks("sll (ix),h", "SLL", "H", "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a),h", "SLL", "H", "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04),h", "SLL", "H", "IX", "-");
            RegisterIndexedBitOpWorks("sll (ix),l", "SLL", "L", "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a),l", "SLL", "L", "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04),l", "SLL", "L", "IX", "-");
            RegisterIndexedBitOpWorks("sll (ix)", "SLL", null, "IX", null);
            RegisterIndexedBitOpWorks("sll (ix+#0a)", "SLL", null, "IX", "+");
            RegisterIndexedBitOpWorks("sll (ix-#04)", "SLL", null, "IX", "-");
        }

        [TestMethod]
        public void SrlIxIndexedBitOperationsWorkAsExpected()
        {
            RegisterIndexedBitOpWorks("srl (ix),a", "SRL", "A", "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a),a", "SRL", "A", "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04),a", "SRL", "A", "IX", "-");
            RegisterIndexedBitOpWorks("srl (ix),b", "SRL", "B", "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a),b", "SRL", "B", "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04),b", "SRL", "B", "IX", "-");
            RegisterIndexedBitOpWorks("srl (ix),c", "SRL", "C", "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a),c", "SRL", "C", "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04),c", "SRL", "C", "IX", "-");
            RegisterIndexedBitOpWorks("srl (ix),d", "SRL", "D", "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a),d", "SRL", "D", "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04),d", "SRL", "D", "IX", "-");
            RegisterIndexedBitOpWorks("srl (ix),e", "SRL", "E", "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a),e", "SRL", "E", "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04),e", "SRL", "E", "IX", "-");
            RegisterIndexedBitOpWorks("srl (ix),h", "SRL", "H", "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a),h", "SRL", "H", "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04),h", "SRL", "H", "IX", "-");
            RegisterIndexedBitOpWorks("srl (ix),l", "SRL", "L", "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a),l", "SRL", "L", "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04),l", "SRL", "L", "IX", "-");
            RegisterIndexedBitOpWorks("srl (ix)", "SRL", null, "IX", null);
            RegisterIndexedBitOpWorks("srl (ix+#0a)", "SRL", null, "IX", "+");
            RegisterIndexedBitOpWorks("srl (ix-#04)", "SRL", null, "IX", "-");
        }

        [TestMethod]
        public void BitOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                RegisterBitManipWorks($"bit {bit},a", "BIT", "A");
                RegisterBitManipWorks($"bit {bit},b", "BIT", "B");
                RegisterBitManipWorks($"bit {bit},c", "BIT", "C");
                RegisterBitManipWorks($"bit {bit},d", "BIT", "D");
                RegisterBitManipWorks($"bit {bit},e", "BIT", "E");
                RegisterBitManipWorks($"bit {bit},h", "BIT", "H");
                RegisterBitManipWorks($"bit {bit},l", "BIT", "L");
                RegisterBitManipWorks($"bit {bit},(hl)", "BIT", "(HL)");
            }
        }

        [TestMethod]
        public void SetOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                RegisterBitManipWorks($"set {bit},a", "SET", "A");
                RegisterBitManipWorks($"set {bit},b", "SET", "B");
                RegisterBitManipWorks($"set {bit},c", "SET", "C");
                RegisterBitManipWorks($"set {bit},d", "SET", "D");
                RegisterBitManipWorks($"set {bit},e", "SET", "E");
                RegisterBitManipWorks($"set {bit},h", "SET", "H");
                RegisterBitManipWorks($"set {bit},l", "SET", "L");
                RegisterBitManipWorks($"set {bit},(hl)", "SET", "(HL)");
            }
        }

        [TestMethod]
        public void ResOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                RegisterBitManipWorks($"res {bit},a", "RES", "A");
                RegisterBitManipWorks($"res {bit},b", "RES", "B");
                RegisterBitManipWorks($"res {bit},c", "RES", "C");
                RegisterBitManipWorks($"res {bit},d", "RES", "D");
                RegisterBitManipWorks($"res {bit},e", "RES", "E");
                RegisterBitManipWorks($"res {bit},h", "RES", "H");
                RegisterBitManipWorks($"res {bit},l", "RES", "L");
                RegisterBitManipWorks($"res {bit},(hl)", "RES", "(HL)");
            }
        }

        [TestMethod]
        public void IndexedBitOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                IndexedBitManipWorks($"bit {bit},(ix)", "BIT", "IX", null);
                IndexedBitManipWorks($"bit {bit},(ix+#03)", "BIT", "IX", "+");
                IndexedBitManipWorks($"bit {bit},(ix-#08)", "BIT", "IX", "-");
            }
        }

        [TestMethod]
        public void IxIndexedSetOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                IndexedBitManipWorks($"set {bit},(ix),a", "SET", "IX", null, "A");
                IndexedBitManipWorks($"set {bit},(ix+#03),a", "SET", "IX", "+", "A");
                IndexedBitManipWorks($"set {bit},(ix-#08),a", "SET", "IX", "-", "A");
                IndexedBitManipWorks($"set {bit},(ix),b", "SET", "IX", null, "B");
                IndexedBitManipWorks($"set {bit},(ix+#03),b", "SET", "IX", "+", "B");
                IndexedBitManipWorks($"set {bit},(ix-#08),b", "SET", "IX", "-", "B");
                IndexedBitManipWorks($"set {bit},(ix),c", "SET", "IX", null, "C");
                IndexedBitManipWorks($"set {bit},(ix+#03),c", "SET", "IX", "+", "C");
                IndexedBitManipWorks($"set {bit},(ix-#08),c", "SET", "IX", "-", "C");
                IndexedBitManipWorks($"set {bit},(ix),d", "SET", "IX", null, "D");
                IndexedBitManipWorks($"set {bit},(ix+#03),d", "SET", "IX", "+", "D");
                IndexedBitManipWorks($"set {bit},(ix-#08),d", "SET", "IX", "-", "D");
                IndexedBitManipWorks($"set {bit},(ix),e", "SET", "IX", null, "E");
                IndexedBitManipWorks($"set {bit},(ix+#03),e", "SET", "IX", "+", "E");
                IndexedBitManipWorks($"set {bit},(ix-#08),e", "SET", "IX", "-", "E");
                IndexedBitManipWorks($"set {bit},(ix),h", "SET", "IX", null, "H");
                IndexedBitManipWorks($"set {bit},(ix+#03),h", "SET", "IX", "+", "H");
                IndexedBitManipWorks($"set {bit},(ix-#08),h", "SET", "IX", "-", "H");
                IndexedBitManipWorks($"set {bit},(ix),l", "SET", "IX", null, "L");
                IndexedBitManipWorks($"set {bit},(ix+#03),l", "SET", "IX", "+", "L");
                IndexedBitManipWorks($"set {bit},(ix-#08),l", "SET", "IX", "-", "L");
                IndexedBitManipWorks($"set {bit},(ix)", "SET", "IX", null);
                IndexedBitManipWorks($"set {bit},(ix+#03)", "SET", "IX", "+");
                IndexedBitManipWorks($"set {bit},(ix-#08)", "SET", "IX", "-");
            }
        }

        [TestMethod]
        public void IxIndexedResOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                IndexedBitManipWorks($"res {bit},(ix),a", "RES", "IX", null, "A");
                IndexedBitManipWorks($"res {bit},(ix+#03),a", "RES", "IX", "+", "A");
                IndexedBitManipWorks($"res {bit},(ix-#08),a", "RES", "IX", "-", "A");
                IndexedBitManipWorks($"res {bit},(ix),b", "RES", "IX", null, "B");
                IndexedBitManipWorks($"res {bit},(ix+#03),b", "RES", "IX", "+", "B");
                IndexedBitManipWorks($"res {bit},(ix-#08),b", "RES", "IX", "-", "B");
                IndexedBitManipWorks($"res {bit},(ix),c", "RES", "IX", null, "C");
                IndexedBitManipWorks($"res {bit},(ix+#03),c", "RES", "IX", "+", "C");
                IndexedBitManipWorks($"res {bit},(ix-#08),c", "RES", "IX", "-", "C");
                IndexedBitManipWorks($"res {bit},(ix),d", "RES", "IX", null, "D");
                IndexedBitManipWorks($"res {bit},(ix+#03),d", "RES", "IX", "+", "D");
                IndexedBitManipWorks($"res {bit},(ix-#08),d", "RES", "IX", "-", "D");
                IndexedBitManipWorks($"res {bit},(ix),e", "RES", "IX", null, "E");
                IndexedBitManipWorks($"res {bit},(ix+#03),e", "RES", "IX", "+", "E");
                IndexedBitManipWorks($"res {bit},(ix-#08),e", "RES", "IX", "-", "E");
                IndexedBitManipWorks($"res {bit},(ix),h", "RES", "IX", null, "H");
                IndexedBitManipWorks($"res {bit},(ix+#03),h", "RES", "IX", "+", "H");
                IndexedBitManipWorks($"res {bit},(ix-#08),h", "RES", "IX", "-", "H");
                IndexedBitManipWorks($"res {bit},(ix),l", "RES", "IX", null, "L");
                IndexedBitManipWorks($"res {bit},(ix+#03),l", "RES", "IX", "+", "L");
                IndexedBitManipWorks($"res {bit},(ix-#08),l", "RES", "IX", "-", "L");
                IndexedBitManipWorks($"res {bit},(ix)", "RES", "IX", null);
                IndexedBitManipWorks($"res {bit},(ix+#03)", "RES", "IX", "+");
                IndexedBitManipWorks($"res {bit},(ix-#08)", "RES", "IX", "-");
            }
        }

        [TestMethod]
        public void IyIndexedSetOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                IndexedBitManipWorks($"set {bit},(iy),a", "SET", "IY", null, "A");
                IndexedBitManipWorks($"set {bit},(iy+#03),a", "SET", "IY", "+", "A");
                IndexedBitManipWorks($"set {bit},(iy-#08),a", "SET", "IY", "-", "A");
                IndexedBitManipWorks($"set {bit},(iy),b", "SET", "IY", null, "B");
                IndexedBitManipWorks($"set {bit},(iy+#03),b", "SET", "IY", "+", "B");
                IndexedBitManipWorks($"set {bit},(iy-#08),b", "SET", "IY", "-", "B");
                IndexedBitManipWorks($"set {bit},(iy),c", "SET", "IY", null, "C");
                IndexedBitManipWorks($"set {bit},(iy+#03),c", "SET", "IY", "+", "C");
                IndexedBitManipWorks($"set {bit},(iy-#08),c", "SET", "IY", "-", "C");
                IndexedBitManipWorks($"set {bit},(iy),d", "SET", "IY", null, "D");
                IndexedBitManipWorks($"set {bit},(iy+#03),d", "SET", "IY", "+", "D");
                IndexedBitManipWorks($"set {bit},(iy-#08),d", "SET", "IY", "-", "D");
                IndexedBitManipWorks($"set {bit},(iy),e", "SET", "IY", null, "E");
                IndexedBitManipWorks($"set {bit},(iy+#03),e", "SET", "IY", "+", "E");
                IndexedBitManipWorks($"set {bit},(iy-#08),e", "SET", "IY", "-", "E");
                IndexedBitManipWorks($"set {bit},(iy),h", "SET", "IY", null, "H");
                IndexedBitManipWorks($"set {bit},(iy+#03),h", "SET", "IY", "+", "H");
                IndexedBitManipWorks($"set {bit},(iy-#08),h", "SET", "IY", "-", "H");
                IndexedBitManipWorks($"set {bit},(iy),l", "SET", "IY", null, "L");
                IndexedBitManipWorks($"set {bit},(iy+#03),l", "SET", "IY", "+", "L");
                IndexedBitManipWorks($"set {bit},(iy-#08),l", "SET", "IY", "-", "L");
                IndexedBitManipWorks($"set {bit},(iy)", "SET", "IY", null);
                IndexedBitManipWorks($"set {bit},(iy+#03)", "SET", "IY", "+");
                IndexedBitManipWorks($"set {bit},(iy-#08)", "SET", "IY", "-");
            }
        }

        [TestMethod]
        public void IyIndexedResOperationsWorkAsExpected()
        {
            for (var bit = 0; bit < 8; bit++)
            {
                IndexedBitManipWorks($"res {bit},(iy),a", "RES", "IY", null, "A");
                IndexedBitManipWorks($"res {bit},(iy+#03),a", "RES", "IY", "+", "A");
                IndexedBitManipWorks($"res {bit},(iy-#08),a", "RES", "IY", "-", "A");
                IndexedBitManipWorks($"res {bit},(iy),b", "RES", "IY", null, "B");
                IndexedBitManipWorks($"res {bit},(iy+#03),b", "RES", "IY", "+", "B");
                IndexedBitManipWorks($"res {bit},(iy-#08),b", "RES", "IY", "-", "B");
                IndexedBitManipWorks($"res {bit},(iy),c", "RES", "IY", null, "C");
                IndexedBitManipWorks($"res {bit},(iy+#03),c", "RES", "IY", "+", "C");
                IndexedBitManipWorks($"res {bit},(iy-#08),c", "RES", "IY", "-", "C");
                IndexedBitManipWorks($"res {bit},(iy),d", "RES", "IY", null, "D");
                IndexedBitManipWorks($"res {bit},(iy+#03),d", "RES", "IY", "+", "D");
                IndexedBitManipWorks($"res {bit},(iy-#08),d", "RES", "IY", "-", "D");
                IndexedBitManipWorks($"res {bit},(iy),e", "RES", "IY", null, "E");
                IndexedBitManipWorks($"res {bit},(iy+#03),e", "RES", "IY", "+", "E");
                IndexedBitManipWorks($"res {bit},(iy-#08),e", "RES", "IY", "-", "E");
                IndexedBitManipWorks($"res {bit},(iy),h", "RES", "IY", null, "H");
                IndexedBitManipWorks($"res {bit},(iy+#03),h", "RES", "IY", "+", "H");
                IndexedBitManipWorks($"res {bit},(iy-#08),h", "RES", "IY", "-", "H");
                IndexedBitManipWorks($"res {bit},(iy),l", "RES", "IY", null, "L");
                IndexedBitManipWorks($"res {bit},(iy+#03),l", "RES", "IY", "+", "L");
                IndexedBitManipWorks($"res {bit},(iy-#08),l", "RES", "IY", "-", "L");
                IndexedBitManipWorks($"res {bit},(iy)", "RES", "IY", null);
                IndexedBitManipWorks($"res {bit},(iy+#03)", "RES", "IY", "+");
                IndexedBitManipWorks($"res {bit},(iy-#08)", "RES", "IY", "-");
            }
        }


        protected void RegisterBitOpWorks(string instruction, string type, string reg)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Register.ShouldBe(reg);
            line.Operand2.ShouldBeNull();
            line.Operand3.ShouldBeNull();
        }

        protected void RegisterIndexedBitOpWorks(string instruction, string type, string reg, string indexReg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Register.ShouldBe(indexReg);
            line.Operand.Sign.ShouldBe(sign);
            if (line.Operand.Sign == null)
            {
                line.Operand.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand.Expression.ShouldNotBeNull();
            }
        }

        protected void RegisterBitManipWorks(string instruction, string type, string reg)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Expression.ShouldNotBeNull();
            line.Operand2.Register.ShouldBe(reg);
        }

        protected void IndexedBitManipWorks(string instruction, string type, string idxReg, string sign, string reg = null)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand2.Register.ShouldBe(idxReg);
            line.Operand2.Sign.ShouldBe(sign);
            if (sign == null)
            {
                line.Operand2.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand2.Expression.ShouldNotBeNull();
            }
            if (reg != null)
            {
                line.Operand3.Register.ShouldBe(reg);
            }
        }
    }
}
using Corewar.Core.Enums;
using Corewar.Core.Memory;
using Corewar.Core.Parser;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Test.Parser
{
    internal class RedcodeToBinaryTests
    {
        [TestCase("DAT 0 20", Opcodes.DAT)]
        [TestCase("MOV 5 20", Opcodes.MOV)]
        [TestCase("ADD 5 20", Opcodes.ADD)]
        [TestCase("SUB 5 20", Opcodes.SUB)]
        [TestCase("MUL 5 20", Opcodes.MUL)]
        [TestCase("DIV 5 20", Opcodes.DIV)]
        [TestCase("MOD 5 20", Opcodes.MOD)]
        [TestCase("JMP 20",   Opcodes.JMP)]
        [TestCase("JMZ 5 20", Opcodes.JMZ)]
        [TestCase("JMN 5 20", Opcodes.JMN)]
        [TestCase("DJN 5 20", Opcodes.DJN)]
        [TestCase("CMP 5 20", Opcodes.CMP)]
        [TestCase("SLT 5 20", Opcodes.SLT)]
        [TestCase("SPL 0", Opcodes.SPL)]
        public void Should_correctly_compile_instructions_opcode(string input, Opcodes expectedOpcode)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].Opcode).Should().Be(expectedOpcode);
        }

        [TestCase("MOV.A 0 0", Modifier.A)]
        [TestCase("MOV.B 0 0", Modifier.B)]
        [TestCase("MOV.AB 0 0", Modifier.AB)]
        [TestCase("MOV.BA 0 0", Modifier.BA)]
        [TestCase("MOV.X 0 0", Modifier.X)]
        [TestCase("MOV.F 0 0", Modifier.F)]
        [TestCase("MOV.I 0 0", Modifier.I)]
        public void Should_correctly_compile_instructions_modifiers(string input, Modifier expectedModifier)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].Modifier).Should().Be(expectedModifier);
        }

        [TestCase("MOV 5 20", 5)]
        [TestCase("MOV #25 20", 25)]
        [TestCase("MOV @32 20", 32)]
        [TestCase("SLT -24, 25", -24)]
        public void Should_correctly_compile_value_for_operand_A(string input, int expectedValue)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].ANumber).Should().Be(expectedValue);
        }

        [TestCase("MOV #42 #63")]
        [TestCase("ADD #42 #63")]
        [TestCase("SUB #42 #63")]
        [TestCase("JMZ #42 #63")]
        [TestCase("CMP #42 #63")]
        public void Should_correctly_compile_immediate_adressing_mode_for_operand_A(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].AMode).Should().Be(AddressMode.Immediate);
        }

        [TestCase("MOV 42 #63")]
        [TestCase("ADD 42 #63")]
        [TestCase("SUB 42 #63")]
        [TestCase("JMZ $42 #63")]
        [TestCase("CMP $42 #63")]
        [TestCase("JMP $42")]
        public void Should_correctly_compile_direct_adressing_mode_for_operand_A(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].AMode).Should().Be(AddressMode.Direct);
        }

        [TestCase("MOV @42 #63")]
        [TestCase("ADD @42 #63")]
        [TestCase("SUB @42 #63")]
        [TestCase("JMZ @42 #63")]
        [TestCase("CMP @42 #63")]
        public void Should_correctly_compile_indirect_adressing_mode_for_operand_A(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].AMode).Should().Be(AddressMode.Indirect);
        }

        [TestCase("MOV <42 #63")]
        public void Should_correctly_compile_decrement_adressing_mode_for_operand_A(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].AMode).Should().Be(AddressMode.Decrement);
        }

        [TestCase("MOV >42 #63")]
        public void Should_correctly_compile_increment_adressing_mode_for_operand_A(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].AMode).Should().Be(AddressMode.Increment);
        }

        [TestCase("DAT 0 #5")]
        [TestCase("MOV 2 #8")]
        [TestCase("MOV @2 #31")]
        [TestCase("MOV #22 #31")]
        public void Should_correctly_compile_immediate_adressing_mode_for_operand_B(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].BMode).Should().Be(AddressMode.Immediate);
        }

        [TestCase("DAT 0 5")]
        [TestCase("MOV 2 8")]
        [TestCase("MOV @2 $1")]
        [TestCase("MOV #2 $1")]
        public void Should_correctly_compile_direct_adressing_mode_for_operand_B(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].BMode).Should().Be(AddressMode.Direct);
        }

        [TestCase("DAT 0 @5")]
        [TestCase("MOV 8 @5")]
        [TestCase("MOV #8 @5")]
        [TestCase("MOV @8 @5")]
        public void Should_correctly_compile_indirect_addressing_mode_for_operand_B(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].BMode).Should().Be(AddressMode.Indirect);
        }

        [TestCase("DAT 0 <5")]
        public void Should_correctly_compile_Decrement_addressing_mode_for_operand_B(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].BMode).Should().Be(AddressMode.Decrement);
        }

        [TestCase("DAT 0 >5")]
        public void Should_correctly_compile_Increment_addressing_mode_for_operand_B(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].BMode).Should().Be(AddressMode.Increment);
        }

        [TestCase("DAT 0 #5", 5)]
        [TestCase("DAT 0 5", 5)]
        [TestCase("DAT 0 $5", 5)]
        [TestCase("DAT 0 @5", 5)]
        [TestCase("MOV 0 25", 25)]
        [TestCase("MOV 0 $25", 25)]
        [TestCase("MOV 0 #25", 25)]
        [TestCase("MOV 0 @25", 25)]
        [TestCase("ADD 0 25", 25)]
        [TestCase("SUB 0 25", 25)]
        [TestCase("MUL 0 25", 25)]
        [TestCase("DIV 0 25", 25)]
        [TestCase("MOD 0 25", 25)]
        [TestCase("JMZ 0 25", 25)]
        [TestCase("JMN 0 25", 25)]
        [TestCase("DJN 0 25", 25)]
        [TestCase("CMP 0 25", 25)]
        [TestCase("SLT 0 25", 25)]
        [TestCase("SLT 0, 25", 25)]
        [TestCase("SLT 0, -245", -245)]
        public void Should_correctly_compile_value_for_operand_B(string input, int expectedValue)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(1);
            (result.Instructions[0].BNumber).Should().Be(expectedValue);
        }

        [TestCase("MOV $2, $1\nDAT #0, #5")]
        [TestCase("MOV $2, $1\r\nDAT #0, #5")]
        [TestCase("MOV $2, $1\rDAT #0, #5")]
        [TestCase("MOV $2, $1\r\n\r\n\r\nDAT #0, #5")]
        public void Should_Be_able_to_parse_multiple_lines(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(2);
            (result.Instructions[0].Opcode).Should().Be(Opcodes.MOV);
            (result.Instructions[1].Opcode).Should().Be(Opcodes.DAT);
        }

        [TestCase("MOV $2, $1\n;comment\nDAT #0, #5")]
        public void Should_Ignore_Comment_Lines(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(2);
            (result.Instructions[0].Opcode).Should().Be(Opcodes.MOV);
            (result.Instructions[1].Opcode).Should().Be(Opcodes.DAT);
        }

        [TestCase("MOV $2, $1 ;comment\nDAT #0, #5")]
        public void Should_Ignore_Comment_At_End_Of_Lines(string input)
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Instructions.Should().HaveCount(2);
            (result.Instructions[0].Opcode).Should().Be(Opcodes.MOV);
            (result.Instructions[0].BNumber).Should().Be(1);
        }

        [Test]
        public void Should_Replace_Labels_With_Their_Relative_Position()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse("target   ADD #0, #0\n" +
                                                 "         MOV #forward, @target\n" +
                                                 "forward  DAT #0, #0");

            result.Instructions.Should().HaveCount(3);
            (result.Instructions[0].Opcode).Should().Be(Opcodes.ADD);
            (result.Instructions[1].Opcode).Should().Be(Opcodes.MOV);
            (result.Instructions[1].ANumber).Should().Be(1);
            (result.Instructions[1].BNumber).Should().Be(-1);
        }

        [Test]
        public void Should_Read_Champion_Name_From_Specific_Comment_Line()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(";name    Toto\n" +
                                                 "         MOV #0, @1");

            result.Instructions.Should().HaveCount(1);
            result.Name.Should().Be("Toto");
        }

        [Test]
        public void Should_Read_Champion_Author_From_Specific_Comment_Line()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(";author    Toto\n" +
                                                 "         MOV #0, @1");

            result.Instructions.Should().HaveCount(1);
            result.Author.Should().Be("Toto");
        }

        [Test]
        public void Should_Read_Champion_Version_From_Specific_Comment_Line()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(";version    19.52.12.2\n" +
                                                 "         MOV #0, @1");

            result.Instructions.Should().HaveCount(1);
            result.Version.Should().Be("19.52.12.2");
        }

        [Test]
        public void Should_Read_Champion_Date_From_Specific_Comment_Line()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(";date    First of May 2022\n" +
                                                 "         MOV #0, @1");

            result.Instructions.Should().HaveCount(1);
            result.Date.Should().Be("First of May 2022");
        }

        [Test]
        public void Should_Read_Champion_Strategy_From_Specific_Comment_Line()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(";strategy      Bombs every fourth instruction.\n" +
                                                 "         MOV #0, @1");

            result.Instructions.Should().HaveCount(1);
            result.Strategy.Should().Be("Bombs every fourth instruction.");
        }

        [Test]
        public void Should_Read_Champion_Origin_From_Specific_Instruction()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse("         ORG  5\n" +
                                                 "         MOV #0, @1");

            result.Instructions.Should().HaveCount(1);
            result.Origin.Should().Be(5);
        }

        [Test]
        public void Should_Read_Champion_Origin_Using_Label()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse("         ORG  start\n" +
                                                 "\n" +
                                                 "         MOV #0, @1\n" +
                                                 "         MOV #0, @1\n" +
                                                 "start    MOV #0, @1");

            result.Origin.Should().Be(2);
        }

        [Test]
        public void Should_Replace_Labels_With_EQU_Opcode()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse("step     EQU 4\n" +
                                                 "         MOV #step, @1\n");

            result.Instructions.Should().HaveCount(1);
            result.Instructions[0].ANumber.Should().Be(4);
        }

        [Test]
        public void Should_Stop_At_END_Opcode()
        {
            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse("         MOV #2, @1\n" +
                                                 "         END\n" +
                                                 "         MOV #step, @1\n");

            result.Instructions.Should().HaveCount(1);
        }

        [Test]
        public void Should_Be_Able_To_Parse_Dwarf_Champion_From_ICWS_94()
        {
            string input = "" +
                ";redcode\n" +
                ";name         Dwarf\n" +
                ";author       A. K.Dewdney\n" +
                ";version      94.1\n" +
                ";date         April 29, 1993\n" +
                "\n" +
                "\n" +
                ";strategy     Bombs every fourth instruction.\n" +
                "\n" +
                "\n" +
                "         ORG start                 ; Indicates the instruction with\n" +
                "                                   ; the label \"start\" should be the\n" +
                "                                   ; first to execute.\n" +
                "\n" +
                "\n" +
                "step     EQU      4                ; Replaces all occurrences of \"step\\n" +
                "                                   ; with the character \"4\".\n" +
                "\n" +
                "\n" +
                "target   DAT.F   #0,      #0       ; Pointer to target instruction.\n" +
                "start    ADD.AB  #step,   target   ; Increments pointer by step.\n" +
                "         MOV.AB  #0,      @target  ; Bombs target instruction.\n" +
                "         JMP.A   start             ; Same as JMP.A - 2.Loops back to\n" +
                "                                   ; the instruction labelled \"start\".\n" +
                "         END\n";

            RedcodeToBinary testedParser = new RedcodeToBinary();

            Champion result = testedParser.Parse(input);

            result.Name.Should().Be("Dwarf");
            result.Author.Should().Be("A. K.Dewdney");
            result.Version.Should().Be("94.1");
            result.Date.Should().Be("April 29, 1993");
            result.Strategy.Should().Be("Bombs every fourth instruction.");

            result.Origin.Should().Be(1);

            result.Instructions.Should().HaveCount(4);
            result.Instructions[0].Should().Be(
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 0, AddressMode.Immediate, 0)
            );
            result.Instructions[1].Should().Be(
                new MemoryCell(Opcodes.ADD, Modifier.AB, AddressMode.Immediate, 4, AddressMode.Direct, -1)
            );
            result.Instructions[2].Should().Be(
                new MemoryCell(Opcodes.MOV, Modifier.AB, AddressMode.Immediate, 0, AddressMode.Indirect, -2)
            );
            result.Instructions[3].Should().Be(
                new MemoryCell(Opcodes.JMP, Modifier.A, AddressMode.Direct, -2, AddressMode.Immediate, 0)
            );
        }
    }
}

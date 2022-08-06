using Corewar.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Memory
{
    internal class Instruction
    {
        private readonly MemoryCell[] _memory;
        private readonly int _size;
        private int _currentInstructionPointer;
        public int NextInstructionPointer { get; private set; }
        public int? NewTaskSpawningAt { get; private set; }

        public bool Success { get; private set; }

        public Instruction(MemoryCell[] memory, int currentInstructionPointer)
        {
            _memory = memory;
            _currentInstructionPointer = currentInstructionPointer;
            _size = memory.Length;
        }

        internal void Execute()
        {
            MemoryCell operation = _memory[_currentInstructionPointer];
            int readPointerA = GetReadPointerA(operation);
            int writePointerB = GetWritePointerB(operation);
            MemoryCell aCell = InstructionAt(readPointerA);
            MemoryCell bCell = InstructionAt(writePointerB);
            Success = true;
            NextInstructionPointer = _currentInstructionPointer + 1;
            switch (operation.Opcode)
            {
                case Opcodes.DAT:
                    DAT();
                    break;
                case Opcodes.MOV:
                    MOV(operation, writePointerB, aCell);
                    break;
                case Opcodes.ADD:
                    ArithmeticInstruction(operation, writePointerB, aCell, bCell, Add);
                    break;
                case Opcodes.SUB:
                    ArithmeticInstruction(operation, writePointerB, aCell, bCell, Sub);
                    break;
                case Opcodes.MUL:
                    ArithmeticInstruction(operation, writePointerB, aCell, bCell, Mul);
                    break;
                case Opcodes.DIV:
                    DivisionInstruction(operation, writePointerB, aCell, bCell, Div);
                    break;
                case Opcodes.MOD:
                    DivisionInstruction(operation, writePointerB, aCell, bCell, Mod);
                    break;
                case Opcodes.JMP:
                    JMP(readPointerA);
                    break;
                case Opcodes.JMZ:
                    JumpInstruction(operation, readPointerA, bCell, IsZero, And);
                    break;
                case Opcodes.JMN:
                    JumpInstruction(operation, readPointerA, bCell, IsNonZero, Or);
                    break;
                case Opcodes.DJN:
                    DJNDecrement(operation, writePointerB, bCell);
                    JumpInstruction(operation, readPointerA, bCell, IsNonZero, Or);
                    break;
                case Opcodes.CMP:
                    CMP(operation, aCell, bCell);
                    break;
                case Opcodes.SLT:
                    SLT(operation, aCell, bCell);
                    break;
                case Opcodes.SPL:
                    SPL(readPointerA);
                    break;
            }
        }

        private static bool And(bool a, bool b) => a && b;
        private static bool Or(bool a, bool b) => a || b;
        private static bool IsNonZero(int number) => number != 0;
        private static bool IsZero(int number) => number == 0;
        private static int Add(int a, int b) => a + b;
        private static int Sub(int a, int b) => a - b;
        private static int Mul(int a, int b) => a * b;
        private static int Div(int a, int b) => a / b;
        private static int Mod(int a, int b) => a % b;

        private void SPL(int readPointerA)
        {
            NewTaskSpawningAt = _currentInstructionPointer + readPointerA;
        }

        private void SLT(MemoryCell operation, MemoryCell aCell, MemoryCell bCell)
        {
            bool skip = false;
            switch (operation.Modifier)
            {
                case Modifier.A:
                    skip = aCell.ANumber < bCell.ANumber;
                    break;
                case Modifier.B:
                    skip = aCell.BNumber < bCell.BNumber;
                    break;
                case Modifier.AB:
                    skip = aCell.ANumber < bCell.BNumber;
                    break;
                case Modifier.BA:
                    skip = aCell.BNumber < bCell.ANumber;
                    break;
                case Modifier.F:
                case Modifier.I:
                    skip = aCell.ANumber < bCell.ANumber
                        && aCell.BNumber < bCell.BNumber;
                    break;
                case Modifier.X:
                    skip = aCell.BNumber < bCell.ANumber
                        && aCell.ANumber < bCell.BNumber;
                    break;
            }
            if (skip)
            {
                NextInstructionPointer = _currentInstructionPointer + 2;
            }
        }

        private void CMP(MemoryCell operation, MemoryCell aCell, MemoryCell bCell)
        {
            bool skip = false;
            switch (operation.Modifier)
            {
                case Modifier.A:
                    skip = bCell.ANumber == aCell.ANumber;
                    break;
                case Modifier.B:
                    skip = bCell.BNumber == aCell.BNumber;
                    break;
                case Modifier.AB:
                    skip = bCell.BNumber == aCell.ANumber;
                    break;
                case Modifier.BA:
                    skip = bCell.ANumber == aCell.BNumber;
                    break;
                case Modifier.F:
                    skip = bCell.ANumber == aCell.ANumber
                        && bCell.BNumber == aCell.BNumber;
                    break;
                case Modifier.X:
                    skip = bCell.BNumber == aCell.ANumber
                        && bCell.ANumber == aCell.BNumber;
                    break;
                case Modifier.I:
                    skip = bCell.Equals(aCell);
                    break;
            }
            if (skip)
            {
                NextInstructionPointer = _currentInstructionPointer + 2;
            }
        }

        private void DJNDecrement(MemoryCell operation, int writePointerB, MemoryCell bCell)
        {
            switch (operation.Modifier)
            {
                case Modifier.A:
                case Modifier.BA:
                    _memory[WrapPointer(writePointerB)].ANumber = bCell.ANumber - 1;
                    break;
                case Modifier.B:
                case Modifier.AB:
                    _memory[WrapPointer(writePointerB)].BNumber = bCell.BNumber - 1;
                    break;
                case Modifier.F:
                case Modifier.X:
                case Modifier.I:
                    _memory[WrapPointer(writePointerB)].ANumber = bCell.ANumber - 1;
                    _memory[WrapPointer(writePointerB)].BNumber = bCell.BNumber - 1;
                    break;
            }
        }

        private void JumpInstruction(MemoryCell operation, int readPointerA, MemoryCell bCell, Func<int, bool> comparison, Func<bool, bool, bool> combination)
        {
            switch (operation.Modifier)
            {
                case Modifier.A:
                case Modifier.BA:
                    if (comparison(bCell.ANumber))
                    {
                        NextInstructionPointer = _currentInstructionPointer + readPointerA;
                    }
                    break;
                case Modifier.B:
                case Modifier.AB:
                    if (comparison(bCell.BNumber))
                    {
                        NextInstructionPointer = _currentInstructionPointer + readPointerA;
                    }
                    break;
                case Modifier.F:
                case Modifier.X:
                case Modifier.I:
                    if (combination(comparison(bCell.ANumber), comparison(bCell.BNumber)))
                    {
                        NextInstructionPointer = _currentInstructionPointer + readPointerA;
                    }
                    break;
            }
        }

        private void JMP(int readPointerA)
        {
            NextInstructionPointer = _currentInstructionPointer + readPointerA;
        }

        private void DivisionInstruction(MemoryCell operation, int writePointerB, MemoryCell aCell, MemoryCell bCell, Func<int, int, int> div)
        {
            switch (operation.Modifier)
            {
                case Modifier.A:
                    if (aCell.ANumber == 0)
                    {
                        Success = false;
                    }
                    else
                    {
                        _memory[WrapPointer(writePointerB)].ANumber = div(bCell.ANumber, aCell.ANumber);
                    }
                    break;
                case Modifier.B:
                    if (aCell.BNumber == 0)
                    {
                        Success = false;
                    }
                    else
                    {
                        _memory[WrapPointer(writePointerB)].BNumber = div(bCell.BNumber, aCell.BNumber);
                    }
                    break;
                case Modifier.AB:
                    if (aCell.ANumber == 0)
                    {
                        Success = false;
                    }
                    else
                    {
                        _memory[WrapPointer(writePointerB)].BNumber = div(bCell.BNumber, aCell.ANumber);
                    }
                    break;
                case Modifier.BA:
                    if (aCell.BNumber == 0)
                    {
                        Success = false;
                    }
                    else
                    {
                        _memory[WrapPointer(writePointerB)].ANumber = div(bCell.ANumber, aCell.BNumber);
                    }
                    break;
                case Modifier.F:
                case Modifier.I:
                    if (aCell.ANumber == 0 || aCell.BNumber == 0)
                    {
                        Success = false;
                    }
                    else
                    {
                        _memory[WrapPointer(writePointerB)].ANumber = div(bCell.ANumber, aCell.ANumber);
                        _memory[WrapPointer(writePointerB)].BNumber = div(bCell.BNumber, aCell.BNumber);
                    }
                    break;
                case Modifier.X:
                    if (aCell.ANumber == 0 || aCell.BNumber == 0)
                    {
                        Success = false;
                    }
                    else
                    {
                        _memory[WrapPointer(writePointerB)].ANumber = div(bCell.ANumber, aCell.BNumber);
                        _memory[WrapPointer(writePointerB)].BNumber = div(bCell.BNumber, aCell.ANumber);
                    }
                    break;
            }
        }

        private void ArithmeticInstruction(MemoryCell operation, int writePointerB, MemoryCell aCell, MemoryCell bCell, Func<int, int, int> op)
        {
            switch (operation.Modifier)
            {
                case Modifier.A:
                    _memory[WrapPointer(writePointerB)].ANumber = op(bCell.ANumber, aCell.ANumber);
                    break;
                case Modifier.B:
                    _memory[WrapPointer(writePointerB)].BNumber = op(bCell.BNumber, aCell.BNumber);
                    break;
                case Modifier.AB:
                    _memory[WrapPointer(writePointerB)].ANumber = op(bCell.ANumber, aCell.BNumber);
                    break;
                case Modifier.BA:
                    _memory[WrapPointer(writePointerB)].BNumber = op(bCell.BNumber, aCell.ANumber);
                    break;
                case Modifier.F:
                case Modifier.I:
                    _memory[WrapPointer(writePointerB)].BNumber = op(bCell.BNumber, aCell.BNumber);
                    _memory[WrapPointer(writePointerB)].ANumber = op(bCell.ANumber, aCell.ANumber);
                    break;
                case Modifier.X:
                    _memory[WrapPointer(writePointerB)].BNumber = op(bCell.ANumber, aCell.BNumber);
                    _memory[WrapPointer(writePointerB)].ANumber = op(bCell.BNumber, aCell.ANumber);
                    break;
            }
        }

        private void MOV(MemoryCell operation, int writePointerB, MemoryCell aCell)
        {
            switch (operation.Modifier)
            {
                case Modifier.I:
                    _memory[WrapPointer(writePointerB)] = aCell;
                    break;
                case Modifier.A:
                    _memory[WrapPointer(writePointerB)].ANumber = aCell.ANumber;
                    break;
                case Modifier.B:
                    _memory[WrapPointer(writePointerB)].BNumber = aCell.BNumber;
                    break;
                case Modifier.AB:
                    _memory[WrapPointer(writePointerB)].BNumber = aCell.ANumber;
                    break;
                case Modifier.BA:
                    _memory[WrapPointer(writePointerB)].ANumber = aCell.BNumber;
                    break;
                case Modifier.F:
                    _memory[WrapPointer(writePointerB)].ANumber = aCell.ANumber;
                    _memory[WrapPointer(writePointerB)].BNumber = aCell.BNumber;
                    break;
                case Modifier.X:
                    _memory[WrapPointer(writePointerB)].ANumber = aCell.BNumber;
                    _memory[WrapPointer(writePointerB)].BNumber = aCell.ANumber;
                    break;
            }
        }

        private void DAT()
        {
            Success = false;
        }

        private int GetWritePointerB(MemoryCell operation)
        {
            int writePointerB = operation.BNumber;
            if (operation.BMode == AddressMode.Decrement
                || operation.BMode == AddressMode.Indirect
                || operation.BMode == AddressMode.Increment)
            {
                var bNumber = InstructionAt(writePointerB).BNumber;
                if (operation.BMode == AddressMode.Decrement)
                {
                    bNumber--;
                    WriteBNumberTo(bNumber, writePointerB);
                }
                writePointerB = operation.BNumber + bNumber;
                if (operation.BMode == AddressMode.Increment)
                {
                    WriteBNumberTo(bNumber + 1, operation.BNumber);
                }
            }

            return writePointerB;
        }

        private int GetReadPointerA(MemoryCell operation)
        {
            if (operation.AMode == AddressMode.Immediate)
                return 0;
            int readPointerA = operation.ANumber;
            if (operation.AMode == AddressMode.Decrement
                || operation.AMode == AddressMode.Indirect
                || operation.AMode == AddressMode.Increment)
            {
                var bNumber = InstructionAt(readPointerA).BNumber;
                if (operation.AMode == AddressMode.Decrement)
                {
                    bNumber--;
                    WriteBNumberTo(bNumber, readPointerA);
                }
                readPointerA = operation.ANumber + bNumber;
                if (operation.AMode == AddressMode.Increment)
                {
                    WriteBNumberTo(bNumber + 1, operation.ANumber);
                }
            }

            return readPointerA;
        }

        private void WriteBNumberTo(int bNumber, int writePointer)
        {
            _memory[WrapPointer(writePointer)].BNumber = bNumber;
        }

        private MemoryCell InstructionAt(int readPointer)
        {
            return _memory[WrapPointer(readPointer)];
        }

        private int WrapPointer(int pointer)
        {
            return (_currentInstructionPointer + pointer + _size) % _size;
        }
    }
}

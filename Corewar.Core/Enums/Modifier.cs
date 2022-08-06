using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Enums
{
    public enum Modifier
    {
        /// <summary>
        /// .A<br/>
        /// Instruction execution proceeds with the A-value set to the A-number of
        /// the A-instruction and the B-value set to the A-number of the
        /// B-instruction.  A write to core replaces the A-number of the
        /// instruction pointed to by the B-pointer.<br/>
        /// <br/>
        /// For example, a CMP.A instruction would compare the A-number of the
        /// A-instruction with the A-number of the B-instruction.  A MOV.A
        /// instruction would replace the A-number of the instruction pointed to
        /// by the B-pointer with the A-number of the A-instruction.
        /// </summary>
        A,

        /// <summary>
        /// .B<br/>
        /// Instruction execution proceeds with the A-value set to the B-number of
        /// the A-instruction and the B-value set to the B-number of the
        /// B-instruction.  A write to core replaces the B-number of the
        /// instruction pointed to by the B-pointer.<br/>
        /// <br/>
        /// For example, a CMP.B instruction would compare the B-number of the
        /// A-instruction with the B-number of the B-instruction.  A MOV.B
        /// instruction would replace the B-number of the instruction pointed to
        /// by the B-pointer with the B-number of the A-instruction
        /// </summary>
        B,

        /// <summary>
        /// .AB<br/>
        /// Instruction execution proceeds with the A-value set to the A-number of
        /// the A-instruction and the B-value set to the B-number of the
        /// B-instruction.  A write to core replaces the B-number of the
        /// instruction pointed to by the B-pointer.<br/>
        /// <br/>
        /// For example, a CMP.AB instruction would compare the A-number of the
        /// A-instruction with the B-number of the B-instruction.  A MOV.AB
        /// instruction would replace the B-number of the instruction pointed to
        /// by the B-pointer with the A-number of the A-instruction.
        /// </summary>
        AB,

        /// <summary>
        /// .BA<br/>
        /// Instruction execution proceeds with the A-value set to the B-number of
        /// the A-instruction and the B-value set to the A-number of the
        /// B-instruction.  A write to core replaces the A-number of the
        /// instruction pointed to by the B-pointer.<br/>
        /// <br/>
        /// For example, a CMP.BA instruction would compare the B-number of the
        /// A-instruction with the A-number of the B-instruction.  A MOV.BA
        /// instruction would replace the A-number of the instruction pointed to
        /// by the B-pointer with the B-number of the A-instruction.
        /// </summary>
        BA,

        /// <summary>
        /// .F<br/>
        /// Instruction execution proceeds with the A-value set to both the
        /// A-number and B-number of the A-instruction (in that order) and the
        /// B-value set to both the A-number and B-number of the B-instruction
        /// (also in that order).  A write to core replaces both the A-number and
        /// the B-number of the instruction pointed to by the B-pointer (in that
        /// order).<br/>
        /// <br/>
        /// For example, a CMP.F instruction would compare the A-number of the
        /// A-instruction to the A-number of the B-instruction and the B-number of
        /// the A-instruction to B-number of the B-instruction.  A MOV.F instruction
        /// would replace the A-number of the instruction pointed to by the
        /// B-pointer with the A-number of the A-instruction and would also replace
        /// the B-number of the instruction pointed to by the B-pointer with the
        /// B-number of the A-instruction.
        /// </summary>
        F,

        /// <summary>
        /// .X<br/>
        /// Instruction execution proceeds with the A-value set to both the
        /// A-number and B-number of the A-instruction (in that order) and the
        /// B-value set to both the B-number and A-number of the B-instruction
        /// (in that order).  A write to to core replaces both the B-number and
        /// the A-number of the instruction pointed to by the B-pointer (in that
        /// order).<br/>
        /// <br/>
        /// For example, a CMP.X instruction would compare the A-number of the
        /// A-instruction to the B-number of the B-instruction and the B-number of the
        /// A-instruction to A-number of the B-instruction.  A MOV.X instruction would
        /// replace the B-number of the instruction pointed to by the B-pointer with the
        /// A-number of the A-instruction and would also replace the A-number of the
        /// instruction pointed to by the B-pointer with the B-number of the
        /// A-instruction.
        /// </summary>
        X,

        /// <summary>
        /// .I<br/>
        /// Instruction execution proceeds with the A-value set to the
        /// A-instruction and the B-value set to the B-instruction.  A write to
        /// core replaces the entire instruction pointed to by the B-pointer.<br/>
        /// <br/>
        /// For example, a CMP.I instruction would compare the A-instruction to
        /// the B-instruction.  A MOV.I instruction would replace the instruction
        /// pointed to by the B-pointer with the A-instruction.
        /// </summary>
        I
    }
}

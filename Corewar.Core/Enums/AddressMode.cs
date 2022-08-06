using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Enums
{
    public enum AddressMode
    {
        /// <summary>
        /// Immediate mode<br/>
        /// Indicated by #<br/>
        /// An immediate mode operand merely serves as storage for data. An
        /// immediate A/B-mode in the current instruction sets the A/B-pointer to
        /// zero.
        /// </summary>
        Immediate = 0,

        /// <summary>
        /// Direct mode<br/>
        /// Indicated by $<br/>
        /// A direct mode operand indicates the offset from the program counter.<br/>
        /// A direct A/B-mode in the current instruction means the A/B-pointer is
        /// a copy of the offset, the A/B-number of the current instruction.
        /// </summary>
        Direct = 1,

        /// <summary>
        /// Indirect mode<br/>
        /// Indicated by @<br/>
        /// An indirect mode operand indicates the primary offset (relative to the
        /// program counter) to the secondary offset (relative to the location
        /// of the instruction in which the secondary offset is contained).  An
        /// indirect A/B-mode indicates that the A/B-pointer is the sum of the
        /// A/B-number of the current instruction (the primary offset) and the
        /// B-number of the instruction pointed to by the A/B-number of the
        /// current instruction (the secondary offset).
        /// </summary>
        Indirect = 2,

        /// <summary>
        /// Predecrement indirect mode<br/>
        /// Indicated by &lt;<br/>
        /// A predecrement indirect mode operand indicates the primary offset
        /// (relative to the program counter) to the secondary offset (relative to
        /// the location of the instruction in which the secondary offset is
        /// contained) which is decremented prior to use.  A predecrement
        /// indirect A/B-mode indicates that the A/B-pointer is the sum of the
        /// A/B-number of the current instruction (the primary offset) and the
        /// decremented B-number of the instruction pointed to by the A/B-number
        /// of the current instruction (the secondary offset).
        /// </summary>
        Decrement = 3,

        /// <summary>
        /// Postincrement indirect mode<br/>
        /// Indicated by &gt;<br/>
        /// A postincrement indirect mode operand indicates the primary offset
        /// (relative to the program counter) to the secondary offset (relative to
        /// the location of the instruction in which the secondary offset is
        /// contained) which is incremented after the results of the operand
        /// evaluation are stored.  A postincrement indirect A/B-mode indicates that
        /// the A/B-pointer is the sum of the A/B-number of the current instruction
        /// (the primary offset) and the B-number of the instruction pointed to by
        /// the A/B-number of the current instruction (the secondary offset).  The
        /// B-number of the instruction pointed to by the A/B-number of the current
        /// instruction is incremented after the A/B-instruction is stored, but
        /// before the B-operand is evaluated (for post-increment A-mode), or
        /// the operation is executed (for post-increment indirect B-mode).
        /// </summary>
        Increment = 4
    }
}

// 
// (C) 2006-2007 The SharpOS Project Team (http://www.sharpos.org)
//
// Authors:
//	Mircea-Cristian Racasan <darx_kies@gmx.net>
//
// Licensed under the terms of the GNU GPL License version 2.
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using SharpOS.AOT.IR;
using SharpOS.AOT.IR.Instructions;
using SharpOS.AOT.IR.Operands;
using SharpOS.AOT.IR.Operators;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;

namespace SharpOS.AOT.X86 {
	public partial class Assembly : IAssembly {
		Engine engine;

		/// <summary>
		/// Initializes a new instance of the <see cref="Assembly"/> class.
		/// </summary>
		public Assembly ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Assembly"/> class.
		/// </summary>
		/// <param name="bits32">if set to <c>true</c> [bits32].</param>
		public Assembly (bool bits32)
		{
			this.bits32 = bits32;
		}

		private bool bits32 = true;

		/// <summary>
		/// Gets a value indicating whether this <see cref="Assembly"/> is bits32.
		/// </summary>
		/// <value><c>true</c> if bits32; otherwise, <c>false</c>.</value>
		public bool Bits32 {
			get {
				return bits32;
			}
		}

		protected List<Instruction> instructions = new List<Instruction> ();

		/// <summary>
		/// Gets the <see cref="SharpOS.AOT.X86.Instruction"/> at the specified index.
		/// </summary>
		/// <value></value>
		public Instruction this [int index] {
			get {
				return this.instructions [index];
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString ()
		{
			StringBuilder stringBuilder = new StringBuilder ();

			foreach (Instruction instruction in this.instructions)
			stringBuilder.Append (instruction.ToString () + "\n");

			return stringBuilder.ToString ();
		}

		/// <summary>
		/// Determines whether the specified value is register.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is register; otherwise, <c>false</c>.
		/// </returns>
		public bool IsRegister (string value)
		{
			return value.StartsWith ("SharpOS.AOT.X86.");
		}

		/// <summary>
		/// Gets the type of the register size.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public SharpOS.AOT.IR.Operands.Operand.InternalSizeType GetRegisterSizeType (string value)
		{
			if (value.Equals ("SharpOS.AOT.X86.R8Type"))
				return SharpOS.AOT.IR.Operands.Operand.InternalSizeType.U1;

			else if (value.StartsWith ("SharpOS.AOT.X86.R16Type"))
				return SharpOS.AOT.IR.Operands.Operand.InternalSizeType.U2;

			else if (value.StartsWith ("SharpOS.AOT.X86.R32Type"))
				return SharpOS.AOT.IR.Operands.Operand.InternalSizeType.U4;

			else
				throw new Exception ("'" + value + "' is not supported.");
		}

		/// <summary>
		/// Determines whether the specified value is instruction.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is instruction; otherwise, <c>false</c>.
		/// </returns>
		public bool IsInstruction (string value)
		{
			return value.StartsWith ("SharpOS.AOT.X86.");
		}

		/// <summary>
		/// Determines whether [is assembly stub] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if [is assembly stub] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsAssemblyStub (string value)
		{
			return value.Equals ("SharpOS.AOT.X86.Asm");
		}

		/// <summary>
		/// BITs the S32.
		/// </summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public void BITS32 (bool value)
		{
			this.instructions.Add (new Bits32Instruction (value));
		}

		/// <summary>
		/// DATAs the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="values">The values.</param>
		public void DATA (string name, string values)
		{
			this.instructions.Add (new ByteDataInstruction (name, values));
		}

		/// <summary>
		/// DATAs the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void DATA (string name, byte value)
		{
			this.instructions.Add (new ByteDataInstruction (name, value));
		}

		/// <summary>
		/// DATAs the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void DATA (string name, UInt16 value)
		{
			this.instructions.Add (new WordDataInstruction (name, value));
		}

		/// <summary>
		/// DATAs the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void DATA (string name, UInt32 value)
		{
			this.instructions.Add (new DWordDataInstruction (name, value));
		}

		/// <summary>
		/// DATAs the specified values.
		/// </summary>
		/// <param name="values">The values.</param>
		public void DATA (string values)
		{
			this.instructions.Add (new ByteDataInstruction (values));
		}

		/// <summary>
		/// DATAs the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DATA (byte value)
		{
			this.instructions.Add (new ByteDataInstruction (value));
		}

		/// <summary>
		/// DATAs the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DATA (UInt16 value)
		{
			this.instructions.Add (new WordDataInstruction (value));
		}

		/// <summary>
		/// DATAs the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void DATA (UInt32 value)
		{
			this.instructions.Add (new DWordDataInstruction (value));
		}

		/// <summary>
		/// OFFSETs the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void OFFSET (UInt32 value)
		{
			this.instructions.Add (new OffsetInstruction (value));
		}

		/// <summary>
		/// ORGs the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void ORG (UInt32 value)
		{
			this.instructions.Add (new OrgInstruction (value));
		}

		/// <summary>
		/// ALIGNs the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void ALIGN (UInt32 value)
		{
			this.instructions.Add (new AlignInstruction (value));
		}

		/// <summary>
		/// TIMESs the specified length.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <param name="value">The value.</param>
		public void TIMES (UInt32 length, Byte value)
		{
			this.instructions.Add (new TimesInstruction (length, value));
		}

		/// <summary>
		/// LABELs the specified label.
		/// </summary>
		/// <param name="label">The label.</param>
		public void LABEL (string label)
		{
			this.instructions.Add (new LabelInstruction (label));
		}

		/// <summary>
		/// MOVs the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="label">The label.</param>
		public void MOV (R16Type target, string label)
		{
			this.instructions.Add (new Instruction (true, string.Empty, label, "MOV", target.ToString () + ", " + label, null, null, target, new UInt32[] { 0 }, new string[] { "o16", "B8+r", "iw" }));
		}

		/// <summary>
		/// MOVs the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="label">The label.</param>
		public void MOV (R32Type target, string label)
		{
			this.instructions.Add (new Instruction (true, string.Empty, label, "MOV", target.ToString () + ", " + label, null, null, target, new UInt32[] { 0 }, new string[] { "o32", "B8+r", "id" }));
		}


		/// <summary>
		/// Gets the field offset.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public int GetFieldOffset (string value)
		{
			int result = 0;

			if (value.IndexOf ("::") == -1)
				throw new Exception ("'" + value + "' is no field definition.");

			string objectName = value.Substring (0, value.IndexOf ("::"));
			string fieldName = value.Substring (value.IndexOf ("::") + 2);

			foreach (Class _class in this.engine) {
				if (_class.ClassDefinition.FullName.Equals (objectName)) {
					foreach (FieldReference field in _class.ClassDefinition.Fields) {
						if (field.Name.Equals (fieldName))
							break;

						result += this.engine.GetFieldSize (field.FieldType.FullName);
					}

					return result;
				}
			}

			throw new Exception ("'" + value + "' has not been found.");
		}

		/// <summary>
		/// Gets the label address.
		/// </summary>
		/// <param name="label">The label.</param>
		/// <returns></returns>
		private UInt32 GetLabelAddress (string label)
		{
			UInt32 address = 0;
			bool found = false;

			foreach (Instruction instruction in this.instructions) {
				if (instruction is Bits32Instruction)
					this.bits32 = (bool) instruction.Value;

				if (instruction.Label.ToLower ().Equals (label.ToLower ())) {
					found = true;
					break;
				}

				if (instruction is OffsetInstruction)
					address = (UInt32) instruction.Value;

				else if (instruction is AlignInstruction) {
					if (address % (UInt32) instruction.Value != 0)
						address += ((UInt32) instruction.Value - address % (UInt32) instruction.Value);

				} else if (instruction is TimesInstruction) {
					TimesInstruction times = instruction as TimesInstruction;

					address += times.Length;

				} else
					address += instruction.Size (this.bits32);
			}

			if (!found)
				throw new Exception ("Label '" + label + "' has not been found.");

			return address;
		}

		/// <summary>
		/// Adds the multiboot header.
		/// </summary>
		/// <param name="addCCTOR">if set to <c>true</c> [add CCTOR].</param>
		public void AddMultibootHeader (bool addCCTOR)
		{
			uint magic = 0x1BADB002;
			uint flags = 0x00010003; //Extra info following and retrieve memory and video modes infos
			uint checksum = (uint) ((int)( (-(magic + flags))));

			this.DATA (magic);
			this.DATA (flags);
			this.DATA (checksum);

			// Header Address
			this.DATA (0x00100000);

			// Load Address
			this.DATA (0x00100000);

			// Load End Address
			this.DATA ( (uint) 0);

			// BSS End Address
			this.DATA ( (uint) 0);

			// Entry End Address (Just after this header)
			this.DATA (0x00100020);

			this.ORG (0x00100000);

			this.MOV (R32.ESP, END_STACK);

			if (addCCTOR)
				this.CALL (KERNEL_CTOR);

			this.CALL (KERNEL_MAIN);

			// Just hang
			this.LABEL (THE_END);

			this.JMP (THE_END);
		}

		private const string KERNEL_CLASS = "SharpOS.Kernel";

		private const string KERNEL_CTOR = "System.Void " + KERNEL_CLASS + "..cctor";

		private const string KERNEL_MAIN = "System.Void " + KERNEL_CLASS + ".BootEntry";

		private const string KERNEL_STRING = KERNEL_CLASS + ".String";

		private const string END_DATA = "[END DATA]";

		private const string END_STACK = "[END STACK]";

		private const string THE_END = "[THE END]";

		internal const string HELPER_LSHL = "LSHL";

		internal const string HELPER_LSHR = "LSHR";

		internal const string HELPER_LSAR = "LSAR";

		private Assembly data;

		/// <summary>
		/// Determines whether [is kernel string] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if [is kernel string] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsKernelString (string value)
		{
			return value.Equals (KERNEL_STRING);
		}

		/// <summary>
		/// Adds the LSHL.
		/// </summary>
		private void AddLSHL ()
		{
			string end = HELPER_LSHL + "_EXIT";
			string hiShift = HELPER_LSHL + "_HI_SHIFT";
			string start = HELPER_LSHL + "_START";

			this.LABEL (HELPER_LSHL);
			this.MOV (R32.ECX, new DWordMemory (null, R32.ESP, null, 0, 12));
			this.MOV (R32.EAX, new DWordMemory (null, R32.ESP, null, 0, 8));
			this.MOV (R32.EDX, new DWordMemory (null, R32.ESP, null, 0, 4));

			this.CMP (R32.ECX, 64);
			this.JB (start);

			this.OR (R32.ECX, (byte) 0x20);

			this.LABEL (start);

			this.AND (R32.ECX, 63);

			this.TEST (R32.ECX, R32.ECX);
			this.JZ (end);

			this.CMP (R32.ECX, 32);
			this.JAE (hiShift);

			this.PUSH (R32.EBX);
			this.PUSH (R32.ESI);

			this.MOV (R32.ESI, R32.EAX);
			this.SHL__CL (R32.ESI);

			this.SHL__CL (R32.EDX);

			this.MOV (R32.EBX, 32);
			this.SUB (R32.EBX, R32.ECX);
			this.MOV (R32.ECX, R32.EBX);

			this.SHR__CL (R32.EAX);

			this.OR (R32.EDX, R32.EAX);

			this.MOV (R32.EAX, R32.ESI);

			this.POP (R32.ESI);
			this.POP (R32.EBX);

			this.JMP (end);
			this.LABEL (hiShift);

			this.MOV (R32.EDX, R32.EAX);
			this.XOR (R32.EAX, R32.EAX);
			this.SUB (R32.ECX, 32);
			this.SHL__CL (R32.EDX);

			this.LABEL (end);
			this.RET ();
		}

		/// <summary>
		/// Adds the LSHR.
		/// </summary>
		private void AddLSHR ()
		{
			string end = HELPER_LSHR + "_EXIT";
			string hiShift = HELPER_LSHR + "_HI_SHIFT";

			this.LABEL (HELPER_LSHR);
			this.MOV (R32.ECX, new DWordMemory (null, R32.ESP, null, 0, 12));
			this.MOV (R32.EAX, new DWordMemory (null, R32.ESP, null, 0, 8));
			this.MOV (R32.EDX, new DWordMemory (null, R32.ESP, null, 0, 4));

			this.AND (R32.ECX, 63);

			this.TEST (R32.ECX, R32.ECX);
			this.JZ (end);

			this.CMP (R32.ECX, 32);
			this.JAE (hiShift);

			this.PUSH (R32.EBX);
			this.PUSH (R32.ESI);

			this.MOV (R32.ESI, R32.EDX);
			this.SHR__CL (R32.ESI);

			this.SHR__CL (R32.EAX);

			this.MOV (R32.EBX, 32);
			this.SUB (R32.EBX, R32.ECX);
			this.MOV (R32.ECX, R32.EBX);

			this.SHL__CL (R32.EDX);

			this.OR (R32.EAX, R32.EDX);

			this.MOV (R32.EDX, R32.ESI);

			this.POP (R32.ESI);
			this.POP (R32.EBX);

			this.JMP (end);
			this.LABEL (hiShift);

			this.MOV (R32.EAX, R32.EDX);
			this.XOR (R32.EDX, R32.EDX);
			this.SUB (R32.ECX, 32);
			this.SHR__CL (R32.EAX);

			this.LABEL (end);
			this.RET ();
		}

		/// <summary>
		/// Adds the LSAR.
		/// </summary>
		private void AddLSAR ()
		{
			string end = HELPER_LSAR + "_EXIT";
			string hiShift = HELPER_LSAR + "_HI_SHIFT";

			this.LABEL (HELPER_LSAR);
			this.MOV (R32.ECX, new DWordMemory (null, R32.ESP, null, 0, 12));
			this.MOV (R32.EAX, new DWordMemory (null, R32.ESP, null, 0, 8));
			this.MOV (R32.EDX, new DWordMemory (null, R32.ESP, null, 0, 4));

			this.AND (R32.ECX, 63);

			this.TEST (R32.ECX, R32.ECX);
			this.JZ (end);

			this.CMP (R32.ECX, 32);
			this.JAE (hiShift);

			this.PUSH (R32.EBX);
			this.PUSH (R32.ESI);

			this.MOV (R32.ESI, R32.EDX);
			this.SAR__CL (R32.ESI);

			this.SAR__CL (R32.EAX);

			this.MOV (R32.EBX, 32);
			this.SUB (R32.EBX, R32.ECX);
			this.MOV (R32.ECX, R32.EBX);

			this.SHL__CL (R32.EDX);

			this.OR (R32.EAX, R32.EDX);

			this.MOV (R32.EDX, R32.ESI);

			this.POP (R32.ESI);
			this.POP (R32.EBX);

			this.JMP (end);
			this.LABEL (hiShift);

			this.MOV (R32.EAX, R32.EDX);
			this.SUB (R32.ECX, 32);
			this.SAR__CL (R32.EAX);

			this.SAR (R32.EDX, 31);

			this.LABEL (end);
			this.RET ();
		}

		/// <summary>
		/// Adds the helper functions.
		/// </summary>
		private void AddHelperFunctions ()
		{
			this.AddLSHL ();
			this.AddLSHR ();
			this.AddLSAR ();
		}

		/// <summary>
		/// Encodes the specified engine.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		public bool Encode (Engine engine, string target)
		{
			MemoryStream memoryStream = new MemoryStream ();
			this.data = new Assembly ();
			this.engine = engine;

			bool addCTOR = false;

			foreach (Class _class in engine) {
				if (!_class.ClassDefinition.FullName.Equals (KERNEL_CLASS))
					continue;

				foreach (Method method in _class) {
					if (method.MethodFullName.Equals (KERNEL_CTOR)) {
						addCTOR = true;
						break;
					}
				}

				if (addCTOR)
					break;
			}

			// TODO add the other CCTOR calls
			this.AddMultibootHeader (addCTOR);

			foreach (Class _class in engine) {
				foreach (Method method in _class) {
					this.engine.WriteLine ("Processing '" + method.MethodFullName + "'.");

					AssemblyMethod assemblyMethod = new AssemblyMethod (this, method);
					assemblyMethod.GetAssemblyCode ();
				}
			}

			this.AddHelperFunctions ();

			foreach (Class _class in engine) {
				if (_class.ClassDefinition.IsEnum)
					continue;

				if (_class.ClassDefinition.IsValueType)
					continue;

				foreach (FieldDefinition field in _class.ClassDefinition.Fields) {
					string fullname = field.DeclaringType.FullName + "::" + field.Name;

					if (!field.IsStatic) {
						Console.WriteLine ("Not processing '" + fullname + "'");

						continue;
					}

					this.LABEL (fullname);

					switch (engine.GetInternalType (field.FieldType.FullName)) {
						case Operand.InternalSizeType.I1:
						case Operand.InternalSizeType.U1:
							this.DATA ( (byte) 0);
							break;

						case Operand.InternalSizeType.I2:
						case Operand.InternalSizeType.U2:
							this.DATA ( (ushort) 0);
							break;

						case Operand.InternalSizeType.I:
						case Operand.InternalSizeType.U:
						case Operand.InternalSizeType.I4:
						case Operand.InternalSizeType.U4:
						case Operand.InternalSizeType.R4:
							this.DATA ( (uint) 0);
							break;

						case Operand.InternalSizeType.I8:
						case Operand.InternalSizeType.U8:
						case Operand.InternalSizeType.R8:
							this.DATA ( (uint) 0);
							this.DATA ( (uint) 0);
							break;

						default:
							throw new Exception ("'" + field.FieldType + "' is not supported.");
					}
				}
			}

			foreach (Instruction instruction in data.instructions)
			this.instructions.Add (instruction);

			this.ALIGN (4096);
			this.LABEL (END_DATA);

			this.TIMES (8192, 0);
			this.LABEL (END_STACK);

			this.Encode (memoryStream);

			FileStream fileStream = new FileStream (target, FileMode.Create);
			memoryStream.WriteTo (fileStream);
			fileStream.Close ();

			return true;
		}

		/// <summary>
		/// Encodes the specified memory stream.
		/// </summary>
		/// <param name="memoryStream">The memory stream.</param>
		/// <returns></returns>
		public bool Encode (MemoryStream memoryStream)
		{
			UInt32 org = 0;

			BinaryWriter binaryWriter = new BinaryWriter (memoryStream);

			// The first pass does the optimization
			// The second pass writes the content
			for (int pass = 0; pass < 2; pass++) {
				bool changed;

				do {
					changed = false;
					UInt32 offset = 0;
					bool bss = false;

					for (int i = 0; i < this.instructions.Count; i++) {
						Instruction instruction = this.instructions [i];

						if (instruction is OrgInstruction)
							org = (UInt32) instruction.Value;
						else if (instruction is Bits32Instruction)
							this.bits32 = (bool) instruction.Value;
						else if (instruction is OffsetInstruction) {
							offset = (UInt32) instruction.Value;

							if (offset < binaryWriter.BaseStream.Length)
								throw new Exception ("Wrong offset '" + offset.ToString () + "'.");

							while (pass == 1 && !bss && binaryWriter.BaseStream.Length < offset)
								binaryWriter.Write ( (byte) 0);

						} else if (instruction is AlignInstruction) {
							if (offset % (UInt32) instruction.Value != 0)
								offset += ( (UInt32) instruction.Value - offset % (UInt32) instruction.Value);

							while (pass == 1 && !bss && binaryWriter.BaseStream.Length < offset)
								binaryWriter.Write ( (byte) 0);

						} else if (instruction is TimesInstruction) {
							TimesInstruction times = instruction as TimesInstruction;

							offset += times.Length;

							while (pass == 1 && !bss && binaryWriter.BaseStream.Length < offset)
								binaryWriter.Write ( (byte) times.Value);
						}

						if (pass == 0) {
							if (instruction.Reference.Length > 0) {
								instruction.Value = new UInt32[] { this.GetLabelAddress (instruction.Reference) };

								if (!instruction.Relative)
									((UInt32 []) instruction.Value) [0] += org;

								else {
									int delta = (int) (((UInt32 []) instruction.Value) [0] - offset);

									if (delta >= -128 && delta <= 127) {
										Assembly temp = new Assembly ();

										if (instruction.Name.Equals ("JMP")
												&& instruction.Encoding [0] == "E9") {
											temp.JMP ((byte) 0);

											instruction.Set (temp [0]);

											changed = true;

										} else if (instruction.Name.Equals ("JNZ")
												&& instruction.Encoding [1] == "85") {
											temp.JNZ ((byte) 0);

											instruction.Set (temp [0]);

											changed = true;

										} else if (instruction.Name.Equals ("JNE")
												&& instruction.Encoding [1] == "85") {
											temp.JNE ((byte) 0);

											instruction.Set (temp [0]);

											changed = true;
										}

										// TODO optimizations for the other jump instructions
									}
								}
							}

							if (instruction.RMMemory != null && instruction.RMMemory.Reference.Length > 0)
								instruction.RMMemory.Displacement = (int) (org + this.GetLabelAddress (instruction.RMMemory.Reference) + instruction.RMMemory.DisplacementDelta);

							// Load End Address
							if (instruction.Label.Equals (END_DATA)) {
								bss = true;

								this.instructions[5].Value = org + offset;
							}

							// BSS End Address
							if (instruction.Label.Equals (END_STACK))
								this.instructions[6].Value = org + offset;

						} else {
							if (!bss)
								instruction.Encode (this.bits32, binaryWriter);
						}

						offset += instruction.Size (this.bits32);
					}

				} while (changed);
			}

			return true;
		}

		/// <summary>
		/// Gets the memory internal.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private Memory GetMemoryInternal (object value)
		{
			if (value is Memory)
				return value as Memory;

			if (!(value is SharpOS.AOT.IR.Operands.Call))
				throw new Exception ("'" + value.ToString () + "' is not supported.");

			SharpOS.AOT.IR.Operands.Call call = value as SharpOS.AOT.IR.Operands.Call;

			if (call.Operands.Length == 1) {
				string parameter = (call.Operands[0] as SharpOS.AOT.IR.Operands.Constant).Value.ToString ();

				if (call.Method.DeclaringType.FullName.EndsWith (".Memory")) {
					return new Memory (parameter);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".ByteMemory")) {
					return new ByteMemory (parameter);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".WordMemory")) {
					return new WordMemory (parameter);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".DWordMemory")) {
					return new DWordMemory (parameter);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".QWordMemory")) {
					return new QWordMemory (parameter);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".TWordMemory")) {
					return new TWordMemory (parameter);

				} else {
					throw new Exception ("'" + call.Method.DeclaringType.FullName + "' is not supported.");
				}

			} else if (call.Operands.Length == 2) {
				SegType segment = Seg.GetByID ( (call.Operands[0] as SharpOS.AOT.IR.Operands.Constant).Value.ToString ());
				string label = (call.Operands[1] as SharpOS.AOT.IR.Operands.Constant).Value.ToString ();

				if (call.Method.DeclaringType.FullName.EndsWith (".Memory")) {
					return new Memory (segment, label);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".ByteMemory")) {
					return new ByteMemory (segment, label);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".WordMemory")) {
					return new WordMemory (segment, label);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".DWordMemory")) {
					return new DWordMemory (segment, label);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".QWordMemory")) {
					return new QWordMemory (segment, label);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".TWordMemory")) {
					return new TWordMemory (segment, label);

				} else {
					throw new Exception ("'" + call.Method.DeclaringType.FullName + "' is not supported.");
				}

			} else if (call.Operands.Length == 3) {
				SegType segment = Seg.GetByID ( (call.Operands[0] as SharpOS.AOT.IR.Operands.Constant).Value.ToString ());
				R16Type _base = R16.GetByID ( (call.Operands[1] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
				R16Type index = R16.GetByID ( (call.Operands[2] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());

				if (call.Method.DeclaringType.FullName.EndsWith (".Memory")) {
					return new Memory (segment, _base, index);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".ByteMemory")) {
					return new ByteMemory (segment, _base, index);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".WordMemory")) {
					return new WordMemory (segment, _base, index);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".DWordMemory")) {
					return new DWordMemory (segment, _base, index);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".QWordMemory")) {
					return new QWordMemory (segment, _base, index);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".TWordMemory")) {
					return new TWordMemory (segment, _base, index);

				} else {
					throw new Exception ("'" + call.Method.DeclaringType.FullName + "' is not supported.");
				}

			} else if (call.Operands.Length == 4) {
				if (call.Method.Parameters[1].ParameterType.FullName.IndexOf ("16") != -1) {
					SegType segment = Seg.GetByID ( (call.Operands[0] as SharpOS.AOT.IR.Operands.Constant).Value.ToString ());
					R16Type _base = R16.GetByID ( (call.Operands[1] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
					R16Type index = R16.GetByID ( (call.Operands[2] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
					Int16 displacement = Convert.ToInt16 ( (call.Operands[3] as SharpOS.AOT.IR.Operands.Constant).Value);

					if (call.Method.DeclaringType.FullName.EndsWith (".Memory")) {
						return new Memory (segment, _base, index, displacement);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".ByteMemory")) {
						return new ByteMemory (segment, _base, index, displacement);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".WordMemory")) {
						return new WordMemory (segment, _base, index, displacement);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".DWordMemory")) {
						return new DWordMemory (segment, _base, index, displacement);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".QWordMemory")) {
						return new QWordMemory (segment, _base, index, displacement);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".TWordMemory")) {
						return new TWordMemory (segment, _base, index, displacement);

					} else {
						throw new Exception ("'" + call.Method.DeclaringType.FullName + "' is not supported.");
					}

				} else {
					SegType segment = Seg.GetByID ( (call.Operands[0] as SharpOS.AOT.IR.Operands.Constant).Value.ToString ());
					R32Type _base = R32.GetByID ( (call.Operands[1] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
					//R32Type index = R32.GetByID ((call.Operands[2] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
					R32Type index = call.Operands[2] is Constant ? null : R32.GetByID ( (call.Operands[2] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
					Byte scale = Convert.ToByte ( (call.Operands[3] as SharpOS.AOT.IR.Operands.Constant).Value);

					if (call.Method.DeclaringType.FullName.EndsWith (".Memory")) {
						return new Memory (segment, _base, index, scale);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".ByteMemory")) {
						return new ByteMemory (segment, _base, index, scale);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".WordMemory")) {
						return new WordMemory (segment, _base, index, scale);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".DWordMemory")) {
						return new DWordMemory (segment, _base, index, scale);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".QWordMemory")) {
						return new QWordMemory (segment, _base, index, scale);

					} else if (call.Method.DeclaringType.FullName.EndsWith (".TWordMemory")) {
						return new TWordMemory (segment, _base, index, scale);

					} else {
						throw new Exception ("'" + call.Method.DeclaringType.FullName + "' is not supported.");
					}
				}

			} else if (call.Operands.Length == 5) {
				SegType segment = Seg.GetByID ( (call.Operands[0] as SharpOS.AOT.IR.Operands.Constant).Value.ToString ());
				R32Type _base = R32.GetByID ( (call.Operands[1] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
				R32Type index = call.Operands[2] is Constant ? null : R32.GetByID ( (call.Operands[2] as SharpOS.AOT.IR.Operands.Field).Value.ToString ());
				Byte scale = Convert.ToByte ( (call.Operands[3] as SharpOS.AOT.IR.Operands.Constant).Value);
				Int32 displacement = Convert.ToInt32 ( (call.Operands[4] as SharpOS.AOT.IR.Operands.Constant).Value);

				if (call.Method.DeclaringType.FullName.EndsWith (".Memory")) {
					return new Memory (segment, _base, index, scale, displacement);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".ByteMemory")) {
					return new ByteMemory (segment, _base, index, scale, displacement);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".WordMemory")) {
					return new WordMemory (segment, _base, index, scale, displacement);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".DWordMemory")) {
					return new DWordMemory (segment, _base, index, scale, displacement);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".QWordMemory")) {
					return new QWordMemory (segment, _base, index, scale, displacement);

				} else if (call.Method.DeclaringType.FullName.EndsWith (".TWordMemory")) {
					return new TWordMemory (segment, _base, index, scale, displacement);

				} else {
					throw new Exception ("'" + call.Method.DeclaringType.FullName + "' is not supported.");
				}

			} else {
				throw new Exception ("'" + call.Method.Name + "' has wrong parameters.");
			}
		}

		/// <summary>
		/// Gets the memory.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public Memory GetMemory (object value)
		{
			return GetMemoryInternal (value);
		}

		/// <summary>
		/// Gets the byte memory.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public ByteMemory GetByteMemory (object value)
		{
			return GetMemoryInternal (value) as ByteMemory;
		}

		/// <summary>
		/// Gets the word memory.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public WordMemory GetWordMemory (object value)
		{
			return GetMemoryInternal (value) as WordMemory;
		}

		/// <summary>
		/// Gets the D word memory.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public DWordMemory GetDWordMemory (object value)
		{
			return GetMemoryInternal (value) as DWordMemory;
		}

		/// <summary>
		/// Gets the Q word memory.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public QWordMemory GetQWordMemory (object value)
		{
			return GetMemoryInternal (value) as QWordMemory;
		}

		/// <summary>
		/// Gets the T word memory.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public TWordMemory GetTWordMemory (object value)
		{
			return GetMemoryInternal (value) as TWordMemory;
		}

		private bool EAX = false, ECX = false, EDX = false;

		/// <summary>
		/// Gets the spare register.
		/// </summary>
		/// <returns></returns>
		internal SharpOS.AOT.X86.R32Type GetSpareRegister ()
		{
			if (!EAX) {
				EAX = true;

				return R32.EAX;

			} else if (!ECX) {
				ECX = true;

				return R32.ECX;

			} else if (!EDX) {
				EDX = true;

				return R32.EDX;

			} else
				throw new Exception ("No spare registers.");
		}

		/// <summary>
		/// Frees the spare register.
		/// </summary>
		/// <param name="register">The register.</param>
		internal void FreeSpareRegister (SharpOS.AOT.X86.R32Type register)
		{
			if (register == R32.EAX) {
				if (EAX)
					EAX = false;
				else
					throw new Exception ("EAX is already free.");

			} else if (register == R32.ECX) {
				if (ECX)
					ECX = false;
				else
					throw new Exception ("ECX is already free.");

			} else if (register == R32.EDX) {
				if (EDX)
					EDX = false;
				else
					throw new Exception ("EDX is already free.");
			}

			/*else {
				//throw new Exception ("'" + register + "' is not a spare register.");
			}*/
		}

		/// <summary>
		/// Get8s the bit register.
		/// </summary>
		/// <param name="register">The register.</param>
		/// <returns></returns>
		internal R8Type Get8BitRegister (SharpOS.AOT.X86.R32Type register)
		{
			if (register == R32.EAX)
				return R8.AL;

			else if (register == R32.ECX)
				return R8.CL;

			else if (register == R32.EDX)
				return R8.DL;

			else if (register == R32.EBX)
				return R8.BL;

			else
				throw new Exception ("'" + register + "' has no 8-Bit register.");
		}

		/// <summary>
		/// Get16s the bit register.
		/// </summary>
		/// <param name="register">The register.</param>
		/// <returns></returns>
		internal R16Type Get16BitRegister (SharpOS.AOT.X86.R32Type register)
		{
			if (register == R32.EAX)
				return R16.AX;

			else if (register == R32.ECX)
				return R16.CX;

			else if (register == R32.EDX)
				return R16.DX;

			else if (register == R32.EBX)
				return R16.BX;

			else
				throw new Exception ("'" + register + "' has no 16-Bit register.");
		}

		enum Registers : int {
			EBX = 0,
			ESI,
			EDI,

			EAX,
			EDX,
			ECX,

			AX,
			DX,
			CX,

			AH,
			DH,
			CH,

			AL,
			DL,
			CL
		}

		/// <summary>
		/// Gets the register.
		/// </summary>
		/// <param name="i">The i.</param>
		/// <returns></returns>
		internal SharpOS.AOT.X86.R32Type GetRegister (int i)
		{
			switch ( (Registers) i) {

				case Registers.EBX:
					return R32.EBX;

				case Registers.ESI:
					return R32.ESI;

				case Registers.EDI:
					return R32.EDI;

				case Registers.EAX:
					return R32.EAX;

				case Registers.EDX:
					return R32.EDX;

				case Registers.ECX:
					return R32.ECX;

				default:
					throw new Exception ("'" + i.ToString () + "' is no valid register.");
			}
		}

		/// <summary>
		/// Gets the available registers count.
		/// </summary>
		/// <value>The available registers count.</value>
		public int AvailableRegistersCount {
			get {
				return 3;
			}
		}

		/// <summary>
		/// Gets the size of the int.
		/// </summary>
		/// <value>The size of the int.</value>
		public int IntSize {
			get {
				return 4;
			}
		}

		/// <summary>
		/// Spills the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public bool Spill (Operand.InternalSizeType type)
		{
			if (type == Operand.InternalSizeType.NotSet)
				throw new Exception ("Size Type not set.");

			if (type == Operand.InternalSizeType.I8
					|| type == Operand.InternalSizeType.R4
					|| type == Operand.InternalSizeType.R8)
				return true;

			return false;
		}

		Dictionary <string, string> strings = new Dictionary <string, string> ();

		/// <summary>
		/// Adds the string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		internal string AddString (string value)
		{
			if (strings.ContainsKey (value))
				return strings [value];

			string label = this.GetFreeResourceLabel;

			strings.Add (value, label);

			data.LABEL (label);

			data.DATA (value);

			data.DATA ( (byte) 0);

			return label;
		}


		private int resourceCounter = 0;

		/// <summary>
		/// Gets the get free resource label.
		/// </summary>
		/// <value>The get free resource label.</value>
		internal string GetFreeResourceLabel {
			get {
				return "Resource_" + this.resourceCounter++;
			}
		}

		private int cmpCounter = 0;

		/// <summary>
		/// Gets the get CMP label.
		/// </summary>
		/// <value>The get CMP label.</value>
		internal string GetCMPLabel {
			get {
				return "CMP_" + this.cmpCounter++;
			}
		}
	}
}
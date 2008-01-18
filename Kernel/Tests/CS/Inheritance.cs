﻿//
// (C) 2006-2007 The SharpOS Project Team (http://www.sharpos.org)
//
// Authors:
//	???
//	Stanisław Pitucha <viraptor@gmail.com>
//
// Licensed under the terms of the GNU GPL v3,
//  with Classpath Linking Exception for Libraries
//

namespace SharpOS.Kernel.Tests.CS {
	public class Inheritance {
		private class Base {
			int number;

			public Base ()
			{
				this.number = 0;
			}

			public Base (int number)
			{
				this.number = number;
			}

			public int GetNumber ()
			{
				return number;
			}

			public int Get37 ()
			{
				return 37;
			}

			public virtual int Get50override100 ()
			{
				return 50;
			}

			public int Get58shadowWith69 ()
			{
				return 58;
			}

			public virtual Enum.IntEnum GetIntEnumAOverrideIntEnumB ()
			{
				return Enum.IntEnum.A;
			}
		}

		private class SubClass : Base {
			public SubClass ()
			{
			}
			public SubClass (int number) : base (number)
			{
			}

			public int GetInheritedNumber ()
			{
				return GetNumber ();
			}

			public override int Get50override100 ()
			{
				return 100;
			}

			public new int Get58shadowWith69 ()
			{
				return 69;
			}

			public override Enum.IntEnum GetIntEnumAOverrideIntEnumB ()
			{
				return Enum.IntEnum.B;
			}
		}

		public static uint CMPCallInherited ()
		{
			if (new SubClass (37).GetNumber () == 37)
				return 1;

			return 0;
		}

		public static uint CMPCallProxiedInherited ()
		{
			if (new SubClass (37).GetInheritedNumber () == 37)
				return 1;

			return 0;
		}

		public static uint CMPCallOverriddenInt ()
		{
			if (new SubClass ().Get50override100 () == 100)
				return 1;

			return 0;
		}

		public static uint CMPCallOverriddenEnum ()
		{
			if (new SubClass ().GetIntEnumAOverrideIntEnumB () == Enum.IntEnum.B)
				return 1;

			return 0;
		}

		public static uint CMPCallShadowedMember ()
		{
			if (new SubClass ().Get58shadowWith69 () == 69)
				return 1;

			return 0;
		}

		public static uint CMPCallShadowedMemberFromBase ()
		{
			if ((new SubClass () as Base).Get58shadowWith69 () == 58)
				return 1;

			return 0;
		}
	}
}

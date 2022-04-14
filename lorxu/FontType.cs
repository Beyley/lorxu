using System;

namespace lorxu; 

[Flags]
public enum FontType {
	Standard = 1 << 0,
	Italic   = 1 << 1,
	Bold     = 1 << 2,
}

public static class FontTypeExtensions {
	public static bool HasFlagFast(this FontType value, FontType flag) {
		return (value & flag) != 0;
	}
}
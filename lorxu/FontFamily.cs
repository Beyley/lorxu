using System.Collections.Generic;

namespace lorxu; 

public class FontFamily {
	public string                     FamilyName;
	public Dictionary<FontType, Font> Fonts;

	public int PxSize;

	public FontFamily(string name, int size) {
		this.FamilyName = name;
		this.Fonts      = new();
		this.PxSize     = size;
	}
}
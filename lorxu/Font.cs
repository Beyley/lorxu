using System.Collections.Generic;

namespace lorxu; 

public class Font {
	public FontType Type;

	public Dictionary<int, Character> Characters;

	public Font(FontType type = FontType.Standard) {
		this.Type       = type;
		this.Characters = new();
	}
}
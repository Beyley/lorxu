using System;
using System.Linq;
using lorxu.Screens;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace lorxu; 

class Program {
	static void Main(string[] args) {
		FontFamily family = new("TestFont", 12);
		family.Fonts.Add(FontType.Standard, new Font());
			
		using Game game = new Game(new EditorScreen(family, FontType.Standard));

		WindowOptions options = WindowOptions.Default;

		options.Size            = new Vector2D<int>(1280, 720);
		options.VSync           = true;
		options.FramesPerSecond = 60;
			
		game.Run(options);
	}
}
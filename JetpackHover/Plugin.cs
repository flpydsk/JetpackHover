﻿//Copyright (c) 2021-Present 18107
using PulsarModLoader;
using PulsarModLoader.Keybinds;

namespace JetpackHover
{
	public class Plugin : PulsarMod, IKeybind
	{
		public override string HarmonyIdentifier()
		{
			return "id107.jetpackHover";
		}

		public override string Author
		{
			get
			{
				return "18107, Fixed By Dragon";
			}
		}

		public override string Version
		{
			get
			{
				return "0.0.9";
			}
		}

		public override string ShortDescription
		{
			get
			{
				return "MIT Licence, Hover with keybind";
			}
		}

        public void RegisterBinds(KeybindManager manager)
        {
            manager.NewBind("Jetpack Hover", "jetpackhoverbind", "Basics", "LEFTCONTROL");
        }
    }
}

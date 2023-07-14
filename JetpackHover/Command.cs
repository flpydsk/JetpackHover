using PulsarModLoader;
using PulsarModLoader.Chat.Commands.CommandRouter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetpackHover
{
    internal class Command : ChatCommand
    {

        public static SaveValue<bool> JetpackHoverModeIsPush = new SaveValue<bool>("JetpackHoverModeIsPush", true);

        public override string[] CommandAliases()
        {
            return new string[]
            {
                "hovermode",
                "hm"
            };
        }

        public override string Description()
        {
            return "Toggle Jetpack Hover Mode";
        }

        public override void Execute(string arguments)
        {
            JetpackHoverModeIsPush.Value = !JetpackHoverModeIsPush.Value;
        }
    }
}

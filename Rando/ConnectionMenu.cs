using System.Collections.Generic;
using UnityEngine;
using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;

namespace PaleCourtCharms.Rando
{
    public class ConnectionMenu
    {
        internal static ConnectionMenu Instance { get; private set; }
        private readonly SmallButton pageRootButton;
        private readonly MenuPage accessPage;
        private readonly MenuElementFactory<RandoSettings> topLevelFactory;

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.pageRootButton;
            bool either = PaleCourtCharms.GlobalSettings.AddCharms
                       || PaleCourtCharms.GlobalSettings.RandomizeCosts;
            button.Text.color = either ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            return true;
        }

        private static void ConstructMenu(MenuPage landingPage)
        {
            Instance = new ConnectionMenu(landingPage);
        }

        private ConnectionMenu(MenuPage landingPage)
        {

            accessPage = new MenuPage("Pale Court Charms", landingPage);
            topLevelFactory = new MenuElementFactory<RandoSettings>(accessPage, RandoMenuProxy.RS);
            _ = new VerticalItemPanel(
                accessPage,
                new Vector2(0, 200),
                48f,
                true,
                topLevelFactory.Elements
            );

            pageRootButton = new SmallButton(landingPage, "Pale Court Charms");
            pageRootButton.AddHideAndShowEvent(landingPage, accessPage);

            landingPage.BeforeShow += () =>
            {
                bool either = RandoMenuProxy.RS.Enabled || RandoMenuProxy.RS.RandomizeCosts;
                pageRootButton.Text.color = either ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            };

            accessPage.BeforeHide += () =>
            {
                var settings = new RandoSettings
                {
                    Enabled = (bool)topLevelFactory.ElementLookup[nameof(RandoSettings.Enabled)].Value,
                    RandomizeCosts = (bool)topLevelFactory.ElementLookup[nameof(RandoSettings.RandomizeCosts)].Value
                };
                RandoMenuProxy.RS = settings;
                topLevelFactory.SetMenuValues(settings);
                PaleCourtCharms.GlobalSettings.AddCharms = settings.Enabled;
                PaleCourtCharms.GlobalSettings.RandomizeCosts = settings.RandomizeCosts;
            };
        }


        public void Apply(RandoSettings settings)
        {
            topLevelFactory.SetMenuValues(settings);
            pageRootButton.Text.color = (settings.Enabled || settings.RandomizeCosts)
                ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }


        public void Disable()
        {
            var off = new RandoSettings { Enabled = false, RandomizeCosts = false };

            PaleCourtCharms.GlobalSettings.AddCharms = false;
            PaleCourtCharms.GlobalSettings.RandomizeCosts = false;

            topLevelFactory.SetMenuValues(off);
        }
    }
}


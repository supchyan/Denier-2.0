using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.Localization;
using System.Collections.Generic;

namespace Denier.mainContent.rifleUI {
	internal class rUI : UIState {
		private UIText Counter;
		private UIElement area;

		public override void OnInitialize() {
			area = new UIElement();
			SetRectangle(area, 0f, 0f, 0f, 0f);
			area.HAlign = 0.5f;
			area.VAlign = 0.1f;

            Counter = new UIText(" ", 1.1f);
			Counter.HAlign = 0.5f;

			SetRectangle(Counter,0f,0f,0f,0f);

			area.Append(Counter);
			Append(area);
		}
		static float scaleValue = 1.1f;
		public override void Update(GameTime gameTime) {
			
			if(!Main.LocalPlayer.dead) {

				Counter.SetText(Language.GetTextValue("Mods.Denier.UI.counterText") + ": " + rifle.dashCount.ToString(), scaleValue, false);

				if(Main.LocalPlayer.statMana < 10) {
					Counter.TextColor = Color.Gray;
					return;
				}

				if (rifle.dashCount == 0)
					Counter.TextColor = new Color(255, 0, 0);
				else if (rifle.dashCount == 1)
					Counter.TextColor = new Color(253, 0, 56);
				else if (rifle.dashCount == 2)
					Counter.TextColor = new Color(238, 0, 93);
				else if (rifle.dashCount == 3)
					Counter.TextColor = new Color(208, 0, 128);
				else if (rifle.dashCount == 4)
					Counter.TextColor = new Color(163, 0, 158);
				else if (rifle.dashCount == 5)
					Counter.TextColor = new Color(120, 0, 168);
				else if (rifle.dashCount == 6)
					Counter.TextColor = new Color(93, 0, 180);
			}				
		}

		private void SetRectangle(UIElement uiElement, float left, float top, float width, float height) {
			uiElement.Left.Set(left, 0f);
			uiElement.Top.Set(top, 0f);
			uiElement.Width.Set(width, 0f);
			uiElement.Height.Set(height, 0f);
		}
		public override void Draw(SpriteBatch spriteBatch) {
			if (Main.LocalPlayer.HeldItem.ModItem is rifle) {
				base.Draw(spriteBatch);
			}
		}
		
	}
	class rifleUISystem : ModSystem {
		private UserInterface rifleUserInterface;
		internal rUI rifleUI;
		public void ShowMyUI() {
			rifleUserInterface?.SetState(rifleUI);
		}
		public void HideMyUI() {
			rifleUserInterface?.SetState(null);
		}
		public override void Load() {
			if (!Main.dedServ) {
				rifleUI = new();
				rifleUserInterface = new();
				rifleUserInterface.SetState(rifleUI);
			}
		}
		public override void UpdateUI(GameTime gameTime) {
			rifleUserInterface?.Update(gameTime);
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"Denier: DashCounter",
					delegate {
						rifleUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
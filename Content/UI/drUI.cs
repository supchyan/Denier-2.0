using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.Localization;
using System.Collections.Generic;
using Denier.Content.Items.Denier;

namespace Denier.Content.UI {
	internal class drUI:UIState {
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

				Counter.SetText(Language.GetTextValue("Mods.Denier.UI.counterText") + ": " + denierRifle.dashCount.ToString(), scaleValue, false);

				if(Main.LocalPlayer.statMana < 10) {
					Counter.TextColor = Color.Gray;
					return;
				}

				if (denierRifle.dashCount == 0)
					Counter.TextColor = new Color(255, 0, 0);
				else if (denierRifle.dashCount == 1)
					Counter.TextColor = new Color(253, 0, 56);
				else if (denierRifle.dashCount == 2)
					Counter.TextColor = new Color(238, 0, 93);
				else if (denierRifle.dashCount == 3)
					Counter.TextColor = new Color(208, 0, 128);
				else if (denierRifle.dashCount == 4)
					Counter.TextColor = new Color(163, 0, 158);
				else if (denierRifle.dashCount == 5)
					Counter.TextColor = new Color(120, 0, 168);
				else if (denierRifle.dashCount == 6)
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
			if (Main.LocalPlayer.HeldItem.ModItem is denierRifle) {
				base.Draw(spriteBatch);
			}
		}
		
	}
	class denierRifleUISystem:ModSystem {
		private UserInterface denierRifleUserInterface;
		internal drUI denierRifleUI;
		public void ShowMyUI() {
			denierRifleUserInterface?.SetState(denierRifleUI);
		}
		public void HideMyUI() {
			denierRifleUserInterface?.SetState(null);
		}
		public override void Load() {
			if (!Main.dedServ) {
				denierRifleUI = new();
				denierRifleUserInterface = new();
				denierRifleUserInterface.SetState(denierRifleUI);
			}
		}
		public override void UpdateUI(GameTime gameTime) {
			denierRifleUserInterface?.Update(gameTime);
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"Denier: DashCounter",
					delegate {
						denierRifleUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
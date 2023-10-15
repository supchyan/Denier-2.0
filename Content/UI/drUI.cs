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
				Counter.TextColor = new Color(255/2,255/2,255/2);
				if(Main.LocalPlayer.statMana < 10) {
					Counter.SetText(
						Language.GetTextValue("Mods.Denier.UI.counterText") + 
						": " + denierRifle.dashCount.ToString() + 
						" (" + Language.GetTextValue("Mods.Denier.UI.counterManaStatus")+")", 
						scaleValue, false
					);
				} else  {
					Counter.SetText(
						Language.GetTextValue("Mods.Denier.UI.counterText") + 
						": " + denierRifle.dashCount.ToString(), 
						scaleValue, false
					);
				}
			}
			else {
				Counter.TextColor = new Color(0,0,0,0);
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
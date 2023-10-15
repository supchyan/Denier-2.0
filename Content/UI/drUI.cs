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
using System;

namespace Denier.Content.UI {
	internal class drUI:UIState {
		private UIText Counter;
		private UIText Length;
		private UIElement Area;
		public override void OnInitialize() {
			Area = new UIElement();
			SetRectangle(Area, 0f, 0f, 0f, 0f);

			Length = new UIText(" ", 0.8f);
			SetRectangle(Length,0f,0f,0f,0f);

            Counter = new UIText(" ", 1.1f);
			SetRectangle(Counter,0f,0f,0f,0f);

			Area.Append(Length);
			Area.Append(Counter);
			Append(Area);
		}
		static float scaleValue = 1.1f;
		public override void Update(GameTime gameTime) {
			SetRectangle(
				Length,
				-8f + Main.MouseScreen.X + (Main.LocalPlayer.Center.ToScreenPosition()-Main.MouseScreen).X/2f,
				-8f + Main.MouseScreen.Y + (Main.LocalPlayer.Center.ToScreenPosition()-Main.MouseScreen).Y/2f,
				32f,32f
			);
			SetRectangle(
				Counter,
				-8f + Main.MouseScreen.X + (Main.LocalPlayer.Center.ToScreenPosition()-Main.MouseScreen).X,
				-8f + Main.MouseScreen.Y + (Main.LocalPlayer.Center.ToScreenPosition()-Main.MouseScreen).Y - 80f,
				32f,32f
			);
			
			if(!Main.LocalPlayer.dead) {
				if(Main.LocalPlayer.statMana >= 10) Counter.TextColor = new Color(255f,0f,0f);
				else Counter.TextColor = new Color(255f/2f,255f/2f,255f/2f);

				Length.TextColor = new Color((float)Math.Round(Main.LocalPlayer.velocity.Length())/20f,0f,0f);
			}
			else {
				Counter.TextColor = new Color(0,0,0,0);
				Length.TextColor = new Color(0,0,0,0);
			}	
			Length.SetText(Math.Round(Main.LocalPlayer.velocity.Length()).ToString(), scaleValue, false);	
			Counter.SetText(denierRifle.dashCount.ToString(), scaleValue, false);	
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
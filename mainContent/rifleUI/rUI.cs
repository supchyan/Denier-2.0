using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent;
using System.Collections.Generic;

namespace Denier.mainContent.rifleUI {
	internal class rUI : UIState {
		private UIText Counter;
		private UIElement area;

		public override void OnInitialize() {
			area = new UIElement();
			SetRectangle(area, 0f, 0f, Main.screenWidth, Main.screenHeight);
			area.HAlign = 0.5f;
			area.VAlign = 0.5f;

			Counter = new UIText(" ", 1.1f);
			Counter.HAlign = 0.5f;
			Counter.VAlign = 0.5f;
			SetRectangle(Counter, rifleUISystem.playerPos.X - Main.screenWidth / 2f, rifleUISystem.playerPos.Y - Main.screenHeight / 2f - 70f, 64f, 64f);	

			area.Append(Counter);
			Append(area);
		}
		static float lerpValue = 0f;
		static float scaleValue = 1.1f;
		public override void Update(GameTime gameTime) {
			SetRectangle(Counter, rifleUISystem.playerPos.X - Main.screenWidth / 2f, rifleUISystem.playerPos.Y - Main.screenHeight / 2f - 70f, 64f, 64f);
			if(!Main.LocalPlayer.dead) {

				lerpValue = 0f;

				Counter.SetText(rifle.dashCount.ToString(), scaleValue, false);

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
			else {
				if(lerpValue < 1f)
					lerpValue += 0.005f;
				Counter.SetText("Dashes can save your life", MathHelper.Lerp(scaleValue, 1.5f, lerpValue), false);
				Counter.TextColor = Color.Lerp(new Color(255, 255, 255, 255), new Color(0, 0, 0, 0), lerpValue);
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
		public static Vector2 playerPos;
		public override void UpdateUI(GameTime gameTime) {
			playerPos = Main.LocalPlayer.Center - Main.screenPosition;
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
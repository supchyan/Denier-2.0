using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Denier.Content.Items.Denier;
using Denier.Content.Utils.TerrariaOverhaulFixes;

namespace Denier.Content.UI {
	internal class DenierUI:UIState {
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
				if((Main.LocalPlayer.HeldItem.ModItem is DenierRifle && Main.LocalPlayer.statMana >= 10) ||
					(Main.LocalPlayer.HeldItem.ModItem is DenierExtend && Main.LocalPlayer.statMana >= 20))
					{
						Counter.TextColor = new Color(255f,0f,0f);
					} else {
						Counter.TextColor = new Color(255f/2f,255f/2f,255f/2f);
					}

				Length.TextColor = new Color((float)Math.Round(Main.LocalPlayer.velocity.Length())/20f,0f,0f);
			}
			else {
				Counter.TextColor = new Color(0,0,0,0);
				Length.TextColor = new Color(0,0,0,0);
			}	
			Length.SetText(Math.Round(Main.LocalPlayer.velocity.Length()).ToString(), scaleValue, false);	
			if(Main.LocalPlayer.HeldItem.ModItem is DenierRifle) {
				Counter.SetText(DenierRifle.dashCount.ToString(), scaleValue, false);	
			} else if(Main.LocalPlayer.HeldItem.ModItem is DenierExtend) {
				Counter.SetText(DenierExtend.dashCountExtended.ToString(), scaleValue, false);	
			}
				
		}

		private void SetRectangle(UIElement uiElement, float left, float top, float width, float height) {
			uiElement.Left.Set(left, 0f);
			uiElement.Top.Set(top, 0f);
			uiElement.Width.Set(width, 0f);
			uiElement.Height.Set(height, 0f);
		}
		public override void Draw(SpriteBatch spriteBatch) {
			if(Main.LocalPlayer.sleeping.isSleeping || !DenierTools.notAtAction(Main.LocalPlayer)) {
                return;
            }
			if (Main.LocalPlayer.HeldItem.ModItem is DenierRifle || Main.LocalPlayer.HeldItem.ModItem is DenierExtend) {
				base.Draw(spriteBatch);
			}
		}
	}
	class DenierUISystem:ModSystem {
		private UserInterface DenierUserInterface;
		internal DenierUI denierUI;
		public void ShowMyUI() {
			DenierUserInterface?.SetState(denierUI);
		}
		public void HideMyUI() {
			DenierUserInterface?.SetState(null);
		}
		public override void Load() {
			if (!Main.dedServ) {
				denierUI = new();
				DenierUserInterface = new();
				DenierUserInterface.SetState(denierUI);
			}
		}
		public override void UpdateUI(GameTime gameTime) {
			DenierUserInterface?.Update(gameTime);
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"Denier: DashCounter",
					delegate {
						DenierUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
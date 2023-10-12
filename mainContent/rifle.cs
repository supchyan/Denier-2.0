using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.GameContent;
using Denier;
using Denier.mainContent.spiritalCircle;
using Denier.mainContent.gunEffects;

namespace Denier.mainContent {
    public class rifle : ModItem {
        
        public override string Texture => "Denier/mainContent/rifleNoOutline";
        SoundStyle shotSound = new SoundStyle("Denier/Sounds/shot");
        SoundStyle bassSound = new SoundStyle("Denier/Sounds/bass");
        public static int dashCount;
        private float dashTimer;
        public static bool scope;
        public static Color outlineColor;
        private Random rand = new Random();
        private int rd;

        public override void SetDefaults() {
            Item.damage = 250;
            Item.width = 65;
            Item.height = 17;
            Item.scale = 2f;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 13;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.knockBack = 10;
            Item.rare = 10;
            Item.shoot = ModContent.ProjectileType<rifleBullet>();
            Item.shootSpeed = 40f;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            
            Texture2D texture = ModContent.Request<Texture2D>("Denier/mainContent/rifleOutline").Value;

            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 frameOrigin = frame.Size() / 2f;

            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset + new Vector2(0, -10f);

            spriteBatch.Draw(texture, drawPos, frame, outlineColor, rotation, frameOrigin, scale * 2f, SpriteEffects.None, 0);

            return false;

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) {  
            foreach (TooltipLine line in tooltips) {
				if (line.Mod == "Terraria" && line.Name == "Damage") {
					line.OverrideColor = Main.errorColor;
                    line.Text = "0 -> ∞";
				}
			}
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            
            Item.autoReuse = false;

            if (Main.mouseRight && squares.canShoot && player.statMana >= 15) {

                squares.canShoot = false;

                SoundEngine.PlaySound(shotSound with {MaxInstances = 3});

                player.statMana -= 15;

                if(rd > 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(), player.Center, player.velocity,
                        ModContent.ProjectileType<coolEffect>(), 0, 0, player.whoAmI
                    );
                }
                else {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(), player.Center, player.velocity,
                        ModContent.ProjectileType<coolEffectAlt>(), 0, 0, player.whoAmI
                    );
                }

                Projectile.NewProjectile(
                        Projectile.GetSource_None(), player.Center, player.velocity,
                        ModContent.ProjectileType<shell>(), 0, 0, player.whoAmI
                    );

                return true;
            }
            else if (!Main.mouseRight && dashCount > 0 && player.statMana >= 10) {

                dashCount--;
                
                for (int i = 0; i < 50; i++) {
                    Vector2 gigaVelocity = Main.rand.NextVector2Unit((player.Center - Main.MouseWorld).ToRotation() - MathHelper.Pi/8, (float)MathHelper.Pi / 4) * 20;
                    Dust dashDust = Dust.NewDustPerfect(player.Center, DustID.PortalBolt, -gigaVelocity, 255, Color.White, 1f);
                    dashDust.noGravity = true;
                }

                player.velocity = player.velocity.DirectionTo((player.Center - Main.MouseWorld))*20f;

                SoundEngine.PlaySound(bassSound with {MaxInstances = 3});

                player.statMana -= 10;

                return false;

            }
            else return false;
        }
        public override void HoldItem(Player player) {

            rd = rand.Next(-100, 100);

            if(dashCount > 6)
                dashCount = 6;
            
            dashTimer++;
            if(dashTimer % 120 == 0 && dashCount < 6)
                dashCount++;

            if(player.manaFlower)
                player.QuickMana();

            if (player.HeldItem.ModItem is not rifle)
                return; 

            if (Main.mouseRight && !player.HasBuff<coolDownBuff>()) {

                Main.cursorAlpha = 0;

                if (squares.canShoot)
                    Lighting.AddLight(Main.MouseWorld, 1f, 0f, 0f);
                else Lighting.AddLight(Main.MouseWorld, 1f, 1f, 1f);

                scope = true;
                
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<mainCircle>()] <= 0) {

                    Projectile.NewProjectile(
                        Projectile.GetSource_None(), player.Center, player.velocity,
                        ModContent.ProjectileType<mainCircle>(), 0, 0, player.whoAmI
                    );
                }                      
            }
            else if(!Main.mouseRight && squares.canShoot) {

                squares.canShoot = false;
                Main.cursorAlpha = 1;
            
            }

        }
        public override Vector2? HoldoutOffset() {

			return new Vector2(-13f, -1f);
        }
        public override void AddRecipes() {

            CreateRecipe()
                .AddIngredient(ItemID.Handgun)
                .AddIngredient(ItemID.TitaniumBar, 75)
                .AddIngredient(ItemID.Wire, 75)
				.AddIngredient(ItemID.GreaterManaPotion, 25)
				.AddTile(TileID.MythrilAnvil)
				.Register();

            CreateRecipe()
                .AddIngredient(ItemID.Handgun)
                .AddIngredient(ItemID.AdamantiteBar, 75)
                .AddIngredient(ItemID.Wire, 75)
				.AddIngredient(ItemID.GreaterManaPotion, 25)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
    public class playerStuff : ModPlayer {
        public override void ModifyScreenPosition() {

            if (rifle.scope) {
                Main.screenPosition = Main.MouseWorld - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) - (Main.MouseWorld - Main.LocalPlayer.Center)/2f;
                rifle.scope = false;
            }
        }
        public override void PreUpdate() {

            rifle.outlineColor = Main.errorColor;

            if (Main.LocalPlayer.HeldItem.ModItem is not rifle) {
                rifle.dashCount = 0;
            }
        }
    }
}
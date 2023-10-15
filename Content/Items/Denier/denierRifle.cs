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
using Terraria.GameContent.Events;
using Denier;
using Denier.Content.Buffs;
using Denier.Content.Projectiles.SpiritalCircle;
using Denier.Content.GunEffects;

namespace Denier.Content.Items.Denier {
    public class denierRifle : ModItem {
        
        public override string Texture => "Denier/Content/Items/Denier/denierRifleNoOutline";
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
            Item.autoReuse = false;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.knockBack = 10;
            Item.mana = 10;
            Item.rare = 10;
            Item.shoot = ModContent.ProjectileType<denierRifleBullet>();
            Item.shootSpeed = 40f;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            Texture2D texture = ModContent.Request<Texture2D>("Denier/Content/Items/Denier/denierRifleOutline").Value;

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
        public override bool AltFunctionUse(Player player) {
            return true;
        }
        public override bool CanUseItem(Player player) {
            if (!Main.mouseRight && player.statMana >= 10 && dashCount > 0) {
				Item.useTime = 20;
				Item.useAnimation = 20;
                Item.mana = 10;
                return true;
			} else
            if (!Main.mouseRight && (player.statMana < 10 || dashCount == 0)) {
                return false;
			} else
            if(Main.mouseRight && player.statMana < 15) {
                return false;
            } else
            if(Main.mouseRight && !sigilSquare.canShoot) {
                return false;
            } else {
                Item.useTime = 45;
				Item.useAnimation = 45;
                Item.mana = 15;
                return true;
            }
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0 && Main.mouseRight && sigilSquare.canShoot) {
                sigilSquare.canShoot = false;
                sigilSquare.rotRes = true;
                sigilSquare45deg.rotRes = true;

                SoundEngine.PlaySound(shotSound with {MaxInstances = 3});

                if(rd > 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(), 
                        player.Center, 
                        Vector2.Zero,
                        ModContent.ProjectileType<coolEffect>(), 0, 0, player.whoAmI
                    );
                }
                else {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(), 
                        player.Center, 
                        Vector2.Zero,
                        ModContent.ProjectileType<coolEffectAlt>(), 0, 0, player.whoAmI
                    );
                }

                Projectile.NewProjectile(
                    Projectile.GetSource_None(),
                    player.Center,
                    player.velocity,
                    ModContent.ProjectileType<shell>(), 0, 0, player.whoAmI
                );

                return true;
            }
            else if (player.altFunctionUse == 0 && !Main.mouseRight && dashCount > 0) {
                dashCount--;
                
                Projectile.NewProjectile(
                        Projectile.GetSource_None(), 
                        player.Center, 
                        Vector2.Zero,
                        ModContent.ProjectileType<dashEffect>(), 0, 0, player.whoAmI
                    );
                player.velocity = player.velocity.DirectionTo(player.Center - Main.MouseWorld)*20f;
                SoundEngine.PlaySound(bassSound with {MaxInstances = 3});

                return false;

            }
            else return false;
        }
        public override void HoldItem(Player player) {
            rd = rand.Next(-100, 100);

            Main.SmartCursorWanted_Mouse = false;        
            Main.SmartCursorWanted_GamePad = false;              

            if(dashCount > 6)
                dashCount = 6;
            
            dashTimer++;
            if(dashTimer % 120 == 0 && dashCount < 6)
                dashCount++;

            if(player.manaFlower)
                player.QuickMana();

            if (player.HeldItem.ModItem is not denierRifle)
                return; 

            if (Main.mouseRight && player.statMana >=15) {
                player.AddBuff(ModContent.BuffType<scopingBuff>(), 1);
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<sigilCircle>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<sigilCircle>(), 0, 0, player.whoAmI
                    );
                }    
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<sigilCircleSmall>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<sigilCircleSmall>(), 0, 0, player.whoAmI
                    );
                } 
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<sigilSquare>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<sigilSquare>(), 0, 0, player.whoAmI
                    );
                } 
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<sigilSquare45deg>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<sigilSquare45deg>(), 0, 0, player.whoAmI
                    );
                }                
            }
            else if(!Main.mouseRight && sigilSquare.canShoot) {
                sigilSquare.canShoot = false;
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
    public class playerStuff:ModPlayer {
        public override void ModifyScreenPosition() {

        }
        public override void PreUpdate() {
            denierRifle.outlineColor = Main.errorColor;
            if (Main.LocalPlayer.HeldItem.ModItem is not denierRifle) {
                denierRifle.dashCount = 0;
            }
        }
    }
}
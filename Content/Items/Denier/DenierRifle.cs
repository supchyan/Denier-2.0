using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Denier.Content.Buffs;
using Denier.Content.GunEffects;
using Denier.Content.Projectiles.SpiritalCircle;
using Denier.Content.Utils.TerrariaOverhaulFixes;

namespace Denier.Content.Items.Denier {
    public class DenierRifle : ModItem {
        public override string Texture => "Denier/Content/Global-Textures/blankPixel";
        public static int dashCount;
        private float dashTimer;
        public static bool scope;
        public static Color outlineColor;
        private Random rand = new Random();
        private int rd;
        public override void SetDefaults() {
            Item.damage = 250;
            Item.width = 64;
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
            Item.shoot = ModContent.ProjectileType<DenierBullet>();
            Item.shootSpeed = 40f;
            Item.alpha = 255;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            Texture2D texture = ModContent.Request<Texture2D>("Denier/Content/Items/Denier/Textures/denierOutline").Value;
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset + new Vector2(0, -10f);
            spriteBatch.Draw(texture, drawPos, frame, outlineColor, rotation, frameOrigin, 2f*scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
			Texture2D texture = ModContent.Request<Texture2D>("Denier/Content/Items/Denier/Textures/denierOutline").Value;
			Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
			Vector2 origin1 = sourceRectangle.Size() / 2f;
			spriteBatch.Draw(texture, position, sourceRectangle, outlineColor, MathHelper.ToRadians(-45), origin1, scale, SpriteEffects.None, 0f);
			return false;
		}
        public override void ModifyTooltips(List<TooltipLine> tooltips) {  
            foreach (TooltipLine line in tooltips) {
				if (line.Mod == "Terraria" && line.Name == "Damage") {
					line.OverrideColor = Main.errorColor;
                    line.Text = Language.GetTextValue("Mods.Denier.BetterTooltips.DenierDamage");
				}
                if (line.Mod == "Terraria" && line.Name == "UseMana") {
					line.OverrideColor = Main.errorColor;
                    line.Text = Language.GetTextValue("Mods.Denier.BetterTooltips.DenierMana");
				}
			}
        }
        public override bool AltFunctionUse(Player player) {
            return true;
        }
        public override bool CanUseItem(Player player) {
            if(!DenierTools.notAtAction(player)) {
                return false;
            }
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
            if(Main.mouseRight && !SigilSquare.canShoot) {
                return false;
            } else {
                Item.useTime = 45;
				Item.useAnimation = 45;
                Item.mana = 15;
                return true;
            }
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0 && Main.mouseRight && SigilSquare.canShoot) {
                SigilSquare.canShoot = false;
                SigilSquare.rotRes = true;
                SigilSquare45deg.rotRes = true;

                if(rd > 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(), 
                        player.Center, 
                        Vector2.Zero,
                        ModContent.ProjectileType<CoolEffect>(), 0, 0, player.whoAmI
                    );
                }
                else {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(), 
                        player.Center, 
                        Vector2.Zero,
                        ModContent.ProjectileType<CoolEffectAlt>(), 0, 0, player.whoAmI
                    );
                }

                Projectile.NewProjectile(
                    Projectile.GetSource_None(),
                    player.Center,
                    player.velocity,
                    ModContent.ProjectileType<Shell>(), 0, 0, player.whoAmI
                );

                return true;
            }
            else if (player.altFunctionUse == 0 && !Main.mouseRight && dashCount > 0) {
                dashCount--;
                
                Projectile.NewProjectile(
                        Projectile.GetSource_None(), 
                        player.Center, 
                        Vector2.Zero,
                        ModContent.ProjectileType<DashEffect>(), 0, 0, player.whoAmI
                    );
                player.velocity = player.velocity.DirectionTo(player.Center - Main.MouseWorld)*20f;

                return false;
            }
            else return false;
        }
        public override void HoldItem(Player player) {
            if(!DenierTools.notAtAction(player)) {
                Item.useStyle = ItemUseStyleID.None;
                return;
            }
            Item.useStyle = ItemUseStyleID.Shoot;

            player.scope = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<DenierInHands>()] == 0) {
                Projectile.NewProjectile(
                    Projectile.GetSource_None(),
                    player.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<DenierInHands>(), 0, 0, player.whoAmI
                );
            }

            rd = rand.Next(-100, 100);

            Main.SmartCursorWanted_Mouse = false;        
            Main.SmartCursorWanted_GamePad = false;              

            if(dashCount > 3)
                dashCount = 3;
            
            dashTimer++;
            if(dashTimer % 120 == 0 && dashCount < 3)
                dashCount++;

            if(player.manaFlower && player.statMana <= 15)
                player.QuickMana();

            if (player.HeldItem.ModItem is not DenierRifle)
                return; 

            if (Main.mouseRight && player.statMana >=15) {
                player.AddBuff(ModContent.BuffType<scopingBuff>(), 1);
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<SigilCircle>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<SigilCircle>(), 0, 0, player.whoAmI
                    );
                }    
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<SigilCircleSmall>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<SigilCircleSmall>(), 0, 0, player.whoAmI
                    );
                } 
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<SigilSquare>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<SigilSquare>(), 0, 0, player.whoAmI
                    );
                } 
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<SigilSquare45deg>()] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_None(),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<SigilSquare45deg>(), 0, 0, player.whoAmI
                    );
                }                
            }
            else if(!Main.mouseRight && SigilSquare.canShoot) {
                SigilSquare.canShoot = false;
            }
        }
        public override Vector2? HoldoutOffset() {
			return new Vector2(-13f, -1f);
        }
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Handgun)
                .AddIngredient(ItemID.EyePatch, 1)
                .AddIngredient(ItemID.SoulofNight, 40)
                .AddIngredient(ItemID.SoulofLight, 40)
                .AddRecipeGroup(Lang.GetItemNameValue(ItemID.TitaniumBar), 25)
                .AddIngredient(ItemID.HellstoneBar, 25)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
    public class playerStuff:ModPlayer {
        public override void ModifyScreenPosition() {

        }
        public override void PreUpdate() {
            DenierRifle.outlineColor = Main.errorColor;
            if (Main.LocalPlayer.HeldItem.ModItem is not DenierRifle) {
                DenierRifle.dashCount = 0;
            }
        }
    }
    public class RecipeTools:ModSystem {
        public override void AddRecipeGroups() {
            RecipeGroup HardModeMetals = new RecipeGroup(() => $"{Lang.GetItemNameValue(ItemID.TitaniumBar)} ({Lang.GetItemNameValue(ItemID.AdamantiteBar)})", ItemID.TitaniumBar, ItemID.AdamantiteBar);
            RecipeGroup.RegisterGroup(Lang.GetItemNameValue(ItemID.TitaniumBar), HardModeMetals);
        }
    }
}
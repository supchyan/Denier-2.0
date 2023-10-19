using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Denier.Content.Items.Denier;

namespace Denier.Content.GunEffects
{
	public class ExampleLastPrismBeam : ModProjectile
	{
		public override string Texture => "Denier/Content/Global-Textures/beam";
		// A helpful math constant for performing beam angling calculations.
		private const float PiBeamDivisor = MathHelper.Pi;

		// Beams increase their scale from 0 to this value as the Prism charges up.
		private const float MaxBeamScale = 1f;

		// The maximum possible range of the beam. Don't set this too high or it will cause significant lag.
		private const float MaxBeamLength = 2400f;

		// The width of the beam in pixels for the purposes of entity hitbox collision.
		// This gets scaled with the beam's scale value, so as the beam visually grows its hitbox gets wider as well.
		private const float BeamHitboxCollisionWidth = 22f;

		// The number of sample points to use when performing a collision hitscan for the beam.
		// More points theoretically leads to a higher quality result, but can cause more lag. 3 tends to be enough.
		private const int NumSamplePoints = 3;

		// How quickly the beam adjusts to sudden changes in length.
		// Every frame, the beam replaces this ratio of its current length with its intended length.
		// Generally you shouldn't need to change this.
		// Setting it too low will make the beam lazily pass through walls before being blocked by them.
		private const float BeamLengthChangeFactor = 0.75f;

		// The charge percentage required on the host prism for the beam to begin visual effects (e.g. impact dust).
		private const float VisualEffectThreshold = 0.1f;

		// The maximum brightness of the light emitted by the beams. Brightness scales from 0 to this value as the Prism's charge increases.
		private const float BeamLightBrightness = 0.75f;

		// This property encloses the internal AI variable Projectile.ai[0]. It makes the code easier to read.
		private float BeamID {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		// This property encloses the internal AI variable Projectile.ai[1].
		private float HostPrismIndex {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		// This property encloses the internal AI variable Projectile.localAI[1].
		// Normally, localAI is not synced over the network. This beam manually syncs this variable using SendExtraAI and ReceiveExtraAI.
		private float BeamLength {
			get => Projectile.localAI[1];
			set => Projectile.localAI[1] = value;
		}

		public override void SetDefaults() {
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			// The beam itself still stops on tiles, but its invisible "source" Projectile ignores them.
			// This prevents the beams from vanishing if the player shoves the Prism into a wall.
			Projectile.tileCollide = false;

			// Using local NPC immunity allows each beam to strike independently from one another.
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		// Send beam length over the network to prevent hitbox-affecting and thus cascading desyncs in multiplayer.
		public override void SendExtraAI(BinaryWriter writer) => writer.Write(BeamLength);
		public override void ReceiveExtraAI(BinaryReader reader) => BeamLength = reader.ReadSingle();

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			// If something has gone wrong with either the beam or the host Prism, destroy the beam.
			Projectile hostPrism = Main.projectile[(int)HostPrismIndex];
			if (Projectile.type != ModContent.ProjectileType<ExampleLastPrismBeam>() || !hostPrism.active || player.HeldItem.ModItem is not DenierExtend) {
				Projectile.Kill();
				return;
			}

			// Grab some variables from the host Prism.
			Vector2 hostPrismDir = Vector2.Normalize(hostPrism.velocity);
			float chargeRatio = MathHelper.Clamp(hostPrism.ai[0], 0f, 1f);

			// This offset is used to make each individual beam orient differently based on its Beam ID.
			float beamIdOffset = BeamID - 1;
			float beamStartSidewaysOffset;
			float beamStartForwardsOffset;

			Projectile.scale = MaxBeamScale;
			Projectile.Opacity = 1f;
			beamStartSidewaysOffset = 6f;
			beamStartForwardsOffset = -17f;

			// The amount to which the angle changes reduces over time so that the beams look like they are focusing.
			float deviationAngle = 0;

			// This trigonometry calculates where the beam is supposed to be pointing.
			Vector2 unitRot = Vector2.UnitY.RotatedBy(deviationAngle);
			Vector2 yVec = new Vector2(4f, beamStartSidewaysOffset);
			float hostPrismAngle = hostPrism.velocity.ToRotation();
			Vector2 beamSpanVector = (unitRot * yVec).RotatedBy(hostPrismAngle);

			// Calculate the beam's emanating position. Start with the Prism's center.
			Projectile.Center = hostPrism.Center;
			// Add a fixed offset to align with the Prism's sprite sheet.
			Projectile.position += hostPrismDir * 16f + new Vector2(0f, -hostPrism.gfxOffY);
			// Add the forwards offset, measured in pixels.
			Projectile.position += hostPrismDir * beamStartForwardsOffset;
			// Add the sideways offset vector, which is calculated for the current angle of the beam and scales with the beam's sideways offset.
			Projectile.position += beamSpanVector;

			// Set the beam's velocity to point towards its current spread direction and sanity check it. It should have magnitude 1.
			Projectile.velocity = hostPrismDir;
			if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero) {
				Projectile.velocity = -Vector2.UnitY;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			// Update the beam's length by performing a hitscan collision check.
			float hitscanBeamLength = PerformBeamHitscan(hostPrism, chargeRatio >= 1f);
			BeamLength = MathHelper.Lerp(BeamLength, hitscanBeamLength, BeamLengthChangeFactor);

			// This Vector2 stores the beam's hitbox statistics. X = beam length. Y = beam width.
			Vector2 beamDims = new Vector2(Projectile.velocity.Length() * BeamLength, Projectile.width * Projectile.scale);

			// Only produce dust and cause water ripples if the beam is above a certain charge level.
			Color beamColor = Color.Red;
			if (chargeRatio >= VisualEffectThreshold) {
				// If the game is rendering (i.e. isn't a dedicated server), make the beam disturb water.
				if (Main.netMode != NetmodeID.Server) {
					ProduceWaterRipples(beamDims);
				}
			}

			// Make the beam cast light along its length. The brightness of the light scales with the charge.
			// v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight.
			DelegateMethods.v3_1 = beamColor.ToVector3() * BeamLightBrightness * chargeRatio;
			Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, beamDims.Y, new Utils.TileActionAttempt(DelegateMethods.CastLight));
		}

		private float PerformBeamHitscan(Projectile prism, bool fullCharge) {
			// By default, the hitscan interpolation starts at the Projectile's center.
			// If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
			Vector2 samplingPoint = Projectile.Center;
			if (fullCharge) {
				samplingPoint = prism.Center;
			}

			// Overriding that, if the player shoves the Prism into or through a wall, the interpolation starts at the player's center.
			// This last part prevents the player from projecting beams through walls under any circumstances.
			Player player = Main.player[Projectile.owner];
			if (!Collision.CanHitLine(player.Center, 0, 0, prism.Center, 0, 0)) {
				samplingPoint = player.Center;
			}

			// Perform a laser scan to calculate the correct length of the beam.
			// Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
			// return MaxBeamLength;
			float[] laserScanResults = new float[NumSamplePoints];
			Collision.LaserScan(samplingPoint, Projectile.velocity, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
			float averageLengthSample = 0f;
			for (int i = 0; i < laserScanResults.Length; ++i) {
				averageLengthSample += laserScanResults[i];
			}
			averageLengthSample /= NumSamplePoints;

			return averageLengthSample;
		}

		// Determines whether the specified target hitbox is intersecting with the beam.
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
			if (projHitbox.Intersects(targetHitbox)) {
				return true;
			}

			// Otherwise, perform an AABB line collision check to check the whole beam.
			float _ = float.NaN;
			Vector2 beamEndPos = Projectile.Center + Projectile.velocity * BeamLength;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
		}

		public override bool PreDraw(ref Color lightColor) {
			// If the beam doesn't have a defined direction, don't draw anything.
			if (Projectile.velocity == Vector2.Zero) {
				return false;
			}

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * 10.5f;
			Vector2 drawScale = new Vector2(Projectile.scale);

			// Reduce the beam length proportional to its square area to reduce block penetration.
			float visualBeamLength = BeamLength - 14.5f * Projectile.scale * Projectile.scale;

			DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
			Vector2 startPosition = centerFloored - Main.screenPosition;
			Vector2 endPosition = startPosition + Projectile.velocity * visualBeamLength;

			// Draw the outer beam.
			drawScale *= 0.1f;
			DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale, Color.Red);

			// Returning false prevents Terraria from trying to draw the Projectile itself.
			return false;
		}

		private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor) {
			Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

			// c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw.
			DelegateMethods.c_1 = beamColor;
			Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
		}
		private void ProduceWaterRipples(Vector2 beamDims) {
			WaterShaderData shaderData = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

			// A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
			float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
			Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);

			// WaveData is encoded as a Color. Not really sure why.
			Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
			shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
		}


		// Automatically iterates through every tile the laser is overlapping to cut grass at all those locations.
		public override void CutTiles() {
			// tilecut_0 is an unnamed decompiled variable which tells CutTiles how the tiles are being cut (in this case, via a Projectile).
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.TileActionAttempt cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);
			Vector2 beamStartPos = Projectile.Center;
			Vector2 beamEndPos = beamStartPos + Projectile.velocity * BeamLength;

			// PlotTileLine is a function which performs the specified action to all tiles along a drawn line, with a specified width.
			// In this case, it is cutting all tiles which can be destroyed by Projectiles, for example grass or pots.
			Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
		}
	}
}
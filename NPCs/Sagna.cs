﻿using ChensGradiusMod.Projectiles.Enemies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace ChensGradiusMod.NPCs
{
  public class Sagna : GradiusEnemy
  {
    private readonly int cancelDeployThreshold = 500;
    private readonly float xSpeed = 3f;
    private readonly float yGravity = .1f;
    private readonly float maxYSpeed = 5f;
    private readonly int jumpCountForSpray = 2;

    private bool initialized = false;
    private int persistDirection = 0;
    private int animateDirection = 1;
    private States mode = States.Fall;
    private int xDirection = 0;
    private int yDirection = 0;
    private int jumpCount = 0;
    private int startFrame = 0;
    private int endFrame = 4;
    private int maxFrame = 8;

    public enum States { Hop, Fall, Spray };

    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Sagna");
      Main.npcFrameCount[npc.type] = 16;
    }

    public override void SetDefaults()
    {
      base.SetDefaults();

      npc.width = 28;
      npc.height = 28;
      npc.damage = 100;
      npc.lifeMax = 130;
      npc.value = 2000f;
      npc.knockBackResist = 0f;
      npc.defense = 50;
      npc.noGravity = true;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
      if (Main.hardMode && spawnInfo.spawnTileY < GradiusHelper.UnderworldTilesYLocation &&
          spawnInfo.spawnTileY > (Main.worldSurface - Main.worldSurface * .1f))
      {
        return .05f;
      }
      else return 0f;
    }

    public override string Texture => "ChensGradiusMod/Sprites/Sagna";

    public override bool PreAI()
    {
      if (!initialized)
      {
        initialized = true;
        npc.TargetClosest(false);
        if (npc.Center.X > Main.player[npc.target].Center.X) persistDirection = -1;
        else persistDirection = 1;
        xDirection = persistDirection;
        yDirection = DecideDeploy();
        if (yDirection < 0)
        {
          startFrame = 8;
          endFrame = 12;
          maxFrame = Main.npcFrameCount[npc.type];
          FrameCounter = startFrame;
        }
      }

      return initialized;
    }

    public override void AI()
    {
      npc.spriteDirection = npc.direction = persistDirection;

      MoveHorizontally();
      MoveVertically();
      if (mode != States.Spray) AdjustMovementBehavior();
    }

    public override void FindFrame(int frameHeight)
    {
      if (++FrameTick >= FrameSpeed)
      {
        FrameTick = 0;
        if (mode == States.Spray)
        {
          FrameCounter += animateDirection;
          if (animateDirection > 0 && FrameCounter >= maxFrame)
          {
            animateDirection = -1;
            FrameCounter -= 1;
            PerformAttack();
          }
          else if (animateDirection < 0 && FrameCounter < endFrame)
          {
            animateDirection = 1;
            mode = States.Fall;
          }
        }
        else
        {
          FrameCounter += animateDirection;
          if (FrameCounter >= endFrame) FrameCounter = startFrame;
        }
      }

      npc.frame.Y = frameHeight * FrameCounter;
    }

    protected override float RetaliationBulletSpeed => base.RetaliationBulletSpeed * 0.9f;

    protected override int RetaliationSpreadBulletNumber => 10;

    protected override float RetaliationSpreadAngleDifference => 180f;

    protected override int FrameSpeed { get; set; } = 6;

    private void SetPositionAndVelocity(Vector2 p, Vector2 v)
    {
      npc.position = p;
      npc.velocity = v;
    }

    private int DecideDeploy()
    {
      Vector2 savedPosition = npc.position,
              upwardV = new Vector2(0, -npc.height),
              downwardV = new Vector2(0, npc.height),
              upwardP = npc.position,
              downwardP = npc.position,
              velocityOnCollide;

      for (int i = 0; i < cancelDeployThreshold; i++)
      {
        velocityOnCollide = Collision.TileCollision(downwardP, downwardV, npc.width, npc.height);
        if (downwardV != velocityOnCollide)
        {
          SetPositionAndVelocity(savedPosition, Vector2.Zero);
          return 1;
        }
        else downwardP += downwardV;

        velocityOnCollide = Collision.TileCollision(upwardP, upwardV, npc.width, npc.height);
        if (upwardV != velocityOnCollide)
        {
          SetPositionAndVelocity(savedPosition, Vector2.Zero);
          return -1;
        }
        else upwardP += upwardV;
      }

      return 1;
    }

    private void MoveHorizontally()
    {
      switch (mode)
      {
        case States.Hop:
        case States.Fall:
          npc.velocity.X = xSpeed * xDirection;
          break;
        case States.Spray:
          npc.velocity.X = 0f;
          break;
      }
    }

    private void MoveVertically()
    {
      switch (mode)
      {
        case States.Hop:
          npc.velocity.Y += yGravity * yDirection;
          if (yDirection > 0) npc.velocity.Y = Math.Min(npc.velocity.Y, 0f);
          else npc.velocity.Y = Math.Max(npc.velocity.Y, 0f);
          break;
        case States.Fall:
          npc.velocity.Y += yGravity * yDirection;
          if (yDirection > 0) npc.velocity.Y = Math.Min(npc.velocity.Y, maxYSpeed);
          else npc.velocity.Y = Math.Max(npc.velocity.Y, -maxYSpeed);
          break;
        case States.Spray:
          npc.velocity.Y = 0f;
          break;
      }
    }

    private void AdjustMovementBehavior()
    {
      Vector2 beforeVelocity = npc.velocity;
      npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height);

      if (beforeVelocity.X != npc.velocity.X) xDirection = -xDirection;
      
      switch (mode)
      {
        case States.Hop:
          if (yDirection > 0 && npc.velocity.Y >= 0f ||
              yDirection < 0 && npc.velocity.Y <= 0f)
          {
            npc.velocity.Y = 0f;
            if (++jumpCount >= jumpCountForSpray)
            {
              jumpCount = 0;
              mode = States.Spray;
            }
            else mode = States.Fall;
          }
          break;
        case States.Fall:
          if (beforeVelocity.Y != npc.velocity.Y)
          {
            mode = States.Hop;
            npc.velocity.Y = maxYSpeed * -yDirection;
          }
          break;
      }
    }

    private void PerformAttack()
    {
      if (GradiusHelper.IsNotMultiplayerClient())
      {
        float radianAngle = 0f;
        for (int i = 0; i < 8; i++)
        {
          Projectile.NewProjectile(npc.Center, radianAngle.ToRotationVector2() * GradiusEnemyBullet.Spd,
                                   ModContent.ProjectileType<GradiusEnemyBullet>(),
                                   GradiusEnemyBullet.Dmg, GradiusEnemyBullet.Kb, Main.myPlayer);
          radianAngle += MathHelper.PiOver4;
        }
      }
    }
  }
}
﻿using ChensGradiusMod.Items.Placeables.MusicBoxes;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ChensGradiusMod.Tiles.MusicBoxes
{
  public class PositionLightMusicBoxTile : GradiusMusicBoxTile
  {
    protected override Color MinimapColor => new Color(200, 200, 200);

    protected override string MusicName => "The Position Light";

    protected override int ItemType => ModContent.ItemType<PositionLightMusicBox>();
  }
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChensGradiusMod.Items.Accessories.Options.Aim
{
  public class AimOptionTwo : AimOptionBase
  {
    public override void SetStaticDefaults()
    {
      base.SetStaticDefaults();

      DisplayName.SetDefault("Option type Aim (Second)");
    }

    public override void SetDefaults()
    {
      base.SetDefaults();

      item.rare = 4;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
      ModPlayer(player).optionTwo = true;
      ModPlayer(player).aimOption = true;

      base.UpdateAccessory(player, hideVisual);
    }

    protected override string ProjectileName => "OptionTwoObject";

    protected override int OptionPosition => 2;

    public override void AddRecipes()
    {
      ModRecipe recipe = new ModRecipe(mod);
      recipe.AddIngredient(mod, "OptionTwo");
      recipe.AddIngredient(ItemID.SuspiciousLookingEye, 10);
      recipe.AddIngredient(ItemID.SoulofLight, 5);
      recipe.AddIngredient(ItemID.SoulofNight, 3);
      recipe.AddRecipeGroup("ChensGradiusMod:CobaltTierBar", 10);
      recipe.AddRecipeGroup("ChensGradiusMod:TinTierBar", 40);
      recipe.AddIngredient(ItemID.Wire, 200);
      recipe.AddTile(TileID.TinkerersWorkbench);
      recipe.AddTile(TileID.AmmoBox);
      recipe.SetResult(this);
      recipe.AddRecipe();
    }
  }
}
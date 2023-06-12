using UnityEngine;

// DO NOT CHANGE THE ORDER OF THE ENUMS, ADD ITEMS TO THE END

public enum CursorType { MELEE, RANGE, MAGIC, PICKAXE, AXE, UNARMED, BUILD, MENU };
public enum PlayerMode { MELEE, RANGE, MAGIC, PICKAXE, AXE, UNARMED, BUILD };
public enum AttackType { MELEE, RANGE, MAGIC, PICKAXE, AXE };
public enum AimingMode { CAMERA, CURSOR };
public enum BlockType { AIR, DIRT, DIRT_2, SAND, SANDSTONE, LAVA, LAVA_2, ROCK, SNOW, ICE, GRASS  };
public enum GAMESTATE { PLAY, GAME_OVER, MAIN_MENU, PAUSE_MENU, SETTINGS_MENU, LOADING, SCREAMER };

public class EnumConverter
{
    public static CursorType CursorTypeFromPlayerMode(PlayerMode mode)
    {
        // Assumes both enums have the same value at the same index, might change in the future
        return (CursorType) mode;
    }

    public static AttackType AttackTypeFromPlayerMode(PlayerMode mode)
    {
        // Assumes both enums have the same value at the same index, might change in the future
        return (AttackType) mode;
    }
    public static PlayerMode PlayerModeFromAttackType(AttackType type)
    {
        // Assumes both enums have the same value at the same index, might change in the future
        return (PlayerMode) type;
    }

    public static string StringFromPlayerMode(PlayerMode mode)
    {
        return mode switch
        {
            PlayerMode.MELEE => "Melee",
            PlayerMode.MAGIC => "Magic",
            PlayerMode.RANGE => "Range",
            PlayerMode.UNARMED => "Unarmed",
            PlayerMode.PICKAXE => "Pickaxe",
            PlayerMode.BUILD => "Build",
            _ => "Unknown",
        };
    }

    public static Color ColorFromBlockType(BlockType type)
    {
        // Used by the minimap
        return type switch
        {
            BlockType.AIR => Color.clear,
            BlockType.DIRT => Color.green,
            BlockType.DIRT_2 => Color.green,
            BlockType.ICE => Color.cyan,
            BlockType.LAVA => Color.red,
            BlockType.LAVA_2 => Color.red,
            BlockType.ROCK => Color.gray,
            BlockType.SAND => Color.yellow,
            BlockType.SANDSTONE => Color.yellow,
            BlockType.SNOW => Color.white,
            _ => Color.clear,
        };
    }
}

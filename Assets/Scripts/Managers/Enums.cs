// DO NOT CHANGE THE ORDER OF THE ENUMS, ADD ITEMS TO THE END
public enum CursorType { MELEE, RANGE, MAGIC, UNARMED, PICKAXE, AXE, MENU };
public enum PlayerMode { MELEE, RANGE, MAGIC, UNARMED, PICKAXE, AXE };
public enum AttackType { MELEE, RANGE, MAGIC };
public enum AimingMode { CAMERA, CURSOR };
public enum GAMESTATE { PLAY, GAME_OVER, MAIN_MENU, PAUSE_MENU, SETTINGS_MENU, LOADING };

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
}

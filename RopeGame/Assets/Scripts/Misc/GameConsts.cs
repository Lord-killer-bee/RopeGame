public class GameConsts
{
    #region Input related

    public const string HORIZONTAL_CODE = "Horizontal";
    public const string VERTICAL_CODE = "Vertical";
    public const string LATCH_CODE = "Latch";
    public const string BAND_CODE = "Band";
    public const string JUMP_CODE = "Jump";

    #endregion

    #region Tags and layers

    public const string LATCHPOINT_TAG = "LatchPoint";
    public const string WINDINGTARGET_TAG = "WindingTarget";
    public const string LEVELEND_TAG = "LevelEnd";

    public const int PLATFORM_LAYER = 1<<8;
    public const int LATCHPOINTS_LAYER = 1<<9;

    #endregion

    #region Player prefs

    public const string CURRENTLEVEL_KEY = "CurrentLevel";

    #endregion
}

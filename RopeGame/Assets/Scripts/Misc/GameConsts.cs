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
    public const string LEVELEND_TAG = "LevelEnd";
    public const string SPIKE_TAG = "Spike";
    public const string PLAYER_TAG = "Player";
    public const string RECORDERBUTTON_TAG = "RecorderButton";
    public const string KEY_TAG = "Key";

    public const int PLATFORM_LAYER = 1<<8;
    public const int LATCHPOINTS_LAYER = 1<<9;

    #endregion

    #region Player prefs

    public const string CURRENTLEVEL_KEY = "CurrentLevel";

    #endregion
}

using TaleWorlds.Localization;

namespace AllHeroesAreVisibleInBattle.Core;

internal static class Texts
{
    #region Settings
    public static readonly TextObject ModName = new(Ids.ModName);
    #endregion

    internal static class Ids
    {
        #region Settings
        public const string ModName = "{=AHAVIB_XGHAHVA}All Heroes Are Visible In Battle";
        public const string GroupGeneral = "{=AHAVIB_qhfqjNf}General";
        public const string HighlightMode = "{=AHAVIB_IGpDb3w}Highlight Mode";
        public const string HighlightModeHint = "{=AHAVIB_mpbOfUj}Hold: highlights only while the key is held. Toggle: press once to turn on, press again to turn off.";
        public const string ModeHold = "{=AHAVIB_MD2uxJv}Hold";
        public const string ModeToggle = "{=AHAVIB_rXGdKCl}Toggle";
        public const string KeyboardKey = "{=AHAVIB_9U5WAxT}Keyboard Key";
        public const string KeyboardKeyHint = "{=AHAVIB_TRmYzya}Keyboard / mouse key that triggers the highlight. Works at the same time as the controller button.";
        public const string ControllerKey = "{=AHAVIB_Z0Sthax}Controller Button";
        public const string ControllerKeyHint = "{=AHAVIB_Hr08eFW}Controller button that triggers the highlight. Default is None to avoid clashing with vanilla combat bindings.";
        public const string HighlightDistance = "{=AHAVIB_G8b0Yhl}Highlight Distance";
        public const string HighlightDistanceHint = "{=AHAVIB_pXZG4OQ}Heroes further than this from the camera are not outlined. Default 30.";
        public const string ShowNames = "{=AHAVIB_M8hnjkJ}Show Names";
        public const string ShowNamesHint = "{=AHAVIB_DBOWj8p}Show the hero's name above their head while highlighted, in the same colour as the outline.";
        public const string GroupTechnical = "{=AHAVIB_4Mz1FTK}Technical settings";
        public const string LoggingEnabled = "{=AHAVIB_Eusi1Fl}Logging for debugging";
        public const string LoggingEnabledHint = "{=AHAVIB_qG1M3vg}Logging for debugging (English only). [Default: disabled]";
        #endregion

        #region Keyboard keys
        public const string KeyNone = "{=AHAVIB_By7fwJl}None";
        public const string KeyLeftAlt = "{=AHAVIB_TaVU9w0}Left Alt";
        public const string KeyRightAlt = "{=AHAVIB_KjnHnX8}Right Alt";
        public const string KeyLeftCtrl = "{=AHAVIB_DgHXJbV}Left Ctrl";
        public const string KeyRightCtrl = "{=AHAVIB_9HQF37j}Right Ctrl";
        public const string KeyLeftShift = "{=AHAVIB_Pr5t93m}Left Shift";
        public const string KeyRightShift = "{=AHAVIB_2vBNAO3}Right Shift";
        public const string KeyCapsLock = "{=AHAVIB_GSOIStK}Caps Lock";
        public const string KeyTilde = "{=AHAVIB_uuLi3dv}Tilde (~)";
        public const string KeySpace = "{=AHAVIB_SlH069C}Space";
        public const string KeyMiddleMouse = "{=AHAVIB_Dbw36Du}Middle Mouse Button";
        public const string KeyMouse4 = "{=AHAVIB_harmWn3}Mouse Button 4";
        public const string KeyMouse5 = "{=AHAVIB_o7wElX9}Mouse Button 5";
        #endregion

        #region Controller buttons
        public const string PadLeftBumper = "{=AHAVIB_S46ybFf}Left Bumper (LB)";
        public const string PadRightBumper = "{=AHAVIB_OGVyJgI}Right Bumper (RB)";
        public const string PadLeftTrigger = "{=AHAVIB_XGdRCo0}Left Trigger (LT)";
        public const string PadRightTrigger = "{=AHAVIB_QyyVpW6}Right Trigger (RT)";
        public const string PadLeftStick = "{=AHAVIB_K8jcDhg}Left Stick Click (L3)";
        public const string PadRightStick = "{=AHAVIB_pWE4Cv4}Right Stick Click (R3)";
        public const string PadView = "{=AHAVIB_fjBAPdL}View / Back";
        public const string PadMenu = "{=AHAVIB_d5tuuHv}Menu / Start";
        public const string PadDPadUp = "{=AHAVIB_kJfinXP}D-Pad Up";
        public const string PadDPadDown = "{=AHAVIB_YRWSWqS}D-Pad Down";
        public const string PadDPadLeft = "{=AHAVIB_I2B3vV3}D-Pad Left";
        public const string PadDPadRight = "{=AHAVIB_A4aYmze}D-Pad Right";
        public const string PadFaceUp = "{=AHAVIB_GE95O2m}Face Up (Y)";
        public const string PadFaceDown = "{=AHAVIB_Jt8H37Y}Face Down (A)";
        public const string PadFaceLeft = "{=AHAVIB_12gprT8}Face Left (X)";
        public const string PadFaceRight = "{=AHAVIB_BKKaO0D}Face Right (B)";
        #endregion
    }
}

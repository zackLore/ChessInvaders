using UnityEngine;
using System.Collections;


namespace Assets.Scripts
{
    public class Consts
    {
        // Reference for keeping the different parts of the game in the right z order
        public const float zPos_Background        = 100.0f;
        public const float zPos_BackgroundSquare  = 50.0f;
        public const float zPos_GameSquare        = 25.0f;
        public const float zPos_Piece             = 0.0f;
        public const float zPos_PlayerUI          = -25.0f;
        public const float zPos_Menu              = -100.0f;

        // Coordinate Boundaries
        public const int minRow = 0;
        public const int maxRow = 7;
        public const int minCol = 0;
        public const int maxCol = 7;

        // Colors
        static private Color _disabledMultiplier = new Color(1, 1, 1, 0.5f);

        // Move Button
        static private Color _moveButtonColor_inactive = Color.black;
        static public Color moveButtonColor_inactive
        {
            get { return _moveButtonColor_inactive; }
        }

        static private Color _moveButtonColor_inactive_disabled = _moveButtonColor_inactive * _disabledMultiplier;
        static public Color moveButtonColor_inactive_disabled
        {
            get { return _moveButtonColor_inactive_disabled; }
        }

        static private Color _moveButtonColor_active = Color.cyan;
        static public Color moveButtonColor_active
        {
            get { return _moveButtonColor_active;  }
        }

        static private Color _moveButtonColor_active_disabled = _moveButtonColor_active * _disabledMultiplier;
        static public Color moveButtonColor_active_disabled
        {
            get { return _moveButtonColor_active_disabled; }
        }

        // Attack Button
        static private Color _attackButtonColor_inactive = Color.black;
        static public Color attackButtonColor_inactive
        {
            get { return _attackButtonColor_inactive; }
        }

        static private Color _attackButtonColor_inactive_disabled = _attackButtonColor_inactive * _disabledMultiplier;
        static public Color attackButtonColor_inactive_disabled
        {
            get { return _attackButtonColor_inactive_disabled; }
        }

        static private Color _attackButtonColor_active = new Color(0.788f, 0.47f, 0.47f, 1f);
        static public Color attackButtonColor_active
        {
            get { return _attackButtonColor_active; }
        }

        static private Color _attackButtonColor_active_disabled = _attackButtonColor_active * _disabledMultiplier;
        static public Color attackButtonColor_active_disabled
        {
            get { return _attackButtonColor_active_disabled; }
        }

        // Defend Button
        static private Color _defendButtonColor_inactive = Color.black;
        static public Color defendButtonColor_inactive
        {
            get { return _defendButtonColor_inactive; }
        }

        static private Color _defendButtonColor_inactive_disabled = _defendButtonColor_inactive * _disabledMultiplier;
        static public Color defendButtonColor_inactive_disabled
        {
            get { return _defendButtonColor_inactive_disabled; }
        }

        static private Color _defendButtonColor_active = new Color(1f, 1f, 0f, 1f);
        static public Color defendButtonColor_active
        {
            get { return _defendButtonColor_active; }
        }

        static private Color _defendButtonColor_active_disabled = _defendButtonColor_active * _disabledMultiplier;
        static public Color defendButtonColor_active_disabled
        {
            get { return _defendButtonColor_active_disabled; }
        }
    }   
}

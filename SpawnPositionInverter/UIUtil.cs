using ColossalFramework.UI;
using UnityEngine;

namespace SpawnPositionInverter
{
    public static class UiUtil
    {
        private const int ButtonSize = 16;

        public static UISprite CreateSwapSptite(UIComponent parentComponent, MouseEventHandler handler)
        {
            return CreateSprite("InvertPositionsButton", null, new Vector3(),
                parentComponent, handler);
        }

        public static UISprite CreateSprite(string buttonName, string tooltip, Vector3 offset,
            UIComponent parentComponent, MouseEventHandler handler)
        {

            var sprite = UIView.GetAView().AddUIComponent(typeof(UISprite)) as UISprite;
            if (sprite == null)
            {
                return null;
            }
            sprite.canFocus = false;
            sprite.name = buttonName;
            sprite.width = ButtonSize;
            sprite.height = ButtonSize;
            sprite.tooltip = tooltip;
            sprite.eventClick += handler;
            sprite.AlignTo(parentComponent, UIAlignAnchor.TopRight);
            sprite.relativePosition = new Vector3 (11, 265)  + offset;
            return sprite;
        }

        public static UILabel CreateLabel(string text, UIComponent parentComponent, Vector3 offset)
        {
            var label = UIView.GetAView().AddUIComponent(typeof(UILabel)) as UILabel;
            if (label == null)
            {
                return null;
            }
            label.text = text;
            label.AlignTo(parentComponent, UIAlignAnchor.TopRight);
            label.relativePosition = new Vector3(11 + ButtonSize + 2, 265) + offset;
            return label;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public abstract class BaseSquare : BaseBehavior
    {
        public SpriteRenderer spriteRenderer;
        public Structs.Coordinate Coord;

        public void SetColor(Color newColor)
        {
            spriteRenderer.color = newColor;
        }
    }
}
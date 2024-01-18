using System;

namespace GXPEngine.Core
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        override public string ToString()
        {
            return "[Vector2 " + x + ", " + y + "]";
        }

        public Vector2 NormalizeVector()
        {
            double length = Math.Sqrt(x * x + y * y);
            return new Vector2((float)(x / length), (float)(y / length));
        }
        public void PingPong(float time, Vector2 startXY, Vector2 endXY)
        {
            float delta = 1 - Mathf.Abs(time % 2f - 1f);
            x = startXY.x + delta * (endXY.x - startXY.x);
            y = startXY.y + delta * (endXY.y - startXY.y);
        }
    }
}


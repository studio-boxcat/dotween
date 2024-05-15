using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public static class JumpUtils
    {
        public static Vector2 Calculate(
            float t,
            float startX, float startY,
            float endX, float endY,
            float jumpPower)
        {
            Assert.AreNotEqual(float.NaN, startX);
            Assert.AreNotEqual(float.NaN, startY);

            // Calculate X
            var posX = Mathf.Lerp(startX, endX, t);

            // Calculate Y
            // Linear part (OutQuad): -t * (t - 2)
            // Jump part (OutQuad with Yoyo): 4 * -t * (t - 1)
            var jumpProgress = 4 * -t * (t - 1); // 0 -> 1 -> 0
            var jumpPart = jumpPower * jumpProgress;
            var linearPart = Mathf.Lerp(startY, endY, -t * (t - 2));
            var posY = linearPart + jumpPart;

            return new Vector2(posX, posY);
        }
    }
}
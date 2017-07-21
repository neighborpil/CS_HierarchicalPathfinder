using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01
{
    public static class Helpers
    {
        /// <summary>
        /// 확장 할 수 있는(이웃과 접할 수 있는) 최대의 가장자리 수 반환
        /// </summary>
        /// <param name="tileType"></param>
        /// <returns></returns>
        public static int GetMaxEdges(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Hex:
                    return 6;
                case TileType.Octile:
                case TileType.OctileUnicost:
                    return 8;
                case TileType.Tile:
                    return 4;
            }
            return 0;
        }

        /// <summary>
        /// 동일한지 반환
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool AreAligned(Position p1, Position p2)
        {
            return p1.X == p2.X || p1.Y == p2.Y;
        }
    }
}

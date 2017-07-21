using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01.Factories
{
    public class ConcreteMapFactory
    {
        public static ConcreteMap CreateConcreteMap(int width, int height, IPassability passability, TileType tilingType = TileType.Octile)
        {
            ConcreteMap tiling = new ConcreteMap(tilingType, width, height, passability);

            return tiling;
        }
    }
}

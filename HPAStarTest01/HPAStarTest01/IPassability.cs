using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01
{
    public interface IPassability
    {
        /// <summary>
        /// Tells whether for a given position this passability class can enter or not.
        /// 이 클래스에 들어갈 수 있는지 없는지 알려줌
        /// </summary>
        /// <returns></returns>
        bool CanEnter(Position pos, out int movementCost);
    }
}

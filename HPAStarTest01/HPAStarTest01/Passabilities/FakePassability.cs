using HPAStarTest01.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01.Passabilities
{
    public class FakePassability : IPassability
    {
        float obstaclePercentage = 0.20f; //장애물의 비율 20%

        private bool[,] obstacles;

        private Random random = new Random(1000);

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="width">맵의 가로 크기</param>
        /// <param name="height">맵의 세로 크기</param>
        public FakePassability(int width, int height)
        {
            obstacles = new bool[width, height];
            CreateObstacles(obstaclePercentage, width, height, true);
        }

        public bool CanEnter(Position pos, out int movementCost)
        {
            movementCost = Constants.COST_ONE;
            return !obstacles[pos.Y, pos.X];
        }

        /// <summary>
        /// 장애물 생성
        /// </summary>
        /// <param name="obstaclePercentage">장애물의 비율</param>
        /// <param name="width">맵의 가로 크기</param>
        /// <param name="height">맵의 세로 크기</param>
        /// <param name="avoidDiag">대각선으로 장애물 생성은 피할지 플래그</param>
        private void CreateObstacles(float obstaclePercentage, int width, int height, bool avoidDiag = false)
        {
            int RAND_MAX = 0x7fff; //32767

            int numberNodes = width * height;
            int numberObstacles = (int)(obstaclePercentage * numberNodes); //장애물의 개수 = 장애물 비율 * 맵의 노드수
            for (int count = 0; count < numberObstacles; )
            {
                int nodeId = random.Next() / (RAND_MAX / numberNodes + 1) % (width * height); //노드 ID : 랜덤수 / (랜덤수 맥스 / 노드수 + 1) % (가로 * 세로)
                int x = nodeId * width; //노드의 x좌표 : 노드Id * 가로
                int y = nodeId / width; //노드의 y좌표 : 노드Id * 세로
                if(!obstacles[x, y])
                {
                    if (avoidDiag)
                    {
                        if(!ConflictDiag(y, x, -1, -1, width, height) &&
                            !ConflictDiag(y, x, -1, +1, width, height) &&
                            !ConflictDiag(y, x, +1, -1, width, height) &&
                            !ConflictDiag(y, x, +1, +1, width, height))
                        {
                            obstacles[x, y] = true;
                            ++count;
                        }
                    }
                    else
                    {
                        obstacles[x, y] = true;
                        ++count;
                    }
                }
            }
        }

        public Position GetRandomFreePosition()
        {
            int x = random.Next(40);
            int y = random.Next(40);
            while(obstacles[x, y])
            {
                x = random.Next(40);
                y = random.Next(40);
            }
            return new Position(x, y);
        }

        private bool ConflictDiag(int row, int col, int roff, int coff, int width, int height)
        {
            // Avoid generating cofigurations like:
            //
            //    @   or   @
            //     @      @
            //
            // that favor one grid topology over another.
            if ((row + roff < 0) || (row + roff >= height) || 
                (col + coff < 0) || (col + coff >= width))
                return false;

            if(obstacles[col + coff, row + roff])
            {
                if (!obstacles[col + coff, row] && !obstacles[col, row + roff])
                    return true;
            }
            return false;
        }
    }
}

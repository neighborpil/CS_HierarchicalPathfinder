﻿using HPAStarTest01.Infrastructure;
using System;
using System.Linq;

namespace HPAStarTest01
{
    public partial class Program
    {
        public class ExamplePassability : IPassability
        {
            private string map = @"
                    0000000000000000000100000000000000000000
                    0111111111111111111111111111111111111110
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0001000000000000000100000000000000000000
                    0001000000000000000100000000000000000000
                    0001000000000000000100000000000000000000
                    0001000000000000000100000000000000000000
                    0001000000000000000100000000000000000000
                    0001000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0111111111111111111100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0000000000000000000100000000000000000000
                    0111111111111111111111111111111111111110
                    0000000000000000000000000000000000000000
                ";

            private bool[,] obstacles;
            Random rnd = new Random(700);

            /// <summary>
            /// string을 읽어들여 2차원 배열 bool[,]로 생성
            /// </summary>
            public ExamplePassability()
            {
                obstacles = new bool[40, 40];
                char[][] charLines = map.Split('\n')
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.ToCharArray()).ToArray();
                for (int j = 0; j < obstacles.GetLength(1); j++)
                {
                    for (int i = 0; i < obstacles.GetLength(0); i++)
                    {
                        obstacles[i, j] = (charLines[i][j] == '1');
                    }
                }

            }

            /// <summary>
            /// 랜덤 위치를 받는다
            /// </summary>
            /// <returns></returns>
            public Position GetRandomFreePosition()
            {
                int x = rnd.Next(40);
                int y = rnd.Next(40);
                while(obstacles[x, y])
                {
                    x = rnd.Next(40);
                    y = rnd.Next(40);
                }

                return new Position(x, y);
            }

            /// <summary>
            /// Position이 열려 있는지 확인
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="movementCost"></param>
            /// <returns></returns>
            public bool CanEnter(Position pos, out int movementCost)
            {
                movementCost = Constants.COST_ONE;
                return !obstacles[pos.Y, pos.X];
            }
        }
    }
    
}
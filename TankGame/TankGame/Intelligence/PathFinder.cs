using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankGame.Terrain;
using TankGame.Network;
using System.Collections;

namespace TankGame.Intelligence
{

    // Pathfinder class should find the best move when the present details are given

    class PathFinder
    {
        private World world;
        BFS bfs;

        public PathFinder(World wrld)
        {
            this.world = wrld;
            bfs = new BFS(world);

        }

        public string getNextMove(int CurrentP)
        {
            //returns the next move direction if no move is found, tank will shoot
            int val = bfs.findNextMove(CurrentP);
            Console.WriteLine(val);
            if (val == 0)
                return ConfigData.up;
            else if (val == 1)
                return ConfigData.down;
            else if (val == 2)
                return ConfigData.left;
            else if (val == 3)
                return ConfigData.right;
            else
                return ConfigData.shoot;
        }

    }

    class BFS
    {
        private World world;
        private int currentPos;
        private int mapSize = ConfigData.MAP_SIZE * ConfigData.MAP_SIZE;
        private int destCell;
        private Cell start;
        private Cell goal;

        public BFS(World w)
        {
            this.world = w;
        }


        public int findNextMove(int currP)
        {
            this.currentPos = currP;
            
            this.start = world.getCell(currentPos);

            //if the tank is on a coin pile or a life pack then that coin pile or life pack will vanish
            start.is_coin = false;
            start.is_Life = false;         

            Queue<Cell> queue = new Queue<Cell>();
            bool[] mark = new bool[mapSize];        // boolean array to store if a cell is visited
            //enque the current position cell
            queue.Enqueue(start);
            // marks current position as visited
            mark[start.id] = true;

            bool found = false;
            int id = 0;

            //untill a path is found
            while (queue.Count != 0)
            {
                
                Cell current = queue.Dequeue();
                //scans all the neighbour cells of the current cell
                foreach (Cell neighbour in current.neighbours)
                {
                    if (neighbour.is_brick || neighbour.is_stone || neighbour.is_water)
                    {
                        mark[neighbour.id] = true;
                    }
                    //if neighbour is not a blocked cell it will be added to the queue
                    if (!mark[neighbour.id])
                    {
                        mark[neighbour.id] = true;
                        neighbour.parent = current;
                        queue.Enqueue(neighbour);
                    }
                    // if a coin pile or life pack is found BFS algorithm will break and return that cell id
                    if (neighbour.is_coin|| neighbour.is_Life)
                    {
                        found = true;

                        Cell curr = neighbour;
                        //backtracks the cell id of next cell where the tank should move
                        while (curr.parent != start)
                        {
                            curr = curr.parent;
                        }

                        id = curr.id;
                        break;
                    }
                    
                }
                // if a goal is found or the size of the queue is large "while" loop will break 
                if (found ||queue.Count>2500)
                    break;
            }

            if (id - currentPos == -1)
                return 0;   //go up
            else if (id - currentPos == 1)
                return 1; //go down
            else if (id - currentPos == -ConfigData.MAP_SIZE)
                return 2; //go left
            else if (id - currentPos == ConfigData.MAP_SIZE)
                return 3; //go right
            else
                return 4;   // no path found "SHOOT"

        }
    }
}

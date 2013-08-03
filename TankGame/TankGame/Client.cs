using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankGame.Intelligence;
using TankGame.Network;
using TankGame.Terrain;
using System.Timers;


namespace TankGame
{

    /*  This is the top most class of the heirarchy that accomodates all other required functional classes.
     *  Instances of Network related, AI related classes must be included appropriately.
     *  This architecture implies that we need only an instance of Client.cs inside the Game1 class.
     *  
     *  All required logic of the game client shall be included in this class
     * 
     */
    class Client
    {
        private Tank tank;
        private Communicator com;
        private PathFinder ai;
        private World gameWorld;
        private Tank[] enemy; //enemy tanks, maximum 4, initialize when S: comes
        private int number_of_players; // total number of players
        
        
        Timer server_update_timer;
        int update_interval=1200; // Slightly increase this value if TOO_QUICK# comes
  
        bool isConnected = false;
        bool isLiving = false;

        

        public Client() {
            com = new Communicator();
            
            tank = new Tank();
            gameWorld = new World(); //Game session will take place in this World instance
            ai = new PathFinder(gameWorld);
                  
           
        }
        public World getWorld()
        {
            return this.gameWorld;
        }
        public Tank getTank()
        {
            return this.tank;
        }
        public Tank[] getEnemyTanks()
        {
            return this.enemy;
        }

        // *** FULLY TESTED AND WORKING 100% *** 
        public void join_game() {

            com.write_to_server(ConfigData.join_req);
            String join_reply = com.read_from_server();

            // test line
            Console.WriteLine(join_reply);
           
            // this is how we identify whether the client is accepted. This logic can be improved
            if (join_reply != null)
            {
                if (join_reply.StartsWith("I:P"))
                {
                    isConnected = true;
                    isLiving = true;
                    Console.WriteLine("CLIENT ACCEPTED");
                    char[] delimit1 = { ':', '#' };
                    String[] arr = join_reply.Split(delimit1);
                    /*
                     * arr[0]="I"
                     * arr[1]=P<player number>
                     * arr[2]=brick co-ordinates
                     * arr[3]= stone co-ordinates
                     * arr[4]= water 
                     */
                    tank.player_number = int.Parse(arr[1].Substring(1, 1));


                    //BRICK DATA
                    char[] delimit2 = { ';' };
                    char[] delimit3 = { ',' };
                    String[] arr_brick = arr[2].Split(delimit2);
                    int number_of_bricks = arr_brick.Length;

                    Cell temp;
                    Console.Write("GENERATING BRICKS...");
                    for (int i = 0; i < number_of_bricks; i++)
                    {
                        String[] brick = arr_brick[i].Split(delimit3);
                        temp = gameWorld.getCell(int.Parse(brick[0]), int.Parse(brick[1]));
                        temp.is_brick = true;
                        temp.brick_level = 0; // new brick-->no damage
                    }
                    Console.WriteLine("DONE !");

                    Console.Write("GENERATING STONE...");
                    //STONE DATA
                    String[] arr_stone = arr[3].Split(delimit2);
                    int number_of_stones = arr_stone.Length;

                    for (int i = 0; i < number_of_stones; i++)
                    {
                        String[] stone = arr_stone[i].Split(delimit3);
                        temp = gameWorld.getCell(int.Parse(stone[0]), int.Parse(stone[1]));
                        temp.is_stone = true;
                    }
                    Console.WriteLine("DONE !");

                    Console.Write("GENERATING WATER...");
                    //WATER DATA

                    String[] arr_water = arr[4].Split(delimit2);
                    int number_of_water = arr_water.Length;

                    for (int i = 0; i < number_of_water; i++)
                    {
                        String[] water = arr_water[i].Split(delimit3);
                        temp = gameWorld.getCell(int.Parse(water[0]), int.Parse(water[1]));
                        temp.is_water = true;
                    }
                    Console.WriteLine("DONE !");

                }
                else if (join_reply == ConfigData.already_added | join_reply == ConfigData.player_full | join_reply == ConfigData.game_started)
                {
                    isConnected = false;
                    Console.WriteLine(join_reply);

                }
                else
                {
                    // bad response from server
                    Console.WriteLine("BAD RESPONSE");
                }
            }
            else 
            {
                Console.WriteLine("NULL REPLY");
            }

        }

       
       /// <summary>
       /// GET SERVER UPDATES AND OBTAIN ALL THE VALUES WITHIN THE STRING.
       /// ASSIGN THESE VALUES TO TANK,CLIENT VARIABLES.
       /// </summary>
        public void get_updates() {

            //isLiving = true; //uncomment for manual resting
            String reply;

            while(isLiving){

                    reply = com.read_from_server();
                    Console.WriteLine(reply);
                    if (reply != null)
                    {
                        if (reply.Equals(ConfigData.dead) || reply.Equals(ConfigData.game_started)) //recheck the logic here
                        {
                            isLiving = false;
                        }
                        else if (reply.StartsWith("S:"))
                        { // this is acceptance message
                            //extract and assign variables

                            /**
                             * S:P1:1,1:0#
                             * arr={"S","P1","1","1","0"}
                             * THIS IS THE FORMAT GIVEN IN THE SLIDES.
                             * BUT WHAT WE GET FROM THE SERVER IS DIFFERENT.
                             * SERVER SEND THE DETAILS OF ALL THE PLAYERS AND THEIR POSITIONS
                             * Eg. IF TWO PLAYERS ARE IN THE GAME, WE GET S:P0;0,0;0:P1;0,19;0# 
                             */
                            char[] delimit1 = { ':', '#' };
                            String[] arr = reply.Split(delimit1);

                            /*
                             *  arr[0]= "S"
                             *  arr[1]= "P<number 0>;<x0>,<y0>;<direction0>"
                             *  arr[2]= "P<number 1>;<x1>,<y1>;<direction1>"
                             *  arr[3]= "P<number 2>;<x2>,<y2>;<direction2>"
                             *  and so on...
                             */

                            number_of_players = arr.Length - 2;
                            enemy = new Tank[number_of_players - 1];

                            char[] delimit2 = { ';', ',' };
                            // WE ALREADY KNOW OUR TANK'S NUMBER. SO UPDATE IT DIRECTLY

                            tank.positionX = int.Parse((arr[tank.player_number + 1].Split(delimit2))[1]);
                            tank.positionY = int.Parse((arr[tank.player_number + 1].Split(delimit2))[2]);
                            tank.direction = int.Parse((arr[tank.player_number + 1].Split(delimit2))[3]);
                            
                            // NOW UPDATE ENEMIES
                            for (int i = 0; i < number_of_players - 1; i++)
                            {


                                enemy[i] = new Tank();
                                if (i < tank.player_number)
                                {
                                    enemy[i].player_number = i;
                                    enemy[i].positionX = int.Parse((arr[i + 1].Split(delimit2))[1]);
                                    enemy[i].positionY = int.Parse((arr[i + 1].Split(delimit2))[2]);
                                    enemy[i].direction = int.Parse((arr[i + 1].Split(delimit2))[3]);
                                }

                                else
                                {
                                    enemy[i].player_number = i + 1;
                                    enemy[i].positionX = int.Parse((arr[i + 2].Split(delimit2))[1]);
                                    enemy[i].positionY = int.Parse((arr[i + 2].Split(delimit2))[2]);
                                    enemy[i].direction = int.Parse((arr[i + 2].Split(delimit2))[3]);
                                }
                          }

                        }
                        else if (reply.StartsWith("G:P"))
                        { //these are global updates
                            /*
                             * "G:<Player0 detail>:<Player1 detail>:....:<brick details>"
                             */
                            char[] delimit1 = { ':', '#' };
                            String[] arr1 = reply.Split(delimit1);

                            /*
                             * arr1[0]="G"
                             * arr1[1]=P1;<location  x>,<location  y>;<Direction>;<whether shot>;<health>;<coins>;<points>
                             * arr1[2]=<Player1 details>
                             * ...and so on..., if there are n players including our tank
                             * arr1[n]=<Playe n detail>
                             * arr1[n+1]=<Brick details>
                             */

                            // UPDATE TANK, ENEMY DETAILS
                            char[] delimit2 = { ';', ',' };
                            for (int i = 0; i < number_of_players; i++)
                            {


                                if (i == tank.player_number)
                                { // this is our tank
                                    String[] arr2 = arr1[i + 1].Split(delimit2);
                                    tank.positionX = int.Parse(arr2[1]);
                                    tank.positionY = int.Parse(arr2[2]);
                                    tank.direction = int.Parse(arr2[3]);
                                    tank.whetherShot = int.Parse(arr2[4]);
                                    // ADD CELLS WHICH ARE AFFECTED BY THIS SHOTTING- DANGER RANGE
                                    tank.health = int.Parse(arr2[5]);
                                    tank.coins = int.Parse(arr2[6]);
                                    tank.points = int.Parse(arr2[7]);

                                }
                                else if (i < tank.player_number)
                                {
                                    String[] arr2 = arr1[i + 1].Split(delimit2);
                                    enemy[i].positionX = int.Parse(arr2[1]);
                                    enemy[i].positionY = int.Parse(arr2[2]);
                                    enemy[i].direction = int.Parse(arr2[3]);
                                    enemy[i].whetherShot = int.Parse(arr2[4]);
                                    enemy[i].health = int.Parse(arr2[5]);
                                    enemy[i].coins = int.Parse(arr2[6]);
                                    enemy[i].points = int.Parse(arr2[7]);
                                    gameWorld.getCell(int.Parse(arr2[1]), int.Parse(arr2[2])).is_coin = false;
                                    gameWorld.getCell(int.Parse(arr2[1]), int.Parse(arr2[2])).is_Life = false;


                                }
                                else
                                {
                                    String[] arr2 = arr1[i + 2].Split(delimit2);
                                    enemy[i - 1].positionX = int.Parse(arr2[1]);
                                    enemy[i - 1].positionY = int.Parse(arr2[2]);
                                    enemy[i - 1].direction = int.Parse(arr2[3]);
                                    enemy[i - 1].whetherShot = int.Parse(arr2[4]);
                                    enemy[i - 1].health = int.Parse(arr2[5]);
                                    enemy[i - 1].coins = int.Parse(arr2[6]);
                                    enemy[i - 1].points = int.Parse(arr2[7]);
                                    gameWorld.getCell(int.Parse(arr2[1]), int.Parse(arr2[2])).is_coin = false;
                                    gameWorld.getCell(int.Parse(arr2[1]), int.Parse(arr2[2])).is_Life = false;



                                }
                            }

                            //  BRICK UPDATES
                            gameWorld.setBrickUpdates(arr1[this.number_of_players + 1]);



                        }
                        else if (reply.StartsWith("C:"))
                        { // these are coin piles
                            char[] delimit = { ':', ',', '#' };
                            String[] arr = reply.Split(delimit);

                            // NOW PUT A COIN PILE TO <x,y> CELL 
                            gameWorld.setCoinPile(arr);

                        }
                        else if (reply.StartsWith("L:"))
                        { // these are life packs
                            char[] delimit = { ':', ',', '#' };
                            String[] arr = reply.Split(delimit);

                            // NOW PUT A LIFE PACK TO <x,y> CELL 
                            gameWorld.setLifePack(arr);
                        }
                        else if (reply.Contains(ConfigData.too_quick))
                        {

                            update_interval += 200;
                            continue;
                        }
                        else if (reply.Contains(ConfigData.cell_occupied))
                        {
                            //DO SOMETHING
                            continue;
                        }
                        else if (reply.Contains(ConfigData.obstacle))
                        {
                            //DO SOMETHING
                            continue;
                        }
                        else if (reply.Contains(ConfigData.invalid_cell))
                        {
                            //DO SOMETHING
                            
                            continue;
                        }

                        else if (reply.Contains(ConfigData.game_over))
                        {
                            // GAME OVER
                            Console.WriteLine("GAME OVER");
                        }
                        else if (reply.Contains(ConfigData.game_finished))
                        {
                            //WHAT IS THE DIFFERENCE WITH THE PREVIOUS ONE?
                        }
                        else if (reply.Contains(ConfigData.game_not_started))
                        {
                            //DO SOMETHING


                        }
                        else if (reply.Contains(ConfigData.not_valid_contestant))
                        {
                            //DO SOMETHING
                            Console.WriteLine(ConfigData.not_valid_contestant);
                        }
                        else if (reply.Contains(ConfigData.dead))
                        {
                            this.isLiving = false;
                            Console.WriteLine("CLIENT DIED");
                            break;
                        }
                        // AFTER SCANNING THE SERVER REPLY,SEND A MOVE TO THE SERVER
                        try
                        {
                            //string s = ai.getNextMove(gameWorld, tank.positionX * ConfigData.MAP_SIZE + tank.positionY);
                            //Console.WriteLine(s);
                            //com.write_to_server(s);
                            //this.send_updates();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("COULD NOT WRITE TO SERVER");
                            break;
                        }
                    }
                    else 
                    {
                        Console.WriteLine("NULL UPDATE FROM SERVER");
                        break;
                    }
            }
        }

        
        //SEND STATUS REPLY TO THE SERVER
        public void send_updates()
        {
            server_update_timer = new Timer(update_interval); // CHANGE THIS VALUE SLIGHTLY AND CHECK
            server_update_timer.Enabled = true;
            server_update_timer.AutoReset = true;
            server_update_timer.Elapsed += new ElapsedEventHandler(update_server_handler);
            GC.KeepAlive(server_update_timer);  
        }

        private void update_server_handler(object o, ElapsedEventArgs e)
        {
            try
            {
                //ai = new PathFinder(gameWorld);
                string s = ai.getNextMove(tank.positionX * ConfigData.MAP_SIZE + tank.positionY);
                Console.WriteLine(s);
                com.write_to_server(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine("COULD NOT WRITE TO SERVER");
            }

        }




        
    }

    
    }


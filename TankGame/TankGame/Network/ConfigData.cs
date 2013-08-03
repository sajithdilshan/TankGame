using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TankGame.Network
{
     public static class ConfigData
     {

         #region terrain

         public const int MAP_SIZE = 10;
         public const int INIT_HEALTH = 100;
         public const double HEALTH_INCREMENT = 0.2;
         public const int BULLET_SPEED = 3;


         #endregion

         #region join_game

        public const string join_req = "JOIN#";
        public const string player_full = "PLAYERS_FULL#";
        public const string already_added = "ALREADY_ADDED#";
        public const string game_started = "GAME_ALREADY_STARTED#";

        #endregion

        #region client_move

        public const string up = "UP#";
        public const string down = "DOWN#";
        public const string right = "RIGHT#";
        public const string left = "LEFT#";
        public const string shoot = "SHOOT#";
        
        #endregion

        #region server_move_replies

        public const string obstacle = "OBSTACLE#";
        public const string cell_occupied = "CELL_OCCUPIED#";
        public const string dead = "DEAD#";
        public const string too_quick = "TOO_QUICK#";
        public const string invalid_cell = "INVALID_CELL#";
        public const string game_over = "GAME_FINISHED#";
        public const string game_finished = "GAME_HAS_FINISHED#";
        public const string game_not_started = "GAME_NOT_STARTED_YET#";
        public const string not_valid_contestant = "NOT_A_VALID_CONTESTANT#";

        #endregion

        #region server_stat

        public static string SERVER_IP = "127.0.0.1"; 
        public static int SERVER_PORT = 6000; //used when write
        public static int CLIENT_PORT = 7000; // used when read
        
        #endregion

        #region directional_constants

         public const int NORTH = 0;
         public const int EAST = 1;
         public const int SOUTH = 2;
         public const int WEST = 3;
        #endregion

    }
}

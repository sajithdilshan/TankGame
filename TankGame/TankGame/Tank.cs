using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankGame
{
    class Tank
    {
        private int _player_number; // number assigned by the server at the begining of the game
        private int _positionX,_positionY; // current cell number
        private int _direction; // current direction
        private int _coins = 0; // current coins earned
        private int _points = 0; // current points achieved
        private int _health = 100; // current health value
        private int _whether_shot = 0; // default 0


       

        #region setters-getters

        public int player_number {
            set { _player_number = value; }
            get { return _player_number; }
        }
        public int positionX
        { 
            set{ _positionX= value; }
            get{return _positionX;}
        }
        public int positionY {
            set { _positionY = value; }
            get { return _positionY; }
        }
        public int direction 
        {
            set { _direction=value;}
            get {return _direction ;}
        }
        public int coins
        {
            set {_coins =value;}
            get {return _coins;}
        }
        public int health 
        {
            set { _health=value;}
            get {return _health ;}
        }
        public int points 
        {
            set { _points=value;}
            get {return _points;}
        }

        public int whetherShot 
        {
            set { _whether_shot = value; }
            get { return _whether_shot; }
        }

        #endregion

        #region update-methods

        public void update_coin_and_points(int amount) 
        {
            _coins = _coins + amount;
            _points = _points + amount;

        }
        public void update_health(int amount) {
            _health = _health + amount;
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorldGen
{
    class Hex
    {

        public enum HexType
        {
            Ocean,
            Shore,
            Land,
            NumHexTypes
        }

        public enum Side
        {
            North,
            Northeast,
            Southeast,
            South,
            Southwest,
            Northwest
        }

        public static Side RotateSideCounterclockwise(Side side)
        {
            if (side == Side.North) return Side.Northwest;
            else return side - 1;
        }

        public static Side RotateSideClockwise(Side side)
        {
            if (side == Side.Northwest) return Side.North;
            else return side + 1;
        }

        private HexType _type;
        public HexType type
        {
            get { return _type; }
            set { _type = value; }
        }



        private bool _canPlace = true;
        public bool CanPlace
        {
            get
            {
                return _canPlace;
            }
            set
            {
                _canPlace = value;
            }
        }

        public Hex()
        {
            _type = HexType.Ocean;
        }

        public Hex(HexType type)
        {
            _type = type;
        }
        public char CharDisplay()
        {
            switch (type)
            {
                case HexType.Ocean:
                    return '~';
                case HexType.Land:
                    return '\u2592';
                default:
                    return '?';
            }
        }

        public Color GetColor()
        {
            Color rv = Colors.White;
            switch (_type)
            {
                case HexType.Ocean:
                    return Colors.SteelBlue;
                case HexType.Shore:
                    return Colors.SkyBlue;
                case HexType.Land:
                    return Colors.Moccasin;
                default:
                    return Colors.Magenta;
            }
        }
    }
}

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

        public enum Elevation
        {
            Low,
            LowMid,
            Mid,
            MidHigh,
            High
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

        private Elevation _elevation = Elevation.Low;
        public Elevation elevation
        {
            get { return _elevation; }
            set { _elevation = value; }
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

        public bool IsLand()
        {
            return (_type != HexType.Ocean 
                    && _type != HexType.Shore);
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

        public Color GetElevationColor()
        {
            if (_type == HexType.Ocean || _type == HexType.Shore)
                return GetColor();
            switch (_elevation)
            {
                case Elevation.Low:
                    return Colors.SlateBlue;
                case Elevation.LowMid:
                    return Colors.Orchid;
                case Elevation.Mid:
                    return Colors.Tomato;
                case Elevation.MidHigh:
                    return Colors.Gold;
                case Elevation.High:
                    return Colors.LawnGreen;
                default:
                    return Colors.Magenta;
            }
        }
    }
}

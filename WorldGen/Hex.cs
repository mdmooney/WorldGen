using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorldGen
{
    /**
     * <summary>
     * Hex class, representing a singular hex, or tile, of the map.
     * </summary>
     * <remarks>
     * This class tracks anything that is specific to a hex, and that is somewhat
     * permanent. The intent is to keep hexes as self-contained as possible and
     * let the HexMap class (and possibly other, similar classes) handle relationships
     * between different Hex objects.
     * </remarks>
     */
    class Hex
    {
        // Number of sides; tiles are hexagonal, so this should never change
        public const int SIDES = 6;

        /**
         * <summary>
         * This enum defines the broad categories of hex, which are handled very
         * differently by the application.
         * </summary>
         */
        public enum HexType
        {
            Ocean,
            Shore,
            Land,
            NumHexTypes
        }

        /**
         * <summary>
         * This enum defines possible elevation levels for a hex in ascending order
         * of height.
         * </summary>
         */
        public enum ElevationLevel
        {
            Low,
            LowMid,
            Mid,
            MidHigh,
            High
        }

        /**
        * <summary>
        * This enum defines possible temperature levels for a hex.
        * </summary>
        */
        public enum TemperatureLevel
        {
            Cold,
            Cool,
            Temperate,
            Warm,
            Hot
        }

        /**
        * <summary>
        * This enum defines possible humidity levels for a hex.
        * </summary>
        */
        public enum HumidityLevel
        {
            Arid,
            SemiArid,
            Average,
            SemiHumid,
            Humid
        }

        /**
         * <summary>
         * This enum standardizes ordering nad naming for the sides of the hex.
         * "Nil" is considered to be a side without meaning, and is used when
         * nullable types are not appropriate.
         * </summary>
         */
        public enum Side
        {
            North,
            Northeast,
            Southeast,
            South,
            Southwest,
            Northwest,
            Nil
        }

        /**
         * <summary>
         * Static method to determine which side is adjacent to another, in
         * counterclockwise order.
         * </summary>
         * <param name="side">The side to rotate.</param>
         * <returns>The Side counterclockwise-adjacent to <paramref name="side"/>.</returns>
         */
        public static Side RotateSideCounterclockwise(Side side)
        {
            if (side == Side.North) return Side.Northwest;
            else return side - 1;
        }

        /**
         * <summary>
         * Static method to determine which side is adjacent to another, in
         * clockwise order.
         * </summary>
         * <param name="side">The side to rotate.</param>
         * <returns>The Side clockwise-adjacent to <paramref name="side"/>.</returns>
         */
        public static Side RotateSideClockwise(Side side)
        {
            if (side == Side.Northwest) return Side.North;
            else return side + 1;
        }

        /**
         * <summary>
         * Static method to get the side opposite of another.
         * </summary>
         * <param name="side">The side to flip.</param>
         * <returns>The Side directly opposite <paramref name="side"/>.</returns>
         */
        public static Side OppositeSide(Side side)
        {
            switch (side)
            {
                case Side.North:
                    return Side.South;
                case Side.Northeast:
                    return Side.Southwest;
                case Side.Southeast:
                    return Side.Northwest;
                case Side.South:
                    return Side.North;
                case Side.Southwest:
                    return Side.Northeast;
                case Side.Northwest:
                    return Side.Southeast;
                default:
                    return side;
            }
        }

        private AffinityMap _affinities;

        public AffinityMap Affinities
        {
            get
            {
                if (_affinities == null)
                {
                    if (HexBiome != null)
                    {
                        _affinities = new AffinityMap(HexBiome.Affinities);
                    }
                    else
                    {
                        _affinities = new AffinityMap();
                    }

                    switch (Elevation)
                    {
                        case ElevationLevel.Mid:
                            _affinities.SetAffinity("mountain", 1);
                            break;
                        case ElevationLevel.MidHigh:
                            _affinities.SetAffinity("mountain", 3);
                            break;
                        case ElevationLevel.High:
                            _affinities.MaximizeAffinity("mountain");
                            break;
                        default:
                            break;
                    }

                    switch (Temperature)
                    {
                        case TemperatureLevel.Cold:
                            _affinities.MaximizeAffinity("cold");
                            break;
                        case TemperatureLevel.Hot:
                            _affinities.MaximizeAffinity("hot");
                            break;
                        default:
                            break;
                    }

                    switch (Humidity)
                    {
                        case HumidityLevel.Arid:
                            _affinities.MaximizeAffinity("arid");
                            break;
                        case HumidityLevel.Humid:
                            _affinities.MaximizeAffinity("humid");
                            break;
                        default:
                            break;
                    }

                    if (HasRiver())
                    {
                        _affinities.MaximizeAffinity("freshwater");
                    }
                }
                return _affinities;
            }

            private set { _affinities = value; }
        }


        /**
         * <summary>
         * The broad type of this hex.
         * </summary>
         */
        public HexType Type { get; set; }

        /**
         * <summary>
         * The elevation level of this hex. Defaults to low.
         * </summary>
         */
        public ElevationLevel Elevation { get; set; } = ElevationLevel.Low;

        /**
         * <summary>
         * The temperature level of this hex. Defaults to temperate.
         * </summary>
         */
        public TemperatureLevel Temperature { get; set; } = TemperatureLevel.Temperate;

        /**
         * <summary>
         * The humidity level of this hex. Defaults to humid.
         * </summary>
         */
        public HumidityLevel Humidity { get; set; } = HumidityLevel.Humid;

        public Biome HexBiome { get; set; }

        /**
         * <summary>
         * Whether this hex is valid for the placement of land or not.
         * A Hex should be considered valid for land placement if:
         *   - It is not already HexType.Land.
         *   - It is not adjacent to another land hex.
         * True if land can be placed here, false otherwise.
         * </summary>
         */
        public bool CanPlace { get; set; } = true;

        /**
         * <summary>
         * The primary RiverSegment for this Hex. A Hex can only have one main
         * RiverSegment; that is, a RiverSegment that enters the Hex and possibly
         * leaves it as well.
         * </summary>
         */
        public RiverSegment MainRiverSegment { get; set; }

        /**
         * <summary>
         * Default constructor for Hex.
         * Simply initializes the Hex as an ocean tile.
         * </summary>
         */
        public Hex()
        {
            Type = HexType.Ocean;
        }

        /**
         * <summary>
         * HexType constructor for Hex.
         * Allows setting the broad type of Hex during instantation.
         * </summary>
         */
        public Hex(HexType type)
        {
            Type = type;
        }

        /**
         * <summary>
         * Simple predicate to determine whether the Hex is a land type
         * Hex or not.
         * </summary>
         * <returns>True if the Hex is a land hex, false otherwise.</returns>
         */
        public bool IsLand()
        {
            return (Type == HexType.Land);
        }

        /**
         * <summary>
         * Predicate: is this Hex a water hex?
         * </summary>
         * <remarks>
         * This considers the general type of Hex; this will return false if the
         * Hex is a land hex that happens to contain water (like a river).
         * </remarks>
         * <returns>True if the Hex is a water hex, false otherwise.</returns>
         */
        public bool IsWater()
        {
            return (Type == HexType.Ocean
                    || Type == HexType.Shore);
        }

        /**
         * <summary>
         * Predicate: does this Hex contain a river?
         * </summary>
         * <returns>True if the Hex has a river, false otherwise.</returns>
         */
        public bool HasRiver()
        {
            return MainRiverSegment != null;
        }

        /**
         * <summary>
         * Method to display a Hex as a char, used for console display.
         * </summary>
         * <returns>
         * A char representing the Hex on a console map.
         * </returns>
         */
        public char CharDisplay()
        {
            switch (Type)
            {
                case HexType.Ocean:
                    return '~';
                case HexType.Land:
                    return '\u2592';
                default:
                    return '?';
            }
        }

        /**
         * <summary>
         * Method to get a hardcoded color for a Hex, by HexType.
         * This will be replaced eventually by a config file, or similar.
         * </summary>
         * <returns>A color from the Colors namespace, specific to the HexType of this Hex.</returns>
         */
        public Color GetBaseColor()
        {
            switch (Type)
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

        /**
         * <summary>
         * Method to get a hardcoded color for a Hex, by ElevationLevel.
         * This will be replaced eventually by a config file, or similar.
         * </summary>
         * <returns>A color from the Colors namespace, specific to the ElevationLevel of this Hex.</returns>
         */
        public Color GetElevationColor()
        {
            if (Type == HexType.Ocean || Type == HexType.Shore)
                return GetBaseColor();
            switch (Elevation)
            {
                case ElevationLevel.Low:
                    return Colors.SlateBlue;
                case ElevationLevel.LowMid:
                    return Colors.Orchid;
                case ElevationLevel.Mid:
                    return Colors.Tomato;
                case ElevationLevel.MidHigh:
                    return Colors.Gold;
                case ElevationLevel.High:
                    return Colors.LawnGreen;
                default:
                    return Colors.Magenta;
            }
        }

        /**
         * <summary>
         * Method to get a hardcoded color for a Hex, by TemperatureLevel.
         * This will be replaced eventually by a config file, or similar.
         * </summary>
         * <returns>A color from the Colors namespace, specific to the TemperatureLevel of this Hex.</returns>
         */
        public Color GetTemperatureColor()
        {
            if (Type == HexType.Ocean || Type == HexType.Shore)
                return GetBaseColor();
            switch (Temperature)
            {
                case TemperatureLevel.Cold:
                    return Colors.DodgerBlue;
                case TemperatureLevel.Cool:
                    return Colors.Turquoise;
                case TemperatureLevel.Temperate:
                    return Colors.GreenYellow;
                case TemperatureLevel.Warm:
                    return Colors.Gold;
                case TemperatureLevel.Hot:
                    return Colors.OrangeRed;
                default:
                    return Colors.Magenta;
            }
        }

        /**
         * <summary>
         * Method to get a hardcoded color for a Hex, by HumidityLevel.
         * This will be replaced eventually by a config file, or similar.
         * </summary>
         * <returns>A color from the Colors namespace, specific to the HumidityLevel of this Hex.</returns>
         */
        public Color GetHumidityColor()
        {
            if (Type == HexType.Ocean || Type == HexType.Shore)
                return GetBaseColor();
            switch (Humidity)
            {
                case HumidityLevel.Humid:
                    return Colors.DeepSkyBlue;
                case HumidityLevel.SemiHumid:
                    return Colors.MediumSpringGreen;
                case HumidityLevel.Average:
                    return Colors.GreenYellow;
                case HumidityLevel.SemiArid:
                    return Colors.LightSalmon;
                case HumidityLevel.Arid:
                    return Colors.OrangeRed;
                default:
                    return Colors.Magenta;
            }
        }

        public Color GetBiomeColor()
        {
            if (HexBiome == null)
                return GetBaseColor();
            return HexBiome.BiomeColor;
        }

    }
}

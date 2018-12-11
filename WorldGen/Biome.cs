﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorldGen
{
    class Biome
    {
        public string Name { get; private set; }
        public HashSet<Hex.TemperatureLevel> TemperatureRange { get; private set; }
        public HashSet<Hex.HumidityLevel> HumidityRange { get; private set; }
        public Color BiomeColor { get; private set; }

        public Biome(string name)
        {
            Name = name;
            TemperatureRange = new HashSet<Hex.TemperatureLevel>();
            HumidityRange= new HashSet<Hex.HumidityLevel>();
            BiomeColor = Colors.Beige;
        }

        public Biome(string name, Color color) : this(name)
        {
            BiomeColor = color;
        }

        public void AddTemperature(Hex.TemperatureLevel temperature)
        {
            TemperatureRange.Add(temperature);
        }

        public void AddHumidity(Hex.HumidityLevel humidity)
        {
            HumidityRange.Add(humidity);
        }

        public bool TemperatureAndHumidityValid(Hex.TemperatureLevel temperature, Hex.HumidityLevel humidity)
        {
            return (TemperatureRange.Contains(temperature) && HumidityRange.Contains(humidity));
        }
    }
}
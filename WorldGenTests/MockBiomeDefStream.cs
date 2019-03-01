using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen.Tests
{
    class MockBiomeDefStream
    {
        private StringBuilder sb = new StringBuilder();
        public bool Finalized { get; private set; }
        private static readonly string rootTagOpen = "<root>";
        private static readonly string rootTagClose = "</root>";

        public MockBiomeDefStream()
        {
            Finalized = false;
            sb.Append(rootTagOpen);
        }

        public void AddInTag(string tag, string value)
        {
            if (!Finalized)
            {
                sb.Append("<");
                sb.Append(tag);
                sb.Append(">");
                sb.Append(value);
                sb.Append("</");
                sb.Append(tag);
                sb.Append(">");
            }
        }

        // Adds a complete biome definition, with sufficient params to fulfill minimum
        // requirements for the definition.
        public void AddBiomeDef(string name, string color, string primaryAspect,
                                string tempLB, string tempUB, string humidityLB, string humidityUB)
        {
            if (!Finalized)
            {
                sb.Append("<biome_def>");
                AddBiomeInternal(name, color, primaryAspect, tempLB, tempUB, humidityLB, humidityUB);
                sb.Append("</biome_def>");
            }
        }

        public void AddDisabledBiomeDef(string name, string color, string primaryAspect,
                                     string tempLB, string tempUB, string humidityLB, string humidityUB)
        {
            if (!Finalized)
            {
                sb.Append("<biome_def disabled=\"true\">");
                AddBiomeInternal(name, color, primaryAspect, tempLB, tempUB, humidityLB, humidityUB);
                sb.Append("</biome_def>");
            }
        }

        private void AddBiomeInternal(string name, string color, string primaryAspect,
                                      string tempLB, string tempUB, string humidityLB, string humidityUB)
        {
            if (name != null)
                AddInTag("name", name);
            if (color != null)
                AddInTag("hex_color", color);
            if (primaryAspect != null)
                AddInTag("primary_aspect", primaryAspect);
            if (tempLB != null)
                AddInTag("temperature_lb", tempLB);
            if (tempUB != null)
                AddInTag("temperature_ub", tempUB);
            if (humidityLB != null)
                AddInTag("humidity_lb", humidityLB);
            if (humidityUB != null)
                AddInTag("humidity_ub", humidityUB);
        }


        public void Reopen()
        {
            if (Finalized)
            {
                sb.Remove(sb.Length - rootTagClose.Length - 1, rootTagClose.Length);
                Finalized = false;
            }
        }

        public Stream FinalizeStream()
        {
            if (!Finalized)
            {
                sb.Append(rootTagClose);
                Finalized = true;
            }

            string str = sb.ToString();
            Byte[] strBytes = Encoding.ASCII.GetBytes(str);
            var stream = new MemoryStream(strBytes);
            return stream;
        }
    }
}

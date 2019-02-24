using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

[assembly: InternalsVisibleTo("WorldGenTests")]
namespace WorldGen
{
    class AspectGlossary
    {
        private static AspectGlossary _instance;
        private HashSet<string> _aspects;
        private Dictionary<string, HashSet<string>> _pools;
        private XDocument _glossary;

        public static AspectGlossary GetInstance()
        {
            if (_instance == null)
                _instance = new AspectGlossary();
            return _instance;
        }

        private AspectGlossary()
        {
            _glossary = XDocument.Load("aspect_defs.xml");
            CreateAspectList();
            GeneratePools();
        }

        private void CreateAspectList()
        {
            _aspects = new HashSet<string>();
            XElement defsRoot = _glossary.Element("root");
            foreach (var node in defsRoot.Elements("aspect_def"))
            {
                _aspects.Add(node.Value);
            }
        }

        private void GeneratePools()
        {
            _pools = new Dictionary<string, HashSet<string>>();
            XElement defsRoot = _glossary.Element("root");
            foreach (var node in defsRoot.Elements("pool"))
            {
                string name = node.Attribute("name").Value;
                var pool = new HashSet<string>();
                foreach (var aspect in node.Elements("aspect"))
                {
                    pool.Add(aspect.Value);
                }
                _pools.Add(name, pool);
            }
        }

        public bool Contains(string aspect)
        {
            if (aspect == null) return false;
            return _aspects.Contains(aspect.ToLower());
        }

        public bool HasPool(string pool)
        {
            if (pool == null) return false;
            return _pools.ContainsKey(pool.ToLower());
        }

        public HashSet<string> GetPool(string pool)
        {
            if (!HasPool(pool)) throw InvalidAspectException.FromPool(pool);
            return new HashSet<string>(_pools[pool]);
        }

    }
}

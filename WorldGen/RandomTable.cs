using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class RandomTable<T>
    {
        private List<Tuple<int, T>> _table;
        private int _max;
        private Random _rand = new Random();

        public RandomTable()
        {
            _table = new List<Tuple<int, T>>();
            _max = 0;
        }

        public void Add(T element, int value)
        {
            _max += value;
            _table.Add(new Tuple<int, T>(_max, element));
        }

        public T Roll()
        {
            if (_table.Count == 0) return default(T);
            if (_table.Count == 1) return _table.First().Item2;

            int r = _rand.Next(_max + 1);

            // modified binary search to get closest match
            int low = 0;
            int high = _table.Count - 1;
            while (low <= high)
            {
                int mid = (low + high) / 2;

                if (r < _table[mid].Item1)
                    high = mid - 1;
                else if (r > _table[mid].Item1)
                    low = mid + 1;
                else
                    return _table[mid].Item2;
            }
            if (high < 0)
                return _table[low].Item2;
            return ((_table[low].Item1 - r) < (r - _table[high].Item1)) ? _table[low].Item2 : _table[high].Item2;
        }
    }
}

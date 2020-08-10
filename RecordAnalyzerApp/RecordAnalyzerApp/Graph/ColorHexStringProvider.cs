using System.Collections.Generic;

namespace RecordAnalyzerApp.Graph
{
    public static class ColorHexStringProvider
    {
        static int _index = 0;
        static List<string> _list = new List<string>();

        static ColorHexStringProvider()
        {
            _list.Add("#BC8F8F");
            _list.Add("#2F4F4F");
            _list.Add("#696969");
            _list.Add("#DB7093");
            _list.Add("#4B0082");
            _list.Add("#00008B");
            _list.Add("#1E90FF");
            _list.Add("#008080");
            _list.Add("#66CDAA");
            _list.Add("#808000");
            _list.Add("#32CD32");
            _list.Add("#BDB76B");
            _list.Add("#FF8C00");
            _list.Add("#FFD700");
            _list.Add("#FF4500");
            _list.Add("#8B0000");
            _list.Add("#FF0000");
            _list.Add("#DC143C");
            _list.Add("#F08080");
            _list.Add("#FFFF00");
            _list.TrimExcess();
        }
        
        public static string NextColorString()
        {
            string value = _list[_index];

            _index++;
            if (_index == _list.Count)
            {
                _index = 0;
            }

            return value;
        }
    }
}

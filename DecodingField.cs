using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Decode
{
    public class DecodingField
    {
        public string Name { get; set; }

        public int Position { get; set; }

        public int Length { get; set; }

        public Dictionary<int, string> ValueDescription { get; set; }

        private uint field_val(uint value)
        {
            uint mask = (1u << Length) - 1;
            return (value >> Position) & mask;
        }

        private string val_desc(uint value)
        {
            int val = (int)field_val(value);
            string desc;
            if(ValueDescription.TryGetValue(val, out desc) == false)
            {
                if(ValueDescription.TryGetValue(-1, out desc) == false)
                {
                    desc = "Invalid";
                }
            }
            return desc;
        }

        public DecodingField() : this("", 0, 0) { }

        public DecodingField(string name, int pos, int len)
        {
            Name = name;
            Position = pos;
            Length = len;
            ValueDescription = new Dictionary<int, string>();
        }

        public DecodingField(string name, int pos, int len, string desc)
        {
            Name = name;
            Position = pos;
            Length = len;
            ValueDescription = new Dictionary<int, string>() { { -1, desc } };
        }

        public string Decode(UInt32 value)
        {
            string s = String.Format("{0}: {1}\n  ", Name, field_val(value));
            return s + val_desc(value);
        }
    }
}

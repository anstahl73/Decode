using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decode
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Decoding : IEnumerable<DecodingField>
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        private List<DecodingField> fields;

        #region IEnumerable
        public IEnumerator<DecodingField> GetEnumerator() => fields.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => fields.GetEnumerator();
        #endregion

        private int field_sorter(DecodingField a, DecodingField b)
        {
            return b.Position.CompareTo(a.Position);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc)
        {
            fields.Sort(field_sorter);
        }

        public void Add(DecodingField item)
        {
            fields.Add(item);
        }

        public Decoding() : this("") { }

        public Decoding(string name)
        {
            Name = name;
            fields = new List<DecodingField>();
        }
    }
}

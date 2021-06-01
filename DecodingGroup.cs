using System.Collections.ObjectModel;

namespace Decode
{
    public class DecodingGroup
    {
        public string Name { get; }

        public ObservableCollection<Decoding> Decodings { get; set; }

        public DecodingGroup(string name)
        {
            this.Name = name;
            this.Decodings = new ObservableCollection<Decoding>();
        }
    }
}

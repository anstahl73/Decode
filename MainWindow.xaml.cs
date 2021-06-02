using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Decode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private uint current_value;
        private bool value_update;
        private void serializeJSON(Decoding decoding)
        {
            string filename = decoding.Name + ".json";
            StringWriter str = new StringWriter();
            JsonSerializer ser = new JsonSerializer();
            ser.Formatting = Formatting.Indented;
            ser.Serialize(str, decoding);
            File.WriteAllText(filename, str.ToString());
        }

        private Decoding deserializeJSON(string filename)
        {
            JsonSerializer ser = new JsonSerializer();
            string json = File.ReadAllText(filename);
            Decoding dec = (Decoding)ser.Deserialize(new StringReader(json), typeof(Decoding));
            return dec;
        }

        private DecodingGroup LoadDecodings(string subdir)
        {
            DecodingGroup root = new DecodingGroup(subdir);
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + subdir;
            foreach(string f in Directory.GetFiles(dir))
            {
                root.Decodings.Add(deserializeJSON(f));
            }
            return root;
        }

        public MainWindow()
        {
            InitializeComponent();

            WriteTestJson();

            DecodingGroup root = LoadDecodings("Data");

            trvDecodings.Items.Add(root);

            current_value = 0;
            value_update = true;
            hexInput.Text = "0";
            decInput.Text = "0";
            value_update = false;
        }

        private void WriteTestJson()
        {
            Decoding dec = new Decoding("Port Control 0");
            DecodingField field = new DecodingField("TXQ split Enable", 13, 1);
            field.ValueDescription.Add(0, "Enable");
            field.ValueDescription.Add(1, "Disable");
            dec.Add(field);

            field = new DecodingField("Port based Priority Classification", 3, 2);
            field.ValueDescription.Add(0, "Priority 0");
            field.ValueDescription.Add(1, "Priority 1");
            field.ValueDescription.Add(2, "Priority 2");
            dec.Add(field);

            serializeJSON(dec);
        }

        private string Range(DecodingField f)
        {
            int from = f.Position;
            int to = from + f.Length - 1;
            if (from < to)
                return string.Format("[{0}:{1}]: ", to, from);
            else
                return string.Format("[{0}]: ", from);
        }

        private void updateDecoding()
        {
            Decoding dec = trvDecodings.SelectedItem as Decoding;
            if (dec != null)
            {
                List<int> cluster = new List<int>();
                int pos = 32;
                tbDecode.Text = "";
                foreach (DecodingField f in dec)
                {
                    if (pos > f.Position + f.Length)
                    {
                        cluster.Add(pos - (f.Position + f.Length));
                    }
                    cluster.Add(f.Length);
                    pos = f.Position;
                    tbDecode.Text += Range(f) + f.Decode(current_value) + Environment.NewLine;
                }
                if (pos > 0)
                    cluster.Add(pos);
                binFrame.Cluster = cluster;
            }
        }

        private void trvDecodings_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            updateDecoding();
        }

        private void hexInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (value_update == false)
            {
                try
                {
                    NumberStyles ns = NumberStyles.HexNumber;
                    current_value = uint.Parse(hexInput.Text, ns);
                    hexInput.ClearValue(BackgroundProperty);
                    decInput.ClearValue(BackgroundProperty);
                    value_update = true;
                    decInput.Text = current_value.ToString();
                    value_update = false;
                    updateDecoding();
                }
                catch (Exception)
                {
                    hexInput.Background = Brushes.Red;
                }
            }
        }

        private void decInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (value_update == false)
            {
                try
                {
                    NumberStyles ns = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
                    current_value = uint.Parse(decInput.Text, ns);
                    hexInput.ClearValue(BackgroundProperty);
                    decInput.ClearValue(BackgroundProperty);
                    value_update = true;
                    hexInput.Text = current_value.ToString("X");
                    value_update = false;
                    updateDecoding();
                }
                catch (Exception)
                {
                    decInput.Background = Brushes.Red;
                }
            }
        }
    }
}

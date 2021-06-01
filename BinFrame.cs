using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Decode
{
    public class BinFrame : Panel
    {
        private GlyphTypeface face;
        private List<int> _cluster;

        private static void OnValueChanged(DependencyObject elem, DependencyPropertyChangedEventArgs args)
        {
            BinFrame bf = elem as BinFrame;
            bf.InvalidateVisual();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(UInt32), typeof(BinFrame), new PropertyMetadata((UInt32)0, new PropertyChangedCallback(OnValueChanged)));

        public UInt32 Value
        {
            get { return (UInt32)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public List<int> Cluster
        {
            set { _cluster = value; InvalidateVisual(); }
        }

        public BinFrame()
        {
            new Typeface("Arial").TryGetGlyphTypeface(out face);
        }

        private GlyphRun GetGlyphRun(char c, Rect cell)
        {
            Point origin = new Point();
            double M = cell.Height - 1;
            ushort index = face.CharacterToGlyphMap[c];

            origin.Y = cell.Top + (cell.Height - face.Height*M) / 2;
            origin.Y += face.Baseline * M;
            origin.X = cell.Left + (cell.Width - face.AdvanceWidths[index]*M) / 2;

            return new GlyphRun(
                face,
                0,
                false,
                M,
                96,
                new ushort[] { index },
                origin,
                new double[] { 0.0 },
                null, null, null, null, null, null);
        }

        private void DrawFrame(DrawingContext dc, Point origin, ushort cells, ushort cell_size, IList<int> cluster)
        {
            Pen pen_b = new Pen(Brushes.Black, 3);
            Pen pen_s = new Pen(Brushes.Black, 1);
            Point p0, p1, p2, p3;

            p0 = origin;
            p1 = origin;
            p1.X += cells * cell_size;
            dc.DrawLine(pen_b, p0, p1);
            p0.Y += cell_size;
            p1.Y += cell_size;
            dc.DrawLine(pen_b, p0, p1);

            p2 = p1 = origin;
            p3 = p0;
            p2.Y = p1.Y + cell_size / 3;
            p3.Y = p0.Y - cell_size / 3;
            int cl_size = 33;
            if((cluster != null) && (cluster.Count > 0))
            {
                cl_size = cluster[0];
                cluster.RemoveAt(0);
            }
            for (int i = 0; i <= 32; i++)
            {
                if ((cl_size == 0) || (i == 0) || (i == 32))
                {
                    dc.DrawLine(pen_b, p0, p1);
                }
                else
                {
                    dc.DrawLine(pen_s, p1, p2);
                    dc.DrawLine(pen_s, p3, p0);
                }

                if (cl_size == 0)
                {
                    if ((cluster != null) && (cluster.Count > 0))
                    {
                        cl_size = cluster[0];
                        cluster.RemoveAt(0);
                    }
                }

                p0.X += cell_size;
                p1.X += cell_size;
                p2.X += cell_size;
                p3.X += cell_size;
                cl_size--;
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            Point origin = new Point(10, 10);
            Rect cell = new Rect(10, 10, 25, 25);
            base.OnRender(dc);

            DrawFrame(dc, origin, 32, 25, _cluster);

            UInt32 val = Value;
            for(int i = 0; i < 32; i++)
            {
                if((val & 0x80000000) == 0)
                    dc.DrawGlyphRun(Brushes.Black, GetGlyphRun('0', cell));
                else
                    dc.DrawGlyphRun(Brushes.Black, GetGlyphRun('1', cell));

                val <<= 1;
                cell.Offset(cell.Width, 0);
            }
        }
    }
}

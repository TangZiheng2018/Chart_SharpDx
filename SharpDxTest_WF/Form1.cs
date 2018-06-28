using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using Color = SharpDX.Color;
using Text = SharpDX.DirectWrite.TextFormat;
    

namespace SharpDxTest_WF
{
    using Device = SharpDX.Direct3D11.Device;
    using AlphaMode = SharpDX.Direct2D1.AlphaMode;
    using Factory = SharpDX.DXGI.Factory;
    public partial class Form1 : Form
    {
        private RenderForm renderForm;
        private Device _device;
        private SwapChain swapChain;
        private Texture2D target;
        private RenderTargetView targetveiw;
        private RenderTarget d2dRenderTarget;
        Size2F size;
        private SharpDX.Direct2D1.Factory d2dFactory;
        private static Text textFormat;
        private string textPosition = "";

        private List<Bar> _bars;
        private double minPoint;
        private double maxPoint;
        private int countPointX;
        private int countPointY;
        private float ratio;

        public Form1()
        {
            InitializeComponent();


            renderForm = new RenderForm("My form");
            var windowSize = new Size(600, 600);
            ClientSize = windowSize;
            //this.size = new Size2F(30, 70);

            //using (SharpDX.DirectWrite.Factory textFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared))
            //{
            //    textFormat = new Text(
            //        textFactory,
            //        "MS Sans Serif",
            //        SharpDX.DirectWrite.FontWeight.SemiBold,
            //        SharpDX.DirectWrite.FontStyle.Normal,
            //        SharpDX.DirectWrite.FontStretch.Medium,
            //        20.0f);
            //}
            CreateBars();
            SwapChainDescription chainDescription = new SwapChainDescription()
            {
                BufferCount = 1,                                
                Flags = SwapChainFlags.None,
                IsWindowed = true,                              
                ModeDescription = new ModeDescription(
                    windowSize.Width,                       
                    windowSize.Height,                      
                    new Rational(60, 1),                        
                    Format.R8G8B8A8_UNorm),                     

                OutputHandle = this.Handle,                     

                SampleDescription = new SampleDescription(1, 0),                            
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(
                SharpDX.Direct3D.DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                new SharpDX.Direct3D.FeatureLevel[] { SharpDX.Direct3D.FeatureLevel.Level_10_0 },
                chainDescription, 
                out _device, 
                out swapChain);
            
            d2dFactory = new SharpDX.Direct2D1.Factory();
            
            var width = this.Width;
            var height = this.Height;
            
            Factory factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);
            
            target = SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
            targetveiw = new RenderTargetView(_device, target);

            Surface surface = target.QueryInterface<Surface>();

            var pixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied);

            d2dRenderTarget = new RenderTarget(d2dFactory, surface,
                new RenderTargetProperties(pixelFormat));

             var brush = new SolidColorBrush(d2dRenderTarget, Color.Black);

            var barBearBrush = new SolidColorBrush(d2dRenderTarget, Color.Red);
            var barBullBrush = new SolidColorBrush(d2dRenderTarget, Color.Green);


            //bool isDrawed = false;

            //var centerForChart = Height / 2 - size.Height / 2;
            //this.MouseMove += MouseHoverHandler;

            //this.KeyPress += (sender, args) =>
            //{
            //    if (args.KeyChar == 'q' && ! isDrawed)
            //    {
            //        isDrawed = true;
            //        //DrawLine();
            //    }
            //    else
            //    {
            //        isDrawed = false;
            //    }
            //};

            //this.ResizeEnd += (sender, args) =>
            //{
            //    width = this.Width;
            //    height = this.Height;
            //};


            var rightXPaddingChart = width * 0.9f;
            var leftXPaddingChart = width * 0.1f;


            var topYPaddingChart = height * 0.9f;
            var bottomYPaddingChart = height * 0.1f;

            var touchingPointOfAxes = new Vector2(rightXPaddingChart, topYPaddingChart);
            var chartAxeVectorX = new Vector2(leftXPaddingChart, topYPaddingChart);
            var chartAxeVectorY = new Vector2(rightXPaddingChart, bottomYPaddingChart);


            var pointPaddingFrom = width * 0.88f;
            var pointPaddingTo = width * 0.04f;


            var chartAxeYPaddingsTop = 0.1f;
            var chartAxeYPaddingsBotton = 0.1f;
            var yAxePaddingChart = chartAxeYPaddingsTop + chartAxeYPaddingsBotton;
            var chartHeight = height * (1 - yAxePaddingChart);
            var partY = chartHeight * 0.05f;
            var countYAxe = chartHeight / partY;

            var barSizeRatio = 0.02f;
            var spaceBetweenBarsRatio = 0.05f;
            var chartAxeXPaddingsLeft = 0.1f;
            var chartAxeXPaddingsRight = 0.1f;
            var xAxePaddingChart = chartAxeXPaddingsLeft + chartAxeXPaddingsRight;
            var chartWidth = width * (1 - xAxePaddingChart);
            var barSize = chartWidth * barSizeRatio;
            var spaceBetweenBars = chartWidth * spaceBetweenBarsRatio;

            var partX = barSize + spaceBetweenBars;
            var countX = (int)(chartWidth / partX);
            var countY = (int)(chartHeight / partY);
            var startDrawPointLinesFrom = touchingPointOfAxes.X;
            var startPositionY = height * 0.88f;
            var finishPositionY = startPositionY + height * 0.04f;


            var rectangle = new RoundedRectangle()
            {
                RadiusX = 1,
                RadiusY = 1,
                Rect = new RawRectangleF(width*.12f,height*.1f,width*.14f,height*.2f)
            };

            //var line = new CustomLine();

            RenderLoop.Run(this, () =>
            {
                d2dRenderTarget.BeginDraw();
                d2dRenderTarget.Clear(Color.AliceBlue);

                d2dRenderTarget.DrawLine(chartAxeVectorX, touchingPointOfAxes, brush,2);
                d2dRenderTarget.DrawLine(chartAxeVectorY, touchingPointOfAxes, brush,2);

                foreach (var bar in _bars)
                {
                    d2dRenderTarget.DrawLine(bar.Line.VectorLow, bar.Line.VectorHigh, brush);

                    d2dRenderTarget.FillRoundedRectangle(bar.Rectangle, bar._isBear == true ? barBearBrush : barBullBrush);
                }

                ////d2dRenderTarget.DrawRoundedRectangle(rectangle, brush);

                //int j = 0;
                //for (float i = 1; i <= chartHeight; i += partY)
                //{
                //    d2dRenderTarget.DrawLine(_bars[j].Line,brush);
                //    j++;
                //}



                //var part = height * 0.05f;
                //var part2 = width * 0.05f + width * 0.02f;
                //var startPos = touchingPointOfAxes.Y - part;

                //for (float i = startPos; i > leftXPaddingChart; i -= part)
                //{
                //    var vectorPoint1 = new Vector2(pointPaddingFrom, i);
                //    var vectorPoint2 = new Vector2(pointPaddingFrom + pointPaddingTo, i);

                //    d2dRenderTarget.DrawLine(vectorPoint1, vectorPoint2, brush, 2);
                //}



                for (float i = 1; i > countY; i++)
                {
                    var vectorPoint1 = new Vector2(pointPaddingFrom, startDrawPointLinesFrom);
                    var vectorPoint2 = new Vector2(pointPaddingFrom + pointPaddingTo, i);

                    d2dRenderTarget.DrawLine(vectorPoint1, vectorPoint2, brush, 2);
                }

                for (float i = 1; i <= countX; i++)
                {
                    var vectorPoint1 = new Vector2(startDrawPointLinesFrom - (partX * i), startPositionY);
                    var vectorPoint2 = new Vector2(startDrawPointLinesFrom - partX * i, finishPositionY);

                    d2dRenderTarget.DrawLine(vectorPoint1, vectorPoint2, brush, 2);
                }

                

                //d2dRenderTarget.DrawLine(new Vector2(0, this.Height / 2), new Vector2(width, this.Height / 2), brush); // center

                //if (isDrawed)
                //{
                //    using (var brush2 = new SolidColorBrush(d2dRenderTarget, Color.RosyBrown))
                //    {
                //        for (int x = 0; x < d2dRenderTarget.Size.Width; x += 30)
                //        {
                //            d2dRenderTarget.DrawLine(new Vector2(x, 0), new Vector2(x, d2dRenderTarget.Size.Height), brush2);
                //        }

                //        for (int y = 0; y < d2dRenderTarget.Size.Height; y += 30)
                //        {
                //            d2dRenderTarget.DrawLine(new Vector2(0, y), new Vector2(d2dRenderTarget.Size.Width, y), brush2);
                //        }
                //    }
                //}

                //d2dRenderTarget.DrawText(textPosition, textFormat, new SharpDX.Mathematics.Interop.RawRectangleF(30,30, d2dRenderTarget.Size.Width, d2dRenderTarget.Size.Height), brush);

                d2dRenderTarget.EndDraw();
                

                swapChain.Present(1, PresentFlags.None);
            });
        }

        private void CreateBars()
        {
            _bars = new List<Bar>();
            var barInfo = new List<BarModelInfo>();
            
                barInfo = new List<BarModelInfo>()
                    {
                        new BarModelInfo(10, 0, 8, 6, DateTime.Now),
                        new BarModelInfo(9, 3, 2, 8, DateTime.Now.AddDays(6)),
                        new BarModelInfo(12, 0, 11, 1, DateTime.Now.AddDays(3)),
                        new BarModelInfo(32, 3, 27, 8, DateTime.Now.AddDays(2)),
                        new BarModelInfo(17, 3, 5, 12, DateTime.Now.AddDays(4)),
                        new BarModelInfo(12, 0, 2, 8, DateTime.Now.AddDays(5)),
                        new BarModelInfo(17, 0, 17, 6, DateTime.Now.AddDays(7)),
                        new BarModelInfo(29, 3, 14, 6, DateTime.Now.AddDays(8)),
                        new BarModelInfo(22, 0, 10, 12, DateTime.Now.AddDays(9)),
                        new BarModelInfo(20, 3, 15, 5, DateTime.Now.AddDays(10)),
                        new BarModelInfo(4, 3, 3.5, 3, DateTime.Now.AddDays(11)),
                        new BarModelInfo(7, 0, 6, 1, DateTime.Now.AddDays(12)),
                        new BarModelInfo(9, 3, 7, 2, DateTime.Now.AddDays(13)),
                        new BarModelInfo(12, 0, 9, 2, DateTime.Now.AddDays(14)),
                        //new BarModelInfo(32, 3, 11, 10.5, DateTime.Now.AddDays(15)),
                        //new BarModelInfo(4, 3, 1, 9, DateTime.Now.AddDays(16)),
                        //new BarModelInfo(7, 0, 18, 10, DateTime.Now.AddDays(17))
                    };
            
                int i = 0;
                minPoint = barInfo.Min(x => x.Low);
                maxPoint = barInfo.Max(x => x.High);
                ratio = Height * 0.8f / Convert.ToSingle(maxPoint);

                _bars.AddRange(barInfo.Select(x => new Bar(x, ++i, Width, Height, ratio)));
            
        }


        //private void MouseHoverHandler(object sender, EventArgs e)
        //{
        //    textPosition = "Position:" +  MousePosition.ToString();
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            //dipose of all objects
            _device.Dispose();
            swapChain.Dispose();
            target.Dispose();
            targetveiw.Dispose();
            base.OnClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
       
    }
    
    public class Bar
    {
        private readonly float _barWidth;
        public readonly bool _isBear;
        private readonly float _locationBar;
        private readonly float _locationLine;
        private readonly int _radius;

        private double ratio;
        //private readonly float spaceBetweenBars;
        //private readonly int barNumber;
        public CustomLine Line { get; private set; }
        public RoundedRectangle Rectangle { get; private set; }
        
        public Bar(BarModelInfo barStruct, int count, float width, float height, float ratio)
        {
            float chartWidth = width*0.8f;
            float chartHeigh = height*0.8f;

            this.ratio = ratio;
            var spaceBetweenBars = chartWidth * 0.02f;
            _barWidth = chartWidth * 0.04f;

            _locationBar = width * 0.1f + (spaceBetweenBars + _barWidth) * count; // locate in the center bar
            _locationLine = width * 0.1f + (spaceBetweenBars + _barWidth) * count + _barWidth/2; // locate in the center line
            
            _isBear = !(barStruct.Open>barStruct.Close);
            _radius = 1;

            CreateLine(barStruct.High*ratio+height*0.1, barStruct.Low*ratio+height*0.1);
            CreateRectangle(barStruct.Open, barStruct.Close, height);
        }

        private void CreateLine(double high, double low)
        {
            Line = new CustomLine(high, low,_locationLine);
        }

        private void CreateRectangle(double open, double close, double height)
        {
            var rawRectangle = new RawRectangleF(_locationBar, Convert.ToSingle(height*0.1f+open*ratio), _locationBar+_barWidth, Convert.ToSingle(height * 0.1f + close*ratio));

            var figure = new RoundedRectangle
            {
                RadiusX = _radius,
                RadiusY = _radius,
                Rect = rawRectangle
                //Rect = new RectangleF{
                //    X = _location,
                //    Y = Convert.ToSingle(open),
                //    Right = _barWidth,
                //    Bottom = Convert.ToSingle(close)
                //}
            };
            /*
                public RawRectangleF(float left, float top, float right, float bottom)
                {
                    this.Left = left;
                    this.Top = top;
                    this.Right = right;
                    this.Bottom = bottom;
                }


                public RectangleF(float x, float y, float width, float height)
                {
                      this.Left = x;
                      this.Top = y;
                      this.Right = x + width;
                      this.Bottom = y + height;
                }
            */
            Rectangle = figure;
        }

    }

    public class CustomLine
    {
        public Vector2 VectorHigh { get; set; }
        public Vector2 VectorLow { get; set; }
        
        public CustomLine(double high, double low, double xLocation)
        {
            VectorHigh = new Vector2(Convert.ToSingle(xLocation),Convert.ToSingle(high));
            VectorLow = new Vector2(Convert.ToSingle(xLocation), Convert.ToSingle(low));
        }

    }

}

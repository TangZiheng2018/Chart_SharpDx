using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using SharpDxTest_WF.BarComponent;
using SharpDxTest_WF.BarComponent.BarTypes;
using SharpDxTest_WF.Drawings;
using SharpDxTest_WF.DrawingsComponent.AdditionalModels;
using SharpDxTest_WF.DrawingsComponent.Base;
using SharpDxTest_WF.Drawings.Figures;
using SharpDxTest_WF.HelperModels;
using Rectangle = SharpDX.Rectangle;

namespace SharpDxTest_WF
{
    using Device = SharpDX.Direct3D11.Device;
    using AlphaMode = SharpDX.Direct2D1.AlphaMode;
    using Factory = SharpDX.DXGI.Factory;
    
    public partial class Chart : Form
    {

        #region ShartpDxFields

            private RenderForm _renderForm;
            private Device _device;
            private SwapChain _swapChain;
            private Texture2D _target;
            private RenderTargetView _targetView;
            private RenderTarget _d2dRenderTarget;
            private RenderLoop.RenderCallback _callback;
            private SharpDX.Direct2D1.Factory _d2dFactory;
            private static Text _textFormat;
            private SwapChainDescription chainDescription;
            private bool _lockZoom;

        #endregion

        private ChartSettings _chart;
        private int _resize;
        private bool _isResized;
        private string _textValue = "";
        private float _textSize;
        private Vector2 _mousePisition;
        private List<SelectedFigureBase> _drawings;
        private SelectedFigureBase _selectedDrawing;
        private SelectedFigureBase _tempDrawing;
        private BarRenderingBase _barRendering;
        private DrawingActions _actions;
        private ChartHelpers _chartHelpers;
        private BarType _barType;

        public Chart()
        {
            InitializeComponent();

            #region Init

            _resize = 0;
            _isResized = true;

            _mousePisition = new Vector2();
            _drawings = new List<SelectedFigureBase>();

            _barType = BarType.Candle;
            _actions = DrawingActions.Default;
            _chartHelpers = ChartHelpers.Net;

            var windowSize = new Size(800, 800);
            _textSize = windowSize.Height * 0.015f;
            SetRenderSettings(windowSize);
            
            _chart = new ChartSettings(ClientSize.Width, ClientSize.Height, _d2dRenderTarget, TimingBy.Minute, 0.01f, 0.008f);
            SetBarsFromKraken();
            SetForChartMinMaxPoints(0);

            #endregion

            #region FormHandlers


            KeyDown += OnKeyDown;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseClick += OnMouseClick;
            MouseWheel += OnMouseWheel;
            MouseMove += OnMouseMove;

            #endregion

            _callback += RenderChart;
            RenderLoop.Run(this, _callback);
        }
        
        #region Rendering

        private void RenderChart()
        {
            _d2dRenderTarget.BeginDraw();
            _d2dRenderTarget.Clear(Color.AliceBlue);
            
            AdditionChartHelpers();

            _barRendering.RenderBars();

            _tempDrawing?.RenderPreview(_mousePisition.X, _mousePisition.Y);

            _selectedDrawing?.RenderSelectedFigure();

            foreach (var figure in _drawings)
            {
                figure.RenderFigure();
            }

            DrawAxesAndValues();

            _d2dRenderTarget.EndDraw();
            _swapChain.Present(2, PresentFlags.None);
        }
        
        private void AdditionChartHelpers()
        {

            if (_chartHelpers == ChartHelpers.Net)
            {
                var startByX = (_chart.PaddingLeftRatio * 0.6f) * Width;
                var startByY = _chart.PaddingTopRatio * Height;

                for (float x = 1; x <= _chart.AxeSettings.CountPointsOnXAxe; x++)
                {
                    var leftPadding = _chart.AxeSettings.touchXYPoint.X -
                                      _chart.Width * _chart.AxeSettings.PointOnEveryPercentAxeX * (x);

                    var vectorPoint1 = new Vector2(leftPadding, startByY);

                    var vectorPoint2 = new Vector2(leftPadding, _chart.PaddingRightRatio * Height);

                    _d2dRenderTarget.DrawLine(vectorPoint1, vectorPoint2, _chart.Brushes.TransparentBlack);
                }

                for (float y = ClientSize.Height * _chart.PaddingTopRatio; y <= ClientSize.Height * _chart.PaddingBottomRatio; y += _chart.Height * _chart.AxeSettings.PointOnEveryPercentAxeY)
                {
                    _d2dRenderTarget.DrawLine(new Vector2(startByX, y), new Vector2(_chart.PaddingRightRatio * Width, y), _chart.Brushes.TransparentBlack);
                }
            }

            if (_chartHelpers == ChartHelpers.Lines)
            {
                for (float y = ClientSize.Height * _chart.PaddingTopRatio; y <= ClientSize.Height * _chart.PaddingBottomRatio; y += _chart.Height * _chart.AxeSettings.PointOnEveryPercentAxeY * 2)
                {
                    _d2dRenderTarget.DrawLine(new Vector2(0, y), new Vector2(_chart.AxeSettings.touchXYPoint.X, y), _chart.Brushes.TransparentBlack);
                }
            }

        }
        
        private void DrawAxesAndValues()
        {

            #region YAxe

            var yVectors = _chart.YAxeVectors();
            _d2dRenderTarget.DrawLine(yVectors[0], yVectors[1], _chart.Brushes.Black, 2);

            //set Y axe and point on it
            for (int i = 1; i <= _chart.AxeSettings.CountPointsOnYAxe; i++)
            {
                var vectorPoint1 = new Vector2(_chart.AxeSettings.yValuePointStart,
                    _chart.TouchAxesPosition - _chart.Height * _chart.AxeSettings.PointOnEveryPercentAxeY * i);

                var vectorPoint2 = new Vector2(_chart.AxeSettings.yValuePointFinish,
                    _chart.TouchAxesPosition - _chart.Height * _chart.AxeSettings.PointOnEveryPercentAxeY * i);

                var diffBetweenValues = (_chart.AxeValues.MaxValueLocation - _chart.AxeValues.MinValueLocation) /
                                        _chart.AxeSettings.CountPointsOnYAxe;

                _textValue = Convert.ToString(Math.Round(_chart.AxeValues.MinValueLocation + diffBetweenValues * i, 2));

                var textPlacement = new RawRectangleF(Width * (_chart.PaddingRightRatio + 0.025f),
                    (_chart.TouchAxesPosition - _chart.Height * _chart.AxeSettings.PointOnEveryPercentAxeY * i)
                    - _textSize / 2 - 0.25f * _textSize,
                    _d2dRenderTarget.Size.Width, _d2dRenderTarget.Size.Height);

                _d2dRenderTarget.DrawText(_textValue, _textFormat, textPlacement, _chart.Brushes.Black);
                _d2dRenderTarget.DrawLine(vectorPoint1, vectorPoint2, _chart.Brushes.Black, 2);
            }


            #endregion

            #region XAxe

            var xVectors = _chart.XAxeVectors();
            _d2dRenderTarget.DrawLine(xVectors[0], xVectors[1], _chart.Brushes.Black, 2);

            //set X axe and point on it
            for (int i = 0; i < _chart.AxeSettings.CountPointsOnXAxe; i++)
            {
                var leftPadding = _chart.AxeSettings.touchXYPoint.X -
                                  _chart.Width * _chart.AxeSettings.PointOnEveryPercentAxeX * (i + 1);

                var vectorPoint1 = new Vector2(leftPadding, _chart.AxeSettings.xDatePointStart);

                var vectorPoint2 = new Vector2(leftPadding, _chart.AxeSettings.xDatePointFinish);

                var minDate = _chart.AxeValues.MinDateLocation;
                var maxDate = _chart.AxeValues.MaxDateLocation;

                var pointCounts = _chart.AxeSettings.CountPointsOnXAxe;

                if (_chart.TimeIn == TimingBy.Minute)
                {
                    var timePerPoint = Math.Ceiling(maxDate.Subtract(minDate).TotalMinutes / pointCounts);
                    var timeForSet = minDate.AddMinutes(timePerPoint * (_chart.AxeSettings.CountPointsOnXAxe - i));
                    _textValue = timeForSet.ToShortTimeString();
                }

                if (_chart.TimeIn == TimingBy.Hour)
                {
                    var timePerPoint = Math.Ceiling(maxDate.Subtract(minDate).TotalHours / pointCounts);
                    var timeForSet = minDate.AddHours(timePerPoint * (_chart.AxeSettings.CountPointsOnXAxe - i));
                    _textValue = timeForSet.ToShortDateString() + " " + timeForSet.ToShortTimeString();
                }

                if (_chart.TimeIn == TimingBy.Day)
                {
                    var timePerPoint = Math.Ceiling(maxDate.Subtract(minDate).TotalDays / pointCounts);
                    var timeForSet = minDate.AddDays(timePerPoint * (_chart.AxeSettings.CountPointsOnXAxe - i));
                    _textValue = timeForSet.ToShortDateString();
                }

                var textPadding = (_textValue.Length * (_textSize * 0.5f)) / 2; //locate text in the center

                var replaceText = new RawRectangleF(leftPadding - textPadding, ClientSize.Height * (_chart.PaddingBottomRatio + 0.025f), _d2dRenderTarget.Size.Width, _d2dRenderTarget.Size.Height);

                _d2dRenderTarget.DrawText(_textValue, _textFormat, replaceText, _chart.Brushes.Black);
                _d2dRenderTarget.DrawLine(vectorPoint1, vectorPoint2, _chart.Brushes.Black, 2);

            }

            #endregion

        }

        #endregion

        #region RenderHandlers

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            var key = args.KeyData;

            switch (key)
            {
                case Keys.Q:
                {
                    if (_chartHelpers != ChartHelpers.Net)
                    {
                        _chartHelpers = ChartHelpers.Net;
                        return;
                    }

                    _chartHelpers = ChartHelpers.Default;
                    return;
                }
                case Keys.W:
                {
                    if (_chartHelpers != ChartHelpers.Lines)
                    {
                        _chartHelpers = ChartHelpers.Lines;
                        return;
                    }

                    _chartHelpers = ChartHelpers.Default;
                    return;
                }
                case Keys.E:
                {
                    _tempDrawing = new LineFigure(_d2dRenderTarget);
                    _actions = DrawingActions.LineDrawing;
                    return;
                }
                case Keys.R:
                {
                    _tempDrawing = new RectangleFigure(_d2dRenderTarget);
                    _actions = DrawingActions.RectangleDrawing;
                    return;
                }
                case Keys.T:
                {
                    _tempDrawing = new EllipseFigure(_d2dRenderTarget);
                    _actions = DrawingActions.EllipseDrawing;
                    return;
                }
                case Keys.A:
                {
                    _actions = DrawingActions.StraightLineXDrawing;
                    return;
                }
                case Keys.S:
                {
                    _actions = DrawingActions.StraightLineYDrawing;
                    return;
                }
                case Keys.Z:
                {
                    _chartHelpers = ChartHelpers.Zoom;
                    return;
                }
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs args)
        {
            switch (_actions)
            {
                case DrawingActions.LineDrawing:
                {
                    SetChanges();
                    return;
                }
                case DrawingActions.RectangleDrawing:
                {
                    SetChanges();
                    return;
                }
                case DrawingActions.EllipseDrawing:
                {
                    SetChanges();
                    return;
                }
                case DrawingActions.StraightLineXDrawing:
                {
                    SetXStraightLine(_mousePisition.X);
                    break;
                }
                case DrawingActions.StraightLineYDrawing:
                {
                    SetYStraightLine(_mousePisition.Y);
                    break;
                }
            }

            CheckCrosing();

            if (_chartHelpers == ChartHelpers.Zoom)
            {
                var x = args.X;
                var y = args.Y;

                _tempDrawing = new RectangleFigure(_d2dRenderTarget);
                var isSetted = _tempDrawing.SetPosition(x, y);

                if (!isSetted) return;
                
                //ZoomUpChart();
                _tempDrawing = null;

                _chartHelpers = ChartHelpers.Default;
            }
        }
        
        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            //if (isEipllseUnderControl == true)
            //{
            //    isEipllseUnderControl = false;
            //    changeLine = false;

            //    var x = args.X;
            //    var y = args.Y;

            //    var customLine = new CustomLine(mousePoint, new Vector2(x, y));
            //    _line.AddFigure(customLine);
            //    mousePoint = Vector2.Zero;
            //}
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            //if (_line.SelectedFigure != null)
            //{
            //    selectedVector2 = checkCrossSelectedEllipse(e.X, e.Y);

            //    if (selectedVector2 == Vector2.Zero)
            //        return;

            //    _line.RemoveFigure(_line.SelectedFigure);

            //    mousePoint = _line.SelectedFigure.Point1 == selectedVector2 ? _line.SelectedFigure.Point2 : _line.SelectedFigure.Point1;

            //    _line.SelectedFigure = null;
            //    changeLine = true;
            //    isEipllseUnderControl = true;
            //}
        }
        
        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            if (args.Delta < 0)
            {
                if (ModifierKeys == Keys.Control)
                {
                    _chart.ResizeDown();
                    _resize--;
                    _isResized = false;
                    return;
                }
                if (_barRendering.Skip - 1 == -1)
                {
                    return;
                }
                
                _barRendering.Skip--;

                SetForChartMinMaxPoints(_barRendering.Skip);
                
                _isResized = false;
            }
            else
            {
                if (ModifierKeys == Keys.Control)
                {
                    _chart.ResizeUp();
                    _resize++;
                    _isResized = false;
                    return;
                }
                if (_barRendering.Skip + 1 == _barRendering.Bars.Count)
                    return;
                
                _barRendering.Skip++;

                SetForChartMinMaxPoints(_barRendering.Skip);

                _isResized = false;
            }
            
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            _mousePisition = new Vector2(args.X, args.Y);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _device.Dispose();
            _swapChain.Dispose();
            _target.Dispose();
            _targetView.Dispose();
            base.OnClosing(e);
        }

        #endregion

        #region Methods

        private void SetXStraightLine(float x)
        {
            var line = new LineFigure(_d2dRenderTarget);

            var lineStart = new Vector2(x, ClientSize.Height * _chart.PaddingTopRatio);
            var lineFinish = new Vector2(x, ClientSize.Height * _chart.PaddingBottomRatio);

            line.Line = new CustomLine(lineStart, lineFinish);

            _drawings.Add((SelectedFigureBase)line);
        }

        private void SetYStraightLine(float y)
        {
            var line = new LineFigure(_d2dRenderTarget);

            var lineStart = new Vector2(ClientSize.Width * _chart.PaddingLeftRatio, y);
            var lineFinish = new Vector2(ClientSize.Width * _chart.PaddingRightRatio, y);

            line.Line = new CustomLine(lineStart, lineFinish);

            _drawings.Add((SelectedFigureBase)line);
        }

        private void CheckCrosing()
        {
            if (_drawings?.Any() == false)
                return;

            foreach (var figure in _drawings)
            {
                if (figure.IsFigureCrossed(_mousePisition.X, _mousePisition.Y))
                {
                    _selectedDrawing = figure;
                    return;
                }
            }

            _selectedDrawing = null;
        }

        private void SetChanges()
        {
            if (_tempDrawing == null)
                return;

            var isComplited = _tempDrawing.SetPosition(_mousePisition.X, _mousePisition.Y);

            if (isComplited)
            {
                _drawings.Add(_tempDrawing);
                _actions = DrawingActions.Default;
                _tempDrawing = null;
            }
        }
        
        private void UpdateResizing()
        {
            if (_resize > 0)
            {
                for (int i = 0; i < _resize; i++)
                {
                    _chart.ResizeUp();
                }
            }
            if (_resize < 0)
            {
                for (int i = _resize; i < 0; i++)
                {
                    _chart.ResizeDown();
                }
            }
        }

        private void SetBarsFromKraken(int interval = 1440)
        {
            var bars = GenerateCndelStickBars(100);
            if (_barType == BarType.Candle)
                _barRendering = new BarCandle(_d2dRenderTarget, bars, _chart);
            else
                _barRendering = new BarOHLC(_d2dRenderTarget, bars, _chart);
        }

        public void SetForChartMinMaxPoints(int skip)
        {
            _barRendering.Skip = skip;
            _chart.SetMinMaxBorders(_barRendering.MinMaxPositions);

            UpdateResizing();
        }

        public void SetRenderSettings(Size windowSize)
        {
            _renderForm = new RenderForm("Windows Form Chart by VmpKalin");

            this.ClientSize = windowSize;

            using (SharpDX.DirectWrite.Factory textFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared))
            {
                _textFormat = new Text(
                    textFactory,
                    "MS Sans Serif",
                    SharpDX.DirectWrite.FontWeight.SemiBold,
                    SharpDX.DirectWrite.FontStyle.Normal,
                    SharpDX.DirectWrite.FontStretch.Medium,
                    _textSize);
            }

            chainDescription = new SwapChainDescription()
            {
                BufferCount = 4,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(
                    ClientSize.Width,
                    ClientSize.Height, // для того, щоб поінтер мишки був точно під нею, треба прописувати, точно так
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
                out _swapChain);

            _d2dFactory = new SharpDX.Direct2D1.Factory();

            var factory = _swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

            _target = SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(_swapChain, 0);
            _targetView = new RenderTargetView(_device, _target);

            var surface = _target.QueryInterface<Surface>();

            var pixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied);

            _d2dRenderTarget = new RenderTarget(_d2dFactory, surface,
                new RenderTargetProperties(pixelFormat));
        }

        public static List<BarModel> GenerateCndelStickBars(int maxBars)
        {
            var random = new Random();
            var lastPrice = (float)random.Next(100);
            var bars = new List<BarModel>();
            var time = 0f;

            var dateTime = DateTime.Now;

            for (int i = 0; i < maxBars; i++)
            {
                var bar = new BarModel
                {
                    Time = dateTime,
                    Open = lastPrice,
                    High = lastPrice + (float)random.Next(1000) / 100,
                    Low = lastPrice - (float)random.Next(1000) / 100,
                };

                bar.Close = (float)Math.Round(random.NextDouble() * (bar.High - bar.Low) + bar.Low, 3);
                bar.High = Math.Max(bar.High, bar.Open);
                bar.Low = Math.Min(bar.Low, bar.Open);
                if (bar.Close < bar.Open)
                    bar.IsBear = true;

                bars.Add(bar);
                lastPrice = (float)bar.Close;
                time += 40f;
                dateTime = dateTime.AddMinutes(1);
            }

            return bars;
        }
        #endregion
    }
}
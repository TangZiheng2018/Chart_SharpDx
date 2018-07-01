using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SharpDxTest_WF.BarComponent;
using SharpDxTest_WF.BarComponent.BarTypes;
using SharpDxTest_WF.BarComponent.Models;
using SharpDxTest_WF.ChartRendering;
using SharpDxTest_WF.DrawingsComponent.AdditionalModels;
using SharpDxTest_WF.DrawingsComponent.Base;
using SharpDxTest_WF.Drawings.Figures;
using SharpDxTest_WF.HelperModels;
using RectangleF = SharpDX.RectangleF;

namespace SharpDxTest_WF
{
    using Device = SharpDX.Direct3D11.Device;
    using AlphaMode = SharpDX.Direct2D1.AlphaMode;
    using Factory = SharpDX.DXGI.Factory;

    delegate void DrawAdditionChartHelper();

    public partial class Chart : Form
    {

        #region ShartpDxFields

        private RenderForm _renderForm;
        private Device _device;
        private SwapChain _swapChain;
        private Texture2D _target;
        private RenderTargetView _targetView;
        private RenderTarget _d2DRenderTarget;
        private RenderLoop.RenderCallback _callback;
        private SharpDX.Direct2D1.Factory _d2DFactory;
        private Text _textFormat;
        private SwapChainDescription _chainDescription;

        #endregion

        #region ChartFields

        private int _resize;
        private bool _isResized;
        private RawRectangleF _borders;


        private DrawingActions _actions;
        private ChartHelpers _chartHelpers;
        private BarType _barType;

        private string _textValue = "";
        private bool _isZoomed;
        private ScreenPoint _mousePisition;
        private ChartDrawing _chart;
        private SelectedFigureBase _selectedDrawing;
        private SelectedFigureBase _tempDrawing;
        private RectangleFigure _zoomArea;
        private List<SelectedFigureBase> _drawings;
        private BarRenderingBase _barRendering;
        private ChartRendering.ChartRendering _chartRendering;
        
        private DrawAdditionChartHelper _drawAdditionChartHelper;

        #endregion
        
        public Chart()
        {
            InitializeComponent();

            #region Init

            _resize = 0;
            _isResized = true;
            _isZoomed = true;

            _mousePisition = new ScreenPoint();
            _drawings = new List<SelectedFigureBase>();

            _barType = BarType.OHLC;
            _actions = DrawingActions.Default;
            _chartHelpers = ChartHelpers.Default;

            var windowSize = new Size(800, 800);
            SetRenderSettings(windowSize);

            _chart = new ChartDrawing(ClientSize.Width, ClientSize.Height, _d2DRenderTarget, TimingBy.Minute);
            _chartRendering = new ChartRendering.ChartRendering(_d2DRenderTarget, _textFormat, _chart);
            GenerateBars();
            SetForChartMinMaxPoints(0);

            _borders = new RectangleF(windowSize.Width * _chart.Paddings.PaddingLeftRatio,
                windowSize.Height * _chart.Paddings.PaddingTopRatio, _chart.ChartWidth, _chart.ChartHeight);
            #endregion

            #region FormHandlers


            KeyDown += OnKeyDown;
            MouseDown += OnMouseDown;
            //MouseUp += OnMouseUp;
            MouseClick += OnMouseClick;
            MouseWheel += OnMouseWheel;
            MouseMove += OnMouseMove;
            Resize += Chart_ResizeEnd;

            #endregion

            _callback += RenderChart;
            RenderLoop.Run(this, _callback);
            this.Hide();
        }

        #region Rendering

        private void RenderChart()
        {
            _d2DRenderTarget.BeginDraw();
            _d2DRenderTarget.Clear(_chart.ChartColor);
            
            _drawAdditionChartHelper?.Invoke();
            
            if(!_isZoomed)
                ZoomChart();
            
            if(_chartHelpers == ChartHelpers.Zoom)
                _zoomArea?.RenderPreview(_mousePisition);

            _d2DRenderTarget.PushAxisAlignedClip(_borders, AntialiasMode.PerPrimitive);

            _barRendering.StartRendering();
            
            _d2DRenderTarget.PopAxisAlignedClip();

            _barRendering.RenderLastPosition();

            _tempDrawing?.RenderPreview(_mousePisition);

            _selectedDrawing?.RenderSelectedFigure();

            _drawings.ForEach(x=>x.StartRendering());
            
            _chartRendering.StartRendering();

            _d2DRenderTarget.EndDraw();
            _swapChain.Present(2, PresentFlags.None);
        }
        
        private void DrawNet()
        {
            var startByX = (_chart.Paddings.PaddingLeftRatio * 0.6f) * Width;
                var startByY = _chart.Paddings.PaddingTopRatio * Height;

                for (float x = 1; x <= _chart.AxeSetting.CountPointsOnXAxe; x++)
                {
                    var leftPadding = _chart.AxeSetting.TouchMiddlePoint.X -
                                      _chart.ChartWidth * _chart.AxeSetting.PointOnEveryPercentAxeX * (x);

                    var vectorPoint1 = new Vector2(leftPadding, startByY);

                    var vectorPoint2 = new Vector2(leftPadding, _chart.Paddings.PaddingRightRatio * Height);

                    _d2DRenderTarget.DrawLine(vectorPoint1, vectorPoint2, _chart.Brushes.TransparentBlack);
                }

                var startPosition = ClientSize.Height * _chart.Paddings.PaddingTopRatio;
                var finishPosition = ClientSize.Height * _chart.Paddings.PaddingBottomRatio;
                var increaseOn = _chart.ChartHeight * _chart.AxeSetting.PointOnEveryPercentAxeY;

                for (float y = startPosition;y <= finishPosition; y += increaseOn)
                {
                    _d2DRenderTarget.DrawLine(new Vector2(startByX, y), new Vector2(_chart.Paddings.PaddingRightRatio * Width, y), _chart.Brushes.TransparentBlack);
                }
        }

        private void DrawLines()
        {
            for (float y = ClientSize.Height * _chart.Paddings.PaddingTopRatio; y <= ClientSize.Height * _chart.Paddings.PaddingBottomRatio; y += _chart.ChartHeight * _chart.AxeSetting.PointOnEveryPercentAxeY * 2)
            {
                _d2DRenderTarget.DrawLine(new Vector2(0, y), new Vector2(_chart.AxeSetting.TouchMiddlePoint.X, y), _chart.Brushes.TransparentBlack);
            }
        }
        

        #endregion

        #region RenderHandlers

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            var key = args.KeyData;

            switch (key)
            {
                case Keys.Escape:
                {
                    _renderForm.Close();
                    this.Hide();
                    this.Close();
                    break;
                }
                case Keys.D1:
                {
                    if (_barType != BarType.Candle)
                    {
                        _barType = BarType.Candle;

                        SetForChartMinMaxPoints(_barRendering.Skip);

                        _barRendering = new BarCandle(_d2DRenderTarget, _barRendering.Bars, _chart, _barRendering.Skip);
                        break;
                    }
                    break;
                }
                case Keys.D2:
                {
                    if (_barType != BarType.OHLC)
                    {
                        _barType = BarType.OHLC;

                        SetForChartMinMaxPoints(_barRendering.Skip);

                        _barRendering = new BarOHLC(_d2DRenderTarget, _barRendering.Bars, _chart, _barRendering.Skip);
                        break;
                    }
                    break;
                }
                case Keys.Q:
                {
                    if (_chartHelpers != ChartHelpers.Net)
                    {
                        _chartHelpers = ChartHelpers.Net;

                        _drawAdditionChartHelper = DrawNet;

                        break;
                    }
                    _drawAdditionChartHelper -= DrawNet;

                    _chartHelpers = ChartHelpers.Default;
                    break;
                }
                case Keys.W:
                {
                    if (_chartHelpers != ChartHelpers.Lines)
                    {
                        _chartHelpers = ChartHelpers.Lines;

                        _drawAdditionChartHelper = DrawLines;
                        break;
                    }

                    _drawAdditionChartHelper -= DrawLines;
                    _chartHelpers = ChartHelpers.Default;
                    break;
                }
                case Keys.E:
                {
                    if (_actions != DrawingActions.LineDrawing)
                    {
                        _actions = DrawingActions.LineDrawing;
                        _tempDrawing = new LineFigure(_d2DRenderTarget);
                        break;
                    }

                    _tempDrawing = null;
                    _actions = DrawingActions.Default;
                    break;
                }
                case Keys.R:
                {
                    if (_actions != DrawingActions.RectangleDrawing)
                    {
                        _tempDrawing = new RectangleFigure(_d2DRenderTarget);
                        _actions = DrawingActions.RectangleDrawing;
                        break;
                    }

                    _tempDrawing = null;
                    _actions = DrawingActions.Default;
                    break;
                }
                case Keys.T:
                {
                    if (_actions != DrawingActions.EllipseDrawing)
                    {
                        _tempDrawing = new EllipseFigure(_d2DRenderTarget);
                        _actions = DrawingActions.EllipseDrawing;
                        break;
                    }

                    _tempDrawing = null;
                    _actions = DrawingActions.Default;
                    break;
                }
                case Keys.A:
                {
                    if(_actions != DrawingActions.StraightLineXDrawing)
                    {
                        _actions = DrawingActions.StraightLineXDrawing;
                        break;
                    }

                    _actions = DrawingActions.Default;
                    break;
                }
                case Keys.S:
                {
                    if (_actions != DrawingActions.StraightLineYDrawing)
                    {
                        _actions = DrawingActions.StraightLineYDrawing;
                        break;
                    }

                    _actions = DrawingActions.Default;
                    break;
                }
                case Keys.Z:
                {
                    if (_chartHelpers != ChartHelpers.Zoom)
                    {
                        _zoomArea = null;
                        _chartHelpers = ChartHelpers.Zoom;
                        break;
                    }

                    _chartHelpers = ChartHelpers.Default;
                    break;
                }
                case Keys.Delete:
                {
                    RemoveFigure();
                    break;
                }
                case Keys.C:
                {
                    _chart.ChangeColor();
                    break;
                }
                case Keys.Enter:
                {
                    ResetChart();
                    break;
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
                    SetStraightLine(_mousePisition.X, true);
                    break;
                }
                case DrawingActions.StraightLineYDrawing:
                {
                    SetStraightLine(_mousePisition.Y,false);
                    break;
                }
            }

            CheckCrosing();

            if (_chartHelpers == ChartHelpers.Zoom)
            {
                var x = args.X;
                var y = args.Y;

                if (_zoomArea == null)
                    _zoomArea = new RectangleFigure(_d2DRenderTarget);
                var isSetted = _zoomArea.SetPosition(new ScreenPoint(x,y));


                if (!isSetted) return;
                _chartHelpers = ChartHelpers.Default;
                _isZoomed = false;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_selectedDrawing == null)
            {
                return;
            }

            var crossedFigure = _selectedDrawing.FigureToReplace(_mousePisition);

            if (crossedFigure == null)
            {
                return;
            }
            _drawings.Remove(_selectedDrawing);

            _selectedDrawing = null;

            _tempDrawing = crossedFigure;

            _actions = DrawingActions.LineDrawing;
        
        }
        
        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            if (args.Delta < 0)
            {
                if (ModifierKeys == Keys.Control)
                {
                    _chart.ResizeUpDown(false);
                    _resize--;
                    _isResized = false;
                    return;
                }

                if (_barRendering.Skip - 1 == -1)
                    return;
                
                _barRendering.Skip--;
                
                SetForChartMinMaxPoints(_barRendering.Skip);

                if (_zoomArea != null)
                    _isZoomed = false;

                _isResized = false;
            }
            else
            {
                if (ModifierKeys == Keys.Control)
                {
                    _chart.ResizeUpDown(true);
                    _resize++;
                    _isResized = false;
                    return;
                }

                if (_barRendering.Skip + 1 == _barRendering.Bars.Count)
                    return;
                
                _barRendering.Skip++;

                SetForChartMinMaxPoints(_barRendering.Skip);

                if (_zoomArea != null)
                    _isZoomed = false;

                _isResized = false;
            }
            
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            _mousePisition.X = args.X;
            _mousePisition.Y = args.Y;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //e.Cancel = true;
            //this.Hide();
            _renderForm.Dispose();
            _d2DRenderTarget.Dispose();
            _d2DFactory.Dispose();
            _textFormat.Dispose();
            _swapChain.Dispose();
            _target.Dispose();
            _targetView.Dispose();
            _device.Dispose();

            //this.Hide();
            base.OnClosing(e);
        }

        private void Chart_ResizeEnd(object sender, EventArgs e)
        {
            var newSize = new Size(ClientSize.Width, ClientSize.Height);
            
            UpdateRender(newSize);
            _chart = new ChartDrawing(ClientSize.Width, ClientSize.Height, _d2DRenderTarget, TimingBy.Minute);
            _chartRendering = new ChartRendering.ChartRendering(_d2DRenderTarget, _textFormat, _chart);
            UpdateBarRendering();
            SetForChartMinMaxPoints(0);

            _borders = new RectangleF(newSize.Width * _chart.Paddings.PaddingLeftRatio,
                newSize.Height * _chart.Paddings.PaddingTopRatio, _chart.ChartWidth, _chart.ChartHeight);
        }

        #endregion

        #region Methods

        private void SetStraightLine(float point, bool isX)
        {
            var line = new LineFigure(_d2DRenderTarget);

            var lineStart = new Vector2();
            var lineFinish = new Vector2();

            if (isX)
            {
                lineStart.X = point;
                lineStart.Y = ClientSize.Height * (_chart.Paddings.PaddingTopRatio / 2);
                lineFinish.X = point;
                lineFinish.Y = ClientSize.Height * _chart.Paddings.PaddingBottomRatio;
            }
            else
            {
                lineStart.X = ClientSize.Width * (_chart.Paddings.PaddingLeftRatio / 2);
                lineStart.Y = point;
                lineFinish.X = ClientSize.Width * _chart.Paddings.PaddingRightRatio;
                lineFinish.Y = point;
            }

            line.Line = new CustomLine(lineStart, lineFinish);

            _drawings.Add((SelectedFigureBase)line);
        }

        private void ResetChart()
        {
            _drawings = new List<SelectedFigureBase>();
            _zoomArea = null;
            _selectedDrawing = null;
            _tempDrawing = null;
            _drawAdditionChartHelper = null;
            _actions = DrawingActions.Default;
            _chartHelpers = ChartHelpers.Default;
            _isZoomed = true;

            SetForChartMinMaxPoints(0);
        }

        private void CheckCrosing()
        {
            if (_drawings?.Any() == false)
                return;

            foreach (var figure in _drawings)
            {
                if (figure.IsFigureCrossed(_mousePisition))
                {
                    _selectedDrawing = figure;
                    return;
                }
            }

            _selectedDrawing = null;
        }

        private void RemoveFigure()
        {
            if (_selectedDrawing == null)
                return;

            _drawings.Remove(_selectedDrawing);
            _selectedDrawing = null;
        }

        private void SetChanges()
        {
            if (_tempDrawing == null)
                return;

            var isComplited = _tempDrawing.SetPosition(_mousePisition);

            if (isComplited)
            {
                _drawings.Add(_tempDrawing);
                _actions = DrawingActions.Default;
                _tempDrawing = null;
            }
        }

        private void ZoomChart()
        {
            _chart.ZoomChart(_zoomArea.RectangleF);
            _isZoomed = true;
        }

        private void UpdateResizing()
        {
            if (_resize > 0)
            {
                for (int i = 0; i < _resize; i++)
                {
                    _chart.ResizeUpDown(true);
                }
            }
            if (_resize < 0)
            {
                for (int i = _resize; i < 0; i++)
                {
                    _chart.ResizeUpDown(false);
                }
            }
        }

        private void GenerateBars()
        {
            var bars = GenerateCndelStickBars(500);

            if (_barType == BarType.Candle)
                _barRendering = new BarCandle(_d2DRenderTarget, bars, _chart);
            else
                _barRendering = new BarOHLC(_d2DRenderTarget, bars, _chart);
        }

        private void UpdateBarRendering()
        {
            if (_barType == BarType.Candle)
                _barRendering = new BarCandle(_d2DRenderTarget, _barRendering.Bars, _chart);
            else
                _barRendering = new BarOHLC(_d2DRenderTarget, _barRendering.Bars, _chart);
        }

        private void SetForChartMinMaxPoints(int skip)
        {
            _barRendering.Skip = skip;

            _chart.SetMinMaxBorders(_barRendering.MinMaxPositions());

            UpdateResizing();
        }

        private void SetRenderSettings(Size windowSize)
        {
            _renderForm = new RenderForm("Windows Form Chart by VmpKalin");

            this.ClientSize = windowSize;

            var textSize = ClientSize.Width * 0.015f;


            using (SharpDX.DirectWrite.Factory textFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared))
            {
                _textFormat = new Text(
                    textFactory,
                    "MS Sans Serif",
                    SharpDX.DirectWrite.FontWeight.SemiBold,
                    SharpDX.DirectWrite.FontStyle.Normal,
                    SharpDX.DirectWrite.FontStretch.Medium,
                    textSize);
            }

            _chainDescription = new SwapChainDescription()
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
                _chainDescription,
                out _device,
                out _swapChain);

            _d2DFactory = new SharpDX.Direct2D1.Factory();

            var factory = _swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

            _target = SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(_swapChain, 0);
            _targetView = new RenderTargetView(_device, _target);

            var surface = _target.QueryInterface<Surface>();

            var pixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied);

            _d2DRenderTarget = new RenderTarget(_d2DFactory, surface,
                new RenderTargetProperties(pixelFormat));
        }

        private void UpdateRender(Size newWindowSize)
        {
            _d2DRenderTarget.Dispose();
            _d2DFactory.Dispose();
            _targetView.Dispose();
            _device.Dispose();
            _swapChain.Dispose();
            _target.Dispose();
            _targetView.Dispose();

            _chainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(
                    ClientSize.Width,
                    ClientSize.Height,
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
                _chainDescription,
                out _device,
                out _swapChain);

            _d2DFactory = new SharpDX.Direct2D1.Factory();

            Factory factory = _swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

            _target = SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(_swapChain, 0);
            _targetView = new RenderTargetView(_device, _target);

            Surface surface = _target.QueryInterface<Surface>();

            var pixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied);

            _d2DRenderTarget = new RenderTarget(_d2DFactory, surface,
                new RenderTargetProperties(pixelFormat));

        }

        private static List<BarModel> GenerateCndelStickBars(int maxBars)
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
                bar.LastPrice = lastPrice;
                time += 40f;
                dateTime = dateTime.AddMinutes(1);
            }

            return bars;
        }

        #endregion
    }
}
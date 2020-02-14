﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MathStudioWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point previousPosition = new Point();
        private bool wasPressed = false;

        private GraphDrawer GraphDrawer { get; set; } = new GraphDrawer();
        private bool window_loaded { get; set; } = false;

        GraphablesViewModel GraphablesViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            GraphablesViewModel = new GraphablesViewModel();
            GraphDrawer.Graph = Graph;
            GraphDrawer.Graphables = GraphablesViewModel.Graphables;
            Focusable = true;

            ExpressionsList.DataContext = GraphablesViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            window_loaded = true;
            PreviewKeyDown += MainWindow_KeyDown;

            AppendToGraphables(new FunctionModel());

            Draw();
            Keyboard.Focus(this);

            MouseLeftButtonDown += Graph_MouseLeftButtonDown;
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (!wasPressed)
                {
                    wasPressed = true;
                    return;
                }
            }
            else
            {
                wasPressed = false;
            }

            Point newPosition = Mouse.GetPosition(Graph);
            if (previousPosition != new Point(0, 0))
            {
                GraphDrawer.Offset(previousPosition, newPosition);
                Draw();
            }

            previousPosition = newPosition;

        }

        private void Add_ButtonClick(object sender, RoutedEventArgs e)
        {
            FunctionModel function = new FunctionModel();
            AppendToGraphables(function);

            Draw();
        }

        private void AppendToGraphables(IGraphable graphable)
        {
            GraphablesViewModel.Graphables.Add(graphable);
            graphable.PropertyChanged += FunctionChange_NotifyEvent;
        }

        private void ReplaceInGraphables(int index, IGraphable graphable)
        {
            GraphablesViewModel.Graphables.RemoveAt(index);
            GraphablesViewModel.Graphables.Insert(index, graphable);
            graphable.PropertyChanged += FunctionChange_NotifyEvent;
        }

        private void FunctionChange_NotifyEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsGraphable")
            {
                Draw();
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                GraphDrawer.ZoomOut();
            }
            else if (e.Delta < 0)
            {
                GraphDrawer.ZoomIn();
            }
            Draw();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

        private void Clear_ButtonClick(object sender, RoutedEventArgs e)
        {
            GraphablesViewModel.Graphables.Clear();
            Draw();
        }

        private void RemoveItem_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            IGraphable function = (IGraphable)button.DataContext;

            GraphablesViewModel.Graphables.Remove(function);
            Draw();
        }

        private void ReplaceItem_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            IGraphable function = (IGraphable)button.DataContext;

            
            int index = GraphablesViewModel.Graphables.IndexOf(function);
            ReplaceInGraphables(index, new FunctionModel());
            Draw();
        }

        public static IEnumerable<Point> GetIntersectionPoints(PointCollection points1, PointCollection points2)
        {
            return points1.Intersect(points2);
        }

        private Point MousePosition { get; set; }
        private bool isCaptured { get; set; }
        private void Graph_MouseMove(object sender, MouseEventArgs e)
        {
            var currentPosition = e.GetPosition(Graph);

            if (e.LeftButton == MouseButtonState.Released)
            {
                Mouse.Capture(null);
                isCaptured = false;
                return;
            }
            if (!isCaptured)
            {
                Mouse.Capture(Graph);
                isCaptured = true;
                MousePosition = currentPosition;
                return;
            }

            GraphDrawer.Offset(MousePosition, currentPosition);
            MousePosition = currentPosition;
            Draw();
        }

        private void Graph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MousePosition = e.GetPosition(Graph);
            Mouse.Capture(Graph);
            isCaptured = true;
        }

        private void PICheckbox_Checked(object sender, RoutedEventArgs e)
        {
            GraphDrawer.IsPiEnabled = (bool)PICheckbox.IsChecked;
            Draw();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
                bool boolX = Keyboard.FocusedElement is TextBox;
                if (boolX)
                {
                    Console.WriteLine("true");
                }
                else
                {
                    Console.WriteLine("false");
                }
            }

            Draw();
        }

        private void Draw()
        {
            if (window_loaded)
            {
                GraphDrawer.DrawGraph();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Canvas = System.Windows.Controls.Canvas;
//using Windows.UI.Xaml;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
//using Windows.UI.Xaml.Controls;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;


namespace ImageRectangles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Point startPosition;
        System.Windows.Point endPosition;
        //List of Rectangles
        List<Rectangle> rects = new List<Rectangle>();
        //Dictionary used to store Rectangle and their selection status boolean as a key - value pair 
        Dictionary<String, bool> rectStatuses = new Dictionary<String, bool>();
        //currently selected rectangle
        Rectangle selectedRect = null;
        //Resizing point
        Point resizeStartPos;
        //resizing direction
        String resizeDir = null;
        //Initialized image as null
        BitmapImage bgImage = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        //Button mapping for Browsing for a background Image
        private void BrowseButton_Click(object sender, RoutedEventArgs e)

        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            //display Files dialog box
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                FileNameLabel.Content = selectedFileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new System.Uri(selectedFileName);
                bitmap.EndInit();
                ((ImageBrush)canvasWindow.Background).ImageSource = bitmap;
                canvasWindow.Height = bitmap.Height;
                canvasWindow.Width = bitmap.Width;
            }
        }

        private void MousePressedDownHandler(object sender, MouseEventArgs e)
        {
            // Get the x and y coordinates of the mouse pointer.
            startPosition = e.GetPosition(sender as FrameworkElement);
        }
        //movement of mouse while pressed 
        private void MouseMovedHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.Source.ToString() == "System.Windows.Shapes.Rectangle")
                {
                    System.Windows.Shapes.Rectangle dragRect = e.Source as Rectangle;
                    DragDrop.DoDragDrop(dragRect, dragRect, DragDropEffects.Move);
                    endPosition = e.GetPosition(sender as FrameworkElement);
                }

            }

        }
        //end of mouse movement Handler ie. mouse lifted
        private void MouseLiftedUpHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                // Get the x and y coordinates of the mouse pointer at release
                endPosition = e.GetPosition(sender as FrameworkElement);

                int X = (int)Math.Min(startPosition.X, endPosition.X);
                int Y = (int)Math.Min(startPosition.X, endPosition.X);
                int width = (int)Math.Abs(startPosition.X - endPosition.X);
                int height = (int)Math.Abs(startPosition.Y - endPosition.Y);

                // Create the rectangle
                System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle()
                {
                    Width = width,
                    Height = height,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 2,
                    Uid = System.Guid.NewGuid().ToString()
                };
                rec.AddHandler(Rectangle.MouseUpEvent, new MouseButtonEventHandler(Rect_OnMouseUp));
                rec.AddHandler(Rectangle.MouseUpEvent, new MouseButtonEventHandler(Rect_OnMouseDown));
                rectStatuses.Add(rec.Uid, false);

                // add multiple rectangles as Children of the Canvas

                Canvas canvas = e.Source as Canvas;
                if (canvas != null)
                {
                    canvas.Children.Add(rec);

                }
                // Conditional Statements for deciding the direction of resizing

                if (endPosition.X >= startPosition.X)
                {
                    Canvas.SetLeft(rec, startPosition.X);
                }
                else
                {
                    Canvas.SetLeft(rec, endPosition.X);
                }

                if (endPosition.Y >= startPosition.Y)
                {
                    Canvas.SetTop(rec, startPosition.Y);
                }
                else
                {
                    Canvas.SetTop(rec, endPosition.Y);
                }

                if (rects != null)
                {
                    rects.Add(rec);
                }

            }
        }
        // Drop rectangle 
        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            Trace.WriteLine("canvas drop");
            Rectangle rect = (Rectangle)e.Data.GetData(e.Data.GetFormats()[0]);
            Point dropPosition = e.GetPosition(canvasWindow);
            Trace.WriteLine("pos test: " + e.GetPosition(rect));
            //check for rectangle's selection
            if (!rectStatuses[rect.Uid])
            {
                Canvas.SetLeft(rect, dropPosition.X);
                Canvas.SetTop(rect, dropPosition.Y);
            }
            else
            {
                //switch cases for new dimensions
       
                switch (resizeDir)
                {
                    case "bottom":
                        double newHeight = dropPosition.Y - Canvas.GetTop(rect);
                        rect.Height = newHeight;
                        unselect(selectedRect);
                        break;
                    case "top":
                        double heightDiff = Canvas.GetTop(rect) - dropPosition.Y;
                        Canvas.SetTop(rect, dropPosition.Y);
                        rect.Height += heightDiff;
                        unselect(selectedRect);
                        break;
                    case "right":
                        double newWidth = dropPosition.X - Canvas.GetLeft(rect);
                        rect.Width = newWidth;
                        unselect(selectedRect);
                        break;
                    case "left":
                        double widthDiff = Canvas.GetLeft(rect) - dropPosition.X;
                        Canvas.SetLeft(rect, dropPosition.X);
                        rect.Width += widthDiff;
                        unselect(selectedRect);
                        break;
                }
            }
            resizeDir = null;
        }

        //Toggle selected rectangle
        private void Rect_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("mouse up");
            Rectangle rect = e.Source as Rectangle;

            bool selected;
            bool success = rectStatuses.TryGetValue(rect.Uid, out selected);

            if (success)
            {
                if (!selected)
                {
                    // Now should become selected
                    select(rect);
                }
                else
                {
                    // Was already clicked on, so unselect
                    unselect(rect);
                }

            }

        }
        //clicking on rectangle
        private void Rect_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("mouse down");
            resizeStartPos = e.GetPosition(this);
            Rectangle rect = e.Source as Rectangle;
            //Conditional statements to pick a direction to resize
            if (rectStatuses[rect.Uid])
            {
                int selectArea = 20;
                if (resizeStartPos.Y >= Canvas.GetTop(rect) + rect.Height - selectArea)
                {
                    // check bottom
                    Trace.WriteLine("clicked rect bottom");
                    resizeDir = "bottom";
                }
                else if (resizeStartPos.Y <= Canvas.GetTop(rect) + selectArea)
                {
                    // check top
                    Trace.WriteLine("clicked rect top");
                    resizeDir = "top";
                }
                else if (resizeStartPos.X >= Canvas.GetLeft(rect) + rect.Width - selectArea)
                {
                    // check right
                    Trace.WriteLine("clicked rect right");
                    resizeDir = "right";
                }
                else if (resizeStartPos.X <= Canvas.GetLeft(rect) + selectArea)
                {
                    // check left
                    Trace.WriteLine("clicked rect left");
                    resizeDir = "left";
                }
            }

        }

        //Menu item to change color of selected Rectangle to Blue
        private void Blue_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRect != null)
            {
                if (rectStatuses[selectedRect.Uid])
                {
                    selectedRect.Stroke = System.Windows.Media.Brushes.Blue;
                    unselect(selectedRect);

                }
            }
        }

        //Menu item to change color of selected Rectangle to Green
        private void Green_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRect != null)
            {
                if (rectStatuses[selectedRect.Uid])
                {
                    selectedRect.Stroke = System.Windows.Media.Brushes.Green;
                    unselect(selectedRect);

                }
            }
        }

        //Menu item to change color of selected Rectangle to Yellow
        private void Yellow_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRect != null)
            {

                if (rectStatuses[selectedRect.Uid])
                {
                    selectedRect.Stroke = System.Windows.Media.Brushes.Yellow;
                    unselect(selectedRect);

                }
            }

        }

        //Menu item to change color of selected Rectangle to Black
        private void Black_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRect != null)
            {

                if (rectStatuses[selectedRect.Uid])
                {
                    selectedRect.Stroke = System.Windows.Media.Brushes.Black;
                    unselect(selectedRect);

                }
            }
        }

        //Menu item to change color of selected Rectangle to Red
        private void Red_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRect != null)
            {
                if (rectStatuses[selectedRect.Uid])
                {
                    selectedRect.Stroke = System.Windows.Media.Brushes.Red;

                    unselect(selectedRect);
                }
            }
        }
        //method to unselect rectangle
        public void unselect(Rectangle rect)
        {
            rect.Fill = System.Windows.Media.Brushes.Transparent;
            rectStatuses[rect.Uid] = false;
            selectedRect = null;
        }
        //method to select rectangle
        public void select(Rectangle rect)
        {
            rect.Fill = System.Windows.Media.Brushes.LightBlue;
            rectStatuses[rect.Uid] = true;
            selectedRect = rect;
        }

        //method to delete rectangle
        public void Delete()
        {
            if (selectedRect != null)
            {
                canvasWindow.Children.Remove(selectedRect);
                rects.Remove(selectedRect);
                rectStatuses.Remove(selectedRect.Uid);
            }
        }

        //maping delete key
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Delete();
        }


        //Save file with rectangles
        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "output"; // Default file name
            dlg.DefaultExt = ".xaml"; // Default file extension
            dlg.Filter = "XAML Files (*.xaml)|*.xaml"; // Filter files by extension
            dlg.InitialDirectory = ".\\SavedData";
            dlg.RestoreDirectory = true;

            // Show save file dialog box
            DialogResult result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == System.Windows.Forms.DialogResult.OK)
            {

                // Save document
                string filename = dlg.FileName + "." + dlg.DefaultExt;
                FileStream fs = File.Open(dlg.FileName, FileMode.Create);
                XamlWriter.Save(canvasWindow, fs);
                fs.Close();
            }
        }

        //Load or open file made through the application

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "XAML Files (*.xaml)|*.xaml"; // Filter files by extension
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                FileNameLabel.Content = selectedFileName;

                FileStream fs = File.Open(selectedFileName, FileMode.Open, FileAccess.Read);
                Canvas savedCanvas = XamlReader.Load(fs) as Canvas;
                fs.Close();
                sp.Children.Add(savedCanvas);
            }
        }

    }
}

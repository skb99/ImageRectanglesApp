
<h1 align="center">CaseGuard Assignment</h1>

# Setup

1. Microsoft Windows OS
2. Microsoft Visual Studio
3. ImageRectangles.exe Executable File


# Running Project

1. Open the provided file `ImageRectangles.exe` through the provided download file
 - The project can also be run after cloning the repository into Visual Studio and running the code



# Tutorial on the Application
This application allows the user to draw rectangles over Images loaded through the application's `Browse` Button. The rectangles can only be made within the Loaded image.

1. Open the App
2. hit the `Browse` button
3. Select an image from your local directories to draw rectangles over
 - This image should appear in the center of the Canvas
4. Now, click and drag within the Image to produce a rectangle. You may produce as many Rectangles as you like!
5. Once the rectangles look good, Hit the `Save` Button on the top Left
6. in the `Save Dialog box` pop up, write your disired file name, save the file as a `.xaml` file
7. Now you may close the application, if you would like to see your previous work, re-open the app and hit `Load` and open the file from the `Dialog box`

# Additional Features Tutorial

## Move Rectangles
1. Once you have the rectangles created, to move them around, click and hold on the rectangle you would like to move, and `Drag` it to the new position within the Image
2. You should have your Image moved to your new location

## Select/Unselect Rectangles
1. To Select a Rectangle, click anywhere within the rectangle that is to be selected. the rectangle should fill with a light blue color. This indicates that your rectangle is selected.
2. To unselect, click anywhere within an already selected rectangle, the rectangle should return to a transparent fill color and with be unselected.

## Delete Rectangle
1. To delete a Rectangle, Select a rectangle by following the steps above, you have selected it, press the `Delete` Key. Your selected Rectangle should be deleted.
 - incase of multiple selected Rectangles, only the latest selected rectangle with be deleted. To Delete subsequent rectangles, please re-select the next rectangle.
 
## Change Rectangle Color
 1. To change the Color of the rectangles, `right click` on any unselected rectangle, the rectangle should get selected, and a drop down menu should appear.
 2. From this drop down menu, you will be presented with 5 color options `Blue`, `Yellow`,` Green`,`Black` and `Red`. 

## Resize Rectangles
*Disclaimer*
This feature can be quite finicky. 
To resize a rectangle.
1. Click on the rectangle, on the edge in the Direction where you want to resize the rectangle. 
2. For example, to change width of the rectangle by adjusting the left size, Click on to the left edge (a 20pixel margin of error is implemented towards the inside of the Rectangle)
3. Now the Rectanlgle should be selected
4. Now click and `Hold`  the edge, and `Drag` it to the desired size, then `Drop` 
6. The Rectangle should be resized
7. To continue changes, make sure the rectangle is unselected, and follow the above steps for the desired edge to be changed.
Finding the sweet spot to select the edge can take some trial and error. 

Please feel free to reach out with any feedback, questions or suggestions at krishnabh99@gmail.com or skb89@drexel.edu

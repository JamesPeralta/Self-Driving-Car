# 3D Self-driving car simulation
---
# Loading simulation scene
 - To load the proper scene, open the project an unity and navigate to 
    /Assets/Scenes/s1
---
# Special keyboard shortcuts
 - a: Slow down playback speed by 1
 - s: Speed up playback speed by 1
 - w: Save weights of neural network
 - space bar: Switch camera angle
---
# Configurations
All of the parameters for the genetic algorithm can be modified through the Terrain game object.
To change configurations go to Hierarchy/Insepector and you will see the configurable parameters for the GA.
---
# Configurable GA parameters
 - mutation rate: integer
 - mutation radius: float (optimal at 0.5)
 - population size: Even positive integer (defaults to 50 when odd)
 - weights file: Needs to be a .txt file that is located in the \Assests\Scripts\NN-Weights folder

# Unity project developed by [Damiano Perri](https://www.damianoperri.it/), [Osvaldo Gervasi](https://ogervasi.unipg.it/), [Marco Simonetti](https://www.researchgate.net/profile/Marco-Simonetti-6) from the University of Perugia

This project enables the automatic generation of datasets that can be used to train neural networks.
The datasets that can be generated are of two types:
Datasets for object recognition where the images portray the object(s).
In our use case, this scene generate pictures of paintings to classify the pictorial style.
Datasets where the images depict scenarios and the network has to classify the image in a category.


# Step 1 - You must download and install Unity, the project is developed with Unity 2019.4.9f1
# Add the project inside the Unity Hub and start the project
Now load the scene that you want to use:
```
objectsGenerator.unity
```
or
```
environmentGenerator.unity
```

# Step 2 - You must create some folders
The output folder: inside this folder the script will save the picture obtained during the dataset generation
The input folder: inside this folder you must put the picture of the paintings 

# Step 3 - Click the camera, check the inspector and adjust the variables as you wish

# Step 4 - Click the play button and wait the execution


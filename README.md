# PhotoOrganizer

This console app is a program that downloads an image and organizes those downloaded images.

- When the program starts running, it will first get the total number of images to download. (count)
- Then it will receive information about how many images are allowed to be downloaded at the same time. (parallelism)
- Then, the program can download random images from any website that are not identical to each other.
The total amount of `count`, up to `parallelism` at the same time by actively downloading the folder.

- Each downloaded image will be named according to its order among the downloads. For example, the 5th downloaded image
It will have the name "5.png".
- Total download progress (progress) will be written on the screen live.

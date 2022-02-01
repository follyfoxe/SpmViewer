# PM-Model-Viewer
A tool for viewing Super Paper Mario model files.

A way to export the models, and map support is planned, but no promises.<br>
Made in unity engine, I initially tried to make it on winforms C# but gave up.

Only tested with Super Paper Mario model files. Might work with TTYD.

# Future
I was planning to re-do this project, as i often tend to do, but without unity (OpenGL + ImGui) and still in C# of course.<br>
But i got very distracted with trying to get a pre-trained GAN neural network running in C# without python. (I succeeded)<br>
And now that i go back to work on that i realized that it was kind of a mess, so i might re-do the re-do.

# File format specs
I found initially [this forum](https://www.emutalk.net/threads/pm-ttyd-model-file-format-pet-project.27613/), 
and along with it, [this website](https://hocuspocus.taloncrossing.com/rii/), which has a bunch of TTYD model viewers and it's source code, 
i used those as a reference, along with a [file spec](https://hocuspocus.taloncrossing.com/rii/tplvtx.txt) found in that same website.

I later found [this repo](https://github.com/PistonMiner/ttyd-tools) which had [this document](https://github.com/PistonMiner/ttyd-tools/blob/master/ttyd-tools/docs/MarioSt_AnimGroupBase.bt); This was a lot more complete and helped a lot.

For map viewing support, i plan using [noclip.website's source code](https://github.com/magcius/noclip.website) as a reference, since they seem to have a really solid viewer.

# Libraries used
[marco-calautti's Rainbow.ImgLib](https://github.com/marco-calautti/Rainbow) for reading TPL textures,<br>
[psydack's uimgui package](https://github.com/psydack/uimgui) for the UI,<br>
[Ookii.Dialogs](http://www.ookii.org/software/dialogs/) for the Open File Dialogs.

Probably only works on Windows, because of the libraries; Not tested on any other platform.

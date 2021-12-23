# PM-Model-Viewer
A tool for viewing Super Paper Mario model files.

A way to export the models, and map support is planned, but no promises.<br>
Made in unity engine, I initially tried to make it on winforms C# but gave up.

# File format specs
I found initially [this forum](https://www.emutalk.net/threads/pm-ttyd-model-file-format-pet-project.27613/), 
and along with it, [this website](https://hocuspocus.taloncrossing.com/rii/), which has a bunch of TTYD model viewers and it's source code, 
i used those as a reference, along with a [file spec](https://hocuspocus.taloncrossing.com/rii/tplvtx.txt) found in that same website.

I later found [this repo](https://github.com/PistonMiner/ttyd-tools) which had [this document](https://github.com/PistonMiner/ttyd-tools/blob/master/ttyd-tools/docs/MarioSt_AnimGroupBase.bt); This was a lot more complete and helped a lot.

# Libraries used
[marco-calautti's Rainbow.ImgLib](https://github.com/marco-calautti/Rainbow) for reading TPL textures,<br>
[psydack's uimgui package](https://github.com/psydack/uimgui) for the UI,<br>
[Ookii.Dialogs](http://www.ookii.org/software/dialogs/) for the Open File Dialogs.

Probably only works on Windows, because of the libraries; Not tested on any other platform.

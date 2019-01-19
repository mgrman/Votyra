# Votyra

Votyra is a middleware designed to generate 'Transport Tycoon' style terrain in Unity 3D.
Currently the terrain is generated based on plane but will be expanded to voxels.

The generation is fast and can be run on separate thread, as not to hinder performance.
The whole terrain can be animated and can be modified during in runtime.

[Demo](https://mgrman.github.io/Votyra/demo.html)

    <script src="TemplateData/UnityProgress.js"></script>
    <script src="Build/UnityLoader.js"></script>
    <script>
        var gameInstance = UnityLoader.instantiate("gameContainer", "Build/WebGL-release.json", {
            onProgress: UnityProgress
        });
    </script>
     <div id="gameContainer" style="width: 960px; height: 600px"></div>
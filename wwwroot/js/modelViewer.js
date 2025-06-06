// wwwroot/js/modelViewer.js

var engine = null;
var scene = null;
var arcRotateCamera = null; // Keep a reference to the camera
// At the top of modelViewer.js, or inside window.blazorModelViewer
window.myJsFunctions = {
    pingBlazor: function () {
        console.log("JavaScript: pingBlazor function was called by Blazor!");
        return "JavaScript says: Pong!";
    }
};

// Keep your existing window.blazorModelViewer = { ... }
window.blazorModelViewer = {
    init: function (canvasId) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error("modelViewer.js: Canvas element not found with ID:", canvasId);
            return false;
        }

        // Dispose of existing engine and scene if they exist (for re-initialization, e.g., on hot reload)
        if (engine) {
            console.log("modelViewer.js: Disposing existing engine due to re-initialization.");
            engine.dispose();
            engine = null;
        }
        // Scene is disposed with engine, but good to nullify
        scene = null;
        arcRotateCamera = null;

        try {
            engine = new BABYLON.Engine(canvas, true, { preserveDrawingBuffer: true, stencil: true, antialias: true });
            scene = new BABYLON.Scene(engine);
            scene.clearColor = new BABYLON.Color4(0.9, 0.9, 0.9, 1); // Light gray background

            // Camera
            arcRotateCamera = new BABYLON.ArcRotateCamera("camera", -Math.PI / 2, Math.PI / 2.5, 15, BABYLON.Vector3.Zero(), scene);
            arcRotateCamera.attachControl(canvas, true);
            arcRotateCamera.wheelPrecision = 50;
            arcRotateCamera.lowerRadiusLimit = 1;   // Prevent zooming too close
            arcRotateCamera.upperRadiusLimit = 100; // Prevent zooming too far (adjust as needed)
            arcRotateCamera.minZ = 0.1; // Important for seeing objects up close

            // Lighting
            const light1 = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(1, 1, 0), scene);
            const light2 = new BABYLON.PointLight("light2", new BABYLON.Vector3(0, 1, -1), scene);
            light1.intensity = 0.7;
            light2.intensity = 0.5;

            engine.runRenderLoop(function () {
                if (scene && scene.activeCamera) {
                    scene.render();
                }
            });

            window.addEventListener("resize", function () {
                if (engine) {
                    engine.resize();
                }
            });

            console.log("modelViewer.js: Babylon.js engine initialized for canvas:", canvasId);
            return true;
        } catch (e) {
            console.error("modelViewer.js: Error during Babylon.js engine initialization:", e);
            const errorP = document.getElementById('modelError');
            if (errorP) errorP.innerText = "Error initializing 3D viewer: " + e.message;
            return false;
        }
    },

    loadModel: function (modelPath) {
        const errorP = document.getElementById('modelError');
        if (errorP) errorP.innerText = ""; // Clear previous errors

        if (!engine || !scene) {
            console.error("modelViewer.js: Scene or Engine not initialized. Call init first.");
            if (errorP) errorP.innerText = "Error: 3D Scene not initialized. Please refresh.";
            return;
        }
        console.log("modelViewer.js: Attempting to load model:", modelPath);

        // Clear previous models thoroughly
        // Iterate backwards when removing items from an array being iterated
        for (let i = scene.meshes.length - 1; i >= 0; i--) {
            // Don't dispose of the camera if it happens to be a mesh (though it's not in this setup)
            if (scene.meshes[i] && scene.meshes[i].name !== "camera") {
                 scene.meshes[i].dispose();
            }
        }
        // Other resources like materials, textures, skeletons might need specific cleanup
        // if you encounter memory leaks with many model loads, but this covers meshes.

        BABYLON.SceneLoader.ImportMesh("", "", modelPath, scene,
            function (newMeshes, particleSystems, skeletons, animationGroups) { // Success callback
                console.log("modelViewer.js: ImportMesh success callback entered.");
                if (newMeshes && newMeshes.length > 0) {
                    console.log(`modelViewer.js: Model loaded successfully. Meshes found: ${newMeshes.length}`);
                    newMeshes.forEach(function(mesh, index) {
                        console.log(`modelViewer.js: Mesh ${index}: Name='${mesh.name}', ID='${mesh.id}', Position=(${mesh.position.x.toFixed(2)}, ${mesh.position.y.toFixed(2)}, ${mesh.position.z.toFixed(2)})`);
                        const boundingInfo = mesh.getBoundingInfo();
                        if (boundingInfo) {
                            console.log(`modelViewer.js: Mesh ${index} BoundingBox Min: (${boundingInfo.boundingBox.minimumWorld.x.toFixed(2)}, ${boundingInfo.boundingBox.minimumWorld.y.toFixed(2)}, ${boundingInfo.boundingBox.minimumWorld.z.toFixed(2)})`);
                            console.log(`modelViewer.js: Mesh ${index} BoundingBox Max: (${boundingInfo.boundingBox.maximumWorld.x.toFixed(2)}, ${boundingInfo.boundingBox.maximumWorld.y.toFixed(2)}, ${boundingInfo.boundingBox.maximumWorld.z.toFixed(2)})`);
                        }
                    });

                    // Basic auto-focusing for ArcRotateCamera
                    if (arcRotateCamera) {
                        // Consider all meshes for bounding box calculation
                        var allLoadedMeshes = newMeshes.filter(m => m !== null); // Filter out any null meshes if any
                        if(allLoadedMeshes.length > 0) {
                            var minWorld = allLoadedMeshes[0].getBoundingInfo().boundingBox.minimumWorld;
                            var maxWorld = allLoadedMeshes[0].getBoundingInfo().boundingBox.maximumWorld;

                            for (var i = 1; i < allLoadedMeshes.length; i++) {
                                var meshMin = allLoadedMeshes[i].getBoundingInfo().boundingBox.minimumWorld;
                                var meshMax = allLoadedMeshes[i].getBoundingInfo().boundingBox.maximumWorld;
                                minWorld = BABYLON.Vector3.Minimize(minWorld, meshMin);
                                maxWorld = BABYLON.Vector3.Maximize(maxWorld, meshMax);
                            }
                            
                            var center = BABYLON.Vector3.Center(minWorld, maxWorld);
                            arcRotateCamera.target = center;

                            var extent = maxWorld.subtract(minWorld);
                            var maxSize = Math.max(extent.x, extent.y, extent.z);
                            
                            // Heuristic for radius: make it so the object roughly fills a good portion of the view.
                            // The factor (e.g., 1.5 or 2) might need adjustment based on typical model sizes and camera FOV.
                            // Also, ensure radius is not smaller than a minimum useful value.
                            arcRotateCamera.radius = Math.max(maxSize * 1.5, arcRotateCamera.lowerRadiusLimit * 5, 5); // Ensure a minimum reasonable radius

                            console.log(`modelViewer.js: Camera target set to: (${center.x.toFixed(2)}, ${center.y.toFixed(2)}, ${center.z.toFixed(2)}), Radius set to: ${arcRotateCamera.radius.toFixed(2)}`);
                        }
                    }

                } else {
                    console.warn("modelViewer.js: Model loaded, but NO meshes found in the model:", modelPath);
                    if (errorP) errorP.innerText = "Model loaded, but it appears to be empty or has no visible meshes.";
                }
            },
            null, // Progress callback
            function (scene, message, exception) { // Error callback
                console.error("modelViewer.js: Error loading model inside SceneLoader.ImportMesh:", modelPath, "Message:", message);
                if (exception) {
                    console.error("modelViewer.js: Exception details:", exception);
                }
                if (errorP) errorP.innerText = "Error loading model: " + message + (exception ? " - " + exception.message : "");
            }
        );
    },

    dispose: function () {
        console.log("modelViewer.js: Dispose called. Cleaning up Babylon.js engine and scene.");
        if (engine) {
            engine.dispose(); // This should also dispose the scene and its contents
            engine = null;
        }
        scene = null; // Ensure scene reference is cleared
        arcRotateCamera = null; // Clear camera reference
    }
    
};
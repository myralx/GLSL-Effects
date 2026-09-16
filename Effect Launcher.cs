using Raylib_cs;

internal static class Program
{
    public static void Main(string[] args)
    {
        // Find all effects folders
        string shadersFolder = "shaders";
        string[] effectFolders = Directory.GetDirectories(shadersFolder);

        string currentFolder;

        // Direct launch an effect through "dotnet run -- effect"
        if (args.Length > 0) // If at least one argument was provided
        {
            string effect = args[0];
            currentFolder = effectFolders.FirstOrDefault(folder =>
                Path.GetFileName(folder).Equals(effect) // Gets directory of the effect
            )!;
        }

        // Simple selection launcher for all effects
        else
        {
            Console.WriteLine("Choose an effect:\n");

            for (int i = 0; i < effectFolders.Length; i++)
            {
                string effect = Path.GetFileName(effectFolders[i]);
                Console.WriteLine($"{i + 1}. {effect}");
            }
            Console.Write("\nEffect: ");

            int choice = int.Parse(Console.ReadLine()!);
            currentFolder = effectFolders[choice - 1];
        }


        // Get the name of the selected effect
        string name = Path.GetFileName(currentFolder);

        // Find the shader inside the selected effect folder
        string[] shaders = Directory.GetFiles(currentFolder, "*.fs");
        string shaderPath = shaders[0];


        // Make the display window
        int screenWidth = 1200;
        int screenHeight = 800;

        Raylib.InitWindow(screenWidth, screenHeight, name);
        Raylib.SetTargetFPS(60);


        // Load fragment shader
        Shader shader = Raylib.LoadShader(null, shaderPath);
        // "null" is the vertex shader, not used here


        // Get the location of the shader uniforms
        int rlocation = // Resolution location
            Raylib.GetShaderLocation(shader, "resolution");

        int tlocation = // Time location
            Raylib.GetShaderLocation(shader, "time");


        // Resolution to send to shader
        float[] resolution =
        {
            screenWidth,
            screenHeight
        };

        Raylib.SetShaderValue(
            shader, // Which shader
            rlocation, // Where to send the value
            resolution, // What value to send
            ShaderUniformDataType.Vec2 // Type of value
        );


        while (!Raylib.WindowShouldClose())
        {
            float time = (float)Raylib.GetTime(); // Get current time

            Raylib.SetShaderValue(
                shader, // Which shader
                tlocation, // Where to send the value
                time, // What value to send
                ShaderUniformDataType.Float // Type of value
            );


            Raylib.BeginDrawing(); // Starts drawing
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginShaderMode(shader); // Starts using shader

            Raylib.DrawRectangle(
                0,
                0,
                screenWidth,
                screenHeight,
                Color.White
            );
            // Draws the rectangle for the effect to be displayed onto

            Raylib.EndShaderMode(); // Stops using shader

            Raylib.EndDrawing(); // Finishes drawing
        }


        Raylib.UnloadShader(shader); // Unload shader from memory
        Raylib.CloseWindow();
    }
}
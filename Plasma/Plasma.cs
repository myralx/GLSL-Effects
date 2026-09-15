using Raylib_cs;

internal static class Program
{
    public static void Main()
    {
        // Make the display window
        int screenWidth = 1200;
        int screenHeight = 800;

        Raylib.InitWindow(screenWidth, screenHeight, "Plasma");
        Raylib.SetTargetFPS(60);

        // Load fragment shader
        Shader shader = Raylib.LoadShader(null, "plasma.fs");
        // "null" is the vertex shader, not used here 

        // Get the location of the shader uniforms
        int rlocation = // resolution location
            Raylib.GetShaderLocation(shader, "resolution");

        int tlocation = // time location
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
                time, // what value to send
                ShaderUniformDataType.Float // type of value
            );

            Raylib.BeginDrawing(); // Starts drawing
            Raylib.ClearBackground(Color.Black);
            Raylib.BeginShaderMode(shader); // Starts using shader
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, Color.White);
            // Draws the rectangle for the effect to be displayed onto
            Raylib.EndShaderMode(); // Stops using shader
            Raylib.EndDrawing(); // Finishes drawing
        }

        Raylib.UnloadShader(shader); // unload shader from memory
        Raylib.CloseWindow(); 
    }
}
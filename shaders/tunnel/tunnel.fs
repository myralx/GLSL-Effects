#version 330

out vec4 color; // Current pixel color
uniform float time; // Create float time
uniform vec2 resolution; // Get resolution from Effect Launcher.cs

const float PI = 3.14159265359;

void main()
{
    vec2 uv = gl_FragCoord.xy / resolution; // Normalise pixel coordinates
    vec2 position = uv - 0.5; // And centre

    position.x *= resolution.x / resolution.y; // Fix aspect ratio

    // Move the centre of the tunnel around
    vec2 lookOffset = vec2(
    sin(time) * 0.25, // Moves left / right
    sin(time * 2.0) * 0.25); // Moves tunnel up / down

    position += lookOffset; // moves viewpoint 

    float distance = 1.0 / length(position); // How deep the pixel is
    float angle = atan(position.y, position.x) / (2.0 * PI); // How far along the wall it is

    distance += time * 0.8; // moves forward through the tunnel
    angle += time * 0.1; // rotates pattern around the tunnel

    // Adds the pattern texture around the tunnel
    float horizontalBands = sin(distance * 8.0); // Bands going into the tunnel
    float verticalBands = sin(angle * PI * 16.0); // Bands going around tunnel
    float pattern = horizontalBands * verticalBands;

    // Convert pattern from -1 to 1 to 0 to 1
    pattern = pattern * 0.5 + 0.5;

    vec3 darkColor = vec3(0.02, 0.0, 0.08);
    vec3 brightColor = vec3(0.3, 0.1, 1.0);

    vec3 tunnelColor = mix(darkColor, brightColor, pattern);

    color = vec4(tunnelColor, 1.0);
}
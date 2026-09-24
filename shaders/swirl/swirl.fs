#version 330

out vec4 color; // Current pixel color
uniform float time; // Create float time
uniform vec2 resolution; // Get resolution from Effect Launcher.cs

const float twist = 80.0; // Controls how much the swirl twists. Higher = less twist
const float bands = 15.0; // Number of bands in the swirl

void main()
{
    // Centre pixel coordinates
    float x = gl_FragCoord.x - resolution.x / 2.0;
    float y = gl_FragCoord.y - resolution.y / 2.0;

    float distanceFromCentre = length(vec2(x, y));

    float angle = 
        atan(x, y) - distanceFromCentre / twist + time; // Create the swirl

    float intensity =
        0.5 + 0.25 * sin(bands * angle); // Controls band brightness

    vec3 rainbowColor =
        0.5 + 0.5 * cos(angle + vec3(0.0, 2.0, 4.0)); // creates the rainbow colors
    
    vec3 finalColor = rainbowColor * intensity;
    color = vec4(finalColor, 1.0); // Final color
}
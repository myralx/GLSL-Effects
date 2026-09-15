#version 330

const vec2 vp = vec2(320, 200);
out vec4 color; // Current pixel color
uniform float time; // Create float time
uniform vec2 resolution; // Gets resolution from Plasma.cs

void main()
{
    float t = time * 10.0; // Controls the animation speed
    
    vec2 uv = gl_FragCoord.xy / resolution; // Converts pixel coordinates to 0-1 UV coords
    vec2 p0 = (uv - 0.5) * vp; // Centres coords at (0,0) and scales it to the viewport 
    vec2 hvp = vp * 0.5; // Half viewport.

    vec2 p1d = vec2(cos( t / 78.0),  sin( t / 162.0)) * hvp - p0;
    vec2 p2d = vec2(sin(-t / 114.0), cos(-t / 124.0)) * hvp - p0;
    vec2 p3d = vec2(cos(-t / 135.0), cos( t / 48.0))  * hvp - p0;
    // Sine waves. Gives the moving effect
    // Bigger number = slower movement 

    float sum = (
        cos(length(p1d) / 32.0) + // Create waves around point 1, spaced by 30
        cos(length(p2d) / 20.0) +
        sin(length(p3d) / 25.0) *
        // Bigger number = wider waves
        sin(p3d.x / 200.0) * // X direction waves
        sin(p3d.y / 105.0) // Y direction waves
        // Adds some cool variation

        );

// Colors
float colorValue = sum * 3.0; // Higher = more color!

float red   = 0.5 + 0.5 * sin(colorValue);
float green = 0.5 + 0.5 * sin(colorValue + 2.0);
float blue  = 0.5 + 0.5 * sin(colorValue + 4.0);

color = vec4(red, green, blue, 1.0);
}

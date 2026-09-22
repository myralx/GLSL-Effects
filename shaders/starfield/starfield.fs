#version 330

out vec4 color; // Current pixel color
uniform float time; // Create float time
uniform vec2 resolution; // Get resolution from Effect Launcher.cs

const float viewScale = 50.0; // How zoomed in the starfield is. Higher = more zoom
const float velocity = 0.08; // How fast. Higher = faster
const float numLayers = 5.0; // Number of star layers. Higher = more stars
const vec3 starTint = vec3(0.9, 0.59, 0.9); // RGB color of the stars
const vec3 sizeTint = vec3(0.0, 0.0, 1.0);  // Color added based on star size

const float TAU = 6.28318530718; // Tau is 2*Pi. Used for star variation.

float star(vec2 uv, float flare) // Makes the glowing star effects
{

    float d = length(uv); // Distance from the pixel to the star
    float brightness = 0.02 / d; // Closer to star = Brighter

    float rays = max(0.0, 0.5 - abs(uv.x * uv.y * 1000.0)); // Creates the cross effect coming from the stars

    brightness += (rays * flare) * 2.0; // Calculate brightness of rays and add to star
    brightness *= 1.0 - smoothstep(0.1, 1.0, d); // fades the glow with distance

    return brightness;
}


float Hash21(vec2 pos) // Helps randomise stars
{
    pos = fract(pos * vec2(123.34, 456.21)); // Hash constants
    pos += dot(pos, pos + 45.32); // more

    return fract(pos.x * pos.y);
}

vec3 genField(vec2 uv) // Generates the layers of stars
{
    vec3 col = vec3(0.0); // stores the total star colors for the current pixel

    vec2 id = floor(uv); // Current grid cell
    vec2 gv = fract(uv); // Position within current grid cell

    for (int y = -1; y <= 1; y++) // Loops through the grid vertically
    {
        for (int x = -1; x <= 1; x++) // Loops through the grid horizontally
        {
            vec2 offset = vec2(x, y); // Offset from current grid cell

            float n = Hash21(id + offset); // Use hash randomiser

            float size = n; // size of the star

            vec2 starPos = // random star position
                gv - offset - vec2(n, fract(n * 34.0)) + 0.5;
           
            float flare =
                smoothstep(0.1, 0.9, size) * 0.46; // Bigger star = bigger flare
 
            float starBrightness =
                star(starPos, flare); // Calculates the star's brightness

            vec3 starColor = 
                sin(vec3(0.2, 0.3, 0.9)* fract(n * 2345.2)* TAU) * 0.25 + 0.75;
            // Further randomises the value to decide star color
            starColor *= 
                starTint + vec3(0.0, 0.0, size); // Adds a blue tint overall

            // Makes each star pulse at a slightly different time
            starBrightness *= sin(time * 0.6 + n * TAU) * 0.5 + 0.5;

            col += starBrightness * size * starColor;
        }
    }

    return col;
}


void main()
{   
    vec2 uv =
        (gl_FragCoord.xy - 0.5 * resolution.xy) / resolution.y; 
    // Centre coordinates and correct for aspect ratio

    vec3 finalColor = vec3(0.0); // Finished starfield color

    for (float layer = 0.0; layer < 1.0; layer += 1.0 / numLayers) // loops once for each layer
    {
        float depth = 
            fract(layer + time * velocity); // How far along the current layer is

        float scale = 
            mix(viewScale, 0.5, depth); // Zoom in the layer over time

        float fade =
            depth * (1.0 - smoothstep(0.9, 1.0, depth)); // fade layer out over time

        vec2 layerUV =
            uv * scale // zoom the layer
            + layer * 453.2  // give each a differnt pattern
            - time * 0.05;  // slowly move the starfield


        finalColor +=
            genField(layerUV) * fade; // generates the layer and adds it to final image
    }

    color = vec4(finalColor, 1.0); // set final pixel color
}
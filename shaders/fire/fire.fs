#version 330

out vec4 color; // Current pixel color
uniform float time; // Create float time
uniform vec2 resolution; // Get resolution from Effect Launcher.cs


float rand(vec2 position)
{
    return fract(sin(dot(position, vec2(12.9898, 78.233))) * 43758.5453);
} // random value generator

float sm(float value) // Smooths the value
{
    return value * value * (3.0 - 2.0 * value);
}

float noise(vec2 position, float frequency)
{
    vec2 scaledPosition = position * frequency; // scales position based on noise

    float x1 = floor(scaledPosition.x);
    float y1 = floor(scaledPosition.y);
    float x2 = floor(scaledPosition.x + 1.0);
    float y2 = floor(scaledPosition.y + 1.0);
    // Gets the 4 corners of the current grid cell

    float xBlend = sm(fract(scaledPosition.x));
    float yBlend = sm(fract(scaledPosition.y));
    // Gets the smoothed position inside the current grid cell

    float bottomNoise = mix(rand(vec2(x1, y1)),rand(vec2(x2, y1)),xBlend);
    // Blends the bottom edge of the grid
    float topNoise = mix(rand(vec2(x1, y2)),rand(vec2(x2, y2)),xBlend);
    // Blends the top edge of the grid
    return mix(bottomNoise, topNoise, yBlend);
    // Blends the bottom and top noise vertically
}

// Creates multiple layers of noise
float layeredNoise(vec2 position,float frequency,int layers,float persistence)
{
    float value = 0.0; // Stores combined noise value
    float amplitude = 1.0; // Strength of current noise layer
    float totalAmplitude = 0.0; // Total strength of all noise layers

    for (int i = 0; i < layers; i++)
    {
        totalAmplitude += amplitude; 
        // Adds current layers amplitude to total amplitude
        // totalAmplitude = totalAmplitude + amplitude

        value += noise(position, frequency) * amplitude;
        // Adds the current noise layer to value

        frequency *= 2.0; // Increases detail each layer
        amplitude *= persistence; // Decreases strength each layer
    }

    return value / totalAmplitude;
}


void main()
{
    vec2 uv = gl_FragCoord.xy / resolution.xy; // Convert pixel coordinates to 0-1 UV coords

    float gradient = 1.0 - uv.y; // lowers gradient as it goes up
    float gradientStep = 0.2; // Gradient offset

    vec2 position = gl_FragCoord.xy / resolution.x;
    position.y -= time * 0.3125; // moves the noise vertically over time

    vec4 bc = vec4(1.0, 0.65, 0.1, 1.0); // brighter color
    vec4 dc = vec4(1.0, 0.0, 0.15, 0.8); // darker color
    vec4 mc = mix(bc, dc, 0.5); // middle color

    float noiseValue = layeredNoise(position,10.0,5,0.5); // noise value of the current pixel

    float fStep = smoothstep(0.0, noiseValue, gradient); // First step, creates the main fire shape
    float dcStep = smoothstep(0.0, noiseValue, gradient - gradientStep); // Creates the inner part of the fire
    float dcPath = fStep - dcStep; // Creates the dark color region

    vec4 fColor = mix(bc,dc,dcPath); // Blends the bright and dark fire colors

    float mcStep = smoothstep(0.0, noiseValue, gradient - gradientStep * 2.0);
    // Creates another inner fire shape
    
    fColor = mix(fColor, mc, dcStep - mcStep); // Blends the middle fire region into the rest

    fColor = mix(vec4(0.0),fColor,fStep); // Mixes the background color in, to create the overall fire shape
    
    color = fColor; // Final pixel color
}